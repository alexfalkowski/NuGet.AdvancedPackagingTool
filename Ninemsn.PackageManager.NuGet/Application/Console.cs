namespace Ninemsn.PackageManager.NuGet.Application
{
    using System;

    using Common.Logging;

    using Ninemsn.PackageManager.NuGet.Properties;

    public class Console
    {
        private readonly Arguments arguments;

        private readonly IPackageInstaller installer;

        private readonly ILog logger = LogManager.GetCurrentClassLogger();

        public Console(Arguments arguments, IPackageInstaller installer)
        {
            if (arguments == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("arguments");
            }

            if (installer == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("installer");
            }

            this.arguments = arguments;
            this.installer = installer;
        }

        public void Start()
        {
            if (!this.arguments.IsValid)
            {
                this.WriteErrorsToConsole();

                return;
            }

            try
            {
                if (this.arguments.Install)
                {
                    this.installer.InstallPackage();
                }

                if (this.arguments.Uninstall)
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
            foreach (var error in this.arguments.Errors)
            {
                System.Console.WriteLine(error);
            }
        }
    }
}