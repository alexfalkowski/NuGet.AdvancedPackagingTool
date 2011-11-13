using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Nu")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Nu", 
        Scope = "namespace", Target = "Ninemsn.PackageManager.NuGet")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Nu", 
        Scope = "namespace", Target = "Ninemsn.PackageManager.NuGet.Application")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Scope = "type", 
        Target = "Ninemsn.PackageManager.NuGet.Application.ArgumentsValidator")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", 
        Scope = "namespace", Target = "Ninemsn.PackageManager.NuGet.Application")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", 
        Scope = "member", Target = "Ninemsn.PackageManager.NuGet.IPackageInstaller.#InstallPackage(System.Version)")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", 
        Scope = "member", Target = "Ninemsn.PackageManager.NuGet.IPackageInstaller.#UninstallPackage(System.Version)")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", 
        Scope = "member", Target = "Ninemsn.PackageManager.NuGet.NullPackageInstaller.#InstallPackage(System.Version)")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", 
        Scope = "member", Target = "Ninemsn.PackageManager.NuGet.NullPackageInstaller.#UninstallPackage(System.Version)")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", 
        Scope = "member", Target = "Ninemsn.PackageManager.NuGet.PackageInstaller.#InstallPackage(System.Version)")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", 
        Scope = "member", Target = "Ninemsn.PackageManager.NuGet.PackageInstaller.#UninstallPackage(System.Version)")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", 
        Scope = "member", Target = "Ninemsn.PackageManager.NuGet.Application.Console.#Start()")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", 
        Scope = "type", Target = "Ninemsn.PackageManager.NuGet.PackageManager")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", 
        Scope = "member", Target = "Ninemsn.PackageManager.NuGet.DefaultProjectSystem.#GetPropertyValue(System.String)")]