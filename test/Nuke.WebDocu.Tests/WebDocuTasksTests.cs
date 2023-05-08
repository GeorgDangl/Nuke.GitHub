using Xunit;

namespace Nuke.WebDocu.Tests
{
    public class WebDocuTasksTests
    {
        [Fact]
        public void ExtractVersion()
        {
            var packagePath = @"C:\\Documents\Project.1.2.3-beta4.nupkg";
            var version = WebDocuTasks.GetVersionFromNuGetPackageFilename(packagePath, "Project");
            Assert.Equal("1.2.3-beta4", version);
        }
    }
}
