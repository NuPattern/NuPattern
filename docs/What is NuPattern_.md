# What Is NuPattern?
The name NuPattern refers to the technology which builds and runs 'Pattern Toolkits'. Concretely, it is **a collection of VS extensions to Visual Studio** that together offer the experience of creating, and using Pattern Toolkits.

The NuPattern components as seen in the Visual Studio Extension Manager.
![NuPattern Toolkits](What is NuPattern?_ExtensionManager.png|http://www.codeplex.com/Download?ProjectName=vspat&DownloadId=364621)

The following is the list of Visual Studio extensions (VSIXes) that can be installed and manged in Visual Studio:
* <![](What is NuPattern?_VsixIconRunTime.png)**NuPattern Toolkit Manager** - This extension provides the automation framework, tools, windows, editors for loading and running pattern toolkits in Visual Studio. 
	* It provides the persistence store for the instances of the patterns contained within the toolkits (called 'solution elements'). 
	* It provides Visual Studio services and a MEF API for manipulating pattern toolkits, and their solution elements. 
	* It also provides the 'Solution Builder' tool window for displaying solution elements, and all the user interface elements for working with an installed pattern toolkit.
* <![](What is NuPattern?_VsixIconPatternToolkit.png)**NuPattern Toolkit Builder** - This extension provides the project templates, modeling designers, automation and extensive guidance for building 'Pattern Toolkit' projects in Visual Studio. 
	* This extension is itself a 'Pattern Toolkit', and as such is used through the 'Solution Builder' window. 
	* It provides solution elements in the 'Solution Builder', and guidance in the 'Guidance Explorer' tool windows to guide you through the creation of your own 'Pattern Toolkit' project. 
	* When a 'Pattern Toolkit' is built, this extension generates a *.VSIX file that can be installed into Visual Studio to apply that pattern.
* <![](What is NuPattern?_VsixIconPatternToolkitLibrary.png)**NuPattern Toolkit Library** - This extension provides a project template, item templates and automation for creating your own custom automation in your own 'Pattern Toolkit' project. 
	* This extension is itself a 'Pattern Toolkit', and as such is used through the 'Solution Builder' window. 
	* This extension integrates with, and extends the 'Pattern Toolkit Builder' extension to provide an 'Automation Library' project for a 'Pattern Toolkit' project.
* <![](What is NuPattern?_VsixIconHOL.png)**NuPattern Toolkit Builder Hands-On Labs** - This extension provides a hands-on labs for guiding you through the process of creating your first 'Pattern Toolkit' projects. 
	* It contains all the guidance and automation to create a 'Pattern Toolkit' project. 
	* It is the recommended starting point for 'Pattern Toolkit' novices.