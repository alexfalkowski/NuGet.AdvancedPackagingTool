namespace NuGet.AdvancedPackagingTool.Command
{
    using System;

    using NuGet.AdvancedPackagingTool.Core;

    using PowerArgs;

    public static class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                var arguments = Args.InvokeAction<Arguments>(args).Args;
                IPackageSourceFileFactory packageSourceFileFactory = new PackageSourceFileFactory();
                var packageSourceFile = packageSourceFileFactory.CreatePackageSourceFile();
                var packageManager = new PackageManagerModule(packageSourceFile);
                var packageSource = string.IsNullOrWhiteSpace(arguments.Source)
                                            ? packageManager.ActiveSource
                                            : packageManager.GetSource(arguments.Source);
                var sourceFactory = new SourcePackageRepositoryFactory(packageSource);
                IPackageInstallerFactory factory = new PackageInstallerFactory(
                    sourceFactory, new SystemConfigurationManager(), new PhysicalDirectorySystem());
                var installer = factory.CreatePackageInstaller(arguments.Destination, arguments.Configuration);
                var program = new Console(arguments, installer);

                return program.Start();
            }
            catch (Exception)
            {
                System.Console.WriteLine(ArgUsage.GetUsage<Arguments>());
                return 1;
            }
        }
    }
}
