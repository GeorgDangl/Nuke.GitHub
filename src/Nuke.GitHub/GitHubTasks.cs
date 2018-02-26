using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Nuke.Common.Git;
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
                Draft = true
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
            if (gitRepository.Endpoint != "github.com")
            {
                throw new ArgumentException("The GitRepository must be from GitHub", nameof(gitRepository));
            }

            var split = gitRepository.Identifier.Split('/');
            return (split[0], split[1]);
        }

        public static string GetCompleteChangeLog(string changeLogFile, bool escapeMsBuildProperty)
        {
            var fileContent = File.ReadAllText(changeLogFile);
            var lines = Regex.Split(fileContent, "\r\n?|\n");
            var changeLogLines = lines
                .SkipWhile(l => !l.StartsWith("##"));
            var stringBuilder = new StringBuilder();
            foreach (var changeLogLine in changeLogLines)
            {
                stringBuilder.AppendLine(changeLogLine);
            }

            var releaseNotes = stringBuilder.ToString();
            if (escapeMsBuildProperty)
            {
                releaseNotes = EscapePropertyForMsBuild(releaseNotes);
            }

            return releaseNotes;
        }

        public static string GetLatestChangeLog(string changeLogFile, bool escapeMsBuildProperty)
        {
            var fileContent = File.ReadAllText(changeLogFile);
            var lines = Regex.Split(fileContent, "\r\n?|\n");
            var changeLogLines = lines
                .SkipWhile(l => !l.StartsWith("##"))
                .TakeWhile((l, i) => i == 0 || !l.StartsWith("##"));
            var stringBuilder = new StringBuilder();
            foreach (var changeLogLine in changeLogLines)
            {
                stringBuilder.AppendLine(changeLogLine);
            }

            var releaseNotes = stringBuilder.ToString();
            if (escapeMsBuildProperty)
            {
                releaseNotes = EscapePropertyForMsBuild(releaseNotes);
            }

            return releaseNotes;
        }

        static string EscapePropertyForMsBuild(string source)
        {
            return source
                .Replace(";", "%3B")
                .Replace(",", "%2C")
                .Replace(" ", "%20")
                .Replace("\r", "%0D")
                .Replace("\n", "%0A");
        }
    }
}
