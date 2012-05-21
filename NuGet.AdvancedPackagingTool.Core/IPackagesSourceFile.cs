namespace NuGet.AdvancedPackagingTool.Core
{
    using System.Collections.Generic;

    using NuGet;

    public interface IPackagesSourceFile
    {
        bool Exists();

        IEnumerable<PackageSource> ReadSources();
    }
}