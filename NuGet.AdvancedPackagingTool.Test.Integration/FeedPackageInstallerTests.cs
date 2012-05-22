namespace NuGet.AdvancedPackagingTool.Test.Integration
{
    using System;
    using System.IO;

    using NuGet.AdvancedPackagingTool.Core;

    using NUnit.Framework;

    [TestFixture]
    public class FeedPackageInstallerTests : PackageInstallerTestsBase
    {
        private PackagesWebServer server;

        [SetUp]
        public void Setup()
        {
            this.server = new PackagesWebServer();
            this.server.Startup();

            var packageSourceFile = PackageSourceFileFactory.CreatePackageSourceFile();
            this.Module = new PackageManagerModule(packageSourceFile);
            this.PackagePath = new Uri("file:///C:/NuGet/TestInstallPackage/").LocalPath;
            this.InstallationPath = Path.Combine(this.PackagePath, "DummyNews");

            this.Installer =
                new ValidPackageInstaller(
                    new DataServicePackageRepository(new Uri(this.Module.GetSource("TestRemoteFeed").Source)),
                    this.PackagePath,
                    "DummyNews");

            if (Directory.Exists(this.PackagePath))
            {
                Directory.Delete(this.PackagePath, true);
            }
        }

        [TearDown]
        public void Teardown()
        {
            this.server.Stop();
        }
    }
}