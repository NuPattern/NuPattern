# Building & Testing NuPattern Latest
* [Obtaining the Source Code](#source) - where to obtain the source code.
* [Suppress Strong Name Verification](#suppress) - required to compile DelaySigned assemblies.
* [Development Tool Pre-Requisites](#devtools) - the development tools that are required to build and test the source code.
* [Building the Source Code](#build) - steps to build the source code.
* [Testing the Source Code](#test) - steps to run the various tests that verify the source code.

## {anchor:source}Obtaining the Source Code
The source code for the latest version of NuPattern is obtained by fetching the 'master' branch on the site to your fork. You can do this with the following git bash commands:
{{
git pull origin master
}}
## {anchor:suppress}Suppressing Strong Name Verification
The source code is Delay-Signed, and Visual Studio will fail to build the source if we don’t instruct it to skip verification for the delay-signed assemblies.
* Open the 'Visual Studio Command Prompt' (as Administrator), and type:
{{
sn -Vr *,24c7786d4a8b1a88
}}
**Note:** When a release is cut, the code is signed using the private key in an internal process at The Outercurve Foundation. We expect only the project owner to have to perform this step on agreed release schedules. This step is not required to build or patch or contribute code to the project.

## {anchor:devtools}Development Tools

### Visual Studio Pre-Requisites
This project will build in either Visual Studio 2010 or Visual Studio 2012.
Each build from each version of Visual Studio delivers a release of NuPattern targeted for that version of Visual Studio only. 
**Note:** At this time, you cannot build a version of NuPattern in either version of Visual Studio that targets both versions of Visual Studio.
#### Visual Studio 2010
To build the code for Visual Studio 2010, you must have the following programs and tools installed on your machine to build, test or run the source code of this project.
# Visual Studio 2010 Professional/Premium/Ultimate (SP1 is optional)
# Visual Studio 2010 SDK (or Visual Studio 2010 SDK SP1 if you have installed the SP1 of Visual Studio 2010)
# Visual Studio 2010 Visualization & Modeling SDK
# [NuGet Package Manager](http://visualstudiogallery.msdn.microsoft.com/27077b70-9dad-4c64-adcf-c7cf6bc9970c) obtained from the Visual Studio Gallery.
# (Optional) [Git Source Control](http://visualstudiogallery.msdn.microsoft.com/63a7e40d-4d71-4fbb-a23b-d262124b8f4c) extension obtained from the Visual Studio Gallery.
#### Visual Studio 2012
To build the code for Visual Studio 2012, you must have the following programs and tools installed on your machine to build, test or run the source code of this project.
# Visual Studio 2012 Professional/Premium/Ultimate
# Visual Studio 2012 SDK
# Visual Studio 2012 Visualization & Modeling SDK
# [NuGet Package Manager](http://visualstudiogallery.msdn.microsoft.com/27077b70-9dad-4c64-adcf-c7cf6bc9970c) obtained from the Visual Studio Gallery.
# (Optional) [Git Source Control](http://visualstudiogallery.msdn.microsoft.com/63a7e40d-4d71-4fbb-a23b-d262124b8f4c) extension obtained from the Visual Studio Gallery.
# [WIX Toolset](http://wixtoolset.org/) obtained from http://wixtoolset.org/ or http://wix.codeplex.com.

### Other Development Tools
We recommend several other optional tools to install into Visual Studio to aid development of this project:
* [Productivity Power Tools](http://visualstudiogallery.msdn.microsoft.com/3a96a4dc-ba9c-4589-92c5-640e07332afd) - for editing project files, removing unused usings and resolving mixed tabs and spaces.
* [Visual T4](http://visualstudiogallery.msdn.microsoft.com/40a887aa-f3be-40ec-a85d-37044b239591) - for syntax highlighting, and intelli-sense for editing text templates
* [Visual Studio 2012 Color Theme Editor](http://visualstudiogallery.msdn.microsoft.com/366ad100-0003-4c9a-81a8-337d4e7ace05?SRC=Home) - for designing the look and feel of icons and dialogs
* [Resharper](http://visualstudiogallery.msdn.microsoft.com/EA4AC039-1B5C-4D11-804E-9BEDE2E63ECF?SRC=Home) - for general code analysis and management, and unit testing support
![Resharper](BuildingMaster_Resharper.png|http://visualstudiogallery.msdn.microsoft.com/EA4AC039-1B5C-4D11-804E-9BEDE2E63ECF?SRC=Home)
**Note:** Exclusive! Regular committers to the project have been offered a **free full license** for Resharper. If you are interested, contact the project owner for more details, and become a regular committer to the project.

## {anchor:build}Building the Solutions
### Building on command line
If you just want to quickly build the source code the fastest way, you can use MSBUILD on the command line, or use the handy included batch files to build the code for either version of Visual Studio.
* Open the folder "Src" folder of your local fork, and double-click on one of the 'make' batch files.
	* If you ONLY have Visual Studio 2010, -> then run the **'Make-VS2010.bat'** file.
	* If you ONLY have Visual Studio 2012, -> then run the **'Make-VS2012.bat'** file.
	* If you have both Visual Studio 2010 AND Visual Studio 2012, -> then run **'Make-All.bat'** file.

The code will compile and build the projects outputs, and a folder will open to show you them built!

### Building in Visual Studio 
If you prefer to build the solutions in Visual Studio, you will need to open the version of the solution in the version of Visual Studio you are targeting, and then select the appropriate build configuration for the target version of Visual Studio.
For example, If you want to build the VS2010 version of NuPattern, you open Visual Studio 2010, and load the solution specifically for VS2010 (e.g. Runtime.vs2010.sln), then select the 'Debug-VS2010' build configuration in the solution configurations. Clean and Rebuild.

NuPattern has multiple solutions that must be compiled in the correct order, please follow these details instructions.

Depending on the version of Visual Studio (.vs201X) you want to build with:
# Build Runtime Solution
	* Open the 'Src\Runtime\Runtime.vs201X.sln' solution in Visual Studio
	* Select the 'Debug-VS201X' solution configuration, and select the 'AnyCPU' platform. 
	* Clean, and Rebuild the solution.
	* Run the unit tests or integration tests to verify the build.
# Build Authoring Solution
	* Open the 'Src\Authoring\Authoring.vs201X.sln' solution in Visual Studio
	* Select the 'Debug-VS201X' solution configuration, and select the 'AnyCPU' platform. 
	* Clean, and Rebuild the solution. 
	* Run the units tests or integration tests to verify the build.
# Regenerate Guidance|PatternModel Code (optional, see Note 4 below)
	* Close Visual Studio
	* Open the Visual Studio command prompt for the version of VS you are building
	* Run: vsixinstaller.exe "<yourlocalpath>\Src\Binaries\NuPatternToolkitBuilder.1X.0.vsix"
	* Open Visual Studio, and open the 'Src\Runtime\Runtime.vs201X.sln' solution
	* In 'Solution Builder', right-click on the 'Runtime.Shell/Assets/Guidance' node, and select 'Build Guidance'
	* Clean and Rebuild the solution.
	* Open the 'Src\Authoring\Authoring.vs201X.sln' solution
	* In 'Solution Builder', right-click on the 'Authoring.PatternToolkit' node, and select 'Transform Templates'
	* In 'Solution Builder', right-click on the 'Authoring.PatternToolkit/Automation/Library' node, and select 'Transform Templates'
	* In 'Solution Builder', right-click on the 'Authoring.PatternToolkit/Assets/Guidance' node, and select 'Build Guidance'
	* Clean and Rebuild the solution.
	* In 'Extension Manager' uninstall the extensions: 'NuPattern Toolkit Builder', 'NuPattern Toolkit Library', and 'NuPattern Toolkit Manager'.

**Note 1:** In all cases the 'Runtime.vs201X.sln' needs to be compiled before the 'Authoring.vs201X.sln' because it compiles, installs and places its assemblies in the folders of the 'Experimental Instance' of Visual Studio so that 'Authoring.vs201X.sln' references.
**Note 2:** There is no need to use the 'Transform Templates' command in Visual Studio on these solutions anymore. These solutions will automatically do their own template transformation when they are re-built, or when those templates are updated. Manually transforming any of the code in the 'GeneratedCode' folder of any DSL projects will cause the wrong code to be generated! Avoid this.
**Note 3:** Some tests fail the first time they are run in a test run, simply re-run them again.
**Note 4:** Rebuilding the Guidance for the runtime/authoring/hol toolkits takes several minutes. You need to perform the 'Regenerate Guidance|PatternModel Code' steps only if you have done any of the following:
* Building the VS2010 targeted version of the code in Visual Studio 2010.
* Modified content within the guidance documents of 'Runtime .Shell' or 'Authoring.PatternToolkit'.
* Modified the 'PatternModel.patterndefinition' in 'Authoring.PatternToolkit' or 'Authoring.PatternToolkitLibrary'.
* Modified the code in the 'InterfaceLayer' text templates in 'Authoring.PatternToolkitLibrary'.
* Modified any of the VSIX attributes of any of the VSIXes in Runtime or Authoring solutions. Unlikely.

## {anchor:test}Testing the Build
### Running the Unit/Integration Tests
Both 'Runtime.vs201X.sln' and 'Authoring.vs201X.sln' include numerous 'Unit Test', 'Integration Test' and 'User Test' projects that can be run using MSTEST.EXE or other testing frameworks, like Resharper.

**Note:** Some tests fail the first time they are run in a test run, simply re-run them again to verify.

### Smoke Testing the Build
There are a small number of 'User Tests' that perform smoke testing in the Authoring solution. You may run these basic tests.
If you wish to manually verify:
* Build the source code
* Open the 'Experimental Instance' of Visual Studio  
* Create a new 'Pattern Toolkit' project
	* Either, open the 'Add New Project' dialog, and create a new 'Pattern Toolkit' project.
	* Or, open the 'Solution Builder' window, click on the 'Create New Empty Solution' hyperlink, and then click on the 'Add New Solution Element' hyperlink, and create a new 'Pattern Toolkit' project.