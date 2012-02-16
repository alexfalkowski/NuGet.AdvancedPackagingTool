namespace NuGet.Enterprise.Test.Integration
{
    using System.Collections.Generic;
    using System.Linq;

    using FluentAssertions;

    using NUnit.Framework;

    using NuGet.Enterprise.Core;

    [TestFixture]
    public static class DiskPackageRepositoryTests
    {
        [Test]
        public static void ShouldGetPackagesFromSource()
        {
            var repository = new DiskPackageRepository(".");

            var packages = repository.GetPackages();
            packages.Count().Should().Be(1);

            DisposePackages(packages);
        }

        private static void DisposePackages(IEnumerable<IPackage> packages)
        {
            foreach (ZipPackage package in packages)
            {
                package.Dispose();
            }
        }
    }
}