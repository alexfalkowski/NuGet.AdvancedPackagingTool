namespace NuGet.Enterprise.Core
{
    using System.IO;

    using NuGet;

    public class NullPackageFile : IPackageFile
    {
        public string Path
        {
            get
            {
                return string.Empty;
            }
        }

        public Stream GetStream()
        {
            return Stream.Null;
        }
    }
}