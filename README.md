# Nuke.GitHub & Nuke WebDocu

[![Build Status](https://jenkins.dangl.me/buildStatus/icon?job=GeorgDangl%2FNuke.GitHub%2Fdevelop)](https://jenkins.dangl.me/job/GeorgDangl/job/Nuke.GitHub/job/develop/)

**Nuke.GitHub**  
![NuGet](https://img.shields.io/nuget/v/Nuke.GitHub.svg)
[![MyGet](https://img.shields.io/myget/dangl/v/Nuke.GitHub.svg)]()  
**Nuke.WebDocu**  
![NuGet](https://img.shields.io/nuget/v/Nuke.WebDocu.svg)
[![MyGet](https://img.shields.io/myget/dangl/v/Nuke.WebDocu.svg)]()

> This repository contains both **Nuke.GitHub** and **Nuke.WebDocu**.

# Nuke.GitHub

This plugin provides some methods to work with GitHub repositories
in [NUKE Build](https://github.com/nuke-build/nuke).

Currently supported:
  * **PublishRelease** to create GitHub releases.
  * **CreatePullRequest**
  * **GetReleases**
  * **GetRepository**

[Link to documentation](https://docs.dangl-it.com/Projects/Nuke.GitHub).

[Changelog](./Changelog_WebDocu.md)

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
            // You'll create a tag for the release, e.g. v1.0.0
            var releaseTag = $"v{GitVersion.MajorMinorPatch}";

            // We also want to fill the changelog with the latest release notes,
            // so we're just reading from a markdown file containing the changelog.
            // Not providing the second, optional parameter gives the latest section
            var changeLogSectionEntries = Nuke.Common.ChangeLog.ExtractChangelogSectionNotes(ChangeLogFile);
            var latestChangeLog = changeLogSectionEntries
                .Aggregate((c, n) => c + Environment.NewLine + n);
            var completeChangeLog = $"## {releaseTag}" + Environment.NewLine + latestChangeLog;

            var repositoryInfo = GetGitHubRepositoryInfo(GitRepository);

            await PublishRelease(x => x
                // We can optionally upload artifacts to the release, for example by finding
                // all nupkg files in the output directory
                .SetArtifactPaths(GlobFiles(OutputDirectory, "*.nupkg").NotEmpty().ToArray())
                .SetCommitSha(GitVersion.Sha)
                .SetReleaseNotes(completeChangeLog)
                .SetRepositoryName(repositoryInfo.repositoryName)
                .SetRepositoryOwner(repositoryInfo.gitHubOwner)
                .SetTag(releaseTag)
                .SetToken(GitHubAuthenticationToken)
            );
        });

# Nuke.WebDocu

This plugin provides a task to upload documentation packages to [WebDocu sites](https://github.com/GeorgDangl/WebDocu).
It's written for the [NUKE Build](https://github.com/nuke-build/nuke) system.

[Link to documentation](https://docs.dangl-it.com/Projects/Nuke.WebDocu).

[Changelog](./Changelog_WebDocu.md)

## CI Builds

All builds are available on MyGet:

    https://www.myget.org/F/dangl/api/v2
    https://www.myget.org/F/dangl/api/v3/index.json

## Example

When publishing to WebDocu, you have to include the version of the docs.

### Getting the Version from Generated NuGet Packages

```
Target UploadDocumentation => _ => _
    .DependsOn(BuildDocumentation)
    .Requires(() => DocuApiKey)
    .Requires(() => DocuBaseUrl)
    .Executes(() =>
    {
        WebDocuTasks.WebDocu(s =>
        {
            var packageVersion = GlobFiles(OutputDirectory, "*.nupkg").NotEmpty()
                .Where(x => !x.EndsWith("symbols.nupkg"))
                .Select(Path.GetFileName)
                .Select(x => WebDocuTasks.GetVersionFromNuGetPackageFilename(x, "Nuke.WebDeploy"))
                .First();

            return s.SetDocuBaseUrl(DocuBaseUrl)
                    .SetDocuApiKey(DocuApiKey)
                    .SetSourceDirectory(OutputDirectory / "docs")
                    .SetVersion(packageVersion);
        });
    });
```

### Getting the Version from GitVersion

```
Target UploadDocumentation => _ => _
    .DependsOn(Push) // To have a relation between pushed package version and published docs version
    .DependsOn(BuildDocumentation)
    .Requires(() => DocuApiKey)
    .Requires(() => DocuApiEndpoint)
    .Executes(() =>
    {
        WebDocuTasks.WebDocu(s => s.SetDocuApiEndpoint(DocuApiEndpoint)
            .SetDocuApiKey(DocuApiKey)
            .SetSourceDirectory(OutputDirectory / "docs")
            .SetVersion(GitVersion.NuGetVersion));
    });
```

The `DocuApiEndpoint` should look like this:

    https://docs.dangl-it.com/API/Projects/Upload

## License

[This project is available under the MIT license.](LICENSE.md)