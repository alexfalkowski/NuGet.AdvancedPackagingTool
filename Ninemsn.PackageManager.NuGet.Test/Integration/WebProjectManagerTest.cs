namespace Ninemsn.PackageManager.NuGet.Test.Integration
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Web;

    using FluentAssertions;

    using Ninemsn.PackageManager.NuGet.Web;

    using NSubstitute;

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
            var httpContext = Substitute.For<HttpContextBase>();
            var packageSourceFile = new PackageSourceFile("Integration/PackageSources.config");
            this.module = new PackageManagerModule(httpContext, packageSourceFile);
            var localSourceUri = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase) + "/App_Data/packages";
            var localSource = new Uri(localSourceUri).LocalPath;
            this.manager = new WebProjectManager(this.module.ActiveSource.Source, localSource);

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
        public void ShouldContainSourceRepositoryNLogPackages()
        {
            var package = this.GetPackage(this.manager.SourceRepository, "NLog");
            package.Should().NotBeNull();
        }

        [Test]
        public void ShouldInstallNLogPackageInLocalRepository()
        {
            var sourcePackage = this.GetPackage(this.manager.SourceRepository, "NLog");

            this.manager.InstallPackage(sourcePackage);

            var localPackage = this.GetPackage(this.manager.LocalRepository, "NLog");
            localPackage.Should().NotBeNull();
        }

        private IPackage GetPackage(IPackageRepository repository, string packageName)
        {
            var packages = this.manager.GetPackages(repository, packageName);

            return packages.FirstOrDefault();
        }
    }
}