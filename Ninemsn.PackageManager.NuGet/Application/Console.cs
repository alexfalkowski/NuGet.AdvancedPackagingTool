namespace Ninemsn.PackageManager.NuGet.Application
{
    using System;
    using System.Collections.Generic;

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
                WriteInformationToConsole(this.arguments.Errors);

                return;
            }

            try
            {
                if (this.arguments.Install)
                {
                    WriteInformationToConsole(this.installer.InstallPackage());
                }

                if (this.arguments.Uninstall)
                {
                    WriteInformationToConsole(this.installer.UninstallPackage());
                }
            }
            catch (Exception e)
            {
                this.logger.Error(Resources.InvalidExecutionErrorMessage, e);

                throw ExceptionFactory.CreateInvalidOperationException(Resources.InvalidExecutionExceptionMessage);
            }
        }

        private static void WriteInformationToConsole(IEnumerable<string> information)
        {
            foreach (var value in information)
            {
                System.Console.WriteLine(value);
            }
        }
    }
}