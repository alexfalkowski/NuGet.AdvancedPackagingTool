﻿namespace Ninemsn.PackageManager.NuGet.Test.Integration
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using FluentAssertions;

    using global::NuGet;

    using NUnit.Framework;

    public abstract class PackageInstallerTestsBase
    {
        private const string DummyNews10 = "DummyNews.1.0";

        private const string Uninstall = @"Uninstall file://";

        private const string Removed = "removed";

        private const string Setup = @"Setup file://";

        private const string Teardown = @"Teardown file://";

        private const string Added = "added";

        private const string Install = @"Install file://";

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
        public void ShouldInstallFirstVersionOfThePackage()
        {
            var version = new SemanticVersion("1.0");

            this.Installer.InstallPackage(version);
            var logs = this.Installer.Logs.ToArray();
            logs.Count().Should().Be(3);

            logs[0].Should().StartWith(Setup);
            logs[1].Should().Contain(Added);
            logs[2].Should().StartWith(Install);
            Directory.EnumerateDirectories(this.PackagePath, DummyNews10).Any().Should().BeTrue();
            this.GetFileVersion().Should().Be(Version10);
        }

        [Test]
        public void ShouldInstallLatestPackage()
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
            var logs = this.Installer.Logs.ToArray();

            logs.Length.Should().Be(9);

            logs[0].Should().StartWith(Setup);
            logs[1].Should().Contain(Added);
            logs[2].Should().StartWith(Install);
            logs[3].Should().StartWith(Uninstall);
            logs[4].Should().Contain(Removed);
            logs[5].Should().StartWith(Teardown);
            logs[6].Should().StartWith(Setup);
            logs[7].Should().Contain(Added);
            logs[8].Should().StartWith(Install);

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
            var logs = this.Installer.Logs.ToArray();

            logs.Length.Should().Be(4);
            logs[0].Should().StartWith(Setup);
            logs[1].Should().Contain(Added);
            logs[2].Should().StartWith(Install);
            logs[3].Should().Contain(Already);
        }

        [Test]
        public void ShouldUninstallFirstVersionOfThePackage()
        {
            var version = new SemanticVersion("1.0");
            this.Installer.InstallPackage(version);
            this.Installer.UninstallPackage(version);
            var logs = this.Installer.Logs.ToArray();

            logs.Length.Should().Be(6);

            logs[0].Should().StartWith(Setup);
            logs[1].Should().Contain(Added);
            logs[2].Should().StartWith(Install);
            logs[3].Should().StartWith(Uninstall);
            logs[4].Should().Contain(Removed);
            logs[5].Should().StartWith(Teardown);
            Directory.Exists(this.InstallationPath).Should().BeFalse(PackageShouldBeinstalled);
        }

        [Test]
        public void ShouldUninstallLatestVersionOfThePackage()
        {
            var version = new SemanticVersion("1.1");
            this.Installer.InstallPackage(version);
            this.Installer.UninstallPackage(version);
            var logs = this.Installer.Logs.ToArray();

            logs.Length.Should().Be(6);

            logs[0].Should().StartWith(Setup);
            logs[1].Should().Contain(Added);
            logs[2].Should().StartWith(Install);
            logs[3].Should().StartWith(Uninstall);
            logs[4].Should().Contain(Removed);
            logs[5].Should().StartWith(Teardown);
            Directory.Exists(this.InstallationPath).Should().BeFalse(PackageShouldBeinstalled);
        }

        private string GetFileVersion()
        {
            var dllFile = Directory.EnumerateFiles(Path.Combine(this.InstallationPath, "bin")).First();
            var info = FileVersionInfo.GetVersionInfo(dllFile);

            return info.FileVersion;
        }
    }
}