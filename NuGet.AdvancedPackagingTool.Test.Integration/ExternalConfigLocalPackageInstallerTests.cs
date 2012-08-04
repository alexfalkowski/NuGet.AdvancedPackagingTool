namespace NuGet.AdvancedPackagingTool.Test.Integration
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    using NuGet.AdvancedPackagingTool.Core;

    using NUnit.Framework;

    [TestFixture]
    public class ExternalConfigLocalPackageInstallerTests : PackageInstallerTestsBase
    {
        private const string ConfigurationExternalText = "Configuration external";

        [SetUp]
        public void BeforeEach()
        {
            var directoryName = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            Debug.Assert(directoryName != null, "directoryName != null");
            var configurationPath = Path.Combine(directoryName, "Configuration.json");
            var packageSourceFile = new PackageSourceFileFactory().CreatePackageSourceFile();
            var module = new PackageManagerModule(packageSourceFile);

            this.Setup(module.GetSource("TestLocalFeed"), ConfigurationExternalText, configurationPath);
        }
    }
}