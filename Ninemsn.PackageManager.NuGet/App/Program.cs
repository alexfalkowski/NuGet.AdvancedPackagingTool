namespace Ninemsn.PackageManager.NuGet.App
{
    using Ninemsn.PackageManager.NuGet.Properties;

    public class Program
    {
        private readonly Arguments args;

        public Program(Arguments args)
        {
            this.args = args;
        }

        public void Start()
        {
            if (this.args.Install && this.args.Uninstall)
            {
                throw ExceptionFactory.CreateArgumentException(Resources.InvalidInstallUninstallFlag);
            }
        }
    }
}