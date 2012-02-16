namespace NuGet.Enterprise.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Common.Logging;

    using NuGet.Enterprise.Core.Properties;

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

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We are logging the exception.")]
        public int Start()
        {
            if (!this.arguments.IsValid)
            {
                WriteInformationToConsole(this.arguments.Errors);

                return 1;
            }

            try
            {
                if (this.arguments.Install)
                {
                    this.installer.InstallPackage(this.arguments.Version);
                    WriteInformationToConsole(this.installer.Logs);
                }

                if (this.arguments.Uninstall)
                {
                    this.installer.UninstallPackage(this.arguments.Version);
                    WriteInformationToConsole(this.installer.Logs);
                }

                return 0;
            }
            catch (Exception e)
            {
                this.logger.Error(Resources.InvalidExecutionErrorMessage, e);

                return 1;
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