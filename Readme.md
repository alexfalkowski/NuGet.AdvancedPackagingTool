
# NuGet.AdvancedPackagingTool

This project is an adaptation of [apt-get](http://en.wikipedia.org/wiki/Advanced_Packaging_Tool) using the nuget as the package format.

## How Do I Create Packages?

Creating packages are done through the [nuget](http://docs.nuget.org/docs/reference/command-line-reference) command

The package can contain special files that make this tool quite interesting. The nuget specification states that a nuget package can contain a [tools](http://docs.nuget.org/docs/creating-packages/creating-and-publishing-a-package) folder. This tool accepts the following PowerShell files

* Setup.ps1
* Install.ps1
* Uninstall.ps1
* Teardown.ps1
* Configuration.json

The script signature for each of these files is as follows:

	param ($installationFolder, $configuration)

These parameters are described in the next section.

## So How Does it Work?

This section describes how all the magic comes together.

### Command Line Usage

There is a command line tool that is adequately called napt-get that you can execute. Here is the help output to guide you along.

	Usage: napt-get <action> options

	Global options:

	   OPTION                TYPE              ORDER   DESCRIPTION
	   -package (-p)         String*           1       The package to install or uninstall
	   -version (-v)         SemanticVersion   2       The version of the package.
	   -source (-s)          String                    The source of the packages.
	   -destination (-d)     String                    The package destination.
	   -configuration (-c)   String                    The location from where to read the JSON configuration file.

	Actions:

	install - Install a package
	uninstall - Uninstall a package

To install the latest version of "PackageName" use the following command (this will install it in the current directory you are in)

	napt-get install -p PackageName

To install a specific version of "PackageName" use the following command (please not that nuget follows [semantic versioning](http://semver.org/) for versions)
	
	napt-get install -p PackageName -v 1.0.0.0

To uninstall the latest version of "PackageName" use the following command

	napt-get install -p PackageName

### Command Line Workflow

When the package is about install it follows the following rules:

* If there is a package installed call the Uninstall.ps1 (passing the configuration that is in the package or the one specified by the -c flag)
* Call the Setup.ps1 (passing the configuration that is in the package or the one specified by the -c flag)
* Unzip the contents of the package (current folder or the location specified by the -d flag)
* Call Install.ps1 (passing the configuration that is in the package or the one specified by the -c flag)

When the package is about to uninstall it follows the following rules:

* Call the Uninstall.ps1 (passing the configuration that is in the package or the one specified by the -c flag)
* Delete the contents of the package from the disk.
* Call the Teardown.ps1 (passing the configuration that is in the package or the one specified by the -c flag)

The usual error checking is done if you pass in bogus information.