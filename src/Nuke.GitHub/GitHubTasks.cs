using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Nuke.Common.Git;
using Nuke.Common;
using Octokit;

namespace Nuke.GitHub
{
    public static partial class GitHubTasks
    {
        public static async Task PublishRelease(GitHubReleaseSettings settings)
        {
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
