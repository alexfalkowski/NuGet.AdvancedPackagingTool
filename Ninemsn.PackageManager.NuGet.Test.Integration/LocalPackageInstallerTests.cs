﻿namespace Ninemsn.PackageManager.NuGet.Test.Integration
{
    using System;
    using System.IO;

    using NUnit.Framework;

    [TestFixture]
    public class LocalPackageInstallerTests : PackageInstallerTestsBase
    {
        [SetUp]
        public void SetUp()
        {
            var packageSourceFile = PackageSourceFileFactory.CreatePackageSourceFile();
            this.Module = new PackageManagerModule(packageSourceFile);
            this.PackagePath = new Uri("file:///C:/Ninemsn/TestInstallPackage/").LocalPath;
            this.InstallationPath = Path.Combine(this.PackagePath, "DummyNews");

            this.NewsInstaller = new PackageInstaller(
                this.Module.GetSource("TestLocalFeed"),
                this.PackagePath, 
                "DummyNews");

            this.SitecoreInstaller = new PackageInstaller(
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