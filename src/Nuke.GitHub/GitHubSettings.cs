using System;
using Nuke.Common.Tooling;

namespace Nuke.GitHub
{
    public partial class GitHubSettings
    {
        /// <summary>
        /// This property is missing in the auto generated code and I have no idea what this should return by default.
        /// </summary>
        public override Action<OutputType, string> ProcessCustomLogger => null;

        public void Validate()
        {
            AssertValid();
        }
    }
}
