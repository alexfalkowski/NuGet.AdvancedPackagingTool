namespace Ninemsn.PackageManager.NuGet.Test.Integration
{
    public static class PackageSourceFileFactory
    {
         public static PackageSourceFile CreatePackageSourceFile()
         {
             return new PackageSourceFile("PackageSources.config");
         }
    }
}