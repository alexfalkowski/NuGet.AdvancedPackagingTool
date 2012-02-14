namespace NuGet.Enterprise.Test.Unit
{
    using System.Linq;

    using FluentAssertions;

    using NUnit.Framework;

    using NuGet.Enterprise.Core;

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
                package.Id.Should().NotBeNull();
                package.Version.Should().NotBeNull();
                package.Title.Should().NotBeNull();
                package.Authors.Should().NotBeNull();
                package.Owners.Should().NotBeNull();
                package.Should().NotBeNull();
                package.LicenseUrl.Should().NotBeNull();
                package.ProjectUrl.Should().NotBeNull();
                package.RequireLicenseAcceptance.Should().BeTrue();
                package.Description.Should().NotBeNull();
                package.Summary.Should().NotBeNull();
                package.ReleaseNotes.Should().NotBeNull();
                package.Language.Should().NotBeNull();
                package.Tags.Should().NotBeNull();
                package.Copyright.Should().NotBeNull();
                package.FrameworkAssemblies.Should().NotBeNull();
                package.Dependencies.Should().NotBeNull();
                package.ReportAbuseUrl.Should().NotBeNull();
                package.DownloadCount.Should().Be(1);
                package.IsAbsoluteLatestVersion.Should().BeTrue();
                package.IsLatestVersion.Should().BeTrue();
                package.Listed.Should().BeTrue();
                package.Published.Should().NotBeNull();
                package.AssemblyReferences.Should().NotBeNull();
            }
        }
    }
}