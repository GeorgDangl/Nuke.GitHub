using JetBrains.Annotations;
using Nuke.Common.Tooling;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Nuke.WebDocu
{
    [PublicAPI]
    [ExcludeFromCodeCoverage]
    public static class WebDocuSettingsExtensions
    {
        [Pure]
        public static WebDocuSettings SetSourceDirectory(this WebDocuSettings toolSettings, string sourceDirectory)
            => toolSettings.CloneAndModify(s => s.SourceDirectory = sourceDirectory);

        [Pure]
        public static WebDocuSettings SetDocuApiKey(this WebDocuSettings toolSettings, string docuApiKey)
            => toolSettings.CloneAndModify(s => s.DocuApiKey = docuApiKey);

        [Pure]
        public static WebDocuSettings SetDocuBaseUrl(this WebDocuSettings toolSettings, string docuBaseUrl)
            => toolSettings.CloneAndModify(s => s.DocuBaseUrl = docuBaseUrl);

        [Pure]
        public static WebDocuSettings SetVersion(this WebDocuSettings toolSettings, string version)
            => toolSettings.CloneAndModify(s => s.Version = version);

        [Pure]
        public static WebDocuSettings SetMarkdownChangelog(this WebDocuSettings toolSettings, string markdownChangelog)
            => toolSettings.CloneAndModify(s => s.MarkdownChangelog = markdownChangelog);

        [Pure]
        public static WebDocuSettings SetAssetFilePaths(this WebDocuSettings toolSettings, string[] assetFilePaths)
            => toolSettings.CloneAndModify(s => s.AssetFilePaths = assetFilePaths);

        [Pure]
        public static WebDocuSettings SetSkipForVersionConflicts(this WebDocuSettings toolSettings, bool skipForVersionConflicts)
            => toolSettings.CloneAndModify(s => s.SkipForVersionConflicts = skipForVersionConflicts);

        private static WebDocuSettings CloneAndModify(this WebDocuSettings toolSettings, Action<WebDocuSettings> modifyAction)
        {
            toolSettings = toolSettings.NewInstance();
            modifyAction(toolSettings);
            return toolSettings;
        }
    }
}
