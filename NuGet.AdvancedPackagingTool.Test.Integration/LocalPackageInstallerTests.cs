namespace NuGet.AdvancedPackagingTool.Test.Integration
{
    using NuGet.AdvancedPackagingTool.Core;

    using NUnit.Framework;

    [TestFixture]
    public class LocalPackageInstallerTests : PackageInstallerTestsBase
    {
        [SetUp]
        public void BeforeEach()
        {
            var packageSourceFile = new PackageSourceFileFactory().CreatePackageSourceFile();
            var module = new PackageManagerModule(packageSourceFile);

            this.Setup(module.GetSource("TestLocalFeed"));
        }
    }
}