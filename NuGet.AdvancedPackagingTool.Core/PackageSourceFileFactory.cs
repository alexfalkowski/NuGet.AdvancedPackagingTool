namespace NuGet.AdvancedPackagingTool.Core
{
    public static class PackageSourceFileFactory
    {
         public static PackageSourceFile CreatePackageSourceFile()
         {
             return new PackageSourceFile("PackageSources.config");
         }
    }
}