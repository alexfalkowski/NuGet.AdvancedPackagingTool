namespace NuGet.Enterprise.Test.Unit
{
    using System.Linq;

    using FluentAssertions;

    using NSubstitute;

    using NuGet;

    using NUnit.Framework;

    using NuGet.Enterprise.Core;

    [TestFixture]
    public static class ConsoleTests
    {
        [Test]
        public static void ShouldNotAllowInstallIfInstallAndUninstallOptionsAreNotSelected()
        {
            var arguments = new Arguments();

            arguments.IsValid.Should().BeFalse(
                "The program should not allow the install if neither install or uninstall option was not selected.");
            arguments.Errors.Count().Should().BeGreaterOrEqualTo(1);
        }

        [Test]
        public static void ShouldNotAllowInstallIfInstallAndUninstallOptionsAreSelected()
        {
            var arguments = new Arguments { Install = true, Uninstall = true };

            arguments.IsValid.Should().BeFalse(
                "The program should not allow the install flag to be set along with the uninstall flag.");
            arguments.Errors.Count().Should().BeGreaterOrEqualTo(1);
        }

        [Test]
        public static void ShouldNotAllowInstallIfPackageNameIsNotSpecified()
        {
            var arguments = new Arguments { Install = true };

            arguments.IsValid.Should().BeFalse(
                "The program should not allow the install if the package is not provided.");
            arguments.Errors.Count().Should().BeGreaterOrEqualTo(1);
        }

        [Test]
        public static void ShouldNotAllowUninstallIfPackageNameIsNotSpecified()
        {
            var arguments = new Arguments { Uninstall = true };

            arguments.IsValid.Should().BeFalse(
                "The program should not allow the uninstall if the package is not provided.");
            arguments.Errors.Count().Should().BeGreaterOrEqualTo(1);
        }

        [Test]
        public static void ShouldAllowInstallIfPackageVersionIsNotSpecified()
        {
            var arguments = new Arguments { Install = true, Package = "DummyNews" };

            arguments.IsValid.Should().BeTrue(
                "The program should allow the install if the package version is not provided.");
            arguments.Errors.Count().Should().Be(0);
        }

        [Test]
        public static void ShouldAllowUninstallIfPackageVersionIsNotSpecified()
        {
            var arguments = new Arguments { Uninstall = true, Package = "DummyNews" };

            arguments.IsValid.Should().BeTrue(
                "The program should allow the uninstall if the package version is not provided.");
            arguments.Errors.Count().Should().Be(0);
        }

        [Test]
        public static void ShouldInstallPackage()
        {
            var version = new SemanticVersion(1, 1, 0, 0);
            var arguments = new Arguments { Install = true, Package = "DummyNews", Version = version };
            var installer = Substitute.For<IPackageInstaller>();
            var program = new Console(arguments, installer);

            program.Start();

            installer.Received().InstallPackage(version);
        }

        [Test]
        public static void ShouldUninstallPackage()
        {
            var version = new SemanticVersion(1, 1, 0, 0);
            var arguments = new Arguments { Uninstall = true, Package = "DummyNews", Version = version };
            var installer = Substitute.For<IPackageInstaller>();
            var program = new Console(arguments, installer);

            program.Start();

            installer.Received().UninstallPackage(version);
        }
    }
}