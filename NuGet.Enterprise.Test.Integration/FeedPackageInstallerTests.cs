namespace NuGet.Enterprise.Test.Integration
{
    using System;
    using System.IO;

    using NUnit.Framework;

    using NuGet.Enterprise.Core;

    [TestFixture]
    public class FeedPackageInstallerTests : PackageInstallerTestsBase
    {
        private PackagesWebServer server;

        [SetUp]
        public void SetUp()
        {
            this.server = new PackagesWebServer();
            this.server.StartUp();

            var packageSourceFile = PackageSourceFileFactory.CreatePackageSourceFile();
            this.Module = new PackageManagerModule(packageSourceFile);
            this.PackagePath = new Uri("file:///C:/Ninemsn/TestInstallPackage/").LocalPath;
            this.InstallationPath = Path.Combine(this.PackagePath, "DummyNews");

            this.NewsInstaller = new PackageInstaller(
                this.Module.GetSource("TestRemoteFeed"),
                this.PackagePath, 
                "DummyNews");

            this.SitecoreInstaller = new PackageInstaller(
                this.Module.GetSource("TestRemoteFeed"),
                this.PackagePath,
                "DummySitecore");

            if (Directory.Exists(this.PackagePath))
            {
                Directory.Delete(this.PackagePath, true);
            }
        }

        [TearDown]
        public void TearDown()
        {
            this.server.Stop();
        }
    }
}