namespace Ninemsn.PackageManager.NuGet
{
    using System.Collections.Generic;

    using global::NuGet;

    public interface IPackageInstaller
    {
        IEnumerable<string> Logs { get; }

        void InstallPackage(SemanticVersion version);

        void UninstallPackage(SemanticVersion version);
    }
}