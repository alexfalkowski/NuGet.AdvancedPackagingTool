namespace NuGet.AdvancedPackagingTool.Core
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using NuGet.AdvancedPackagingTool.Core.Properties;

    public class PowerShellPackageFile : IShellPackageFile
    {
        private readonly IProcess process;

        private readonly IPackageManager manager;

        public PowerShellPackageFile(IProcess process, IPackageManager manager)
        {
            this.process = process;
            this.manager = manager;
        }

        public void Execute(IPackageFile file, IPackage package, ILogger logger)
        {
            if (file == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("file");
            }

            if (logger == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("logger");
            }

            var installationPath = Path.Combine(
                this.manager.FileSystem.Root, this.manager.PathResolver.GetPackageDirectory(package));
            var console = new PowerShellConsole(package, this.process, file.GetStream().ReadToEnd());
            var processExitInfo = console.Start(installationPath);

            if (processExitInfo.ExitCode > 0)
            {
                throw ExceptionFactory.CreateInvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.PowershellErrorMessage,
                        file.Path,
                        processExitInfo.ErrorMessage));
            }

            Func<string, bool> predicate = message => !string.IsNullOrWhiteSpace(message);
            foreach (var message in processExitInfo.OutputMessage.Split('\n').Where(predicate))
            {
                logger.Log(MessageLevel.Info, message.Trim());
            }
        }
    }
}