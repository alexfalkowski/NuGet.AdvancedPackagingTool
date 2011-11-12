namespace Ninemsn.PackageManager.NuGet.Test.Integration
{
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using FluentAssertions;

    using NUnit.Framework;

    [TestFixture]
    public class ConsoleTests
    {
        private PackagesWebServer server;

        public string PackagePath
        {
            get
            {
                return ConfigurationManager.AppSettings["PackagePath"];
            }
        }

        [SetUp]
        public void SetUp()
        {
            this.server = new PackagesWebServer();
            this.server.StartUp();
        }

        [TearDown]
        public void TearDown()
        {
            this.server.Stop();
        }

        [Test]
        public void ShouldInstallAndUninstallPackage()
        {
            RunPackageManagerProcess("/i /p DummyNews /s LocalFeed /d DummyNews");

            Directory.EnumerateDirectories(this.PackagePath, "DummyNews.1.0").Any().Should().BeTrue(
                "The package DummyNews should be installed.");

            RunPackageManagerProcess("/u /p DummyNews /s LocalFeed /d DummyNews");

            Directory.Exists(this.PackagePath).Should().BeFalse("The package DummyNews should not be installed.");
        }

        private static void RunPackageManagerProcess(string arguments)
        {
            var process = Process.Start("npm.exe", arguments);

            if (process == null)
            {
                return;
            }

            process.WaitForExit();
        }
    }
}