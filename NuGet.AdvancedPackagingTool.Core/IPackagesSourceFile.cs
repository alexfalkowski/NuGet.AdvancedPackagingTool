namespace NuGet.AdvancedPackagingTool.Core
{
    using System.Collections.Generic;

    using NuGet;

    public interface IPackagesSourceFile
    {
        IEnumerable<PackageSource> ReadSources();
    }
}