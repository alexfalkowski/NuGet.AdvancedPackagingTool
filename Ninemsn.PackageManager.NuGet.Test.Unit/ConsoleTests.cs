namespace Ninemsn.PackageManager.NuGet.Test.Unit
{
    using System.Linq;

    using FluentAssertions;

    using Ninemsn.PackageManager.NuGet.Application;

    using NSubstitute;

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
        public void ShouldInstallPackage()
        {
            var arguments = new Arguments { Install = true, Package = "DummyNews" };
            var installer = Substitute.For<IPackageInstaller>();
            var program = new Console(arguments, installer);

            program.Start();

            installer.Received().InstallPackage();
        }

        [Test]
        public void ShouldUninstallPackage()
        {
            var arguments = new Arguments { Uninstall = true, Package = "DummyNews" };
            var installer = Substitute.For<IPackageInstaller>();
            var program = new Console(arguments, installer);

            program.Start();

            installer.Received().UninstallPackage();
        }
    }
}