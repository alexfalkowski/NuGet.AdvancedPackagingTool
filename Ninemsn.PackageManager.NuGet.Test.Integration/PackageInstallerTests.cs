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
            Directory.CreateDirectory(localSource);
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
            this.installer.InstallPackage();
            this.installer.Logs.Count().Should().Be(3);

            var logs = this.installer.Logs.ToArray();

            logs[0].Should().Be("Init");
            logs[1].Should().StartWith("Successfully");
            logs[2].Should().Be("Install");

            this.installer.IsPackageInstalled().Should().BeTrue();
            Directory.GetFiles(this.installationPath).Length.Should().BeGreaterThan(1);
            Directory.GetDirectories(this.installationPath, "bin").Length.Should().Be(1);
        }

        [Test]
        public void ShouldUninstallLocalPackageInLocalRepository()
        {
            this.installer.InstallPackage();
            this.installer.UninstallPackage();

            this.installer.Logs.Count().Should().Be(4);

            var logs = this.installer.Logs.ToArray();

            logs[3].Should().Be("Uninstall");
            Directory.GetDirectories(this.installationPath).Length.Should().Be(0);
        }

        [Test]
        public void ShouldContainContentFolderInPackage()
        {
            this.installer.Package.GetToolsFiles().Count().Should().Be(3);
        }

        [Test]
        public void ShouldContainPowershellScriptsInToolsFolder()
        {
            Action initPackageFile = () => this.installer.Package.GetInitPackageFile();

            initPackageFile.ShouldNotThrow<InvalidOperationException>();

            Action installPackageFile = () => this.installer.Package.GetInstallPackageFile();

            installPackageFile.ShouldNotThrow<InvalidOperationException>();

            Action unsitallPackageFile = () => this.installer.Package.GetUninstallPackageFile();

            unsitallPackageFile.ShouldNotThrow<InvalidOperationException>();
        }

        [Test]
        public void ShouldExecutePowerShellScript()
        {
            var file = this.installer.Package.GetInitPackageFile();
            this.installer.ExecutePowerShell(file);

            this.installer.Logs.Count().Should().Be(1, "There was no output.");
        }
    }
}