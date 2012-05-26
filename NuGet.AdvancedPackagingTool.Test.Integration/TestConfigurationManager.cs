namespace NuGet.AdvancedPackagingTool.Test.Integration
{
    using System;

    using NuGet.AdvancedPackagingTool.Core;

    public class TestConfigurationManager : IConfigurationManager
    {
        public string PackagePath
        {
            get
            {
                return new Uri("file:///C:/NuGet/TestInstallPackage/").LocalPath;
            }
        }
    }
}