namespace Ninemsn.PackageManager.NuGet.App
{
    public class Program
    {
        private readonly Arguments args;

        private readonly IPackageInstaller installer;

        public Program(Arguments args, IPackageInstaller installer)
        {
            this.args = args;
            this.installer = installer;
        }

        public void Start()
        {
            if (!this.args.IsValid())
            {
                return;
            }

            if (this.args.Install)
            {
                this.installer.InstallPackage();
            }

            if (this.args.Uninstall)
            {
                this.installer.UninstallPackage();
            }
        }
    }
}