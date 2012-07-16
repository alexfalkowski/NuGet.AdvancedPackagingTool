namespace NuGet.AdvancedPackagingTool.Test.Integration
{
    using System;
    using System.IO;
    using System.Linq;

    using FluentAssertions;

    using NuGet.AdvancedPackagingTool.Core;

    using NUnit.Framework;

    [TestFixture]
    public class ConsoleTests
    {
        private PackagesWebServer server;

        private IConfigurationManager configurationManager;

        private IDirectorySystem directorySystem;

        [SetUp]
        public void BeforeEach()
        {
            this.configurationManager = new SystemConfigurationManager();
            this.directorySystem = new PhysicalDirectorySystem();
            this.server = new PackagesWebServer();
            this.server.Startup();

            if (Directory.Exists(this.configurationManager.PackagePath))
            {
                Directory.Delete(this.configurationManager.PackagePath, true);
            }
        }

        [TearDown]
        public void AfterEach()
        {
            this.server.Stop();
        }

        [Test]
        public void ShouldInstallAndUninstallVersion10Package()
        {
            RunPackageManagerProcess("/i /p DummyNews /v 1.0");

            Directory.EnumerateDirectories(this.configurationManager.PackagePath, "DummyNews.1.0").Any().Should().BeTrue(
                "The package DummyNews should be installed.");

            var path = Path.Combine(this.directorySystem.CurrentDirectory, PackageInstallerTestsBase.DummyNews10Folder);
            Directory.EnumerateDirectories(path).Any().Should().BeTrue("Should have directories under " + path);

            RunPackageManagerProcess("/u /p DummyNews /v 1.0");

            Directory.Exists(this.configurationManager.PackagePath).Should().BeFalse("The package DummyNews should not be installed.");
        }

        [Test]
        public void ShouldInstallAndUninstallVersion10PackageUnderSpecificFolder()
        {
            this.directorySystem = new TestDirectorySystem();
            RunPackageManagerProcess("/i /p DummyNews /v 1.0 /d " + this.directorySystem.CurrentDirectory);

            Directory.EnumerateDirectories(this.configurationManager.PackagePath, "DummyNews.1.0").Any().Should().BeTrue(
                "The package DummyNews should be installed.");

            var path = Path.Combine(this.directorySystem.CurrentDirectory, PackageInstallerTestsBase.DummyNews10Folder);
            Directory.EnumerateDirectories(path).Any().Should().BeTrue("Should have directories under " + path);

            RunPackageManagerProcess("/u /p DummyNews /v 1.0 /d " + this.directorySystem.CurrentDirectory);

            Directory.Exists(this.configurationManager.PackagePath).Should().BeFalse("The package DummyNews should not be installed.");
        }

        [Test]
        public void ShouldInstallAndUninstallVersion11Package()
        {
            RunPackageManagerProcess("/i /p DummyNews /v 1.1");

            Directory.EnumerateDirectories(this.configurationManager.PackagePath, "DummyNews.1.1").Any().Should().BeTrue(
                "The package DummyNews should be installed.");

            var path = Path.Combine(this.directorySystem.CurrentDirectory, PackageInstallerTestsBase.DummyNews11Folder);
            Directory.EnumerateDirectories(path).Any().Should().BeTrue("Should have directories under " + path);

            RunPackageManagerProcess("/u /p DummyNews /v 1.1");

            Directory.Exists(this.configurationManager.PackagePath).Should().BeFalse("The package DummyNews should not be installed.");
        }

        [Test]
        public void ShouldInstallAndUninstallVersion11PackageUnderSpecificFolder()
        {
            this.directorySystem = new TestDirectorySystem();
            RunPackageManagerProcess("/i /p DummyNews /v 1.1 /d " + this.directorySystem.CurrentDirectory);

            Directory.EnumerateDirectories(this.configurationManager.PackagePath, "DummyNews.1.1").Any().Should().BeTrue(
                "The package DummyNews should be installed.");

            var path = Path.Combine(this.directorySystem.CurrentDirectory, PackageInstallerTestsBase.DummyNews11Folder);
            Directory.EnumerateDirectories(path).Any().Should().BeTrue("Should have directories under " + path);

            RunPackageManagerProcess("/u /p DummyNews /v 1.1 /d " + this.directorySystem.CurrentDirectory);

            Directory.Exists(this.configurationManager.PackagePath).Should().BeFalse("The package DummyNews should not be installed.");
        }

        [Test]
        public void ShouldInstallAndUninstallLatestVersionOfPackage()
        {
            RunPackageManagerProcess("/i /p DummyNews");

            Directory.EnumerateDirectories(this.configurationManager.PackagePath, "DummyNews.1.1").Any().Should().BeTrue(
                "The package DummyNews should be installed.");

            RunPackageManagerProcess("/u /p DummyNews");

            Directory.Exists(this.configurationManager.PackagePath).Should().BeFalse("The package DummyNews should not be installed.");
        }

        [Test]
        public void ShouldUpgradeAlreadyInstalledPackage()
        {
            RunPackageManagerProcess("/i /p DummyNews /v 1.0");

            Directory.EnumerateDirectories(this.configurationManager.PackagePath, "DummyNews.1.0").Any().Should().BeTrue(
                "The package DummyNews should be installed.");

            RunPackageManagerProcess("/i /p DummyNews /v 1.1");

            Directory.EnumerateDirectories(this.configurationManager.PackagePath, "DummyNews.1.1").Any().Should().BeTrue(
                "The package DummyNews should be installed.");
        }

        private static void RunPackageManagerProcess(string arguments)
        {
            var info = new BackgroundProcess().ExecuteProcess("napt-get.exe", arguments);

            Console.WriteLine(info.OutputMessage);
            Console.WriteLine(info.ErrorMessage);

            info.ExitCode.Should().Be(0);
        }
    }
}