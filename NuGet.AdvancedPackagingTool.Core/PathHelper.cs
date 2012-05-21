namespace NuGet.AdvancedPackagingTool.Core
{
    using System.IO;

    public static class PathHelper
    {
        public static void SafeDelete(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }
    }
}