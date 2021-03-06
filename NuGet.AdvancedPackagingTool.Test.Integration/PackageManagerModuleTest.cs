﻿namespace NuGet.AdvancedPackagingTool.Test.Integration
{
    using FluentAssertions;

    using NuGet;
    using NuGet.AdvancedPackagingTool.Core;

    using NUnit.Framework;

    [TestFixture]
    public class PackageManagerModuleTest
    {
        private const string FirstSourceName = "TestLocalFeed";

        private const string FirstFeedUrl = @"C:\Projects\OSS\NuGet.AdvancedPackagingTool\NuGet.AdvancedPackagingTool.Service\Packages\";

        private PackageManagerModule module;

        [SetUp]
        public void Setup()
        {
            var packageSourceFile = new PackageSourceFileFactory().CreatePackageSourceFile();
            this.module = new PackageManagerModule(packageSourceFile);
        }

        [Test]
        public void ShouldHavePackageSources()
        {
            this.module.PackageSources.Should().HaveCount(2);
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
