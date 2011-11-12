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

        [SetUp]
        public void SetUp()
        {
            this.server = new PackagesWebServer();
            this.server.StartUp();

            var packageSourceFile = PackageSourceFileFactory.CreatePackageSourceFile();
            this.module = new PackageManagerModule(packageSourceFile);
            var localSourceUri = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase) + "/App_Data/packages";
            var localSource = new Uri(localSourceUri).LocalPath;
            this.installationPath = Path.Combine(localSource, "DummyNews");

            this.installer = new PackageInstaller(
                this.module.GetSource("LocalFeed"),
                localSource, 
                "DummyNews", 
                this.installationPath);

            Directory.Delete(localSource, true);
        }

        [TearDown]
        public void TearDown()
        {
            this.server.Stop();
        }

        [Test]
        public void ShouldInstallLocalPackageInLocalRepository()
        {
            this.installer.IsPackageInstalled().Should().BeFalse();
            var logs = this.installer.InstallPackage().ToArray();
            logs.Count().Should().Be(3);

            logs[0].Should().Be("Init");
            logs[1].Should().Contain("added");
            logs[2].Should().Be("Install");

            this.installer.IsPackageInstalled().Should().BeTrue();
            Directory.GetFiles(this.installationPath).Length.Should().BeGreaterThan(1);
            Directory.GetDirectories(this.installationPath, "bin").Length.Should().Be(1);
        }

        [Test]
        public void ShouldUninstallLocalPackageInLocalRepository()
        {
            var logs = this.installer.InstallPackage().Union(this.installer.UninstallPackage()).ToArray();

            logs.Length.Should().Be(5);

            logs[3].Should().Be("Uninstall");
            logs[4].Should().Contain("removed");
            Directory.Exists(this.installationPath).Should().BeFalse("The package DummyNews should not be installed.");
        }
    }
}