namespace NuGet.AdvancedPackagingTool.Command
{
    using Args;

    using NuGet.AdvancedPackagingTool.Core;

    public static class Program
    {
        public static int Main(string[] args)
        {
            var arguments = Configuration.Configure<Arguments>().CreateAndBind(args);
            IPackageSourceFileFactory packageSourceFileFactory = new PackageSourceFileFactory();
            var packageSourceFile = packageSourceFileFactory.CreatePackageSourceFile();
            var packageManager = new PackageManagerModule(packageSourceFile);
            var packageSource = string.IsNullOrWhiteSpace(arguments.Source)
                                        ? packageManager.ActiveSource
                                        : packageManager.GetSource(arguments.Source);
            var sourceFactory = new SourcePackageRepositoryFactory(packageSource);
            IPackageInstallerFactory factory = new PackageInstallerFactory(
                sourceFactory, new SystemConfigurationManager(), new PhysicalDirectorySystem());
            var installer = factory.CreatePackageInstaller(
                arguments.IsValid, arguments.Destination, arguments.Configuration);
            var program = new Console(arguments, installer);

            return program.Start();
        }
    }
}
