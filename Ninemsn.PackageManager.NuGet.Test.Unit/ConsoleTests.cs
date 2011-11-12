namespace Ninemsn.PackageManager.NuGet.Test.Unit
{
    using FluentAssertions;

    using Ninemsn.PackageManager.NuGet.App;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class ConsoleTests
    {
        [Test]
        public void ShouldNotAllowInstallAndUninstallOptions()
        {
            var arguments = new Arguments { Install = true, Uninstall = true };

            arguments.IsValid().Should().BeFalse(
                "The program should not allow the install flag to be set along with the uninstall flag.");
        }

        [Test]
        public void ShouldInstallPackage()
        {
            var arguments = new Arguments { Install = true };
            var installer = Substitute.For<IPackageInstaller>();
            var program = new Program(arguments, installer);

            program.Start();

            installer.Received().InstallPackage();
        }

        [Test]
        public void ShouldUninstallPackage()
        {
            var arguments = new Arguments { Uninstall = true };
            var installer = Substitute.For<IPackageInstaller>();
            var program = new Program(arguments, installer);

            program.Start();

            installer.Received().UninstallPackage();
        }
    }
}