namespace Ninemsn.PackageManager.NuGet.Test.Integration
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;

    using FluentAssertions;

    using NUnit.Framework;

    public abstract class PackageInstallerTestsBase
    {
        // ReSharper disable InconsistentNaming
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Reviewed. Suppression is OK here.")]
        protected PackageManagerModule module;

        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Reviewed. Suppression is OK here.")]
        protected PackageInstaller installer;

        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Reviewed. Suppression is OK here.")]
        protected string installationPath;

        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Reviewed. Suppression is OK here.")]
        protected string packagePath;
        // ReSharper restore InconsistentNaming
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