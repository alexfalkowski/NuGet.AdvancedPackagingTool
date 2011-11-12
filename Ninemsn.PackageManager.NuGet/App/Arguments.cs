namespace Ninemsn.PackageManager.NuGet.App
{
    public class Arguments
    {
        public bool Install { get; set; }

        public bool Uninstall { get; set; }

        public string Package { get; set; }

        public string Destination { get; set; }
    }
}