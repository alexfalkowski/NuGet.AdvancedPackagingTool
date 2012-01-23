﻿namespace Ninemsn.PackageManager.NuGet.Test.Unit
{
    using System.Linq;

    using FluentAssertions;

    using Ninemsn.PackageManager.NuGet.Application;

    using NSubstitute;

    using global::NuGet;

    using NUnit.Framework;

    [TestFixture]
    public class ConsoleTests
    {
        [Test]
        public void ShouldNotAllowInstallIfInstallAndUninstallOptionsAreNotSelected()
        {
            var arguments = new Arguments();

            arguments.IsValid.Should().BeFalse(
                "The program should not allow the install if neither install or uninstall option was not selected.");
            arguments.Errors.Count().Should().BeGreaterOrEqualTo(1);
        }

        [Test]
        public void ShouldNotAllowInstallIfInstallAndUninstallOptionsAreSelected()
        {
            var arguments = new Arguments { Install = true, Uninstall = true };

            arguments.IsValid.Should().BeFalse(
                "The program should not allow the install flag to be set along with the uninstall flag.");
            arguments.Errors.Count().Should().BeGreaterOrEqualTo(1);
        }

        [Test]
        public void ShouldNotAllowInstallIfPackageNameIsNotSpecified()
        {
            var arguments = new Arguments { Install = true };

            arguments.IsValid.Should().BeFalse(
                "The program should not allow the install if the package is not provided.");
            arguments.Errors.Count().Should().BeGreaterOrEqualTo(1);
        }

        [Test]
        public void ShouldNotAllowUninstallIfPackageNameIsNotSpecified()
        {
            var arguments = new Arguments { Uninstall = true };

            arguments.IsValid.Should().BeFalse(
                "The program should not allow the uninstall if the package is not provided.");
            arguments.Errors.Count().Should().BeGreaterOrEqualTo(1);
        }

        [Test]
        public void ShouldNotAllowInstallIfPackageVersionIsNotSpecified()
        {
            var arguments = new Arguments { Install = true, Package = "DummyNews" };

            arguments.IsValid.Should().BeFalse(
                "The program should not allow the install if the package version is not provided.");
            arguments.Errors.Count().Should().BeGreaterOrEqualTo(1);
        }

        [Test]
        public void ShouldNotAllowUninstallIfPackageVersionIsNotSpecified()
        {
            var arguments = new Arguments { Uninstall = true, Package = "DummyNews" };

            arguments.IsValid.Should().BeFalse(
                "The program should not allow the uninstall if the package version is not provided.");
            arguments.Errors.Count().Should().BeGreaterOrEqualTo(1);
        }

        [Test]
        public void ShouldInstallPackage()
        {
            var version = new SemanticVersion(1, 1, 0, 0);
            var arguments = new Arguments { Install = true, Package = "DummyNews", Version = version };
            var installer = Substitute.For<IPackageInstaller>();
            var program = new Console(arguments, installer);

            program.Start();

            installer.Received().InstallPackage(version);
        }

        [Test]
        public void ShouldUninstallPackage()
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