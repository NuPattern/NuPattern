# Building a Release

These notes document the process for building a planned release of NuPattern. (updated as of version: 1.4.24.0)

This process is expected to be executed by a project coordinator/developer, working with The Outercurve Foundation who provide the signing services for assemblies, and VSIXes and MSIs. 
The OCF contact for signing is currently Eric Schultz (wwahammy@hotmail.com). 
The signing process is fully automated,and implemented in MSBUILD scripts.

The release process delivers a set of binary assets (VSIXes, MSI, Documents etc.) that are then posted on this project site as a 'Release' containing downloads and information.

Table of Contents:
* [Versioning the Codebase](#versioningcodebase)
* [Building the Binary Assets](#buildingbinaries)
* [Compiling the Release Notes](#compilingreleasenotes)
* [Compiling the Online Guidance](#compilingguidance)
* [Labelling the Source Code](#labelsourcecode)
* [Creating a Release Page](#createreleasepage)
* [Publishing to the VS Gallery](#publishvsgallery)
* [Backing up the Released Binaries](#backingup)
* [Announcing the Release](#creatingannouncements)

## {anchor:versioningcodebase}Versioning the Codebase
The outcome of this section is:
* A incremented version in the codebase

### Overview
The version number of NuPattern is controlled centrally in a single file, and all code and deployable assets in the codebase use this version number for each release. 
The version number for each release must be incremented.
 
While much of the of the code and many of the assets in the codebase and deliverables are updated automatically as part of every build, some assets still require updating manually.

To complete this process, please follow the instructions detailed in the [Version Releases](Version-Releases) page.

Once the codebase version number has been incremented, you can proceed to building and signing the binaries.

## {anchor:buildingbinaries}Building the Binary Assets
The outcomes of this section are:
* An authenticode signed Authoring VSIX
* An authenticode signed HOL VSIX
* An authenticode signed MSI

Note: all VSIXes contain assemblies which are themselves both strong-name signed and authenticode signed.
Note: all VSIXes and MSI are authenticode signed.

### Overview
A delivered and signed VSIX or MSI contains a number of assemblies and other VSIXes which are themselves signed. 

Similar to a russian doll, a VSIX will be created from nested parts (i.e. Assemblies and VSIXes), where each nested part will require signing. This makes the process of signing a deliverable VSIX a muti-stage signing process. Requiring several signing and repackaging passes.

* All VSIXes contain at least 1 NuPattern assembly, which must be strong-name signed first. But all assemblies from all VSIXes can be signed in one pass. They are strong-named signed and then authenticode signed.
* The child nested VSIXes are then upgraded with the signed assemblies, and then authenticode signed themselves, in the next pass.
* The nesting (parent) VSIXes are then upgraded with the signed child VSIXes, and then signed themselves, in the next pass.
* Finally, the MSI is rebuilt with all the signed VSIXes, and then authenticode signed.

In addition to signing VSIXes, VSIXes are also recompressed as ZIP files with the highest compression ratio to minimize file size. We use additional MSBUILD targets and tasks in NuPattern.Build.Tasks.dll to achieve the maximum zip compression ratio.

### Process Overview

This process is fully automated by a number of MSBUILD scripts and targets which can be used in an automated build. Or easily invoked manually by a batch script in the 'Src\Release\' folder.

In either case, the signing service which is provided by the Outercurve Foundation requires that does the actual signing of the artifacts requires access using a authorized account. The credentials used for access must be provided to the MSBUILD scripts in some way. 
The credentials cannot be published publically, or reside in artifacts in the source tree.
* To run the MSBUILD scripts from an automated build server (i.e. teamcity.codebetter), the credentials can be defined in the build configuration, for the properties: $(SignUserName) and $(SignUserPassword).
* To run the MSBUILD scripts from the batch file in the 'Src\Release' folder, the credentials must be provided manually in the console window when prompted for by the batch file.

The automated process: builds all solutions in both VS2010 and VS2012 flavors, then signs all assemblies in all the built VSIXes. Repackages the signed assemblies into the VSIXes, signs those VSIXes, repackages those VSIXes into their containing VSIXes, signs those containing VSIXes, then rebuilds the MSI installer, and finally signs the installer.

Once the automated process is complete, the signed MSI installer and signed VSIXes can all be found in the 'Src\Release\Processed\Signed' folders.

## {anchor:compilingreleasenotes}Compiling the Release Notes
The outcomes of this section are:
* An online version of the Release Notes

### Process
* Open the [Release Notes](Release-Notes) page.
* Add highlights of release by summarising changes and commits in source tree.

## {anchor:compilingguidance}Compiling the Online Guidance
The outcomes of this section are:
* An online version of the Guidance for runtime and authoring.

### Process
For each of the following files: RuntimeGuidance.docx, AuthoringToolkitGuidance.docx in the 'Src\Common\Guidance' folder.
* Open the document
* Insert the caret after the first line 'title' in the document, and press ENTER
* File | Save. You should be promoted to expand sub documents. Answer 'Yes'.
* Insert the caret at the strat of the first blank line after the 'title' in the document.
* References | Table Of Contents | Insert Table of Contents
* Select: 
	* Formats: From Template
	* Show Levels: 4
* Press OK.
* File | Save As | PDF (in same location)
* Close the document, DONT SAVE IT!

## {anchor:labelsourcecode}Labelling the Source Code
The outcome of this section is a labelled source code marker

### Process
* Using the GIT tools, add a 'tag' to the current commit, using the format: Release-1-2-X-0
* Push the changes, and ensure you push the tags from your client.

## {anchor:createreleasepage}Creating a Release Page
The outcomes of this section are:
* A release page on this CodePlex project site - displayed [on this page](https://nupattern.codeplex.com/releases)
* Updated Wiki Pages

### Process
* Create a new Release on the CodePlex site. (You can copy and paste from a previous release and adapt the content for this release.)
* Use the version number in the title (i.e. NuPattern 1.3.20.0)
* Ensure the release is made 'public', and replaces the current release
* Give clear instructions at the top for what to install (i.e. the MSI Installer). If you copy the content of the previous release please ensure you update the hyperlinks to the files for this release.
* Add the MSI installer to the release, categorized as 'Binary', and set as the 'Default'
* Add both the Authoring and Runtime guidance PDF files, categorized as 'Documentation'.
* Add a brief  explanation of the release, reasons for it, and some technical details. You may copy the bulk of this from a previous release and adapt for this release.
* Add the labelled commit as the 'Change Set Number'

* Update the hyperlinks on the wiki: Home Page, Getting Started Page, others that reference the latest version

## {anchor:publishvsgallery}Publishing to the VS Gallery
The outcome of this section is:
* A release on the VS Gallery - displayed [on this page](http://visualstudiogallery.msdn.microsoft.com/332f060b-2352-41c9-b8dc-95d8ad21329b)

### Process
* Login to the VS Gallery as (msvspat@live.com)
* Edit the [current page](http://visualstudiogallery.msdn.microsoft.com/332f060b-2352-41c9-b8dc-95d8ad21329b)
* Upload a new version of the Installer.
* Update the Version number

## {anchor:backingup}Backing up the Released Binaries
The outcome of this section is:
* A backup and history of deliverables of the release [on this Skydrive](https://skydrive.live.com/?cid=1C8F13692E8E8DC0&id=1C8F13692E8E8DC0%21130)

## Process
* Create a new folder with the title of the release (i.e. 1.3.21.0)
* Upload the following:
	* MSI installer
	* VSIXes for all version of VS (in a zip file). i.e. 10.0.zip & 11.0.zip
	* Guidance (runtime and authoring) PDFs

## {anchor:creatingannouncements}Announcing the Release
The outcomes of this section are:
* Announcement on Twitter
* Announcement on the Project Site
* Announcement on Contributors Blogs