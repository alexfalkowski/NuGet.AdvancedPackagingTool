namespace Ninemsn.PackageManager.NuGet.Test.Integration
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;

    using FluentAssertions;

    using global::NuGet;

    using NUnit.Framework;

    public abstract class PackageInstallerTestsBase
    {
        private const string DummyNews10 = "DummyNews.1.0";

        private const string Uninstall = "Uninstall";

        private const string Removed = "removed";

        private const string Setup = "Setup";

        private const string Teardown = "Teardown";

        private const string Added = "added";

        private const string Install = "Install";

        private const string PackageShouldBeinstalled = "The package DummyNews should not be installed.";

        private const string Already = "already";

        private const string DummyNews11 = "DummyNews.1.1";

        private const string Version10 = "1.0.0.0";

        private const string Version11 = "1.1.0.0";

        protected PackageManagerModule Module { get; set; }

        protected PackageInstaller Installer { get; set; }

        protected string InstallationPath { get; set; }

        protected string PackagePath { get; set; }

        [Test]
        public void ShouldInstallVersion10Package()
        {
            var version = new SemanticVersion("1.0");

            this.Installer.InstallPackage(version);

            var log = GetLog(this.Installer.Logs);

            log.Should().Contain(Setup);
            log.Should().Contain(Added);
            log.Should().Contain(Install);
            Directory.EnumerateDirectories(this.PackagePath, DummyNews10).Any().Should().BeTrue();
            this.GetFileVersion().Should().Be(Version10);
        }

        [Test]
        public void ShouldInstallVersion11Package()
        {
            this.Installer.InstallPackage(new SemanticVersion("1.1"));

            Directory.EnumerateDirectories(this.PackagePath, DummyNews11).Any().Should().BeTrue();
            this.GetFileVersion().Should().Be(Version11);
        }

        [Test]
        public void ShouldUpgradeAlreadyInstalledPackage()
        {
            this.Installer.InstallPackage(new SemanticVersion("1.0"));
            this.Installer.InstallPackage(new SemanticVersion("1.1"));
            var log = GetLog(this.Installer.Logs);

            log.Should().Contain(Setup);
            log.Should().Contain(Added);
            log.Should().Contain(Install);
            log.Should().Contain(Uninstall);
            log.Should().Contain(Removed);
            log.Should().Contain(Teardown);
            log.Should().Contain(Setup);
            log.Should().Contain(Added);
            log.Should().Contain(Install);

            Directory.EnumerateDirectories(this.PackagePath, DummyNews10).Any().Should().BeFalse();
            Directory.EnumerateDirectories(this.PackagePath, DummyNews11).Any().Should().BeTrue();
            this.GetFileVersion().Should().Be(Version11);
        }

        [Test]
        public void ShouldNotInstallTheSameVersionOfThePackage()
        {
            var version = new SemanticVersion("1.0");
            this.Installer.InstallPackage(version);
            this.Installer.InstallPackage(version);
            var log = GetLog(this.Installer.Logs);

            log.Should().Contain(Setup);
            log.Should().Contain(Added);
            log.Should().Contain(Install);
            log.Should().Contain(Already);
        }

        [Test]
        public void ShouldUninstallVersion10Package()
        {
            var version = new SemanticVersion("1.0");
            this.Installer.InstallPackage(version);
            this.Installer.UninstallPackage(version);
            var log = GetLog(this.Installer.Logs);

            log.Should().Contain(Setup);
            log.Should().Contain(Added);
            log.Should().Contain(Install);
            log.Should().Contain(Uninstall);
            log.Should().Contain(Removed);
            log.Should().Contain(Teardown);
            Directory.Exists(this.InstallationPath).Should().BeFalse(PackageShouldBeinstalled);
        }

        [Test]
        public void ShouldUninstallVersion11Package()
        {
            var version = new SemanticVersion("1.1");
            this.Installer.InstallPackage(version);
            this.Installer.UninstallPackage(version);
            var log = GetLog(this.Installer.Logs);

            log.Should().Contain(Setup);
            log.Should().Contain(Added);
            log.Should().Contain(Install);
            log.Should().Contain(Uninstall);
            log.Should().Contain(Removed);
            log.Should().Contain(Teardown);
            Directory.Exists(this.InstallationPath).Should().BeFalse(PackageShouldBeinstalled);
        }

        private static string GetLog(IEnumerable<string> logs)
        {
            var builder = new StringBuilder();

            foreach (var log in logs)
            {
                builder.AppendLine(log);
            }

            return builder.ToString();
        }

        private string GetFileVersion()
        {
            var dllFile = Directory.EnumerateFiles(Path.Combine(this.InstallationPath, "bin")).First();
            var info = FileVersionInfo.GetVersionInfo(dllFile);

            return info.FileVersion;
        }
    }
}