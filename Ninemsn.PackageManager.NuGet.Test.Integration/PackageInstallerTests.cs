namespace Ninemsn.PackageManager.NuGet.Test.Integration
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using FluentAssertions;

    using NUnit.Framework;

    [TestFixture]
    public class PackageInstallerTests
    {
        private PackageManagerModule module;

        private PackagesWebServer server;

        private PackageInstaller installer;

        private string installationPath;

        private string packagePath;

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

            Directory.Delete(this.packagePath, true);
        }

        [TearDown]
        public void TearDown()
        {
            this.server.Stop();
        }

        [Test]
        public void ShouldInstallFirstVersionOfThePackage()
        {
            this.installer.Version = new Version(1, 0, 0, 0);

            this.installer.IsPackageInstalled().Should().BeFalse();
            var logs = this.installer.InstallPackage().ToArray();
            logs.Count().Should().Be(3);

            logs[0].Should().Be("Init");
            logs[1].Should().Contain("added");
            logs[2].Should().Be("Install");

            Directory.EnumerateDirectories(this.packagePath, "DummyNews.1.0").Any().Should().BeTrue();
            Directory.EnumerateDirectories(this.installationPath, "bin").Any().Should().BeTrue();
        }

        [Test]
        public void ShouldInstallLatestPackage()
        {
            this.installer.IsPackageInstalled().Should().BeFalse();
            this.installer.InstallPackage();

            Directory.EnumerateDirectories(this.packagePath, "DummyNews.1.1").Any().Should().BeTrue();
            Directory.EnumerateDirectories(this.installationPath, "bin").Any().Should().BeTrue();
        }

        [Test]
        public void ShouldUninstallFirstVersionOfThePackage()
        {
            this.installer.Version = new Version(1, 0, 0, 0);
            var logs = this.installer.InstallPackage().Union(this.installer.UninstallPackage()).ToArray();

            logs.Length.Should().Be(5);

            logs[3].Should().Be("Uninstall");
            logs[4].Should().Contain("removed");
            Directory.Exists(this.installationPath).Should().BeFalse("The package DummyNews should not be installed.");
        }
    }
}