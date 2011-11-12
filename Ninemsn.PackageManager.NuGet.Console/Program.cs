namespace Ninemsn.PackageManager.NuGet.Console
{
    using Args;

    using Ninemsn.PackageManager.NuGet.Application;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var arguments = Configuration.Configure<Arguments>().CreateAndBind(args);

            var program = new Console(arguments, PackageInstallerFactory.CreatePackageInstaller(arguments));
            program.Start();
        }
    }
}
