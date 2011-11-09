namespace Ninemsn.PackageManager.NuGet
{
    using System.Collections.Generic;

    public interface IPackagesSourceFile
    {
        bool Exists();

        IEnumerable<PackageSource> ReadSources();

        void WriteSources(IEnumerable<PackageSource> sources);
    }
}