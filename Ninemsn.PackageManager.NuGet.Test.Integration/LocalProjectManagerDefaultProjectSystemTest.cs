namespace Ninemsn.PackageManager.NuGet.Test.Integration
{
    using System;
    using System.Collections.Generic;
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
            var packageSourceFile = PackageSourceFileFactory.CreatePackageSourceFile();
            this.module = new PackageManagerModule(packageSourceFile);
            var localSourceUri = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase) + "/App_Data/packages";
            var localSource = new Uri(localSourceUri).LocalPath;
            var installationPath = Path.Combine(localSource, "DummyNews");
            var webProjectSystem = new DefaultProjectSystem(localSource, installationPath);
            this.manager = new NuGet.ProjectManager(
                this.module.GetSource("LocalFeed").Source, localSource, webProjectSystem);

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
        public void ShouldContainContentFolderInPackage()
        {
            var package = GetPackage(this.manager.SourceRepository);
            var toolsFiles = this.manager.GetToolsFiles(package);

            toolsFiles.Count().Should().Be(3);
        }

        [Test]
        public void ShouldContainPowershellScriptsInToolsFolder()
        {
            var package = GetPackage(this.manager.SourceRepository);

            var initPowershellFile = GetToolFile(this.manager.GetToolsFiles(package), "Init.ps1");
            initPowershellFile.Should().NotBeNull();

            var installPowershellFile = GetToolFile(this.manager.GetToolsFiles(package), "Install.ps1");
            installPowershellFile.Should().NotBeNull();

            var unistallPowershellFile = GetToolFile(this.manager.GetToolsFiles(package), "Uninstall.ps1");
            unistallPowershellFile.Should().NotBeNull();
        }

        [Test]
        public void ShouldExecutePowerShellScript()
        {
            var package = GetPackage(this.manager.SourceRepository);
            var toolsFiles = this.manager.GetToolsFiles(package);
            var output = this.manager.ExecutePowerShell(toolsFiles.ToList()[0]);

            output.Should().NotBeNullOrEmpty("There was no output.");
        }

        [Test]
        public void ShouldInstallLocalPackageInLocalRepository()
        {
            var sourcePackage = GetPackage(this.manager.SourceRepository);

            this.manager.InstallPackage(sourcePackage);

            var localPackage = GetPackage(this.manager.LocalRepository);
            localPackage.Should().NotBeNull();
        }

        private static IPackageFile GetToolFile(IEnumerable<IPackageFile> toolsFiles, string fileName)
        {
            var initPowershellFile = toolsFiles.Where(packgeFile => packgeFile.Path.EndsWith(fileName)).FirstOrDefault();
            return initPowershellFile;
        }

        private static IPackage GetPackage(IPackageRepository repository)
        {
            var packages = repository.GetPackages();

            return packages.FirstOrDefault();
        }
    }
}