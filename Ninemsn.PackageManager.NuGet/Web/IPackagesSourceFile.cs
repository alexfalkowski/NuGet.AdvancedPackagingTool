namespace Ninemsn.PackageManager.NuGet.Web
{
    using System.Collections.Generic;

    public interface IPackagesSourceFile
    {
        bool Exists();

        IEnumerable<WebPackageSource> ReadSources();

        void WriteSources(IEnumerable<WebPackageSource> sources);
    }
}