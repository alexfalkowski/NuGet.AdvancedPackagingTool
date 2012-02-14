namespace Ninemsn.PackageManager.NuGet.Command
{
    using Args;

    public static class Program
    {
        public static int Main(string[] args)
        {
            var arguments = Configuration.Configure<Arguments>().CreateAndBind(args);

            var program = new Console(arguments, PackageInstallerFactory.CreatePackageInstaller(arguments));
            return program.Start();
        }
    }
}
