using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.AzureKeyVault;
using Nuke.Common.Tools.DocFX;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.Teams;
using Nuke.Common.Utilities.Collections;
using Nuke.GitHub;
using Nuke.WebDocu;
using System;
using System.IO;
using System.Linq;
using static Nuke.CodeGeneration.CodeGenerator;
using static Nuke.Common.ChangeLog.ChangelogTasks;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.Globbing;
using static Nuke.Common.Tools.DocFX.DocFXTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.GitHub.ChangeLogExtensions;
using static Nuke.GitHub.GitHubTasks;
using static Nuke.WebDocu.WebDocuTasks;

class Build : NukeBuild
{
    // Console application entry. Also defines the default target.
    public static int Main() => Execute<Build>(x => x.Compile);

    [AzureKeyVaultConfiguration(
        BaseUrlParameterName = nameof(KeyVaultBaseUrl),
        ClientIdParameterName = nameof(KeyVaultClientId),
        ClientSecretParameterName = nameof(KeyVaultClientSecret))]
    readonly AzureKeyVaultConfiguration KeyVaultSettings;

    [Parameter] string KeyVaultBaseUrl;
    [Parameter] string KeyVaultClientId;
    [Parameter] string KeyVaultClientSecret;
    [GitVersion] readonly GitVersion GitVersion;
    [GitRepository] readonly GitRepository GitRepository;

    [Parameter] readonly string Configuration = IsLocalBuild ? "Debug" : "Release";

    [AzureKeyVaultSecret] string DocuBaseUrl;
    [AzureKeyVaultSecret] string GitHubAuthenticationToken;
    [AzureKeyVaultSecret] string PublicMyGetSource;
    [AzureKeyVaultSecret] string PublicMyGetApiKey;
    [AzureKeyVaultSecret("NukeGitHub-DocuApiKey")] string NukeGitHubDocuApiKey;
    [AzureKeyVaultSecret("NukeWebDocu-DocuApiKey")] string NukeWebDocuDocuApiKey;
    [AzureKeyVaultSecret] string NuGetApiKey;
    [AzureKeyVaultSecret] readonly string DanglCiCdTeamsWebhookUrl;

    [Solution("Nuke.GitHub.sln")] readonly Solution Solution;
    AbsolutePath SolutionDirectory => Solution.Directory;
    AbsolutePath OutputDirectory => SolutionDirectory / "output";
    AbsolutePath SourceDirectory => SolutionDirectory / "src";

    string NukeGitHubDocFxFile => SolutionDirectory / "docs" / "GitHub" / "docfx_GitHub.json";
    string NukeWebDocuDocFxFile => SolutionDirectory / "docs" / "WebDocu" / "docfx_WebDocu.json";

    string NukeGitHubChangeLogFile => RootDirectory / "CHANGELOG_GitHub.md";
    string NukeWebDocuChangeLogFile => RootDirectory / "CHANGELOG_WebDocu.md";

    protected override void OnTargetFailed(string target)
    {
        if (IsServerBuild)
        {
            SendTeamsMessage("Build Failed", $"Target {target} failed for Nuke.GitHub & Nuke.WebDocu, " +
                        $"Branch: {GitRepository.Branch}", true);
        }
    }

    private void SendTeamsMessage(string title, string message, bool isError)
    {
        if (!string.IsNullOrWhiteSpace(DanglCiCdTeamsWebhookUrl))
        {
            var themeColor = isError ? "f44336" : "00acc1";
            TeamsTasks
                .SendTeamsMessage(m => m
                    .SetTitle(title)
                    .SetText(message)
                    .SetThemeColor(themeColor),
                    DanglCiCdTeamsWebhookUrl);
        }
    }

