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
                this.module.GetSource("InstallPath"), 
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
            this.installer.Logs.Count().Should().Be(4, "There was no output.");
            this.installer.IsPackageInstalled().Should().BeTrue();
            Directory.GetFiles(this.installationPath).Length.Should().BeGreaterThan(1);
            Directory.GetDirectories(this.installationPath, "bin").Length.Should().Be(1);
        }

        [Test]
        public void ShouldContainContentFolderInPackage()
        {
            this.installer.Package.GetToolsFiles().Count().Should().Be(3);
        }

        [Test]
        public void ShouldContainPowershellScriptsInToolsFolder()
        {
            Action action = () => this.installer.Package.GetPowerShellFiles();

            action.ShouldNotThrow<InvalidOperationException>();
        }

        [Test]
        public void ShouldExecutePowerShellScript()
        {
            var files = this.installer.Package.GetPowerShellFiles();
            this.installer.ExecutePowerShell(files.Item1);

            this.installer.Logs.Count().Should().Be(1, "There was no output.");
        }
    }
}