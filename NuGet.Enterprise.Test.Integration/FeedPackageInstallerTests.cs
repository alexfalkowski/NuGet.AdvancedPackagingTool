namespace NuGet.Enterprise.Test.Integration
{
    using System;
    using System.IO;

    using NuGet.Enterprise.Core;

    using NUnit.Framework;

    [TestFixture]
    [Ignore("Don't know if we want to support remote packages. Git is a better solution.")]
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
        public void Teardown()
        {
            this.server.Stop();
        }
    }
}