# Nuke.GitHub

[![Build Status](https://jenkins.dangl.me/buildStatus/icon?job=Nuke.GitHub/develop)](https://jenkins.dangl.me/job/Nuke.GitHub/develop)  
[![Built with Nuke](http://nuke.build/rounded)](https://www.nuke.build)

![NuGet](https://img.shields.io/nuget/v/Nuke.GitHub.svg)
[![MyGet](https://img.shields.io/myget/dangl/v/Nuke.GitHub.svg)]()

This plugin provides some methods to work with GitHub repositories
in [NUKE Build](https://github.com/nuke-build/nuke).

Currently supported:
  * **PublishRelease** to create GitHub releases.
  * **CreatePullRequest**
  * **GetReleases**
  * **GetRepository**

[Link to documentation](https://docs.dangl-it.com/Projects/Nuke.GitHub).

[Changelog](./Changelog.md)

## CI Builds

All builds are available on MyGet:

    https://www.myget.org/F/dangl/api/v2
    https://www.myget.org/F/dangl/api/v3/index.json

## Example

    using static Nuke.GitHub.GitHubTasks;
    using static Nuke.GitHub.ChangeLogExtensions;
    using static Nuke.Common.ChangeLog.ChangelogTasks;

    Target PublishGitHubRelease => _ => _
        .DependsOn(Pack)
        .Requires(() => GitHubAuthenticationToken)
        .OnlyWhen(() => GitVersion.BranchName.Equals("master") || GitVersion.BranchName.Equals("origin/master"))
        .Executes<Task>(async () =>
        {
            var releaseTag = $"v{GitVersion.MajorMinorPatch}";

            // Not providing the second, optional parameter gives the latest section
            var changeLogSectionEntries = ExtractChangelogSectionNotes(ChangeLogFile);
            var latestChangeLog = changeLogSectionEntries
                .Aggregate((c, n) => c + Environment.NewLine + n);
            var completeChangeLog = $"## {releaseTag}" + Environment.NewLine + latestChangeLog;

            var repositoryInfo = GetGitHubRepositoryInfo(GitRepository);

            await PublishRelease(x => x
                .SetArtifactPaths(GlobFiles(OutputDirectory, "*.nupkg").NotEmpty().ToArray())
                .SetCommitSha(GitVersion.Sha)
                .SetReleaseNotes(completeChangeLog)
                .SetRepositoryName(repositoryInfo.repositoryName)
                .SetRepositoryOwner(repositoryInfo.gitHubOwner)
                .SetTag(releaseTag)
                .SetToken(GitHubAuthenticationToken)
            );
        });
