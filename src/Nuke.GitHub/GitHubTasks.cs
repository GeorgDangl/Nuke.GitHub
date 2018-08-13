using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Nuke.Common.Git;
using Nuke.Common;
using Nuke.Common.Tooling;
using Octokit;

namespace Nuke.GitHub
{
    public static partial class GitHubTasks
    {
        public static async Task PublishRelease(Configure<GitHubReleaseSettings> configure)
        {
            var settings = configure.Invoke(new GitHubReleaseSettings());
            settings.Validate();
            var releaseTag = settings.Tag;
            var client = GetAuthenticatedClient(settings.Token);
            var existingReleases = await client.Repository.Release.GetAll(settings.RepositoryOwner, settings.RepositoryName);

            if (existingReleases.Any(r => r.TagName == releaseTag))
            {
                // Release already present
                return;
            }

            var newRelease = new NewRelease(releaseTag)
            {
                TargetCommitish = settings.CommitSha,
                Name = releaseTag,
                Body = settings.ReleaseNotes,
                Draft = true,
                Prerelease = settings.Prerelease ?? false
            };
            var releaseCreationResult = await client.Repository.Release.Create(settings.RepositoryOwner, settings.RepositoryName, newRelease);

            var createdRelease = await client.Repository.Release.Get(settings.RepositoryOwner, settings.RepositoryName, releaseCreationResult.Id);

            if (settings.ArtifactPaths != null)
            {
                foreach (var artifactPath in settings.ArtifactPaths)
                {
                    using (var artifactStream = File.OpenRead(artifactPath))
                    {
                        var fileName = Path.GetFileName(artifactPath);
                        var assetUpload = new ReleaseAssetUpload
                        {
                            FileName = fileName,
                            ContentType = "application/octet-stream",
                            RawData = artifactStream,
                        };

                        await client.Repository.Release.UploadAsset(createdRelease, assetUpload);
                    }
                }
            }

            var updatedRelease = createdRelease.ToUpdate();
            updatedRelease.Draft = false;
            await client.Repository.Release.Edit(settings.RepositoryOwner, settings.RepositoryName, createdRelease.Id, updatedRelease);
        }

        public static async Task CreatePullRequest(Configure<GitHubPullRequestSettings> configure)
        {
            var settings = configure.Invoke(new GitHubPullRequestSettings());
            settings.Validate();
            var client = GetAuthenticatedClient(settings.Token);
            var repository = await GetRepository(x => settings);
            var pullRequests = await client.Repository.PullRequest.GetAllForRepository(repository.Id,
                new PullRequestRequest
                {
                    State = ItemStateFilter.Open,
                    Head = settings.Head,
                    Base = settings.Base
                });

            if (pullRequests.Count == 0) Logger.Info($"Pull request from branch '{settings.Head}' into '${settings.Base}' already exists");
            await client.PullRequest.Create(repository.Id, new NewPullRequest(settings.Title, settings.Head, settings.Base) { Body = settings.Body });
        }

        public static async Task<IReadOnlyList<Release>> GetReleases(Configure<GitHubSettings> configure, int? maxNumberOfReleases)
        {
            var settings = configure.Invoke(new GitHubSettings());
            settings.Validate();
            var client = GetAuthenticatedClient(settings.Token);

            var apiOptions = new ApiOptions { PageSize = maxNumberOfReleases };
            return await client.Repository.Release.GetAll(settings.RepositoryOwner, settings.RepositoryName, apiOptions);
        }

        public static async Task<Repository> GetRepository(Configure<GitHubSettings> configurator)
        {
            var settings = configurator.Invoke(new GitHubSettings());
            settings.Validate();
            var client = GetAuthenticatedClient(settings.Token);
            return await client.Repository.Get(settings.RepositoryOwner, settings.RepositoryName);
        }

        static GitHubClient GetAuthenticatedClient(string token)
        {
            var client = new GitHubClient(new ProductHeaderValue("dangl-bot"))
            {
                Credentials = new Credentials(token)
            };
            return client;
        }

        public static (string gitHubOwner, string repositoryName) GetGitHubRepositoryInfo(GitRepository gitRepository)
        {
            ControlFlow.Assert(gitRepository.IsGitHubRepository(), $"The {nameof(gitRepository)} parameter must reference a GitHub repository.");   

            var split = gitRepository.Identifier.Split('/');
            return (split[0], split[1]);
        }
    }
}
