namespace Ninemsn.PackageManager.NuGet.App
{
    using System;

    public class Program
    {
        private readonly Arguments args;

        private readonly IPackageInstaller installer;

        public Program(Arguments args, IPackageInstaller installer)
        {
            if (args == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("args");
            }

            if (installer == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("installer");
            }

            this.args = args;
            this.installer = installer;
        }

        public void Start()
        {
            if (!this.args.IsValid)
            {
                this.WriteErrorsToConsole();

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

        private void WriteErrorsToConsole()
        {
            foreach (var error in this.args.Errors)
            {
                Console.WriteLine(error);
            }
        }
    }
}