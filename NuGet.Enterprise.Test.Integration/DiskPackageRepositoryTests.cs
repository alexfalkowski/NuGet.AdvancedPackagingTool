namespace NuGet.Enterprise.Test.Integration
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using FluentAssertions;

    using NuGet.Enterprise.Core;

    using NUnit.Framework;

    [TestFixture]
    public static class DiskPackageRepositoryTests
    {
        private static string CurrentDirectoryPath
        {
            get
            {
                var currentDirectoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);

                if (currentDirectoryPath == null)
                {
                    throw ExceptionFactory.CreateInvalidOperationException(string.Empty);
                }

                return new Uri(currentDirectoryPath).LocalPath;
            }
        }

        [Test]
        public static void ShouldGetPackagesFromSource()
        {
            var repository = new DiskPackageRepository(CurrentDirectoryPath);

            repository.GetPackages(packages => packages.Count().Should().Be(1));
        }

        [Test]
        public static void ShouldGetSpecificVersionFromSource()
        {
            var repository = new DiskPackageRepository(CurrentDirectoryPath);

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