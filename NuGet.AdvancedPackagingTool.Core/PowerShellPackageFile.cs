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

        private readonly IFileSystem fileSystem;

        private readonly string configurationPath;

        public PowerShellPackageFile(
            IProcess process, IPackageManager manager, IFileSystem fileSystem, string configurationPath)
        {
            this.process = process;
            this.manager = manager;
            this.fileSystem = fileSystem;
            this.configurationPath = configurationPath;
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
            IShellConsole console = new PowerShellConsole(package, this.process, this.fileSystem, file);
            var processExitInfo = console.Start(installationPath, this.configurationPath);

            Func<string, bool> predicate = message => !string.IsNullOrWhiteSpace(message);
            foreach (var message in processExitInfo.OutputMessage.Split('\n').Where(predicate))
            {
                logger.Log(MessageLevel.Info, message.Trim());
            }

            foreach (var message in processExitInfo.ErrorMessage.Split('\n').Where(predicate))
            {
                logger.Log(MessageLevel.Error, message.Trim());
            }

            if (processExitInfo.ExitCode > 0)
            {
                throw ExceptionFactory.CreateInvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.PowershellErrorMessage,
                        file.Path,
                        processExitInfo.ErrorMessage));
            }
        }
    }
}