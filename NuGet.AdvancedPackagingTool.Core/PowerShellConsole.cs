namespace NuGet.AdvancedPackagingTool.Core
{
    using System;
    using System.Globalization;

    using NuGet;

    public class PowerShellConsole : IShellConsole
    {
        private const string ScriptTemplateFormat = @"$environment = import-clixml {0}; & '{1}' '{2}' $environment";
        
        private const string ParameterFormat = "-inputformat none -NoProfile -ExecutionPolicy unrestricted -Command \"{0} \"";

        private readonly IPackage package;

        private readonly IPackageFile packageFile;

        private readonly IProcess process;

        private readonly IFileSystem fileSystem;

        public PowerShellConsole(IPackage package, IProcess process, IFileSystem fileSystem, IPackageFile packageFile)
        {
            this.fileSystem = fileSystem;
            this.package = package;
            this.process = process;
            this.packageFile = packageFile;
        }

        public ProcessExitInfo Start(string installationPath)
        {
            var configurationFile = this.package.GetConfigurationPackageFile();

            var configurationTempPath = Guid.NewGuid() + ".xml";
            this.fileSystem.AddFile(configurationTempPath, configurationFile.GetStream());

            var scriptTempPath = Guid.NewGuid() + ".ps1";
            this.fileSystem.AddFile(scriptTempPath, this.packageFile.GetStream());

            var executableScript = string.Format(
                CultureInfo.CurrentCulture,
                ScriptTemplateFormat,
                this.fileSystem.GetFullPath(configurationTempPath),
                this.fileSystem.GetFullPath(scriptTempPath),
                installationPath);
            var parameters = string.Format(CultureInfo.CurrentCulture, ParameterFormat, executableScript);
            var info = this.process.ExecuteProcess("powershell.exe", parameters);

            this.fileSystem.DeleteFile(configurationTempPath);
            this.fileSystem.DeleteFile(scriptTempPath);

            // For some reason powershell if it throws a compilation error the executable returns 0.
            if (!string.IsNullOrWhiteSpace(info.ErrorMessage) && info.ExitCode == 0)
            {
                info.ExitCode = 1;
            }

            return info;
        }
    }
}