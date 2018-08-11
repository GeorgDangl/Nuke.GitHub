// Copyright Matthias Koch, Sebastian Karasek 2018.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

// Generated with Nuke.CodeGeneration, Version: 0.6.0 [CommitSha: 5a428f0d].

using JetBrains.Annotations;
using Newtonsoft.Json;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Tooling;
using Nuke.Common.Tools;
using Nuke.Common.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;

namespace Nuke.GitHub
{
    #region GitHubReleaseSettings
    /// <summary><p>Used within <see cref="GitHubTasks"/>.</p></summary>
    [PublicAPI]
    [ExcludeFromCodeCoverage]
    [Serializable]
    public partial class GitHubReleaseSettings : GitHubSettings
    {
        /// <summary><p>Optional file paths for files that should be appended to a release</p></summary>
        public virtual string[] ArtifactPaths { get; internal set; }
        /// <summary><p>The message for the GitHub release</p></summary>
        public virtual string ReleaseNotes { get; internal set; }
        /// <summary><p>The tag that should be used for the release, e.g. "v1.0.0"</p></summary>
        public virtual string Tag { get; internal set; }
        /// <summary><p>The commit SHA on which to create the release</p></summary>
        public virtual string CommitSha { get; internal set; }
        /// <summary><p>Whether this is a pre-release</p></summary>
        public virtual bool? Prerelease { get; internal set; } = false;
        protected override void AssertValid()
        {
            base.AssertValid();
            ControlFlow.Assert(Tag != null, "Tag != null");
            ControlFlow.Assert(CommitSha != null, "CommitSha != null");
        }
    }
    #endregion
    #region GitHubPullRequestSettings
    /// <summary><p>Used within <see cref="GitHubTasks"/>.</p></summary>
    [PublicAPI]
    [ExcludeFromCodeCoverage]
    [Serializable]
    public partial class GitHubPullRequestSettings : GitHubSettings
    {
        /// <summary><p>The name of the branch you want the changes pulled into</p></summary>
        public virtual string Base { get; internal set; }
        /// <summary><p>The name of the branch where your changes are implemented</p></summary>
        public virtual string Head { get; internal set; }
        /// <summary><p>The title of the pull request</p></summary>
        public virtual string Title { get; internal set; }
        /// <summary><p>The optional contents of the pull request</p></summary>
        public virtual string Body { get; internal set; }
        protected override void AssertValid()
        {
            base.AssertValid();
            ControlFlow.Assert(Base != null, "Base != null");
            ControlFlow.Assert(Head != null, "Head != null");
            ControlFlow.Assert(Title != null, "Title != null");
        }
    }
    #endregion
    #region GitHubSettings
    /// <summary><p>Used within <see cref="GitHubTasks"/>.</p></summary>
    [PublicAPI]
    [ExcludeFromCodeCoverage]
    [Serializable]
    public partial class GitHubSettings : ToolSettings
    {
        /// <summary><p>The account under which the repository is hosted</p></summary>
        public virtual string RepositoryOwner { get; internal set; }
        /// <summary><p>The name of the repository</p></summary>
        public virtual string RepositoryName { get; internal set; }
        /// <summary><p>The Token for the GitHub API</p></summary>
        public virtual string Token { get; internal set; }
        protected override void AssertValid()
        {
            base.AssertValid();
            ControlFlow.Assert(RepositoryOwner != null, "RepositoryOwner != null");
            ControlFlow.Assert(RepositoryName != null, "RepositoryName != null");
            ControlFlow.Assert(Token != null, "Token != null");
        }
    }
    #endregion
    #region GitHubReleaseSettingsExtensions
    /// <summary><p>Used within <see cref="GitHubTasks"/>.</p></summary>
    [PublicAPI]
    [ExcludeFromCodeCoverage]
    public static partial class GitHubReleaseSettingsExtensions
    {
        #region ArtifactPaths
        /// <summary><p><em>Sets <see cref="GitHubReleaseSettings.ArtifactPaths"/>.</em></p><p>Optional file paths for files that should be appended to a release</p></summary>
        [Pure]
        public static GitHubReleaseSettings SetArtifactPaths(this GitHubReleaseSettings toolSettings, string[] artifactPaths)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.ArtifactPaths = artifactPaths;
            return toolSettings;
        }
        /// <summary><p><em>Resets <see cref="GitHubReleaseSettings.ArtifactPaths"/>.</em></p><p>Optional file paths for files that should be appended to a release</p></summary>
        [Pure]
        public static GitHubReleaseSettings ResetArtifactPaths(this GitHubReleaseSettings toolSettings)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.ArtifactPaths = null;
            return toolSettings;
        }
        #endregion
        #region ReleaseNotes
        /// <summary><p><em>Sets <see cref="GitHubReleaseSettings.ReleaseNotes"/>.</em></p><p>The message for the GitHub release</p></summary>
        [Pure]
        public static GitHubReleaseSettings SetReleaseNotes(this GitHubReleaseSettings toolSettings, string releaseNotes)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.ReleaseNotes = releaseNotes;
            return toolSettings;
        }
        /// <summary><p><em>Resets <see cref="GitHubReleaseSettings.ReleaseNotes"/>.</em></p><p>The message for the GitHub release</p></summary>
        [Pure]
        public static GitHubReleaseSettings ResetReleaseNotes(this GitHubReleaseSettings toolSettings)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.ReleaseNotes = null;
            return toolSettings;
        }
        #endregion
        #region Tag
        /// <summary><p><em>Sets <see cref="GitHubReleaseSettings.Tag"/>.</em></p><p>The tag that should be used for the release, e.g. "v1.0.0"</p></summary>
        [Pure]
        public static GitHubReleaseSettings SetTag(this GitHubReleaseSettings toolSettings, string tag)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.Tag = tag;
            return toolSettings;
        }
        /// <summary><p><em>Resets <see cref="GitHubReleaseSettings.Tag"/>.</em></p><p>The tag that should be used for the release, e.g. "v1.0.0"</p></summary>
        [Pure]
        public static GitHubReleaseSettings ResetTag(this GitHubReleaseSettings toolSettings)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.Tag = null;
            return toolSettings;
        }
        #endregion
        #region CommitSha
        /// <summary><p><em>Sets <see cref="GitHubReleaseSettings.CommitSha"/>.</em></p><p>The commit SHA on which to create the release</p></summary>
        [Pure]
        public static GitHubReleaseSettings SetCommitSha(this GitHubReleaseSettings toolSettings, string commitSha)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.CommitSha = commitSha;
            return toolSettings;
        }
        /// <summary><p><em>Resets <see cref="GitHubReleaseSettings.CommitSha"/>.</em></p><p>The commit SHA on which to create the release</p></summary>
        [Pure]
        public static GitHubReleaseSettings ResetCommitSha(this GitHubReleaseSettings toolSettings)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.CommitSha = null;
            return toolSettings;
        }
        #endregion
        #region Prerelease
        /// <summary><p><em>Sets <see cref="GitHubReleaseSettings.Prerelease"/>.</em></p><p>Whether this is a pre-release</p></summary>
        [Pure]
        public static GitHubReleaseSettings SetPrerelease(this GitHubReleaseSettings toolSettings, bool? prerelease)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.Prerelease = prerelease;
            return toolSettings;
        }
        /// <summary><p><em>Resets <see cref="GitHubReleaseSettings.Prerelease"/>.</em></p><p>Whether this is a pre-release</p></summary>
        [Pure]
        public static GitHubReleaseSettings ResetPrerelease(this GitHubReleaseSettings toolSettings)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.Prerelease = null;
            return toolSettings;
        }
        /// <summary><p><em>Enables <see cref="GitHubReleaseSettings.Prerelease"/>.</em></p><p>Whether this is a pre-release</p></summary>
        [Pure]
        public static GitHubReleaseSettings EnablePrerelease(this GitHubReleaseSettings toolSettings)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.Prerelease = true;
            return toolSettings;
        }
        /// <summary><p><em>Disables <see cref="GitHubReleaseSettings.Prerelease"/>.</em></p><p>Whether this is a pre-release</p></summary>
        [Pure]
        public static GitHubReleaseSettings DisablePrerelease(this GitHubReleaseSettings toolSettings)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.Prerelease = false;
            return toolSettings;
        }
        /// <summary><p><em>Toggles <see cref="GitHubReleaseSettings.Prerelease"/>.</em></p><p>Whether this is a pre-release</p></summary>
        [Pure]
        public static GitHubReleaseSettings TogglePrerelease(this GitHubReleaseSettings toolSettings)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.Prerelease = !toolSettings.Prerelease;
            return toolSettings;
        }
        #endregion
    }
    #endregion
    #region GitHubPullRequestSettingsExtensions
    /// <summary><p>Used within <see cref="GitHubTasks"/>.</p></summary>
    [PublicAPI]
    [ExcludeFromCodeCoverage]
    public static partial class GitHubPullRequestSettingsExtensions
    {
        #region Base
        /// <summary><p><em>Sets <see cref="GitHubPullRequestSettings.Base"/>.</em></p><p>The name of the branch you want the changes pulled into</p></summary>
        [Pure]
        public static GitHubPullRequestSettings SetBase(this GitHubPullRequestSettings toolSettings, string @base)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.Base = @base;
            return toolSettings;
        }
        /// <summary><p><em>Resets <see cref="GitHubPullRequestSettings.Base"/>.</em></p><p>The name of the branch you want the changes pulled into</p></summary>
        [Pure]
        public static GitHubPullRequestSettings ResetBase(this GitHubPullRequestSettings toolSettings)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.Base = null;
            return toolSettings;
        }
        #endregion
        #region Head
        /// <summary><p><em>Sets <see cref="GitHubPullRequestSettings.Head"/>.</em></p><p>The name of the branch where your changes are implemented</p></summary>
        [Pure]
        public static GitHubPullRequestSettings SetHead(this GitHubPullRequestSettings toolSettings, string head)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.Head = head;
            return toolSettings;
        }
        /// <summary><p><em>Resets <see cref="GitHubPullRequestSettings.Head"/>.</em></p><p>The name of the branch where your changes are implemented</p></summary>
        [Pure]
        public static GitHubPullRequestSettings ResetHead(this GitHubPullRequestSettings toolSettings)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.Head = null;
            return toolSettings;
        }
        #endregion
        #region Title
        /// <summary><p><em>Sets <see cref="GitHubPullRequestSettings.Title"/>.</em></p><p>The title of the pull request</p></summary>
        [Pure]
        public static GitHubPullRequestSettings SetTitle(this GitHubPullRequestSettings toolSettings, string title)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.Title = title;
            return toolSettings;
        }
        /// <summary><p><em>Resets <see cref="GitHubPullRequestSettings.Title"/>.</em></p><p>The title of the pull request</p></summary>
        [Pure]
        public static GitHubPullRequestSettings ResetTitle(this GitHubPullRequestSettings toolSettings)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.Title = null;
            return toolSettings;
        }
        #endregion
        #region Body
        /// <summary><p><em>Sets <see cref="GitHubPullRequestSettings.Body"/>.</em></p><p>The optional contents of the pull request</p></summary>
        [Pure]
        public static GitHubPullRequestSettings SetBody(this GitHubPullRequestSettings toolSettings, string body)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.Body = body;
            return toolSettings;
        }
        /// <summary><p><em>Resets <see cref="GitHubPullRequestSettings.Body"/>.</em></p><p>The optional contents of the pull request</p></summary>
        [Pure]
        public static GitHubPullRequestSettings ResetBody(this GitHubPullRequestSettings toolSettings)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.Body = null;
            return toolSettings;
        }
        #endregion
    }
    #endregion
    #region GitHubSettingsExtensions
    /// <summary><p>Used within <see cref="GitHubTasks"/>.</p></summary>
    [PublicAPI]
    [ExcludeFromCodeCoverage]
    public static partial class GitHubSettingsExtensions
    {
        #region RepositoryOwner
        /// <summary><p><em>Sets <see cref="GitHubSettings.RepositoryOwner"/>.</em></p><p>The account under which the repository is hosted</p></summary>
        [Pure]
        public static GitHubSettings SetRepositoryOwner(this GitHubSettings toolSettings, string repositoryOwner)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.RepositoryOwner = repositoryOwner;
            return toolSettings;
        }
        /// <summary><p><em>Resets <see cref="GitHubSettings.RepositoryOwner"/>.</em></p><p>The account under which the repository is hosted</p></summary>
        [Pure]
        public static GitHubSettings ResetRepositoryOwner(this GitHubSettings toolSettings)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.RepositoryOwner = null;
            return toolSettings;
        }
        #endregion
        #region RepositoryName
        /// <summary><p><em>Sets <see cref="GitHubSettings.RepositoryName"/>.</em></p><p>The name of the repository</p></summary>
        [Pure]
        public static GitHubSettings SetRepositoryName(this GitHubSettings toolSettings, string repositoryName)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.RepositoryName = repositoryName;
            return toolSettings;
        }
        /// <summary><p><em>Resets <see cref="GitHubSettings.RepositoryName"/>.</em></p><p>The name of the repository</p></summary>
        [Pure]
        public static GitHubSettings ResetRepositoryName(this GitHubSettings toolSettings)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.RepositoryName = null;
            return toolSettings;
        }
        #endregion
        #region Token
        /// <summary><p><em>Sets <see cref="GitHubSettings.Token"/>.</em></p><p>The Token for the GitHub API</p></summary>
        [Pure]
        public static GitHubSettings SetToken(this GitHubSettings toolSettings, string token)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.Token = token;
            return toolSettings;
        }
        /// <summary><p><em>Resets <see cref="GitHubSettings.Token"/>.</em></p><p>The Token for the GitHub API</p></summary>
        [Pure]
        public static GitHubSettings ResetToken(this GitHubSettings toolSettings)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.Token = null;
            return toolSettings;
        }
        #endregion
    }
    #endregion
}
