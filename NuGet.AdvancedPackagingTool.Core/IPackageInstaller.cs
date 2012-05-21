namespace NuGet.AdvancedPackagingTool.Core
{
    using System.Collections.Generic;

    using NuGet;

    public interface IPackageInstaller
    {
        IEnumerable<string> Logs { get; }

        void InstallPackage(SemanticVersion version);

        void UninstallPackage(SemanticVersion version);
    }
}