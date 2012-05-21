namespace NuGet.AdvancedPackagingTool.Command
{
    using Args;

    using NuGet.AdvancedPackagingTool.Core;

    public static class Program
    {
        public static int Main(string[] args)
        {
            var arguments = Configuration.Configure<Arguments>().CreateAndBind(args);

            var program = new Console(
                arguments,
                PackageInstallerFactory.CreatePackageInstaller(arguments.Source, arguments.Package, arguments.IsValid));
            return program.Start();
        }
    }
}
