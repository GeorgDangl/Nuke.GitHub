using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.Tools.DocFx;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Core;
using Nuke.Core.Utilities;
using Nuke.Core.Utilities.Collections;
using Nuke.GitHub;
using Nuke.WebDocu;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Core.IO.FileSystemTasks;
using static Nuke.Core.IO.PathConstruction;
using static Nuke.Core.EnvironmentInfo;
using static Nuke.CodeGeneration.CodeGenerator;
using static Nuke.Common.Tools.DocFx.DocFxTasks;
using static Nuke.WebDocu.WebDocuTasks;
using static Nuke.GitHub.GitHubTasks;

class Build : NukeBuild
{
    // Console application entry. Also defines the default target.
    public static int Main() => Execute<Build>(x => x.Compile);

    // Auto-injection fields:

    [GitVersion] readonly GitVersion GitVersion;
    // Semantic versioning. Must have 'GitVersion.CommandLine' referenced.

    [GitRepository] readonly GitRepository GitRepository;
    // Parses origin, branch name and head from git config.

    [Parameter] string MyGetSource;
    [Parameter] string MyGetApiKey;
    [Parameter] string DocuApiKey;
    [Parameter] string DocuApiEndpoint;
    [Parameter] string GitHubAuthenticationToken;

    string DocFxFile => SolutionDirectory / "docfx.json";
    string ChangeLogFile => RootDirectory / "CHANGELOG.md";

    Target Clean => _ => _
        .Executes(() =>
        {
            DeleteDirectories(GlobDirectories(SourceDirectory, "**/bin", "**/obj"));
            EnsureCleanDirectory(OutputDirectory);
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(s => DefaultDotNetRestore);
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => DefaultDotNetBuild
                .SetFileVersion(GitVersion.AssemblySemVer));
        });

    Target Pack => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            var changeLog = GetCompleteChangeLog(ChangeLogFile, escapeMsBuildProperty: true);
            DotNetPack(s => DefaultDotNetPack
                .SetPackageReleaseNotes(changeLog));
        });

    Target Push => _ => _
        .DependsOn(Pack)
        .Requires(() => MyGetSource)
        .Requires(() => MyGetApiKey)
        .Requires(() => Configuration.EqualsOrdinalIgnoreCase("Release"))
        .Executes(() =>
        {
            GlobFiles(OutputDirectory, "*.nupkg").NotEmpty()
                .Where(x => !x.EndsWith("symbols.nupkg"))
                .ForEach(x =>
                {
                    DotNetNuGetPush(s => s
                        .SetTargetPath(x)
                        .SetSource(MyGetSource)
                        .SetApiKey(MyGetApiKey));
                });
        });

    Target BuildDocFxMetadata => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            if (IsLocalBuild)
            {
                SetVariable("VSINSTALLDIR", @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional");
                SetVariable("VisualStudioVersion", "15.0");
            }

            DocFxMetadata(DocFxFile, s => s.SetLogLevel(DocFxLogLevel.Verbose));
        });

    Target BuildDocumentation => _ => _
        .DependsOn(Clean)
        .DependsOn(BuildDocFxMetadata)
        .Executes(() =>
        {
            // Using README.md as index.md
            if (File.Exists(SolutionDirectory / "index.md"))
            {
                File.Delete(SolutionDirectory / "index.md");
            }

            File.Copy(SolutionDirectory / "README.md", SolutionDirectory / "index.md");

            DocFxBuild(DocFxFile, s => s
                .ClearXRefMaps()
                .SetLogLevel(DocFxLogLevel.Verbose));

            File.Delete(SolutionDirectory / "index.md");
            Directory.Delete(SolutionDirectory / "api", true);
            Directory.Delete(SolutionDirectory / "obj", true);
        });

    Target UploadDocumentation => _ => _
        .DependsOn(Push) // To have a relation between pushed package version and published docs version
        .DependsOn(BuildDocumentation)
        .Requires(() => DocuApiKey)
        .Requires(() => DocuApiEndpoint)
        .Executes(() =>
        {
            WebDocu(s => s
                .SetDocuApiEndpoint(DocuApiEndpoint)
                .SetDocuApiKey(DocuApiKey)
                .SetSourceDirectory(OutputDirectory / "docs")
                .SetVersion(GitVersion.NuGetVersion)
            );
        });

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

    Target Generate => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            GenerateCode(
                metadataDirectory: RootDirectory / "src" / "Nuke.GitHub" / "MetaData",
                generationBaseDirectory: RootDirectory / "src" / "Nuke.GitHub",
                baseNamespace: "Nuke.GitHub"
            );
        });
}