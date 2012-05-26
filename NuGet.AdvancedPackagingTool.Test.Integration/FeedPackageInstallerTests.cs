namespace NuGet.AdvancedPackagingTool.Test.Integration
{
    using NuGet.AdvancedPackagingTool.Core;

    using NUnit.Framework;

    [TestFixture]
    public class FeedPackageInstallerTests : PackageInstallerTestsBase
    {
        private PackagesWebServer server;

        [SetUp]
        public void BeforeEach()
        {
            this.server = new PackagesWebServer();
            this.server.Startup();

            var packageSourceFile = new PackageSourceFileFactory().CreatePackageSourceFile();
            var module = new PackageManagerModule(packageSourceFile);

            this.Setup(module.GetSource("TestRemoteFeed"));
        }

        [TearDown]
        public void Teardown()
        {
            this.server.Stop();
        }
    }
}