    Target Clean => _ => _
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(d => d.DeleteDirectory());
            OutputDirectory.CreateOrCleanDirectory();
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore();
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(x => x
                .SetConfiguration(Configuration)
                .EnableNoRestore()
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetInformationalVersion(GitVersion.InformationalVersion));
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(x => x
                .SetNoBuild(true)
                .SetProjectFile(RootDirectory / "test" / "Nuke.WebDocu.Tests")
                .SetTestAdapterPath(".")
                .CombineWith(c => new[] { "net5.0" }
                    .Select(framework => c.SetFramework(framework).SetLoggers($"xunit;LogFilePath={OutputDirectory / $"tests-{framework}.xml"}"))
                ), degreeOfParallelism: Environment.ProcessorCount, completeOnFailure: true);
        });

    Target Pack => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            var packData = new[]
            {
                new
                {
                    Changelog = NukeGitHubChangeLogFile,
                    Title = "GitHub for NUKE Build - www.dangl-it.com",
                    Project = SourceDirectory / "Nuke.GitHub"
                },
                new
                {
                    Changelog = NukeWebDocuChangeLogFile,
                    Title = "WebDocu for NUKE Build - www.dangl-it.com",
                    Project = SourceDirectory / "Nuke.WebDocu"
                }
            };

            foreach (var packInfo in packData)
            {
                var changeLog = GetCompleteChangeLog(packInfo.Changelog)
                    .EscapeStringPropertyForMsBuild();

                DotNetPack(x => x
                    .SetConfiguration(Configuration)
                    .SetProject(packInfo.Project)
                    .SetPackageReleaseNotes(changeLog)
                    .SetTitle(packInfo.Title)
                    .EnableNoBuild()
                    .SetOutputDirectory(OutputDirectory)
                    .SetVersion(GitVersion.NuGetVersion));
            }
        });

    Target Push => _ => _
        .DependsOn(Pack)
        .Requires(() => Configuration == "Release")
        .Executes(() =>
        {
            if (string.IsNullOrWhiteSpace(PublicMyGetSource))
            {
                Assert.Fail(nameof(PublicMyGetSource) + " is required");
            }

            if (string.IsNullOrWhiteSpace(PublicMyGetApiKey))
            {
                Assert.Fail(nameof(PublicMyGetApiKey) + " is required");
            }

            var packages = GlobFiles(OutputDirectory, "*.nupkg")
                .Where(x => !x.EndsWith("symbols.nupkg"))
                .ToList();
            Assert.NotEmpty(packages);
            packages.ForEach(x =>
            {
                DotNetNuGetPush(s => s
                    .EnableSkipDuplicate()
                    .SetTargetPath(x)
                    .SetSource(PublicMyGetSource)
                    .SetApiKey(PublicMyGetApiKey));
            });

            if (GitVersion.BranchName.Equals("master") || GitVersion.BranchName.Equals("origin/master"))
            {
                // Stable releases are published to NuGet
                packages.ForEach(x => DotNetNuGetPush(s => s
                    .SetTargetPath(x)
                    .SetSource("https://api.nuget.org/v3/index.json")
                    .SetApiKey(NuGetApiKey)));

                SendTeamsMessage("New Release", $"New release available for Nuke.GitHub & Nuke.WebDocu: {GitVersion.NuGetVersion}", false);
            }
        });

    Target BuildDocFxMetadata => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DocFXMetadata(x => x.AddProjects(NukeGitHubDocFxFile));
            DocFXMetadata(x => x.AddProjects(NukeWebDocuDocFxFile));
        });

    Target BuildDocumentation => _ => _
        .DependsOn(Clean)
        .DependsOn(BuildDocFxMetadata)
        .Executes(() =>
        {
            var configs = new[]
            {
                NukeGitHubDocFxFile,
                NukeWebDocuDocFxFile
            };

            foreach (var config in configs)
            {
                var docsPath = (AbsolutePath)Path.GetDirectoryName(config);
                Serilog.Log.Information(docsPath);

                // Using README.md as index.md
                if (File.Exists(docsPath / "index.md"))
                {
                    File.Delete(docsPath / "index.md");
                }

                File.Copy(SolutionDirectory / "README.md", docsPath / "index.md", overwrite: true);
                File.Copy(SolutionDirectory / "app-logo.png", docsPath / "app-logo.png", overwrite: true);
                File.Copy(NukeGitHubChangeLogFile, docsPath / "CHANGELOG_GitHub.md", overwrite: true);
                File.Copy(NukeWebDocuChangeLogFile, docsPath / "CHANGELOG_WebDocu.md", overwrite: true);

                DocFXBuild(x => x.SetConfigFile(config));

                File.Delete(docsPath / "index.md");
                File.Delete(docsPath / "app-logo.png");
                File.Delete(docsPath / "CHANGELOG_GitHub.md");
                File.Delete(docsPath / "CHANGELOG_WebDocu.md");
                Directory.Delete(docsPath / "api", true);
                Directory.Delete(docsPath / "obj", true);
            }
        });

    Target UploadDocumentation => _ => _
        .DependsOn(Push) // To have a relation between pushed package version and published docs version
        .DependsOn(BuildDocumentation)
        .Executes(() =>
        {
            if (string.IsNullOrWhiteSpace(DocuBaseUrl))
            {
                Assert.Fail(nameof(DocuBaseUrl) + " is required");
            }

            if (string.IsNullOrWhiteSpace(NukeGitHubDocuApiKey))
            {
                Assert.Fail(nameof(NukeGitHubDocuApiKey) + " is required");
            }

            if (string.IsNullOrWhiteSpace(NukeWebDocuDocuApiKey))
            {
                Assert.Fail(nameof(NukeWebDocuDocuApiKey) + " is required");
            }

            WebDocu(s => s
                .SetSkipForVersionConflicts(true)
                .SetDocuBaseUrl(DocuBaseUrl)
                .SetDocuApiKey(NukeGitHubDocuApiKey)
                .SetSourceDirectory(OutputDirectory / "docs" / "github")
                .SetVersion(GitVersion.NuGetVersion));

            WebDocu(s => s
                .SetSkipForVersionConflicts(true)
                .SetDocuBaseUrl(DocuBaseUrl)
                .SetDocuApiKey(NukeWebDocuDocuApiKey)
                .SetSourceDirectory(OutputDirectory / "docs" / "webdocu")
                .SetVersion(GitVersion.NuGetVersion));
        });

    Target PublishGitHubRelease => _ => _
        .DependsOn(Pack)
        .OnlyWhenDynamic(() => GitVersion.BranchName.Equals("master") || GitVersion.BranchName.Equals("origin/master"))
        .Executes(async () =>
        {
            if (string.IsNullOrWhiteSpace(GitHubAuthenticationToken))
            {
                Assert.Fail(nameof(GitHubAuthenticationToken) + " is required");
            }

            var releaseTag = $"v{GitVersion.MajorMinorPatch}";

            var gitHubChangeLogSectionEntries = ExtractChangelogSectionNotes(NukeGitHubChangeLogFile)
                .Aggregate((c, n) => c + Environment.NewLine + n);
            var webDocuChangeLogSectionEntries = ExtractChangelogSectionNotes(NukeWebDocuChangeLogFile)
                .Aggregate((c, n) => c + Environment.NewLine + n);
            var completeChangeLog = $"## {releaseTag}" + Environment.NewLine;
            completeChangeLog += "## Nuke.GitHub" + Environment.NewLine + gitHubChangeLogSectionEntries + Environment.NewLine;
            completeChangeLog += "## Nuke.WebDocu" + Environment.NewLine + webDocuChangeLogSectionEntries;

            var repositoryInfo = GetGitHubRepositoryInfo(GitRepository);

            var packages = GlobFiles(OutputDirectory, "*.nupkg").ToArray();
            Assert.NotEmpty(packages);

            await PublishRelease(x => x
                    .SetArtifactPaths(packages)
                    .SetCommitSha(GitVersion.Sha)
                    .SetReleaseNotes(completeChangeLog)
                    .SetRepositoryName(repositoryInfo.repositoryName)
                    .SetRepositoryOwner(repositoryInfo.gitHubOwner)
                    .SetTag(releaseTag)
                    .SetToken(GitHubAuthenticationToken)
                );
        });

    Target Generate => _ => _
        .Executes(() =>
        {
            GenerateCodeFromDirectory(RootDirectory / "src" / "Nuke.GitHub" / "MetaData",
                outputFileProvider: x => RootDirectory / "src" / "Nuke.GitHub" / "GitHubTasks.Generated.cs",
                namespaceProvider: x => "Nuke.GitHub");
        });
}
