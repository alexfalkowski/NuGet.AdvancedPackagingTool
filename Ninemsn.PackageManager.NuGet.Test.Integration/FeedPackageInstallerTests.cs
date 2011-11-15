namespace Ninemsn.PackageManager.NuGet.Test.Integration
{
    using System;
    using System.IO;
    using System.Reflection;

    using NUnit.Framework;

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
            this.module = new PackageManagerModule(packageSourceFile);
            var localSourceUri = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase) + "/App_Data/packages";
            this.packagePath = new Uri(localSourceUri).LocalPath;
            this.installationPath = Path.Combine(this.packagePath, "DummyNews");

            this.installer = new PackageInstaller(
                this.module.GetSource("LocalFeed"),
                this.packagePath, 
                "DummyNews", 
                this.installationPath);

            if (Directory.Exists(this.packagePath))
            {
                Directory.Delete(this.packagePath, true);
            }
        }

        [TearDown]
        public void TearDown()
        {
            this.server.Stop();
        }
    }
}