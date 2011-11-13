namespace Ninemsn.PackageManager.NuGet
{
    using System.Collections.Generic;

    using global::NuGet;

    public interface IPackagesSourceFile
    {
        bool Exists();

        IEnumerable<PackageSource> ReadSources();
    }
}