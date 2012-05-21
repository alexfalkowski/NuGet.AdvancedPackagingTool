namespace NuGet.AdvancedPackagingTool.Core
{
    public static class ConfigurationManager
    {
        public static string PackagePath
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["PackagePath"];
            }
        }
    }
}