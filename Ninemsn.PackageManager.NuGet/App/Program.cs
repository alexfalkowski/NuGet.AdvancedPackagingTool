namespace Ninemsn.PackageManager.NuGet.App
{
    using System;

    using Common.Logging;

    using Ninemsn.PackageManager.NuGet.Properties;

    public class Program
    {
        private readonly Arguments args;

        private readonly IPackageInstaller installer;

        private readonly ILog logger = LogManager.GetCurrentClassLogger();

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

            try
            {
                if (this.args.Install)
                {
                    this.installer.InstallPackage();
                }

                if (this.args.Uninstall)
                {
                    this.installer.UninstallPackage();
                }
            }
            catch (Exception e)
            {
                this.logger.Error(Resources.InvalidExecutionErrorMessage, e);

                throw ExceptionFactory.CreateInvalidOperationException(Resources.InvalidExecutionExceptionMessage);
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