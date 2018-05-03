// Copyright Matthias Koch, Sebastian Karasek 2018.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

// Generated with Nuke.CodeGeneration, Version: 0.4.0 [CommitSha: c494ebb7].

using JetBrains.Annotations;
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
    public partial class GitHubReleaseSettings : ToolSettings
    {
        /// <summary><p>Optional file paths for files that should be appended to a release</p></summary>
        public virtual string[] ArtifactPaths { get; internal set; }
        /// <summary><p>The message for the GitHub release</p></summary>
        public virtual string ReleaseNotes { get; internal set; }
        /// <summary><p>The tag that should be used for the release, e.g. "v1.0.0"</p></summary>
        public virtual string Tag { get; internal set; }
        /// <summary><p>The Token for the GitHub API</p></summary>
        public virtual string Token { get; internal set; }
        /// <summary><p>The commit SHA on which to create the release</p></summary>
        public virtual string CommitSha { get; internal set; }
        /// <summary><p>The account under which the repository is hosted</p></summary>
        public virtual string RepositoryOwner { get; internal set; }
        /// <summary><p>The name of the repository</p></summary>
        public virtual string RepositoryName { get; internal set; }
        protected override void AssertValid()
        {
            base.AssertValid();
            ControlFlow.Assert(Tag != null, "Tag != null");
            ControlFlow.Assert(Token != null, "Token != null");
            ControlFlow.Assert(CommitSha != null, "CommitSha != null");
            ControlFlow.Assert(RepositoryOwner != null, "RepositoryOwner != null");
            ControlFlow.Assert(RepositoryName != null, "RepositoryName != null");
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
        #region Token
        /// <summary><p><em>Sets <see cref="GitHubReleaseSettings.Token"/>.</em></p><p>The Token for the GitHub API</p></summary>
        [Pure]
        public static GitHubReleaseSettings SetToken(this GitHubReleaseSettings toolSettings, string token)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.Token = token;
            return toolSettings;
        }
        /// <summary><p><em>Resets <see cref="GitHubReleaseSettings.Token"/>.</em></p><p>The Token for the GitHub API</p></summary>
        [Pure]
        public static GitHubReleaseSettings ResetToken(this GitHubReleaseSettings toolSettings)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.Token = null;
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
        #region RepositoryOwner
        /// <summary><p><em>Sets <see cref="GitHubReleaseSettings.RepositoryOwner"/>.</em></p><p>The account under which the repository is hosted</p></summary>
        [Pure]
        public static GitHubReleaseSettings SetRepositoryOwner(this GitHubReleaseSettings toolSettings, string repositoryOwner)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.RepositoryOwner = repositoryOwner;
            return toolSettings;
        }
        /// <summary><p><em>Resets <see cref="GitHubReleaseSettings.RepositoryOwner"/>.</em></p><p>The account under which the repository is hosted</p></summary>
        [Pure]
        public static GitHubReleaseSettings ResetRepositoryOwner(this GitHubReleaseSettings toolSettings)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.RepositoryOwner = null;
            return toolSettings;
        }
        #endregion
        #region RepositoryName
        /// <summary><p><em>Sets <see cref="GitHubReleaseSettings.RepositoryName"/>.</em></p><p>The name of the repository</p></summary>
        [Pure]
        public static GitHubReleaseSettings SetRepositoryName(this GitHubReleaseSettings toolSettings, string repositoryName)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.RepositoryName = repositoryName;
            return toolSettings;
        }
        /// <summary><p><em>Resets <see cref="GitHubReleaseSettings.RepositoryName"/>.</em></p><p>The name of the repository</p></summary>
        [Pure]
        public static GitHubReleaseSettings ResetRepositoryName(this GitHubReleaseSettings toolSettings)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.RepositoryName = null;
            return toolSettings;
        }
        #endregion
    }
    #endregion
}
