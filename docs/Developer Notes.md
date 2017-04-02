# Developer's Notes
_These notes are provided here for developers and project contributors wishing to either: download the code to understand it, or download, compile, and create a patch for the code._

If you are simply looking to install NuPattern and use it, please go to the [Getting Started](Getting-Started) page.

So, you want to get coding?

Now you need to decide whether: 
# You just want to browse some of the code, see the structure of the code and some files in it, 
# You want a local copy of the code to compile it compile or investigate it get an understanding of it, 
# You know for sure you want to 'fork' the code, perhaps fix a bug or add a new feature, and start contributing changes back to the project. 
Don't worry, you can change your mind later at any time, we just want to get you going as quick as possible. 

If you are thinking (1) don't waste any more time here, just go to the [Source Code](http://nupattern.codeplex.com/SourceControl/BrowseLatest) page and navigate through the code on the left hand side pane. You can see each of the files in the source code, the structure of the code and projects, and even browse history of each file. You can change the branch of the code from the drop down at the top.

If you are thinking (2) or (3) you will need to download the source code to your machine. There are two ways to do that. You can either (2) just download the latest code as a zip file, and do as you please with it on your machine (but it is disconnected from the project), or (3) create your own fork of the code so that you can contribute changes back to the project.

If you are thinking (2) at this point, then jump to the [Preparation](#prepare).

If you know for sure you want (3) and you have used the GIT tools before, you may want to jump straight to the Quickstart below, else jump to the [Preparation](#prepare).

* [Build Quickstart](#quickstart) - skip all the details, and jump ahead to forking and compiling the code, if you already have Visual Studio, VSSDK, VMSDK, NuGet, WIX and GIT tools already installed.
Otherwise for (2) and (3) follow these topics:
* [Preparing to Build the Source Code](#prepare)
* [Building the Source Code](#build)
* [Contributing to the Project](#contrib)
* [Coding Guidelines](#guidelines)

## {anchor:quickstart}Build Quickstart
_"spare me the details! I want to fork of the code and get coding. I've got Visual Studio, and have installed the 'Visual Studio SDK', the 'Visualizaton and Modeling Tools', the 'WIX toolset' and 'NuGet', and got the GIT tools installed.   Just show me the code!_

This is the standard contributor's procedure for creating a fork of the latest code (with 'GIT for Windows' command line):
* Create yourself a fork in the 'NuPattern' project:
	* Go to the [Source Code](http://nupattern.codeplex.com/SourceControl/BrowseLatest) page, and click the 'Fork' link.
	* Name your fork and hit 'Save'.
	* Then hit the 'Clone' button and copy the link it presents you. (i.e. https://git01.codeplex.com/forks/<yourusername>/<yourforkname>)
	* Create a new directory on your 'C:\' drive called "Projects", and open that new folder in Windows Explorer.
	* Right-click and select 'Git Bash' on the menu. A git command window pops up.
	* type into the command window:
{{
git clone https://git01.codeplex.com/forks/<yourusername>/<yourforkname> Codeplex/NuPattern
}} Where: <yourusername> is your codeplex site username, and <yourforkname> is the name of your new fork.
* Wait for the clone to finish, reaching 100%, no errors.
* Skip assembly verification (the code is delay-signed):
	* Open the 'Visual Studio Command Prompt' (as Administrator), and type:
{{
sn -Vr *,24c7786d4a8b1a88
}}
* Now let's build the code:
	* Open the folder "Codeplex/NuPattern/Src", and double-click on one of the 'Make-*.bat' batch files.
		* If you _ONLY_ have Visual Studio 2010, then run the 'Make-VS2010.bat' file.
		* If you _ONLY_ have Visual Studio 2012, then run the 'Make-VS2012.bat' file.
		* If you have both Visual Studio 2010 _AND_ Visual Studio 2012, then run 'Make-All.bat' file.

The code will compile and build the projects outputs, and a folder will open to show you them built!

You can now skip the rest of the details in this document, or read it for better understanding about what just happened! or if something went wrong for you!

## {anchor:prepare}Preparing to Build
The following steps help you setup up your development environment, install the correct source control and development tools, obtain the source code, and prepare to download and build the code.

Please be aware that each branch of the source code (which correspond roughly to each major version of the project) may have slightly different requirements for development tools, and development guidelines.

### Development Environment
Building this project has been verified in Visual Studio 2010 and Visual Studio 20120 on Windows Vista and Windows 7 only.

**Note:** Building this project has not yet been verified on Windows XP or Windows 8. If you have verified either of those OS's please let us know and we can verify that for others. Send a note to the [Discussion List](http://nupattern.codeplex.com/discussions).

### Obtaining the Source
Decide whether you just want to download the code, possibly compile it and work with it offline from the project, or you know for sure you want to fork the code and make contributions back to the project.

#### Download the Code
You don't need any special tools installed to fetch the code for this.
* Create a new directory on your 'C:\' drive called "Projects", and open that new folder in Windows Explorer.
* Create new sub folders called 'CodePlex\NuPattern', and open that folder.
* Go to the [Source Code](http://nupattern.codeplex.com/SourceControl/BrowseLatest) page, and click the 'Download' link.
* Save the downloaded zip file, and unzip it to the 'C:\Project\CodePlex\NuPattern' folder.

You will have the latest version of the code, which you can investigate, compile and run.

#### Install the GIT tools
If you wish to make a contribution to the project, you must install some GIT tools on your machine in order to fork the code and submit changes. You **DO NOT** need any of these tools to just download the zipped code, view and build the code.

You must install some GIT tools, some common ones are:
# [Download GIT for Windows](http://code.google.com/p/msysgit/downloads/list?can=3&q=official+Git).
# [Download Tortoise GIT](http://code.google.com/p/tortoisegit/) (requires GIT for Windows above as well)

**Note:** If asked, make sure you select the recommended settings for handling text files in the installer (i.e. autocrlf = false)

#### {anchor:forking}Creating Your Own Fork
* Install the git tools (above).
* Create yourself a fork in the project:
	* Go to the [Source Code](http://nupattern.codeplex.com/SourceControl/BrowseLatest) page, and click the 'Fork' link.
	* Name your fork and hit 'Save'.
	* Then hit the 'Clone' button and copy the link it presents you. (i.e. https://git01.codeplex.com/forks/<yourusername>/<yourforkname>)
* Create a new directory on your 'C:\' drive called "Projects", and open that new folder in Windows Explorer.
	* Right-click and select 'Git Bash' on the menu. A git command window pops up.
	* type into the command window:
{{
git clone https://git01.codeplex.com/forks/<yourusername>/<yourforkname> Codeplex/NuPattern
}} Where: <yourusername> is your codeplex site username, and <yourforkname> is the name of your new fork.
* Wait for the clone to finish, reaching 100%, no errors.

**Note:** Your fork names are unique across all CodePlex projects, and so they must be named uniquely across all projects at CodePlex.

### {anchor:skipverification}Skip Strong Name Verification
The source code is Delay-Signed in all versions, and Visual Studio will fail to build the source code (with all kinds of bazaar errors) if we don’t instruct it to 'skip verification' for the delay-signed assemblies which we are building.

Depending on the version of the source code being built, you will need to skip verification for the public key token of the signing key {"(*.snk)"} file that the assemblies use to delay-sign with.
For example:
* Open the 'Visual Studio Command Prompt' (as Administrator), and type:
{{
sn -Vr *,<publickeytoken>
}}
**Note:** The value of the actual <publickeytoken> for the branch of the code you want to build may be different. See the next section.

**Note:** As a point of process, when a release is cut the code is signed using the private key in an internal process at The Outercurve Foundation. We expect only the core contributors to have to perform this step on agreed release schedules. This step is not required by regular contributors just to contribute code to the project. So no need to worry about it!

## {anchor:build}Building the Source Code
There are several versions of the source code divided into branches seen in the drop down list at the [Source Control](http://nupattern.codeplex.com/SourceControl/BrowseLatest) page. By default, 'master' is selected, and this brach represents the latest code fo the latest release of the code. Other branches are for code in development that is not yet released.

The 'master' branch is the main trunk of the code as it evolves over time, it will represent the current and latest released version of the code. The version branches (i.e 1.2.19.0, 1.3.20.0) represent specific versions of  the project as they were developed. They may be just snapshots of the code at some point in time, or branches being developed on that are not released yet. Other branches may be created as the project moves forward.

Make sure you follow the correct build process for the branch of code you are interested in building:
* master (current) [Building & Testing NuPattern Latest](BuildingMaster)
* {"[version 1.3.20.0](version-1.3.20.0)"} (in development) [Building & Testing NuPattern 1.3.20.0](Building13200)
* {"[version 1.2.19.0](version-1.2.19.0)"} (obsolete) [Building & Testing VSPAT 1.2.19.0](Building12190)

## {anchor:contrib}Contributing to the Project
If you wish to contribute a patch, fix or new feature, whatever; please feel free to do so at your own pace. 

Here is the basic process:
* First [create your own fork](#forking) of the code. 
* Get familiar with the [Coding Guidelines](#guidelines) below.
* Follow the [Coding Checklist](#checklist) to ensure you meet the basic quality bar for the project.
* Create a Pull Request

As you make changes to the project you commit them at your leisure to your local repo (GIT repository).
When you are ready to share those with the project or other people, and you have ensured your code is good to share, you 'push' those changes from your local repo to the repo at the 'origin'. This is the central repo on CodePlex. Where others can see them, and the project developers can apply your changes to the project.
Then you would go to your fork on the CodePlex site, and submit a 'Pull Request', and select the branch you wish to submit your patch to. This mechanism allow you to tell the project you have made some changes, and please could they apply them to the project. The project contributors will then be notified of your new patch' and it will be considered for integrating into the project. You will get notified of the status of your patch, and you may received some detailed comments that will help the patch get applied. In some cases, you may need to rework your patch, but ideally it just gets applied to the project!

**Note:** You don't need to be a developer or contributor to the project. Just go for it at your own pace!
See the [Contributing to NuPattern](Contributing-to-NuPattern) page for ideas on what we need help with, and the [Easy Contributions](Easy-Contributions) page for quick and small improvements that can be made to get started with.

## {anchor:checklist} Code Contributor Checklist
We have posted the [Coding Checklist](Coding-Checklist) for you that defines the minimal quality bar of code that should be contributed to the project. Please get familar with the simple checklist that ensures your code contribution can be accepted into the project quickly.

## {anchor:guidelines}Coding Guidelines & Standards
As with any open source development project we have established some architectural and coding guidelines and standards that we prefer that all contributors use so that we can maintain consistency across the codebase, and ease understanding of the project for newcomers. In addition to that, we have captured some more details about how the codebase is divided up, the patterns we use, where features are grouped together, and how to determine where your code should go. 

Please make yourself familiar with the information on the [Coding Guidelines](Coding-Guidelines) page.

## Creating a Release
When a release is determined, it needs to be packaged, signed and deployed. Normally a task for the project owners. See the [Building Releases](Building-Releases) page for more details on how to execute that process.