using System;
using System.Linq;
using JetBrains.Annotations;
using Nuke.Common.Tooling;

namespace Nuke.GitHub
{
    /// <summary><p>Used within <see cref="GitHubTasks"/>.</p></summary>
    public static partial class GitHubSettingsExtensions
    {
        #region RepositoryOwner
        /// <summary><p><em>Sets <see cref="GitHubSettings.RepositoryOwner"/>.</em></p><p>The account under which the repository is hosted</p></summary>
        [Pure]
        public static T SetRepositoryOwner<T>(this T toolSettings, string repositoryOwner) where T : GitHubSettings
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.RepositoryOwner = repositoryOwner;
            return toolSettings;
        }
        /// <summary><p><em>Resets <see cref="GitHubSettings.RepositoryOwner"/>.</em></p><p>The account under which the repository is hosted</p></summary>
        [Pure]
        public static T ResetRepositoryOwner<T>(this T toolSettings) where T : GitHubSettings
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.RepositoryOwner = null;
            return toolSettings;
        }
        #endregion
        #region RepositoryName
        /// <summary><p><em>Sets <see cref="GitHubSettings.RepositoryName"/>.</em></p><p>The name of the repository</p></summary>
        [Pure]
        public static T SetRepositoryName<T>(this T toolSettings, string repositoryName) where T : GitHubSettings
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.RepositoryName = repositoryName;
            return toolSettings;
        }
        /// <summary><p><em>Resets <see cref="GitHubSettings.RepositoryName"/>.</em></p><p>The name of the repository</p></summary>
        [Pure]
        public static T ResetRepositoryName<T>(this T toolSettings) where T : GitHubSettings
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.RepositoryName = null;
            return toolSettings;
        }
        #endregion
        #region Token
        /// <summary><p><em>Sets <see cref="GitHubSettings.Token"/>.</em></p><p>The Token for the GitHub API</p></summary>
        [Pure]
        public static T SetToken<T>(this T toolSettings, string token) where T : GitHubSettings
        {
            var x = new GitHubReleaseSettings();
            toolSettings = toolSettings.NewInstance();
            toolSettings.Token = token;
            return toolSettings;
        }
        /// <summary><p><em>Resets <see cref="GitHubSettings.Token"/>.</em></p><p>The Token for the GitHub API</p></summary>
        [Pure]
        public static T ResetToken<T>(this T toolSettings) where T : GitHubSettings
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.Token = null;
            return toolSettings;
        }
        #endregion
    }
}