namespace Ninemsn.PackageManager.NuGet.Test.Integration
{
    using System;
    using System.IO;
    using System.Reflection;

    using NUnit.Framework;

    [TestFixture]
    public class LocalPackageInstallerTests : PackageInstallerTestsBase
    {
        [SetUp]
        public void SetUp()
        {
            var packageSourceFile = PackageSourceFileFactory.CreatePackageSourceFile();
            this.Module = new PackageManagerModule(packageSourceFile);
            var localSourceUri = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase) + "/App_Data/packages";
            this.PackagePath = new Uri(localSourceUri).LocalPath;
            this.InstallationPath = Path.Combine(this.PackagePath, "DummyNews");

            this.Installer = new PackageInstaller(
                this.Module.GetSource("LocalFolder"),
                this.PackagePath, 
                "DummyNews", 
                this.InstallationPath);

            if (Directory.Exists(this.PackagePath))
            {
                Directory.Delete(this.PackagePath, true);
            }
        }
    }
}