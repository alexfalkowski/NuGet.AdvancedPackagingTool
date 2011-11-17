namespace Ninemsn.PackageManager.NuGet.Test.Integration
{
    using System;
    using System.DirectoryServices;
    using System.IO;
    using System.Linq;

    using FluentAssertions;

    using NSubstitute;

    using global::NuGet;

    using NUnit.Framework;

    [TestFixture]
    public class PowerShellTests
    {
        [Test]
        public void ShouldExecuteWebInstall()
        {
            var uri =
                new Uri(
                    "file:///C:/Ninemsn/PackageManager/Ninemsn.PackageManager.NuGet.Test.Integration/bin/Debug/App_Data/packages/DummyNews/");
            var logger = new PackageLogger();
            var packageFile = Substitute.For<IPackageFile>();

            packageFile.GetStream().Returns(new FileStream(@"Scripts/Web/Install.ps1", FileMode.Open));

            packageFile.ExecutePowerShell(uri, logger);
            logger.Logs.Should().Contain("Created site www.dummynews.com.au");

            DoesWebsiteExist(Environment.MachineName, "DummyNews").Should().BeTrue();
        }

        private static bool DoesWebsiteExist(string serverName, string websiteName)
        {
            var webServer = new DirectoryEntry(string.Format("IIS://{0}/w3svc", serverName));

            var query = from DirectoryEntry site in webServer.Children
                        select site.Properties["ServerComment"]
                        into serverComponent 
                        where serverComponent != null 
                        where serverComponent.Value != null
                        where string.Compare(serverComponent.Value.ToString(), websiteName, false) == 0
                        select serverComponent;

            return query.Any();
        }
    }
}