namespace Ninemsn.PackageManager.NuGet.Test.Integration
{
    using System;
    using System.IO;

    using FluentAssertions;

    using NSubstitute;

    using global::NuGet;

    using NUnit.Framework;

    [TestFixture]
    public class PowerShellTests
    {
        private Uri projectUrl;

        private PackageLogger logger;

        private IPackageFile packageFile;

        [SetUp]
        public void SetUp()
        {
            this.projectUrl =
                new Uri(
                    "file:///C:/Ninemsn/PackageManager/Ninemsn.PackageManager.NuGet.Test.Integration/bin/Debug/App_Data/packages/DummyNews/");

            this.logger = new PackageLogger();
            this.packageFile = Substitute.For<IPackageFile>();
        }

        [Test]
        public void ShouldExecuteWebInstall()
        {
            this.packageFile.GetStream().Returns(new FileStream(@"Scripts/Web/Install.ps1", FileMode.Open));
            this.packageFile.ExecutePowerShell(this.projectUrl, this.logger);
            this.logger.Logs.Should().Contain("Installed site www.dummynews.com.au");
        }

        [Test]
        public void ShouldExecuteWebUninstall()
        {
            this.packageFile.GetStream().Returns(new FileStream(@"Scripts/Web/Uninstall.ps1", FileMode.Open));
            this.packageFile.ExecutePowerShell(this.projectUrl, this.logger);
            this.logger.Logs.Should().Contain("Uninstalled site www.dummynews.com.au");
        }
    }
}