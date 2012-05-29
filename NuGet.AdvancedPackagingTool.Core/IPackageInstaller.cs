namespace NuGet.AdvancedPackagingTool.Core
{
    using System.Collections.Generic;

    using NuGet;

    public interface IPackageInstaller
    {
        IEnumerable<string> Logs { get; }

        void InstallPackage(string packageId, SemanticVersion version);

        void UninstallPackage(string packageId, SemanticVersion version);
    }
}