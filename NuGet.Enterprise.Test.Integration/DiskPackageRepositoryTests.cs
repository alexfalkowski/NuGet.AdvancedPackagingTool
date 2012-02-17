namespace NuGet.Enterprise.Test.Integration
{
    using System.Linq;

    using FluentAssertions;

    using NuGet.Enterprise.Core;

    using NUnit.Framework;

    [TestFixture]
    public static class DiskPackageRepositoryTests
    {
        [Test]
        public static void ShouldGetPackagesFromSource()
        {
            var repository = new DiskPackageRepository(".");

            repository.GetPackages(packages => packages.Count().Should().Be(1));
        }

        [Test]
        public static void ShouldGetSpecificVersionFromSource()
        {
            var repository = new DiskPackageRepository(".");

            const string PackageId = "DummyNews";
            var semanticVersion = new SemanticVersion("1.0");

            repository.FindPackage(
                PackageId,
                semanticVersion,
                package =>
                    {
                        package.Should().NotBeNull();
                        package.Id.Should().Be(PackageId);
                        package.Version.Should().Be(semanticVersion);
                    });
        }
    }
}