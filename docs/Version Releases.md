# Versioning Releases

This process outlines the current process for updating a version (i.e. the assembly version number 1.x.x.x) of the NuPattern codebase.

## Pre-Requisites
This process is expected to be performed by a project developer/coordinator before a new release is deployed. See [Building Releases](Building-Releases) for more information. 

Special permissions (and credentials) are required to sign and publish the binaries. No special permissions (or credentials) are required to update the source (without signing the binaries).

For changing the version numbers in source you are only required to have Visual Studio 2013 (and VSSDK2013) installed. But for building and signing all the required binaries you will need to have VS2010, VS2102 and VS2013 (and respective VSSDK's) installed side-by-side, and the WIX toolset in VS2013.

## Versioning Process
The process below was refined in the last release, and is expected to change or vary with each release.

IMPORTANT: After following these steps and verifying the results for each release, please update this wiki page to reflect the current process.

Last Updated (and verified) for Release: **1.4.24.0**

## Preparation
For all versions of Visual Studio (VS2010 & VS2012 & VS2013):
# Uninstall all NUPATTERN extensions from Extension Manager. Recycle Visual Studio. 
# Uninstall all NUPATTERN extensions from Extension Manager in all Experimental Instances of all versions of VS. Or run 'Reset Experimental Instance' command line tool for all versions of VS. Recycle Experimental Instance of VS for all versions of VS.

## Process
# Run **‘Src\CleanCode.bat’**
# Open solution **‘Src\Runtime\Runtime.vs2013.sln’**
# Update assembly version numbers in **'Runtime.Versioning\MasterVersion.tt'**
# Clean & Rebuild the solution.
# Manually Find All, and Replace all instances of the old version number, in the following files:
	* Runtime.Shell\GeneratedCode\Guidance\**GuidanceWorkflow.gen.cs**
	* **Runtime.vs2013.sln**, **Runtime.vs2012.sln**, **Runtime.vs2010.sln** file 
	* **Runtime.slnbldr** file
	* **Assemblies.dgml**
	* **ToolkitInfoPerf.psess** file
# Clean & Rebuild the solution.
# Open solution **‘Src\Authoring\Authoring.vs2013.sln’**
# Clean & Rebuild the solution.
# Find All, and Replace all instances of the old version number, in the following files:
	* GeneratedCode\Guidance\**GuidanceWorkflow.gen.cs** in all toolkit projects
	* **Authoring.vs2013.sln**, **Authoring.vs2012.sln**, **Authoring.vs2010.sln** file 
	* **Authoring.slnbldr**
	* **Assemblies.dgml**
# Clean & Rebuild the solution.
# Close Visual Studio
# Install the **NuPatternToolkitBuilder.vsix** from **'Src\Binaries\12.0'**
# Open solution **‘Src\Authoring\Authoring.vs2013.sln’**
# In ‘Solution Builder’ tool window:
	* Select ‘Transform Templates’ on the _‘Pattern Toolkit’_ and _‘Library’_ elements for each of the 3  toolkits in Solution Builder.
	* Select ‘Transform Toolkit Info’ on the _‘Toolkit Info’_ element of each of the toolkits in Solution Builder.
# Clean & Rebuild the solution.
# Open solution **‘Src\Authoring\Authoring.Setup.vs2013.sln’**
# Clean & Rebuild the solution. _It should fail_ at this point because it is missing VSIXes built for the 10.0 and 11.0 version of NuPattern.
# Close the solution.
# Find All Files in **‘Src\’** for old version number to verify that all replacements have been made in all files in the source tree.
# Close Visual Studio.
# Run **‘Src\Clean Clode.bat’**
# Run **‘Src\ Make-All.bat’**, ensure all solutions build correctly, and MSI is produced as a result.
# Commit the changed source files.

Return the to [Building Releases](Building-Releases) page to complete the release process.