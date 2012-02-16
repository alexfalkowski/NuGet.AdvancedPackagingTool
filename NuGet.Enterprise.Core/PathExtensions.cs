namespace NuGet.Enterprise.Core
{
    using System.IO;

    internal static class PathExtensions
    {
        public static void Copy(this DirectoryInfo source, DirectoryInfo destination)
        {
            if (!destination.Exists)
            {
                destination.Create();
            }

            var files = source.GetFiles();
            foreach (var file in files)
            {
                file.CopyTo(Path.Combine(destination.FullName, file.Name));
            }

            var dirs = source.GetDirectories();
            foreach (var dir in dirs)
            {
                var destinationDir = Path.Combine(destination.FullName, dir.Name);
                Copy(dir, new DirectoryInfo(destinationDir));
            }
        }
    }
}