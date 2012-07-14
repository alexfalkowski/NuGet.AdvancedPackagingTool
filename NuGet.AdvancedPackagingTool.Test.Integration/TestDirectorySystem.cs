namespace NuGet.AdvancedPackagingTool.Test.Integration
{
    using System;

    using NuGet.AdvancedPackagingTool.Core;

    public class TestDirectorySystem : IDirectorySystem
    {
        public string CurrentDirectory
        {
            get
            {
                return new Uri("file:///C:/NuGet/TestInstallPackage/").LocalPath;
            }
        }
    }
}