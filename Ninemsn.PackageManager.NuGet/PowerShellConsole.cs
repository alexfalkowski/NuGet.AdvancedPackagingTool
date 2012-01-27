namespace Ninemsn.PackageManager.NuGet
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;

    using global::NuGet;

    public class PowerShellConsole
    {
        private readonly IPackage package;

        private readonly string script;

        public PowerShellConsole(IPackage package, string script)
        {
            if (package == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("package");
            }

            if (string.IsNullOrWhiteSpace("script"))
            {
                throw ExceptionFactory.CreateArgumentNullException("script");
            }

            this.package = package;
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

        public ProcessExitInfo Start()
        {
            var configurationFile = this.package.GetConfigurationPackageFile();
            var configurationFileContent = configurationFile.GetStream().ReadToEnd();
            var firstLineFromScript = this.script.Substring(0, this.script.IndexOf(Environment.NewLine, StringComparison.CurrentCulture));
            var restOfScript = this.script.Replace(firstLineFromScript, string.Empty);
            var completeScript = string.Concat(
                firstLineFromScript, Environment.NewLine, this.ModulesScript, Environment.NewLine, restOfScript);
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
                this.package.ProjectUrl.AbsolutePath);
            var parameters = string.Format(CultureInfo.CurrentCulture, ParameterFormat, executableScript);

            var info = ProcessHelper.ExecuteBackgroundProcess("powershell.exe", parameters);
            PathHelper.SafeDelete(scriptTempFile);
            PathHelper.SafeDelete(configurationTempFile);

            return info;
        }
    }
}