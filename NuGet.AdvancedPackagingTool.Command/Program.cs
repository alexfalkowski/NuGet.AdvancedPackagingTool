namespace NuGet.AdvancedPackagingTool.Command
{
    using Args;

    using NuGet.AdvancedPackagingTool.Core;

    public static class Program
    {
        public static int Main(string[] args)
        {
            var arguments = Configuration.Configure<Arguments>().CreateAndBind(args);
            var packageSourceFileFactory = new PackageSourceFileFactory();
            var packageSourceFile = packageSourceFileFactory.CreatePackageSourceFile();
            var packageManager = new PackageManagerModule(packageSourceFile);
            var packageSource = string.IsNullOrWhiteSpace(arguments.Source)
                                        ? packageManager.ActiveSource
                                        : packageManager.GetSource(arguments.Source);
            var sourceFactory = new SourcePackageRepositoryFactory(packageSource);
            var factory = new PackageInstallerFactory(sourceFactory, new SystemConfigurationManager());
            var installer = factory.CreatePackageInstaller(arguments.Source, arguments.Package, arguments.IsValid);
            var program = new Console(arguments, installer);

            return program.Start();
        }
    }
}
