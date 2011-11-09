namespace Ninemsn.PackageManager.NuGet.Test.Integration
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using FluentAssertions;

    using Ninemsn.PackageManager.NuGet.Web;

    using global::NuGet;

    using NUnit.Framework;

    [TestFixture]
    public class WebProjectManagerTest
    {
        private PackageManagerModule module;

        private WebProjectManager manager;

        [SetUp]
        public void SetUp()
        {
            var packageSourceFile = new PackageSourceFile("Integration/PackageSources.config");
            this.module = new PackageManagerModule(packageSourceFile);
            var localSourceUri = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase) + "/App_Data/packages";
            var localSource = new Uri(localSourceUri).LocalPath;
            var installationPath = Path.Combine(localSource, "NuGet");
            this.manager = new WebProjectManager(this.module.ActiveSource.Source, localSource, installationPath);

            Directory.Delete(localSource, true);
            Directory.CreateDirectory(localSource);
        }

        [Test]
        public void ShouldContainLocalRepository()
        {
            this.manager.LocalRepository.Should().NotBeNull();
        }

        [Test]
        public void ShouldContainSourceRepository()
        {
            this.manager.SourceRepository.Should().NotBeNull();
        }

        [Test]
        public void ShouldContainSourceRepositoryPackages()
        {
            this.manager.SourceRepository.GetPackages().FirstOrDefault().Should().NotBeNull();
        }

        [Test]
        public void ShouldContainLocalRepositoryPackages()
        {
            this.manager.LocalRepository.GetPackages().FirstOrDefault().Should().BeNull();
        }

        [Test]
        public void ShouldContainSourceRepositoryNuGetPackages()
        {
            var package = this.GetPackage(this.manager.SourceRepository, "NuGet");
            package.Should().NotBeNull();
        }

        [Test]
        public void ShouldInstallNuGetPackageInLocalRepository()
        {
            var sourcePackage = this.GetPackage(this.manager.SourceRepository, "NuGet");

            this.manager.InstallPackage(sourcePackage);

            var localPackage = this.GetPackage(this.manager.LocalRepository, "NuGet");
            localPackage.Should().NotBeNull();

            this.manager.LocalRepository.GetPackages().FirstOrDefault().Should().NotBeNull();
        }

        private IPackage GetPackage(IPackageRepository repository, string packageName)
        {
            var packages = this.manager.GetPackages(repository, packageName);

            return packages.FirstOrDefault();
        }
    }
}