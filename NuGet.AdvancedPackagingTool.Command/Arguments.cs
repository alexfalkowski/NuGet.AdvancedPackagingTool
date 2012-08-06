namespace NuGet.AdvancedPackagingTool.Command
{
    using System.ComponentModel;

    using NuGet.AdvancedPackagingTool.Core;

    using PowerArgs;

    public class Arguments
    {
        [ArgRequired]
        [ArgPosition(0)]
        [ArgIgnoreCase]
        [ArgDescription("The action to either install or uninstall")]
        public ActionStatus Action { get; set; }

        [ArgRequired]
        [ArgShortcut("p")]
        [ArgPosition(1)]
        [ArgDescription("The package to install or uninstall")]
        public string Package { get; set; }

        [ArgShortcut("v")]
        [ArgPosition(2)]
        [ArgDescription("The version of the package.")]
        [TypeConverter(typeof(VersionTypeConverter))]
        public SemanticVersion Version { get; set; }

        [ArgShortcut("s")]
        [ArgDescription("The package to install or uninstall")]
        public string Source { get; set; }

        [ArgShortcut("d")]
        [ArgDescription("The package destination.")]
        public string Destination { get; set; }

        [ArgShortcut("c")]
        [ArgDescription("The location from where to read the JSON configuration file.")]
        public string Configuration { get; set; }

        [ArgDescription("Install a package")]
        public ActionArguments InstallArgs { get; set; }

        [ArgDescription("Uninstall a package")]
        public ActionArguments UninstallArgs { get; set; }

        public static void Install(ActionArguments args)
        {
        }

        public static void Uninstall(ActionArguments args)
        {
        }
    }
}