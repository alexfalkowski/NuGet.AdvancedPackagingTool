﻿namespace Ninemsn.PackageManager.NuGet.Test.Integration
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using FluentAssertions;

    using global::NuGet;

    using NUnit.Framework;

    public class LocalProjectManagerDefaultProjectSystemTest
    {
        private PackageManagerModule module;

        private NuGet.ProjectManager manager;

        private PackagesWebServer server;

        [SetUp]
        public void SetUp()
        {
            var packageSourceFile = new PackageSourceFile("Integration/PackageSources.config");
            this.module = new PackageManagerModule(packageSourceFile);
            var localSourceUri = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase) + "/App_Data/packages";
            var localSource = new Uri(localSourceUri).LocalPath;
            var installationPath = Path.Combine(localSource, "DummyNews");
            var webProjectSystem = new DefaultProjectSystem(localSource, installationPath);
            this.manager = new NuGet.ProjectManager(this.module.GetSource("LocalFeed").Source, localSource, webProjectSystem);

            Directory.Delete(localSource, true);
            Directory.CreateDirectory(localSource);
            Directory.CreateDirectory(installationPath);

            this.server = new PackagesWebServer();
            this.server.StartUp();
        }

        [TearDown]
        public void TearDown()
        {
            this.server.Stop();
        }

        [Test]
        public void ShouldContainSourceRepositoryPackages()
        {
            var packages = this.manager.SourceRepository.GetPackages();

            packages.Count().Should().Be(1);
            packages.FirstOrDefault().Should().NotBeNull();
        }

        [Test]
        public void ShouldInstallLocalPackageInLocalRepository()
        {
            var sourcePackage = this.GetPackage(this.manager.SourceRepository);

            this.manager.InstallPackage(sourcePackage);

            var localPackage = this.GetPackage(this.manager.LocalRepository);
            localPackage.Should().NotBeNull();
        }

        private IPackage GetPackage(IPackageRepository repository)
        {
            var packages = repository.GetPackages();

            return packages.FirstOrDefault();
        }
    }
}