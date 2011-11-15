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

        [Test]
        public void ShouldInstallFirstVersionOfThePackage()
        {
            var version = new Version(1, 0);

            this.installer.InstallPackage(version);
            var logs = this.installer.Logs.ToArray();
            logs.Count().Should().Be(3);

            logs[0].Should().Be("Init");
            logs[1].Should().Contain("added");
            logs[2].Should().Be("Install");

            Directory.EnumerateDirectories(this.packagePath, "DummyNews.1.0").Any().Should().BeTrue();
            Directory.EnumerateDirectories(this.installationPath, "Views").Any().Should().BeTrue();
        }

        [Test]
        public void ShouldInstallLatestPackage()
        {
            this.installer.InstallPackage();

            Directory.EnumerateDirectories(this.packagePath, "DummyNews.1.1").Any().Should().BeTrue();
            Directory.EnumerateDirectories(this.installationPath, "Account").Any().Should().BeTrue();
        }

        [Test]
        public void ShouldUpgradeAlreadyInstalledPackage()
        {
            this.installer.InstallPackage(new Version(1, 0));
            this.installer.InstallPackage(new Version(1, 1));
            var logs = this.installer.Logs.ToArray();

            logs.Length.Should().Be(8);

            logs[0].Should().Be("Init");
            logs[1].Should().Contain("added");
            logs[2].Should().Be("Install");
            logs[3].Should().Be("Uninstall");
            logs[4].Should().Contain("removed");
            logs[5].Should().Be("Init");
            logs[6].Should().Contain("added");
            logs[7].Should().Be("Install");

            Directory.EnumerateDirectories(this.packagePath, "DummyNews.1.0").Any().Should().BeFalse();
            Directory.EnumerateDirectories(this.packagePath, "DummyNews.1.1").Any().Should().BeTrue();
            Directory.EnumerateDirectories(this.installationPath, "Account").Any().Should().BeTrue();
        }

        [Test]
        public void ShouldNotInstallTheSameVersionOfThePackage()
        {
            this.installer.InstallPackage();
            this.installer.InstallPackage();
            var logs = this.installer.Logs.ToArray();

            logs.Length.Should().Be(4);
            logs[0].Should().Be("Init");
            logs[1].Should().Contain("added");
            logs[2].Should().Be("Install");
            logs[3].Should().Contain("already");
        }

        [Test]
        public void ShouldUninstallFirstVersionOfThePackage()
        {
            var version = new Version(1, 0);
            this.installer.InstallPackage(version);
            this.installer.UninstallPackage(version);
            var logs = this.installer.Logs.ToArray();

            logs.Length.Should().Be(5);

            logs[0].Should().Be("Init");
            logs[1].Should().Contain("added");
            logs[2].Should().Be("Install");
            logs[3].Should().Be("Uninstall");
            logs[4].Should().Contain("removed");
            Directory.Exists(this.installationPath).Should().BeFalse("The package DummyNews should not be installed.");
        }

        [Test]
        public void ShouldUninstallLatestVersionOfThePackage()
        {
            this.installer.InstallPackage();
            this.installer.UninstallPackage();
            var logs = this.installer.Logs.ToArray();

            logs.Length.Should().Be(5);

            logs[0].Should().Be("Init");
            logs[1].Should().Contain("added");
            logs[2].Should().Be("Install");
            logs[3].Should().Be("Uninstall");
            logs[4].Should().Contain("removed");
            Directory.Exists(this.installationPath).Should().BeFalse("The package DummyNews should not be installed.");
        }
    }
}