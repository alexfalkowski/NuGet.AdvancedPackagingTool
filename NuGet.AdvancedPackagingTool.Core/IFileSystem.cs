namespace NuGet.Enterprise.Core
{
    public interface IFileSystem : NuGet.IFileSystem
    {
        void CreateDirectory(string path);
    }
}