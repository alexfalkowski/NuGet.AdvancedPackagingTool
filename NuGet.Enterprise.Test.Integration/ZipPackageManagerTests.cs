namespace NuGet.Enterprise.Test.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;

    using FluentAssertions;

    using NuGet.Enterprise.Core;

    using NUnit.Framework;

    [TestFixture]
    public class ZipPackageManagerTests
    {
        private const string PackageFileName = "DummyNews.1.0.nupkg";

        private PackageManagerModule module;

        private PackageSource source;

        private string packagePath;

        private string installationPath;

        [SetUp]
        public void Setup()
        {
            this.module = new PackageManagerModule(PackageSourceFileFactory.CreatePackageSourceFile());
            this.packagePath = new Uri("file:///C:/Ninemsn/TestInstallPackage/").LocalPath;
            this.installationPath = Path.Combine(this.packagePath, "DummyNews");
            this.source = this.module.GetSource("TestLocalFeed");

            if (Directory.Exists(this.packagePath))
            {
                Directory.Delete(this.packagePath, true);
            }
        }
 
        [Test]
        public void ShouldInstallExistingPackage()
        {
            using (var package = new ZipPackage(PackageFileName))
            {
                var collection = new Collection<string>();
                var sourceRepository = new DiskPackageRepository(this.source.Source);
                var localRepository =
                    new DiskPackageRepository(this.packagePath);
                var defaultPackagePathResolver = new DefaultPackagePathResolver(this.packagePath);
                var manager = new ZipPackageManager(
                    localRepository,
                    sourceRepository,
                    new DefaultFileSystem(this.installationPath, true),
                    defaultPackagePathResolver);

                SetupAllEvents(manager, collection);
                manager.InstallPackage(package, true, true);
                AssertInstallEventsWereCalled(collection);
                Directory.EnumerateFiles(this.installationPath, "*.*", SearchOption.AllDirectories).Count().Should().Be(16);
                File.Exists(defaultPackagePathResolver.GetInstallFileName(package)).Should().BeTrue();
            }
        }

        [Test]
        public void ShouldUninstallExistingPackage()
        {
            using (var package = new ZipPackage(PackageFileName))
            {
                var collection = new Collection<string>();
                var sourceRepository = new DiskPackageRepository(this.source.Source);
                var localRepository =
                    new DiskPackageRepository(this.packagePath);
                var defaultPackagePathResolver = new DefaultPackagePathResolver(this.packagePath);
                var manager = new ZipPackageManager(
                    localRepository,
                    sourceRepository,
                    new DefaultFileSystem(this.installationPath, true),
                    defaultPackagePathResolver);

                manager.InstallPackage(package, true, true);
                SetupAllEvents(manager, collection);
                manager.UninstallPackage(package, true, true);
                AssertUninstallEventsWereCalled(collection);
                Directory.Exists(this.installationPath).Should().BeFalse();
                File.Exists(defaultPackagePathResolver.GetInstallFileName(package)).Should().BeFalse();
            }
        }

        [Test]
        public void ShouldInstallSpecificVersionPackage()
        {
            var collection = new Collection<string>();
            var sourceRepository = new DiskPackageRepository(this.source.Source);
            var localRepository =
                new DiskPackageRepository(this.packagePath);
            var defaultPackagePathResolver = new DefaultPackagePathResolver(this.packagePath);
            var manager = new ZipPackageManager(
                localRepository,
                sourceRepository,
                new DefaultFileSystem(this.installationPath, true),
                defaultPackagePathResolver);

            SetupAllEvents(manager, collection);
            manager.InstallPackage("DummyNews", new SemanticVersion("1.0"), true, true);
            AssertInstallEventsWereCalled(collection);
            Directory.EnumerateFiles(this.installationPath, "*.*", SearchOption.AllDirectories).Count().Should().Be(16);
            File.Exists(Path.Combine(this.packagePath, "DummyNews.1.0", PackageFileName)).Should().BeTrue();
        }

        [Test]
        public void ShouldUninstallSpecificVersionPackage()
        {
            var collection = new Collection<string>();
            var sourceRepository = new DiskPackageRepository(this.source.Source);
            var localRepository =
                new DiskPackageRepository(this.packagePath);
            var defaultPackagePathResolver = new DefaultPackagePathResolver(this.packagePath);
            var manager = new ZipPackageManager(
                localRepository,
                sourceRepository,
                new DefaultFileSystem(this.installationPath, true),
                defaultPackagePathResolver);

            const string PackageId = "DummyNews";
            var version1 = new SemanticVersion("1.0");
            manager.InstallPackage(PackageId, version1, true, true);
            SetupAllEvents(manager, collection);
            manager.UninstallPackage(PackageId, version1, true, true);
            AssertUninstallEventsWereCalled(collection);
            Directory.Exists(this.installationPath).Should().BeFalse();
            File.Exists(Path.Combine(this.packagePath, "DummyNews.1.0", PackageFileName)).Should().BeFalse();
        }

        private static void SetupAllEvents(IPackageManager manager, ICollection<string> list)
        {
            manager.PackageInstalling += (sender, e) => list.Add("PackageInstalling");
            manager.PackageInstalled += (sender, e) => list.Add("PackageInstalled");
            manager.PackageUninstalling += (sender, e) => list.Add("PackageUninstalling");
            manager.PackageUninstalled += (sender, e) => list.Add("PackageUninstalled");
        }

        private static void AssertInstallEventsWereCalled(IList<string> list)
        {
            list[0].Should().Be("PackageInstalling");
            list[1].Should().Be("PackageInstalled");
        }

        private static void AssertUninstallEventsWereCalled(IList<string> list)
        {
            list[0].Should().Be("PackageUninstalling");
            list[1].Should().Be("PackageUninstalled");
        }
    }
}