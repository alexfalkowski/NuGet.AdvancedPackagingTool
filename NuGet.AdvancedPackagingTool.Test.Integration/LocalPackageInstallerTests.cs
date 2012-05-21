namespace NuGet.AdvancedPackagingTool.Test.Integration
{
    using System;
    using System.IO;

    using NuGet.AdvancedPackagingTool.Core;

    using NUnit.Framework;

    [TestFixture]
    public class LocalPackageInstallerTests : PackageInstallerTestsBase
    {
        [SetUp]
        public void Setup()
        {
            var packageSourceFile = PackageSourceFileFactory.CreatePackageSourceFile();
            this.Module = new PackageManagerModule(packageSourceFile);
            this.PackagePath = new Uri("file:///C:/NuGet/TestInstallPackage/").LocalPath;
            this.InstallationPath = Path.Combine(this.PackagePath, "DummyNews");

            this.NewsInstaller = new ZipPackageInstaller(
                this.Module.GetSource("TestLocalFeed"),
                this.PackagePath, 
                "DummyNews");

            this.SitecoreInstaller = new ZipPackageInstaller(
                this.Module.GetSource("TestLocalFeed"),
                this.PackagePath,
                "DummySitecore");

            if (Directory.Exists(this.PackagePath))
            {
                Directory.Delete(this.PackagePath, true);
            }
        }
    }
}