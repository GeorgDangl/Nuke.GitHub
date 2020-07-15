﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Nuke.Common.Git;
using Nuke.Common;
using Nuke.Common.Tooling;
using Octokit;
using Nuke.Common.Tools.GitHub;

namespace Nuke.GitHub
{
    public static partial class GitHubTasks
    {
        public static async Task PublishRelease(Configure<GitHubReleaseSettings> configure)
        {
            var settings = configure.Invoke(new GitHubReleaseSettings());
            settings.Validate();
            var releaseTag = settings.Tag;
            var client = GetAuthenticatedClient(settings.Token, settings.Url);
            var existingReleases = await client.Repository.Release.GetAll(settings.RepositoryOwner, settings.RepositoryName);

            if (existingReleases.Any(r => r.TagName == releaseTag))
            {
                // Release already present
                return;
            }

            var newRelease = new NewRelease(releaseTag)
            {
                TargetCommitish = settings.CommitSha,
                Name = settings.Name ?? releaseTag,
                Body = settings.ReleaseNotes,
                Draft = true,
                Prerelease = settings.Prerelease ?? false
            };
            Logger.Info("Creating release draft...");
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

                        Logger.Info($"Uploading artifact {artifactPath}...");
                        await client.Repository.Release.UploadAsset(createdRelease, assetUpload);
                    }
                }
            }

            var updatedRelease = createdRelease.ToUpdate();
            updatedRelease.Draft = false;
            await client.Repository.Release.Edit(settings.RepositoryOwner, settings.RepositoryName, createdRelease.Id, updatedRelease);
            Logger.Info($"Release {settings.Name ?? releaseTag} was successfully created");
        }

        public static async Task CreatePullRequest(Configure<GitHubPullRequestSettings> configure)
        {
            var settings = configure.Invoke(new GitHubPullRequestSettings());
            settings.Validate();
            var client = GetAuthenticatedClient(settings.Token, settings.Url);
            var repository = await GetRepository(x => settings);
            var pullRequests = await client.Repository.PullRequest.GetAllForRepository(repository.Id,
                new PullRequestRequest
                {
                    State = ItemStateFilter.Open,
                    Head = settings.Head,
                    Base = settings.Base
                });

            if (pullRequests.Any())
            {
                Logger.Info($"Pull request from branch '{settings.Head}' into '{settings.Base}' already exists");
                return;
            }
            await client.PullRequest.Create(repository.Id, new NewPullRequest(settings.Title, settings.Head, settings.Base) { Body = settings.Body });
        }

        public static async Task<IReadOnlyList<Release>> GetReleases(Configure<GitHubSettings> configure, int? numberOfReleases)
        {
            ControlFlow.Assert(numberOfReleases == null || numberOfReleases > 0, "numberOfReleases == null || numberOfReleases > 0");

            var settings = configure.Invoke(new GitHubSettings());
            settings.Validate();

            var apiOptions = new ApiOptions();
            if (numberOfReleases.HasValue)
            {
                const int maxPageSize = 100;
                apiOptions.PageSize = Math.Min(numberOfReleases.Value, maxPageSize);
                apiOptions.PageCount = (int) Math.Ceiling((double) numberOfReleases.Value / 100);
            }

            var client = GetAuthenticatedClient(settings.Token, settings.Url);
            return await client.Repository.Release.GetAll(settings.RepositoryOwner, settings.RepositoryName, apiOptions);
        }

        public static async Task<Repository> GetRepository(Configure<GitHubSettings> configurator)
        {
            var settings = configurator.Invoke(new GitHubSettings());
            settings.Validate();
            var client = GetAuthenticatedClient(settings.Token, settings.Url);
            return await client.Repository.Get(settings.RepositoryOwner, settings.RepositoryName);
        }

        static GitHubClient GetAuthenticatedClient(string token, string url)
        {
            if (String.IsNullOrEmpty(url))
            {
             return new GitHubClient(new ProductHeaderValue("dangl-bot"), new Uri(url))
              {

                Credentials = new Credentials(token)
              };
            }
            else
            {
              return new GitHubClient(new ProductHeaderValue("dangl-bot"), new Uri(url))
              {

                Credentials = new Credentials(token)
              };
            }           
        }

        public static (string gitHubOwner, string repositoryName) GetGitHubRepositoryInfo(GitRepository gitRepository)
        {
            ControlFlow.Assert(gitRepository.IsGitHubRepository(), $"The {nameof(gitRepository)} parameter must reference a GitHub repository.");   

            var split = gitRepository.Identifier.Split('/');
            return (split[0], split[1]);
        }
    }
}
