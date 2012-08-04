namespace NuGet.AdvancedPackagingTool.Test.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;

    using FluentAssertions;

    using NuGet;
    using NuGet.AdvancedPackagingTool.Core;

    using NUnit.Framework;

    public abstract class PackageInstallerTestsBase
    {
        internal const string DummyNews10Folder = "DummyNews.1.0";

        internal const string DummyNews11Folder = "DummyNews.1.1";

        private const string ShortVersion10 = "1.0";

        private const string DummyNews10File = "DummyNews.1.0.nupkg";

        private const string DummyNews11File = "DummyNews.1.1.nupkg";

        private const string DummyNews = "DummyNews";

        private const string Uninstall = "Uninstall";

        private const string Uninstalled = "uninstalled";

        private const string SetupText = "Setup";

        private const string Teardown = "Teardown";

        private const string Installed = "installed";

        private const string Install = "Install";

        private const string PackageShouldBeInstalled = "The package DummyNews should not be installed.";

        private const string Already = "already";

        private const string Version10 = "1.0.0.0";

        private const string Version11 = "1.1.0.0";

        private const string ShortVersion11 = "1.1";

        private const string ConfigurationInternalText = "Configuration internal";

        private IPackageInstaller Installer { get; set; }

        private string PackagePath { get; set; }

        private string ConfigurationText { get; set; }

        [Test]
        public void ShouldNotUninstallNotInstalledPackage()
        {
            var version = new SemanticVersion(ShortVersion10);
            this.Installer.UninstallPackage(DummyNews, version);

            ShouldContainLogEntry(this.Installer.Logs, "Unable to find package 'DummyNews'.");
        }

        [Test]
        public void ShouldInstallVersion10Package()
        {
            var version = new SemanticVersion(ShortVersion10);

            this.Installer.InstallPackage(DummyNews, version);

            ShouldContainLogEntry(this.Installer.Logs, SetupText);
            ShouldContainLogEntry(this.Installer.Logs, Installed);
            ShouldContainLogEntry(this.Installer.Logs, Install);
            ShouldContainLogEntry(this.Installer.Logs, this.ConfigurationText);

            var installPath = Path.Combine(this.PackagePath, DummyNews10Folder);
            File.Exists(Path.Combine(installPath, DummyNews10File)).Should().BeTrue();
            var contentPath = Path.Combine(this.PackagePath, DummyNews10Folder, "content");
            GetFileVersion(contentPath).Should().Be(Version10);
            GetWebsiteVersion(contentPath).Should().Be(Version10);
        }

        [Test]
        public void ShouldInstallVersion11Package()
        {
            var version = new SemanticVersion(ShortVersion11);

            this.Installer.InstallPackage(DummyNews, version);

            ShouldContainLogEntry(this.Installer.Logs, SetupText);
            ShouldContainLogEntry(this.Installer.Logs, Installed);
            ShouldContainLogEntry(this.Installer.Logs, Install);
            ShouldContainLogEntry(this.Installer.Logs, this.ConfigurationText);

            var installPath = Path.Combine(this.PackagePath, DummyNews11Folder);
            File.Exists(Path.Combine(installPath, DummyNews11File)).Should().BeTrue();
            var contentPath = Path.Combine(this.PackagePath, DummyNews11Folder, "content");
            GetFileVersion(contentPath).Should().Be(Version11);
            GetWebsiteVersion(contentPath).Should().Be(Version11);
        }

        [Test]
        public void ShouldUpgradeAlreadyInstalledPackage()
        {
            this.Installer.InstallPackage(DummyNews, new SemanticVersion(ShortVersion10));
            this.Installer.InstallPackage(DummyNews, new SemanticVersion(ShortVersion11));

            ShouldContainLogEntry(this.Installer.Logs, SetupText);
            ShouldContainLogEntry(this.Installer.Logs, Installed);
            ShouldContainLogEntry(this.Installer.Logs, Install);
            ShouldContainLogEntry(this.Installer.Logs, Uninstall);
            ShouldContainLogEntry(this.Installer.Logs, Uninstalled);
            ShouldContainLogEntry(this.Installer.Logs, SetupText);
            ShouldContainLogEntry(this.Installer.Logs, Installed);
            ShouldContainLogEntry(this.Installer.Logs, Install);
            ShouldContainLogEntry(this.Installer.Logs, this.ConfigurationText);

            File.Exists(Path.Combine(this.PackagePath, DummyNews10Folder, DummyNews10File)).Should().BeFalse();
            File.Exists(Path.Combine(this.PackagePath, DummyNews11Folder, DummyNews11File)).Should().BeTrue();
            var contentPath = Path.Combine(this.PackagePath, DummyNews11Folder, "content");
            GetFileVersion(contentPath).Should().Be(Version11);
            GetWebsiteVersion(contentPath).Should().Be(Version11);
        }

        [Test]
        public void ShouldNotInstallTheSameVersionOfThePackage()
        {
            var version = new SemanticVersion(ShortVersion10);
            this.Installer.InstallPackage(DummyNews, version);
            this.Installer.InstallPackage(DummyNews, version);

            ShouldContainLogEntry(this.Installer.Logs, SetupText);
            ShouldContainLogEntry(this.Installer.Logs, Installed);
            ShouldContainLogEntry(this.Installer.Logs, Install);
            ShouldContainLogEntry(this.Installer.Logs, this.ConfigurationText);
            ShouldContainLogEntry(this.Installer.Logs, Already);
        }

        [Test]
        public void ShouldUninstallVersion10Package()
        {
            var version = new SemanticVersion(ShortVersion10);
            this.Installer.InstallPackage(DummyNews, version);
            this.Installer.UninstallPackage(DummyNews, version);

            ShouldContainLogEntry(this.Installer.Logs, SetupText);
            ShouldContainLogEntry(this.Installer.Logs, Installed);
            ShouldContainLogEntry(this.Installer.Logs, Install);
            ShouldContainLogEntry(this.Installer.Logs, this.ConfigurationText);
            ShouldContainLogEntry(this.Installer.Logs, Uninstall);
            ShouldContainLogEntry(this.Installer.Logs, Uninstalled);
            ShouldContainLogEntry(this.Installer.Logs, Teardown);
            Directory.Exists(Path.Combine(this.PackagePath, DummyNews10Folder)).Should().BeFalse(
                PackageShouldBeInstalled);
        }

        [Test]
        public void ShouldUninstallVersion11Package()
        {
            var version = new SemanticVersion(ShortVersion11);
            this.Installer.InstallPackage(DummyNews, version);
            this.Installer.UninstallPackage(DummyNews, version);

            ShouldContainLogEntry(this.Installer.Logs, SetupText);
            ShouldContainLogEntry(this.Installer.Logs, Installed);
            ShouldContainLogEntry(this.Installer.Logs, Install);
            ShouldContainLogEntry(this.Installer.Logs, this.ConfigurationText);
            ShouldContainLogEntry(this.Installer.Logs, Uninstall);
            ShouldContainLogEntry(this.Installer.Logs, Uninstalled);
            ShouldContainLogEntry(this.Installer.Logs, Teardown);
            Directory.Exists(Path.Combine(this.PackagePath, DummyNews11Folder)).Should().BeFalse(
                PackageShouldBeInstalled);
        }

        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "This is test code.")]
        protected void Setup(PackageSource source, string configurationText = null, string configurationPath = null)
        {
            var configurationManager = new TestConfigurationManager();
            this.PackagePath = configurationManager.PackagePath;
            this.ConfigurationText = configurationText ?? ConfigurationInternalText;

            var factory = new PackageInstallerFactory(
                new SourcePackageRepositoryFactory(source), configurationManager, new TestDirectorySystem());

            this.Installer = factory.CreatePackageInstaller(true, this.PackagePath, configurationPath);

            if (Directory.Exists(this.PackagePath))
            {
                Directory.Delete(this.PackagePath, true);
            }
        }

        private static void ShouldContainLogEntry(IEnumerable<string> logs, string entry)
        {
            logs.Should().Contain(
                s => s.Contains(entry, StringComparison.CurrentCultureIgnoreCase), "The log should contain {0}", entry);
        }

        private static string GetFileVersion(string path)
        {
            var dllFile = Directory.EnumerateFiles(Path.Combine(path, "bin")).First();
            var info = FileVersionInfo.GetVersionInfo(dllFile);

            return info.FileVersion;
        }

        private static string GetWebsiteVersion(string path)
        {
            var versionFile = Directory.EnumerateFiles(path).Where(x => x.EndsWith("WebsiteVersion.txt", StringComparison.CurrentCultureIgnoreCase)).First();
            return File.ReadAllText(versionFile);
        }
    }
}