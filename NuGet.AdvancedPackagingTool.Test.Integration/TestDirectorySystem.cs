namespace NuGet.AdvancedPackagingTool.Test.Integration
{
    using System;

    using NuGet.AdvancedPackagingTool.Core;

    public class TestDirectorySystem : IDirectorySystem
    {
        private readonly string localPath = new Uri("file:///C:/NuGet/TestInstallPackage/").LocalPath;

        public string CurrentDirectory
        {
            get
            {
                return this.localPath;
            }
        }

        public string TemporaryPath
        {
            get
            {
                return this.localPath;
            }
        }
    }
}