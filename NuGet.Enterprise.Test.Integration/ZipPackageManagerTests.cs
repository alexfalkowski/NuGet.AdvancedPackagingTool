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
            using (var package = new ZipPackage("DummyNews.1.0.nupkg"))
            {
                var collection = new Collection<string>();
                var sourceRepository = new DiskPackageRepository(this.source.Source);
                var localRepository =
                    new DiskPackageRepository(this.packagePath);
                var manager = new ZipPackageManager(
                    localRepository,
                    sourceRepository,
                    new DefaultFileSystem(this.installationPath, true),
                    new ZipPackagePathResolver());

                SetupAllEvents(manager, collection);
                manager.InstallPackage(package, true, true);
                AssertInstallEventsWereCalled(collection);
                Directory.EnumerateFiles(this.installationPath, "*.*", SearchOption.AllDirectories).Count().Should().Be(16);
            }
        }

        [Test]
        public void ShouldInstallSpecificVersionPackage()
        {
            var collection = new Collection<string>();
            var sourceRepository = new DiskPackageRepository(this.source.Source);
            var localRepository =
                new DiskPackageRepository(this.packagePath);
            var manager = new ZipPackageManager(
                localRepository,
                sourceRepository,
                new DefaultFileSystem(this.installationPath, true),
                new ZipPackagePathResolver());

            SetupAllEvents(manager, collection);
            manager.InstallPackage("DummyNews", new SemanticVersion("1.0"), true, true);
            AssertInstallEventsWereCalled(collection);
            Directory.EnumerateFiles(this.installationPath, "*.*", SearchOption.AllDirectories).Count().Should().Be(16);
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
    }
}