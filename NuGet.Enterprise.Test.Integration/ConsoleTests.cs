namespace NuGet.Enterprise.Test.Integration
{
    using System.IO;
    using System.Linq;

    using FluentAssertions;

    using NUnit.Framework;

    using NuGet.Enterprise.Core;

    [TestFixture]
    public class ConsoleTests
    {
        private PackagesWebServer server;

        [SetUp]
        public void Setup()
        {
            this.server = new PackagesWebServer();
            this.server.Startup();

            if (Directory.Exists(ConfigurationManager.PackagePath))
            {
                Directory.Delete(ConfigurationManager.PackagePath, true);
            }
        }

        [TearDown]
        public void Teardown()
        {
            this.server.Stop();
        }

        [Test]
        public static void ShouldInstallAndUninstallVersion10Package()
        {
            RunPackageManagerProcess("/i /p DummyNews /v 1.0");

            Directory.EnumerateDirectories(ConfigurationManager.PackagePath, "DummyNews.1.0").Any().Should().BeTrue(
                "The package DummyNews should be installed.");

            RunPackageManagerProcess("/u /p DummyNews /v 1.0");

            Directory.Exists(ConfigurationManager.PackagePath).Should().BeFalse("The package DummyNews should not be installed.");
        }

        [Test]
        public static void ShouldInstallAndUninstallVersion11Package()
        {
            RunPackageManagerProcess("/i /p DummyNews /v 1.1");

            Directory.EnumerateDirectories(ConfigurationManager.PackagePath, "DummyNews.1.1").Any().Should().BeTrue(
                "The package DummyNews should be installed.");

            RunPackageManagerProcess("/u /p DummyNews /v 1.1");

            Directory.Exists(ConfigurationManager.PackagePath).Should().BeFalse("The package DummyNews should not be installed.");
        }

        [Test]
        public static void ShouldInstallAndUninstallLatestVersionOfPackage()
        {
            RunPackageManagerProcess("/i /p DummyNews");

            Directory.EnumerateDirectories(ConfigurationManager.PackagePath, "DummyNews.1.1").Any().Should().BeTrue(
                "The package DummyNews should be installed.");

            RunPackageManagerProcess("/u /p DummyNews");

            Directory.Exists(ConfigurationManager.PackagePath).Should().BeFalse("The package DummyNews should not be installed.");
        }

        [Test]
        public static void ShouldUpgradeAlreadyInstalledPackage()
        {
            RunPackageManagerProcess("/i /p DummyNews /v 1.0");

            Directory.EnumerateDirectories(ConfigurationManager.PackagePath, "DummyNews.1.0").Any().Should().BeTrue(
                "The package DummyNews should be installed.");

            RunPackageManagerProcess("/i /p DummyNews /v 1.1");

            Directory.EnumerateDirectories(ConfigurationManager.PackagePath, "DummyNews.1.1").Any().Should().BeTrue(
                "The package DummyNews should be installed.");
        }

        private static void RunPackageManagerProcess(string arguments)
        {
            var info = ProcessHelper.ExecuteBackgroundProcess("npm.exe", arguments);

            info.ExitCode.Should().Be(0);
        }
    }
}