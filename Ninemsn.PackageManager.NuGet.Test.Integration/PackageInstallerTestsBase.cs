namespace Ninemsn.PackageManager.NuGet.Test.Integration
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using FluentAssertions;

    using NUnit.Framework;

    public abstract class PackageInstallerTestsBase
    {
        public PackageManagerModule Module { get; set; }

        public PackageInstaller Installer { get; set; }

        public string InstallationPath { get; set; }

        public string PackagePath { get; set; }

        [Test]
        public void ShouldInstallFirstVersionOfThePackage()
        {
            var version = new Version(1, 0);

            this.Installer.InstallPackage(version);
            var logs = this.Installer.Logs.ToArray();
            logs.Count().Should().Be(3);

            logs[0].Should().Be("Init");
            logs[1].Should().Contain("added");
            logs[2].Should().Be("Install");
            Directory.EnumerateDirectories(this.PackagePath, "DummyNews.1.0").Any().Should().BeTrue();
            this.GetFileVersion().Should().Be("1.0.0.0");
        }

        [Test]
        public void ShouldInstallLatestPackage()
        {
            this.Installer.InstallPackage();

            Directory.EnumerateDirectories(this.PackagePath, "DummyNews.1.1").Any().Should().BeTrue();
            this.GetFileVersion().Should().Be("1.1.0.0");
        }

        [Test]
        public void ShouldUpgradeAlreadyInstalledPackage()
        {
            this.Installer.InstallPackage(new Version(1, 0));
            this.Installer.InstallPackage(new Version(1, 1));
            var logs = this.Installer.Logs.ToArray();

            logs.Length.Should().Be(8);

            logs[0].Should().Be("Init");
            logs[1].Should().Contain("added");
            logs[2].Should().Be("Install");
            logs[3].Should().Be("Uninstall");
            logs[4].Should().Contain("removed");
            logs[5].Should().Be("Init");
            logs[6].Should().Contain("added");
            logs[7].Should().Be("Install");

            Directory.EnumerateDirectories(this.PackagePath, "DummyNews.1.0").Any().Should().BeFalse();
            Directory.EnumerateDirectories(this.PackagePath, "DummyNews.1.1").Any().Should().BeTrue();
            this.GetFileVersion().Should().Be("1.1.0.0");
        }

        [Test]
        public void ShouldNotInstallTheSameVersionOfThePackage()
        {
            this.Installer.InstallPackage();
            this.Installer.InstallPackage();
            var logs = this.Installer.Logs.ToArray();

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
            this.Installer.InstallPackage(version);
            this.Installer.UninstallPackage(version);
            var logs = this.Installer.Logs.ToArray();

            logs.Length.Should().Be(5);

            logs[0].Should().Be("Init");
            logs[1].Should().Contain("added");
            logs[2].Should().Be("Install");
            logs[3].Should().Be("Uninstall");
            logs[4].Should().Contain("removed");
            Directory.Exists(this.InstallationPath).Should().BeFalse("The package DummyNews should not be installed.");
        }

        [Test]
        public void ShouldUninstallLatestVersionOfThePackage()
        {
            this.Installer.InstallPackage();
            this.Installer.UninstallPackage();
            var logs = this.Installer.Logs.ToArray();

            logs.Length.Should().Be(5);

            logs[0].Should().Be("Init");
            logs[1].Should().Contain("added");
            logs[2].Should().Be("Install");
            logs[3].Should().Be("Uninstall");
            logs[4].Should().Contain("removed");
            Directory.Exists(this.InstallationPath).Should().BeFalse("The package DummyNews should not be installed.");
        }

        private string GetFileVersion()
        {
            var dllFile = Directory.EnumerateFiles(Path.Combine(this.InstallationPath, "bin")).First();
            var info = FileVersionInfo.GetVersionInfo(dllFile);

            return info.FileVersion;
        }
    }
}