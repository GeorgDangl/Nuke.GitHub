﻿using JetBrains.Annotations;
using Nuke.Common.Tooling;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Nuke.WebDocu
{
    [PublicAPI]
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class WebDocuSettings : ISettingsEntity
    {
        public virtual string SourceDirectory { get; internal set; }
        public virtual string DocuApiKey { get; internal set; }
        public virtual string DocuBaseUrl { get; internal set; }
        public virtual string Version { get; internal set; }
        public virtual string MarkdownChangelog { get; internal set; }
        public virtual string[] AssetFilePaths { get; set; }
        /// <summary>
        /// If WebDocu returns a Http 409 status code, this means that the combination of
        /// package and version already exists. If this property is set to true, it will
        /// just skip the process then and exit, otherwise it will report a failure. 
        /// When it skips, assets are also not uploaded.
        /// </summary>
        public virtual bool SkipForVersionConflicts { get; internal set; }
    }
}
