namespace Ninemsn.PackageManager.NuGet.Test.Unit
{
    using System;

    using FluentAssertions;

    using Ninemsn.PackageManager.NuGet.App;

    using NUnit.Framework;

    [TestFixture]
    public class ConsoleTests
    {
        [Test]
        public void ShouldNotAllowInstallAndUninstallOptions()
        {
            var arguments = new Arguments { Install = true, Uninstall = true };
            var program = new Program(arguments);
            Action action = program.Start;

            action.ShouldThrow<ArgumentException>("The program should not allow the install flag to be set along with the uninstall flag.");
        }
    }
}