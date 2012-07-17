namespace NuGet.AdvancedPackagingTool.Core
{
    using System.IO;

    public class PhysicalDirectorySystem : IDirectorySystem
    {
        public string CurrentDirectory
        {
            get
            {
                return Directory.GetCurrentDirectory();
            }
        }

        public string TemporaryPath
        {
            get
            {
                return Path.GetTempPath();
            }
        }
    }
}