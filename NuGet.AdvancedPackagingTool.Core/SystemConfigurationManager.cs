namespace NuGet.AdvancedPackagingTool.Core
{
    using System.Configuration;

    public class SystemConfigurationManager : IConfigurationManager
    {
        public string PackagePath
        {
            get
            {
                return ConfigurationManager.AppSettings["PackagePath"];
            }
        }
    }
}