namespace NuGet.AdvancedPackagingTool.Test.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Text;

    using FluentAssertions;

    using NuGet;
    using NuGet.AdvancedPackagingTool.Core;

    using NUnit.Framework;

    public abstract class PackageInstallerTestsBase
    {
        private const string ShortVersion10 = "1.0";

        private const string DummyNews10File = "DummyNews.1.0.nupkg";

        private const string DummyNews10Folder = "DummyNews.1.0";

        private const string DummyNews11File = "DummyNews.1.1.nupkg";

        private const string DummyNews11Folder = "DummyNews.1.1";

        private const string Uninstall = "Uninstall";

        private const string Removed = "removed";

        private const string Setup = "Setup";

        private const string Teardown = "Teardown";

        private const string Added = "added";

        private const string Install = "Install";

        private const string NotInstalled = "not installed";

        private const string PackageShouldBeInstalled = "The package DummyNews should not be installed.";

        private const string Already = "already";

        private const string Version10 = "1.0.0.0";

        private const string Version11 = "1.1.0.0";

        private const string ShortVersion11 = "1.1";

        protected PackageManagerModule Module { get; set; }

        protected ZipPackageInstaller NewsInstaller { get; set; }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sitecore", Justification = "Sitecore is a company.")]
        protected ZipPackageInstaller SitecoreInstaller { get; set; }

        protected string InstallationPath { get; set; }

        protected string PackagePath { get; set; }

        [Test]
        public void ShouldNotUninstallNotInstalledPackage()
        {
            var version = new SemanticVersion(ShortVersion10);

            this.NewsInstaller.UninstallPackage(version);

            var log = GetLog(this.NewsInstaller.Logs);

            log.Should().Contain(NotInstalled);
        }

        [Test]
        public void ShouldInstallVersion10Package()
        {
            var version = new SemanticVersion(ShortVersion10);

            this.NewsInstaller.InstallPackage(version);

            var log = GetLog(this.NewsInstaller.Logs);

            log.Should().Contain(Setup);
            log.Should().Contain(Added);
            log.Should().Contain(Install);
            File.Exists(Path.Combine(this.PackagePath, DummyNews10Folder, DummyNews10File)).Should().BeTrue();
            this.GetFileVersion().Should().Be(Version10);
            this.GetWebsiteVersion().Should().Be(Version10);
        }

        [Test]
        public void ShouldInstallVersion11Package()
        {
            this.NewsInstaller.InstallPackage(new SemanticVersion(ShortVersion11));

            File.Exists(Path.Combine(this.PackagePath, DummyNews11Folder, DummyNews11File)).Should().BeTrue();
            this.GetFileVersion().Should().Be(Version11);
            this.GetWebsiteVersion().Should().Be(Version11);
        }

        [Test]
        public void ShouldUpgradeAlreadyInstalledPackage()
        {
            this.NewsInstaller.InstallPackage(new SemanticVersion(ShortVersion10));
            this.NewsInstaller.InstallPackage(new SemanticVersion(ShortVersion11));
            var log = GetLog(this.NewsInstaller.Logs);

            log.Should().Contain(Setup);
            log.Should().Contain(Added);
            log.Should().Contain(Install);
            log.Should().Contain(Uninstall);
            log.Should().Contain(Removed);
            log.Should().Contain(Setup);
            log.Should().Contain(Added);
            log.Should().Contain(Install);

            File.Exists(Path.Combine(this.PackagePath, DummyNews10Folder, DummyNews10File)).Should().BeFalse();
            File.Exists(Path.Combine(this.PackagePath, DummyNews11Folder, DummyNews11File)).Should().BeTrue();
            this.GetFileVersion().Should().Be(Version11);
            this.GetWebsiteVersion().Should().Be(Version11);
        }

        [Test]
        public void ShouldInstallOnePackageThenAnotherPackageShouldOverwriteFiles()
        {
            this.SitecoreInstaller.InstallPackage(new SemanticVersion(ShortVersion10));
            this.NewsInstaller.InstallPackage(new SemanticVersion(ShortVersion10));
            
            var log = GetLog(this.NewsInstaller.Logs);

            log.Should().Contain(Setup);
            log.Should().Contain(Added);
            log.Should().Contain(Install);

            File.Exists(Path.Combine(this.PackagePath, DummyNews10Folder, DummyNews10File)).Should().BeTrue();
            this.GetFileVersion().Should().Be(Version10);
            this.GetWebsiteVersion().Should().Be(Version10);
        }

        [Test]
        public void ShouldNotInstallTheSameVersionOfThePackage()
        {
            var version = new SemanticVersion(ShortVersion10);
            this.NewsInstaller.InstallPackage(version);
            this.NewsInstaller.InstallPackage(version);
            var log = GetLog(this.NewsInstaller.Logs);

            log.Should().Contain(Setup);
            log.Should().Contain(Added);
            log.Should().Contain(Install);
            log.Should().Contain(Already);
        }

        [Test]
        public void ShouldUninstallVersion10Package()
        {
            var version = new SemanticVersion(ShortVersion10);
            this.NewsInstaller.InstallPackage(version);
            this.NewsInstaller.UninstallPackage(version);
            var log = GetLog(this.NewsInstaller.Logs);

            log.Should().Contain(Setup);
            log.Should().Contain(Added);
            log.Should().Contain(Install);
            log.Should().Contain(Uninstall);
            log.Should().Contain(Removed);
            log.Should().Contain(Teardown);
            Directory.Exists(this.InstallationPath).Should().BeFalse(PackageShouldBeInstalled);
        }

        [Test]
        public void ShouldUninstallVersion11Package()
        {
            var version = new SemanticVersion(ShortVersion11);
            this.NewsInstaller.InstallPackage(version);
            this.NewsInstaller.UninstallPackage(version);
            var log = GetLog(this.NewsInstaller.Logs);

            log.Should().Contain(Setup);
            log.Should().Contain(Added);
            log.Should().Contain(Install);
            log.Should().Contain(Uninstall);
            log.Should().Contain(Removed);
            log.Should().Contain(Teardown);
            Directory.Exists(this.InstallationPath).Should().BeFalse(PackageShouldBeInstalled);
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

        private string GetWebsiteVersion()
        {
            var versionFile = Directory.EnumerateFiles(this.InstallationPath).Where(x => x.EndsWith("WebsiteVersion.txt", StringComparison.CurrentCultureIgnoreCase)).First();
            return File.ReadAllText(versionFile);
        }
    }
}