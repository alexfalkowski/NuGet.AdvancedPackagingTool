namespace NuGet.AdvancedPackagingTool.Test.Unit
{
    using FluentAssertions;

    using NuGet.AdvancedPackagingTool.Core;

    using NUnit.Framework;

    [TestFixture]
    public static class SourcePackageRepositoryFactoryTests
    {
        [Test]
        public static void ShouldCreateLocalRepository()
        {
            var factory = new SourcePackageRepositoryFactory(new PackageSource("C:\\Donkey"));
            var repository = factory.CreatePackageRepository();

            repository.Should().BeOfType<LocalPackageRepository>();
        }

        [Test]
        public static void ShouldCreateRemoteRepository()
        {
            var factory = new SourcePackageRepositoryFactory(new PackageSource("http://donkey.com"));
            var repository = factory.CreatePackageRepository();

            repository.Should().BeOfType<DataServicePackageRepository>();
        }
    }
}