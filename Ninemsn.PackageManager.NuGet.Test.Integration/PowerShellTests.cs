namespace Ninemsn.PackageManager.NuGet.Test.Integration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using FluentAssertions;

    using NSubstitute;

    using global::NuGet;

    using NUnit.Framework;

    [TestFixture]
    public class PowerShellTests
    {
        private PackageLogger logger;

        private IPackageFile packageFile;

        private IPackage package;

        [SetUp]
        public void SetUp()
        {
            this.package = Substitute.For<IPackage>();
            this.package.ProjectUrl.Returns(
                new Uri(
                    "file:///C:/Ninemsn/PackageManager/Ninemsn.PackageManager.NuGet.Test.Integration/bin/Debug/App_Data/packages/DummyNews/"));

            this.logger = new PackageLogger();
            this.packageFile = Substitute.For<IPackageFile>();
        }

        [Test]
        public void ShouldExecuteWebInstall()
        {
            this.packageFile.GetStream().Returns(new FileStream(@"Scripts/Web/Install.ps1", FileMode.Open));
            this.packageFile.ExecutePowerShell(this.package, this.logger);
            GetLog(this.logger.Logs).Should().Contain("Installed site www.dummynews.com.au");
        }

        [Test]
        public void ShouldExecuteWebUninstall()
        {
            this.packageFile.GetStream().Returns(new FileStream(@"Scripts/Web/Uninstall.ps1", FileMode.Open));
            this.packageFile.ExecutePowerShell(this.package, this.logger);
            GetLog(this.logger.Logs).Should().Contain("Uninstalled site www.dummynews.com.au");
        }

        private static string GetLog(IEnumerable<string> logs)
        {
            var builder = new StringBuilder();

            foreach (var log in logs)
            {
                builder.AppendLine(log);
            }

            return builder.ToString();
        }
    }
}