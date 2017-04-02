# Building & Testing NuPattern version 1.2.19.0
* [Obtaining the Source Code](#source) - where to obtain the source code.
* [Suppress Strong Name Verification](#suppress) - required to compile DelaySigned assemblies.
* [Development Tool Pre-Requisites](#devtools) - the development tools that are required to build and test the source code.
* [Building the Source Code](#build) - steps to build the source code.
* [Testing the Source Code](#test) - steps to run the various tests that verify the source code.
* [Project Contributions](#contributions) - how we are managing contributions for this version of the code.

## {anchor:source}Obtaining the Source Code
The source code for the 1.2.19.0 version of NuPattern is obtained by fetching the '1.2.19.0' branch from the master branch on the site to your fork. You can do this with the following git bash commands:
{{
<need git commands for this>
}}
**Note: Requests for contributing patches and new features to this version of the code (1.2.19.0) will be tightly restricted because the project is no longer owned or supported by Microsoft. We have reduced all support for this version of the project. All new features and fixes should be submitted to the current version of the code (version 1.3.20.0 or later).**

## {anchor:suppress}Suppressing Strong Name Verification
The source code is Delay-Signed, and Visual Studio will fail to build the source if we don’t instruct it to skip verification for the delay-signed assemblies.
* Open the 'Visual Studio Command Prompt' (as Administrator), and type:
{{
sn -Vr {"**"},**31bf3856ad364e35**
}}
**Note:** Typically, when a release is cut, the code is signed using the private key in an internal process by the owner of the project. In the case of this version (1.2.19.0) of the project, the original owner (Microsoft) will no longer sign the code with their private key as they no longer own or support the project.

## {anchor:devtools}Development Tools

### Visual Studio Pre-Requisites
This project will only build in Visual Studio 2010 (and **NOT** in Visual Studio 2012).
This build delivers a release of NuPattern targeted for Visual Studio 2010 only. 
**Note:** You cannot build a version of NuPattern in Visual Studio 2010 that targets Visual Studio 2012.
#### Visual Studio 2010
To build the code for Visual Studio 2010, you must have the following programs and tools installed on your machine to build, test or run the source code of this project.
# Visual Studio 2010 Professional/Premium/Ultimate (SP1 is optional)
# Visual Studio 2010 SDK (or Visual Studio 2010 SDK SP1 if you have installed the SP1 of Visual Studio 2010)
# Visual Studio 2010 Visualization & Modeling SDK
# [NuGet Package Manager](http://visualstudiogallery.msdn.microsoft.com/27077b70-9dad-4c64-adcf-c7cf6bc9970c) obtained from the Visual Studio Gallery.
# (Optional) [Git Source Control](http://visualstudiogallery.msdn.microsoft.com/63a7e40d-4d71-4fbb-a23b-d262124b8f4c) extension obtained from the Visual Studio Gallery.

### Other Development Tools
We recommend several other optional tools to install into Visual Studio to aid development of this project:
* [Visual T4](http://visualstudiogallery.msdn.microsoft.com/40a887aa-f3be-40ec-a85d-37044b239591) - for syntax highlighting, and intelli-sense for editing text templates
* [Visual Studio 2012 Color Theme Editor](http://visualstudiogallery.msdn.microsoft.com/366ad100-0003-4c9a-81a8-337d4e7ace05?SRC=Home) - for designing the look and feel of icons and dialogs
* [Resharper](http://visualstudiogallery.msdn.microsoft.com/EA4AC039-1B5C-4D11-804E-9BEDE2E63ECF?SRC=Home) - for general code analysis and management, and unit testing support
![Resharper](Building12190_Resharper.png|http://visualstudiogallery.msdn.microsoft.com/EA4AC039-1B5C-4D11-804E-9BEDE2E63ECF?SRC=Home)
**Note:** Exclusive! Regular committers to the project have been offered a **free full license** for Resharper. If you are interested, contact the project owner for more details, and become a regular committer to the project.

## {anchor:build}Building the Solutions

In Visual Studio 2010:
# Build Runtime Solution
	* Open the 'Src\Runtime\Runtime.sln' solution in Visual Studio
	* Select the 'Debug' solution configuration, and select the 'AnyCPU' platform. 
	* 'Transform All Templates' for the whole solution.
	* Clean, and Rebuild the solution.
	* Run the unit tests or integration tests to verify the build.
# Build Authoring Solution
	* Open the 'Src\Authoring\Authoring.sln' solution in Visual Studio
	* Select the 'Debug' solution configuration, and select the 'AnyCPU' platform. 
	* 'Transform All Templates' for the whole solution.
	* Clean, and Rebuild the solution. 
	* Run the units tests or integration tests to verify the build.
# Regenerate Guidance|PatternModel Code (optional, see Note 4 below)
	* Close Visual Studio
	* Open the Visual Studio 2010 command prompt
	* Run: vsixinstaller.exe "<yourlocalpath>\Src\Binaries\PatternToolkitBuilder.vsix"
	* Open Visual Studio, and open the 'Src\Runtime\Runtime.sln' solution
	* In 'Solution Builder', right-click on the 'Runtime.Shell/Assets/Guidance' node, and select 'Build Guidance'
	* Clean and Rebuild the solution.
	* Open the 'Src\Authoring\Authoring.sln' solution
	* In 'Solution Builder', right-click on the 'Authoring.Toolkit' node, and select 'Transform Templates'
	* In 'Solution Builder', right-click on the 'Authoring.Toolkit/Automation/Library' node, and select 'Transform Templates'
	* In 'Solution Builder', right-click on the 'Authoring.Toolkit/Assets/Guidance' node, and select 'Build Guidance'
	* Clean and Rebuild the solution.
	* In 'Extension Manager' uninstall the extensions: 'Pattern Toolkit Builder', 'Pattern Toolkit Support Library', and 'Pattern Toolkit Manager'.

**Note 1:** In all cases the 'Runtime.sln' needs to be compiled before the 'Authoring.sln' because it compiles, installs and places its assemblies in the folders of the 'Experimental Instance' of Visual Studio so that 'Authoring.sln' references.
**Note 2:** There is no need to always 'Transform All Templates' command in Visual Studio on these solutions unless you make changes to any text templates. 
**Note 3:** Some tests fail the first time they are run in a test run, simply re-run them again.
**Note 4:** Rebuilding the Guidance for the runtime/authoring/hol toolkits takes several minutes. You need to perform the 'Regenerate Guidance|PatternModel Code' steps only if you have done any of the following:
* Modified content within the guidance documents of 'Runtime .Shell' or 'Authoring.Toolkit'.
* Modified the 'PatternModel.patterndefinition' in 'Authoring.Toolkit' or 'Authoring.AutomationLibrary'.
* Modified the code in the 'InterfaceLayer' text templates in 'Authoring.AutomationLibrary'.
* Modified any of the VSIX attributes of any of the VSIXes in Runtime or Authoring solutions. Unlikely.

## {anchor:test}Testing the Build
### Running the Unit/Integration Tests
Both 'Runtime.sln' and 'Authoring.sln' include numerous 'Unit Test', 'Integration Test' and 'User Test' projects that can be run using MSTEST.EXE or other testing frameworks, like Resharper.

**Note:** Some tests fail the first time they are run in a test run, simply re-run them again to verify.

### Smoke Testing the Build
Youu mst manually smoke test this version of the code:
* Build the source code
* Open the 'Experimental Instance' of Visual Studio  
* Create a new 'Pattern Toolkit' project
	* Either, open the 'Add New Project' dialog, and create a new 'Pattern Toolkit' project.
	* Or, open the 'Add New Project' dialog, and create a new 'Blank Solution'. Then open the 'Solution Builder' window, click on the 'Add New Solution Element' hyperlink, and create a new 'Pattern Toolkit' project.

### {anchor:contributions}Project Contributions

Requests for contributing patches and new features to this version of the code (1.2.19.0) will be tightly restricted because the project is no longer owned by Microsoft, and therefore we are unable to release new binaries signed by Microsoft. We have reduced all support for this version of the project. All new features and fixes should be submitted to the current version of the code (version 1.3.20.0 and later)