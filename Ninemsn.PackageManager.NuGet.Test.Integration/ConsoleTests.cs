namespace Ninemsn.PackageManager.NuGet.Test.Integration
{
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using FluentAssertions;

    using Ninemsn.PackageManager.NuGet.Configuration;

    using NUnit.Framework;

    [TestFixture]
    public class ConsoleTests
    {
        private PackagesWebServer server;

        [SetUp]
        public void SetUp()
        {
            this.server = new PackagesWebServer();
            this.server.StartUp();

            if (Directory.Exists(ConfigurationManager.PackagePath))
            {
                Directory.Delete(ConfigurationManager.PackagePath, true);
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

            Directory.EnumerateDirectories(ConfigurationManager.PackagePath, "DummyNews.1.0").Any().Should().BeTrue(
                "The package DummyNews should be installed.");

            RunPackageManagerProcess("/u /p DummyNews /d DummyNews /v 1.0");

            Directory.Exists(ConfigurationManager.PackagePath).Should().BeFalse("The package DummyNews should not be installed.");
        }

        [Test]
        public void ShouldInstallAndUninstallLatestVersionPackage()
        {
            RunPackageManagerProcess("/i /p DummyNews /d DummyNews");

            Directory.EnumerateDirectories(ConfigurationManager.PackagePath, "DummyNews.1.1").Any().Should().BeTrue(
                "The package DummyNews should be installed.");

            RunPackageManagerProcess("/u /p DummyNews /d DummyNews");

            Directory.Exists(ConfigurationManager.PackagePath).Should().BeFalse("The package DummyNews should not be installed.");
        }

        [Test]
        public void ShouldUpgradeAlreadyInstalledPackageAndUninstall()
        {
            RunPackageManagerProcess("/i /p DummyNews /d DummyNews /v 1.0");

            Directory.EnumerateDirectories(ConfigurationManager.PackagePath, "DummyNews.1.0").Any().Should().BeTrue(
                "The package DummyNews should be installed.");

            RunPackageManagerProcess("/i /p DummyNews /d DummyNews /v 1.1");

            Directory.EnumerateDirectories(ConfigurationManager.PackagePath, "DummyNews.1.1").Any().Should().BeTrue(
                "The package DummyNews should be installed.");

            RunPackageManagerProcess("/u /p DummyNews /d DummyNews");

            Directory.Exists(ConfigurationManager.PackagePath).Should().BeFalse("The package DummyNews should not be installed.");
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