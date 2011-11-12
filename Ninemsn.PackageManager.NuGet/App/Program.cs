namespace Ninemsn.PackageManager.NuGet.App
{
    using System;

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
                foreach (var error in this.args.Errors)
                {
                    Console.WriteLine(error);
                }

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