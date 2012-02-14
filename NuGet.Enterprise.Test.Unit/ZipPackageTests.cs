namespace NuGet.Enterprise.Test.Unit
{
    using System;
    using System.Linq;

    using FluentAssertions;

    using NuGet.Enterprise.Core;

    using NUnit.Framework;

    [TestFixture]
    public static class ZipPackageTests
    {
        [Test]
        public static void ShouldReturnFiles()
        {
            using (var package = new ZipPackage("DummyNews.1.0.nupkg"))
            {
                package.GetFiles().Count().Should().Be(22);
            }
        }

        [Test]
        public static void ShouldNotReturnModuleFiles()
        {
            using (var package = new ZipPackage("DummyNews.1.0.nupkg"))
            {
                package.GetModuleFiles().Count().Should().Be(0);
            }
        }

        [Test]
        public static void ShouldReturnPowerShellFiles()
        {
            using (var package = new ZipPackage("DummyNews.1.0.nupkg"))
            {
                package.GetInstallPackageFile().GetType().Should().Be<ZipPackageFile>();
                package.GetUninstallPackageFile().GetType().Should().Be<ZipPackageFile>();
                package.GetSetupPackageFile().GetType().Should().Be<ZipPackageFile>();
                package.GetTeardownPackageFile().GetType().Should().Be<ZipPackageFile>();
            }
        }

        [Test]
        public static void ShouldHaveValidInformation()
        {
            using (var package = new ZipPackage("DummyNews.1.0.nupkg"))
            {
                package.Id.Should().Be("DummyNews");
                package.Version.Should().Be(new SemanticVersion("1.0"));
                package.ProjectUrl.Should().Be(new Uri("file:///C:/Ninemsn/TestInstallPackage/DummyNews/"));
                package.AssemblyReferences.Count().Should().Be(0);
            }
        }
    }
}