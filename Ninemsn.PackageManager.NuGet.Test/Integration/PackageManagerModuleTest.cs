﻿namespace Ninemsn.PackageManager.NuGet.Test.Integration
{
    using System.Web;

    using FluentAssertions;

    using Ninemsn.PackageManager.NuGet.Web;

    using NSubstitute;

    using global::NuGet;

    using NUnit.Framework;

    [TestFixture]
    public class PackageManagerModuleTest
    {
        private const string DefaultSourceName = "DefaultPackageSourceName";

        private PackageManagerModule module;

        [SetUp]
        public void SetUp()
        {
            var httpContext = Substitute.For<HttpContextBase>();
            var packageSourceFile = new PackageSourceFile("Integration/PackageSources.config");
            this.module = new PackageManagerModule(httpContext, packageSourceFile);
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
            var packageSource = this.module.GetSource(DefaultSourceName);

            ShouldBeDefaultSourcePackage(packageSource);
        }

        private static void ShouldBeDefaultSourcePackage(PackageSource packageSource)
        {
            packageSource.Should().NotBeNull();
            packageSource.Name.Should().Be(DefaultSourceName);
            packageSource.Source.Should().Be("http://go.microsoft.com/fwlink/?LinkID=206971");
        }
    }
}
