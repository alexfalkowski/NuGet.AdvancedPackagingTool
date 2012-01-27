namespace Ninemsn.PackageManager.NuGet.Test.Integration
{
    using FluentAssertions;

    using global::NuGet;

    using NUnit.Framework;

    [TestFixture]
    public class PackageManagerModuleTest
    {
        private const string FirstSourceName = "TestRemoteFeed";

        private const string FirstFeedUrl = "http://localhost:1544/DataServices/Packages.svc";

        private PackageManagerModule module;

        [SetUp]
        public void SetUp()
        {
            var packageSourceFile = PackageSourceFileFactory.CreatePackageSourceFile();
            this.module = new PackageManagerModule(packageSourceFile);
        }

        [Test]
        public void ShouldHavePackageSources()
        {
            this.module.PackageSources.Should().HaveCount(4);
        }

        [Test]
        public void ShouldHaveActiveSource()
        {
            var packageSource = this.module.ActiveSource;

            ShouldBeDefaultSourcePackage(packageSource);
        }

        [Test]
        public void ShouldHaveDefaultPackageSourceName()
        {
            var packageSource = this.module.GetSource(FirstSourceName);

            ShouldBeDefaultSourcePackage(packageSource);
        }

        private static void ShouldBeDefaultSourcePackage(PackageSource packageSource)
        {
            packageSource.Should().NotBeNull();
            packageSource.Name.Should().Be(FirstSourceName);
            packageSource.Source.Should().Be(FirstFeedUrl);
        }
    }
}
