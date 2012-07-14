namespace NuGet.AdvancedPackagingTool.Core
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;

    using NuGet;

    public class PowerShellConsole : IShellConsole
    {
        private const string ScriptSignature = "param ([string]$installationFolder, $configuration)";

        private readonly IPackage package;

        private readonly string script;

        private readonly IProcess process;

        public PowerShellConsole(IPackage package, IProcess process, string script)
        {        
            this.package = package;
            this.process = process;
            this.script = script;
        }

        private string ModulesScript
        {
            get
            {
                var modules = this.package.GetModuleFiles();
                var moduleStringBuilder = new StringBuilder();

                foreach (var packageFile in modules)
                {
                    moduleStringBuilder.AppendLine(packageFile.GetStream().ReadToEnd());
                }

                return moduleStringBuilder.ToString();
            }
        }

        public ProcessExitInfo Start(string installationPath)
        {
            var configurationFile = this.package.GetConfigurationPackageFile();
            var configurationFileContent = configurationFile.GetStream().ReadToEnd();
            var restOfScript = this.script.Replace(ScriptSignature, string.Empty);
            var completeScript = string.Concat(
                ScriptSignature, Environment.NewLine, this.ModulesScript, Environment.NewLine, restOfScript);
            var scriptTempFile = Path.GetTempPath() + @"\" + Guid.NewGuid() + ".ps1";
            var configurationTempFile = Path.GetTempPath() + @"\" + Guid.NewGuid() + ".xml";

            File.WriteAllText(scriptTempFile, completeScript);
            File.WriteAllText(configurationTempFile, configurationFileContent);

            const string ScriptTemplateFormat = @"$environment = import-clixml {0}; & '{1}' '{2}' $environment";
            const string ParameterFormat = "-inputformat none -NoProfile -ExecutionPolicy unrestricted -Command \"{0} \"";
            var executableScript = string.Format(
                CultureInfo.CurrentCulture,
                ScriptTemplateFormat,
                configurationTempFile,
                scriptTempFile,
                installationPath);
            var parameters = string.Format(CultureInfo.CurrentCulture, ParameterFormat, executableScript);

            var info = this.process.ExecuteProcess("powershell.exe", parameters);
            PathHelper.SafeDelete(scriptTempFile);
            PathHelper.SafeDelete(configurationTempFile);

            // For some reason powershell if it throws a compilation error the executable returns 0.
            if (!string.IsNullOrWhiteSpace(info.ErrorMessage) && info.ExitCode == 0)
            {
                info.ExitCode = 1;
            }

            return info;
        }
    }
}