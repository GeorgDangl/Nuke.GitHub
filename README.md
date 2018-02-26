# Nuke.GitHub


![NuGet](https://img.shields.io/nuget/v/Nuke.GitHub.svg)
[![MyGet](https://img.shields.io/myget/dangl/v/Nuke.GitHub.svg)]()

This plugin provides some methods to work with GitHub repositories
in [NUKE Build](https://github.com/nuke-build/nuke).

Currently supported:
  * **PublishRelease** to create GitHub releases.

[Link to documentation](https://docs.dangl-it.com/Projects/Nuke.GitHub).

## CI Builds

All builds are available on MyGet:

    https://www.myget.org/F/dangl/api/v2
    https://www.myget.org/F/dangl/api/v3/index.json

## Example

    using static Nuke.GitHub.GitHubTasks;

    Target PublishGitHubRelease => _ => _
        .DependsOn(Pack)
        .Requires(() => GitHubAuthenticationToken)
        .OnlyWhen(() => GitVersion.BranchName.Equals("master") || GitVersion.BranchName.Equals("origin/master"))
        .Executes<Task>(async () =>
        {
            var changeLog = GetLatestChangeLog(ChangeLogFile, escapeMsBuildProperty: false);
            var repositoryInfo = GetGitHubRepositoryInfo(GitRepository);
            var releaseTag = $"v{GitVersion.MajorMinorPatch}";

            await PublishRelease(new GitHubReleaseSettings()
                .SetArtifactPaths(GlobFiles(OutputDirectory, "*.nupkg").NotEmpty().ToArray())
                .SetCommitSha(GitVersion.Sha)
                .SetReleaseNotes(changeLog)
                .SetRepositoryName(repositoryInfo.repositoryName)
                .SetRepositoryOwner(repositoryInfo.gitHubOwner)
                .SetTag(releaseTag)
                .SetToken(GitHubAuthenticationToken)
            );
        });
