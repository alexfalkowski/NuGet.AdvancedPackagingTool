namespace Ninemsn.PackageManager.NuGet
{
    using System.IO;

    using global::NuGet;

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