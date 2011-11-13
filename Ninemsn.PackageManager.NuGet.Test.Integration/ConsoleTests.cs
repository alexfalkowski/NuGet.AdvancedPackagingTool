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

            if (Directory.Exists(this.PackagePath))
            {
                Directory.Delete(this.PackagePath, true);
            }
        }

        [TearDown]
        public void TearDown()
        {
            this.server.Stop();
        }

        [Test]
        public void ShouldInstallAndUninstallFirstVersionPackage()
        {
            RunPackageManagerProcess("/i /p DummyNews /d DummyNews /v 1.0");

            Directory.EnumerateDirectories(this.PackagePath, "DummyNews.1.0").Any().Should().BeTrue(
                "The package DummyNews should be installed.");

            RunPackageManagerProcess("/u /p DummyNews /d DummyNews /v 1.0");

            Directory.Exists(this.PackagePath).Should().BeFalse("The package DummyNews should not be installed.");
        }

        [Test]
        public void ShouldInstallAndUninstallLatestVersionPackage()
        {
            RunPackageManagerProcess("/i /p DummyNews /d DummyNews");

            Directory.EnumerateDirectories(this.PackagePath, "DummyNews.1.1").Any().Should().BeTrue(
                "The package DummyNews should be installed.");

            RunPackageManagerProcess("/u /p DummyNews /d DummyNews");

            Directory.Exists(this.PackagePath).Should().BeFalse("The package DummyNews should not be installed.");
        }

        [Test]
        public void ShouldUpgradeAlreadyInstalledPackageAndUninstall()
        {
            RunPackageManagerProcess("/i /p DummyNews /d DummyNews /v 1.0");

            Directory.EnumerateDirectories(this.PackagePath, "DummyNews.1.0").Any().Should().BeTrue(
                "The package DummyNews should be installed.");

            RunPackageManagerProcess("/i /p DummyNews /d DummyNews /v 1.1");

            Directory.EnumerateDirectories(this.PackagePath, "DummyNews.1.1").Any().Should().BeTrue(
                "The package DummyNews should be installed.");

            RunPackageManagerProcess("/u /p DummyNews /d DummyNews");

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