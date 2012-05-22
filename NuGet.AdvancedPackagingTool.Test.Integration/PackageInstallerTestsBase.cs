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
        private const string ShortVersion10 = "1.0";

        private const string DummyNews10File = "DummyNews.1.0.nupkg";

        private const string DummyNews10Folder = "DummyNews.1.0";

        private const string DummyNews11File = "DummyNews.1.1.nupkg";

        private const string DummyNews11Folder = "DummyNews.1.1";

        private const string Uninstall = "Uninstall";

        private const string Uninstalled = "uninstalled";

        private const string Setup = "Setup";

        private const string Teardown = "Teardown";

        private const string Installed = "installed";

        private const string Install = "Install";

        private const string PackageShouldBeInstalled = "The package DummyNews should not be installed.";

        private const string Already = "already";

        private const string Version10 = "1.0.0.0";

        private const string Version11 = "1.1.0.0";

        private const string ShortVersion11 = "1.1";

        protected PackageManagerModule Module { get; set; }

        protected ValidPackageInstaller NewsInstaller { get; set; }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sitecore", Justification = "Sitecore is a company.")]
        protected ValidPackageInstaller SitecoreInstaller { get; set; }

        protected string InstallationPath { get; set; }

        protected string PackagePath { get; set; }

        [Test]
        public void ShouldNotUninstallNotInstalledPackage()
        {
            var version = new SemanticVersion(ShortVersion10);
            Action action = () => this.NewsInstaller.UninstallPackage(version);

            action.ShouldThrow<InvalidOperationException>().WithMessage("Unable to find package 'DummyNews'.");
        }

        [Test]
        public void ShouldInstallVersion10Package()
        {
            var version = new SemanticVersion(ShortVersion10);

            this.NewsInstaller.InstallPackage(version);

            ShouldContainLogEntry(this.NewsInstaller.Logs, Setup);
            ShouldContainLogEntry(this.NewsInstaller.Logs, Installed);
            ShouldContainLogEntry(this.NewsInstaller.Logs, Install);

            var installPath = Path.Combine(this.PackagePath, DummyNews10Folder);
            File.Exists(Path.Combine(installPath, DummyNews10File)).Should().BeTrue();
            var contentPath = Path.Combine(installPath, "content");
            GetFileVersion(contentPath).Should().Be(Version10);
            GetWebsiteVersion(contentPath).Should().Be(Version10);
        }

        [Test]
        public void ShouldInstallVersion11Package()
        {
            var version = new SemanticVersion(ShortVersion11);

            this.NewsInstaller.InstallPackage(version);

            ShouldContainLogEntry(this.NewsInstaller.Logs, Setup);
            ShouldContainLogEntry(this.NewsInstaller.Logs, Installed);
            ShouldContainLogEntry(this.NewsInstaller.Logs, Install);

            var installPath = Path.Combine(this.PackagePath, DummyNews11Folder);
            File.Exists(Path.Combine(installPath, DummyNews11File)).Should().BeTrue();
            var contentPath = Path.Combine(installPath, "content");
            GetFileVersion(contentPath).Should().Be(Version11);
            GetWebsiteVersion(contentPath).Should().Be(Version11);
        }

        [Test]
        public void ShouldUpgradeAlreadyInstalledPackage()
        {
            this.NewsInstaller.InstallPackage(new SemanticVersion(ShortVersion10));
            this.NewsInstaller.InstallPackage(new SemanticVersion(ShortVersion11));

            ShouldContainLogEntry(this.NewsInstaller.Logs, Setup);
            ShouldContainLogEntry(this.NewsInstaller.Logs, Installed);
            ShouldContainLogEntry(this.NewsInstaller.Logs, Install);
            ShouldContainLogEntry(this.NewsInstaller.Logs, Uninstall);
            ShouldContainLogEntry(this.NewsInstaller.Logs, Uninstalled);
            ShouldContainLogEntry(this.NewsInstaller.Logs, Setup);
            ShouldContainLogEntry(this.NewsInstaller.Logs, Installed);
            ShouldContainLogEntry(this.NewsInstaller.Logs, Install);

            File.Exists(Path.Combine(this.PackagePath, DummyNews10Folder, DummyNews10File)).Should().BeFalse();
            File.Exists(Path.Combine(this.PackagePath, DummyNews11Folder, DummyNews11File)).Should().BeTrue();
            var installPath = Path.Combine(this.PackagePath, DummyNews11Folder);
            var contentPath = Path.Combine(installPath, "content");
            GetFileVersion(contentPath).Should().Be(Version11);
            GetWebsiteVersion(contentPath).Should().Be(Version11);
        }

        [Test]
        public void ShouldNotInstallTheSameVersionOfThePackage()
        {
            var version = new SemanticVersion(ShortVersion10);
            this.NewsInstaller.InstallPackage(version);
            this.NewsInstaller.InstallPackage(version);

            ShouldContainLogEntry(this.NewsInstaller.Logs, Setup);
            ShouldContainLogEntry(this.NewsInstaller.Logs, Installed);
            ShouldContainLogEntry(this.NewsInstaller.Logs, Install);
            ShouldContainLogEntry(this.NewsInstaller.Logs, Already);
        }

        [Test]
        public void ShouldUninstallVersion10Package()
        {
            var version = new SemanticVersion(ShortVersion10);
            this.NewsInstaller.InstallPackage(version);
            this.NewsInstaller.UninstallPackage(version);

            ShouldContainLogEntry(this.NewsInstaller.Logs, Setup);
            ShouldContainLogEntry(this.NewsInstaller.Logs, Installed);
            ShouldContainLogEntry(this.NewsInstaller.Logs, Install);
            ShouldContainLogEntry(this.NewsInstaller.Logs, Uninstall);
            ShouldContainLogEntry(this.NewsInstaller.Logs, Uninstalled);
            ShouldContainLogEntry(this.NewsInstaller.Logs, Teardown);
            Directory.Exists(this.InstallationPath).Should().BeFalse(PackageShouldBeInstalled);
        }

        [Test]
        public void ShouldUninstallVersion11Package()
        {
            var version = new SemanticVersion(ShortVersion11);
            this.NewsInstaller.InstallPackage(version);
            this.NewsInstaller.UninstallPackage(version);

            ShouldContainLogEntry(this.NewsInstaller.Logs, Setup);
            ShouldContainLogEntry(this.NewsInstaller.Logs, Installed);
            ShouldContainLogEntry(this.NewsInstaller.Logs, Install);
            ShouldContainLogEntry(this.NewsInstaller.Logs, Uninstall);
            ShouldContainLogEntry(this.NewsInstaller.Logs, Uninstalled);
            ShouldContainLogEntry(this.NewsInstaller.Logs, Teardown);
            Directory.Exists(this.InstallationPath).Should().BeFalse(PackageShouldBeInstalled);
        }

        private static void ShouldContainLogEntry(IEnumerable<string> logs, string entry)
        {
            var query = logs.Where(s => s.Contains(entry, StringComparison.CurrentCultureIgnoreCase));
            var foundEntry = query.FirstOrDefault();

            foundEntry.Should().NotBeNullOrEmpty("The logs should contain: " + entry);
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