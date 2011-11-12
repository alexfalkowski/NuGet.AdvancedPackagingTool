namespace Ninemsn.PackageManager.NuGet
{
    public static class PackageSourceFileFactory
    {
         public static PackageSourceFile CreatePackageSourceFile()
         {
             return new PackageSourceFile("PackageSources.config");
         }
    }
}