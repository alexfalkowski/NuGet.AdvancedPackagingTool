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
            var packageSourceFile = new PackageSourceFileFactory().CreatePackageSourceFile();
            this.Module = new PackageManagerModule(packageSourceFile);
            this.PackagePath = new Uri("file:///C:/NuGet/TestInstallPackage/").LocalPath;
            this.InstallationPath = Path.Combine(this.PackagePath, "DummyNews");

            this.Installer = new ValidPackageInstaller(
                new LocalPackageRepository(this.Module.GetSource("TestLocalFeed").Source),
                this.PackagePath, 
                "DummyNews");

            if (Directory.Exists(this.PackagePath))
            {
                Directory.Delete(this.PackagePath, true);
            }
        }
    }
}