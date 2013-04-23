﻿//------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by a tool.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using NuPattern.Runtime.Composition;
using NuPattern.Runtime.Guidance;
using NuPattern.Runtime.Guidance.Extensions;
using NuPattern.Runtime.Guidance.Workflow;

namespace NuPattern.Authoring.HandsOnLabs.Guidance
{
	/// <summary>
	/// Defines a base class for the guidance workflow for this guidance extension.
	/// </summary>
	[CLSCompliant(false)]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	[System.CodeDom.Compiler.GeneratedCode("NuPattern Toolkit Builder", "1.3.20.0")]
	public partial class ProcessWorkflow : GuidanceWorkflow
	{
		/// <summary>
		/// Gets the composition service.
		/// </summary>
		[Import]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		private INuPatternCompositionService Composition { get; set; }

		/// <summary>
		/// Gets whether to ignore all post conditions and enable all actions.
		/// </summary>
		public override bool IgnorePostConditions { get { return true; } }

		/// <summary>
		/// Initializes the workflow.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals")]
		protected override void OnInitialize()
		{
			base.OnInitialize();
			this.Name = "GuidanceWorkflow";
			var initial1 = new Initial
			{
				Name = "Hands-On Labs for Creating Pattern Toolkits",
			};
			this.ConnectTo(initial1);
			var fork2 = new Fork
			{
				Name = "Hands-On Lab: Getting Started",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/HandsOnLabGettingStarted.mht",
			};
			initial1.ConnectTo(fork2);
			var fork3 = new Fork
			{
				Name = "Setup for Getting Started Lab",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/SetupforGettingStartedLab.mht",
			};
			fork2.ConnectTo(fork3);
			var guidanceaction4 = new GuidanceAction
			{
				Name = "Prepare the Experimental Instance of Visual Studio",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/PreparetheExperimentalInstanceofVisualStudio.mht",
			};
			fork3.ConnectTo(guidanceaction4);
			var join5 = new Join
			{
				Name = "Join1",
			};
			guidanceaction4.ConnectTo(join5);
			var fork6 = new Fork
			{
				Name = "Part 1: Create a Pattern Toolkit Project",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/Part1CreateaPatternToolkitProject.mht",
			};
			join5.ConnectTo(fork6);
			var guidanceaction7 = new GuidanceAction
			{
				Name = "Add the Widget Element to the Pattern Model",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/AddtheWidgetElementtothePatternModel.mht",
			};
			fork6.ConnectTo(guidanceaction7);
			var guidanceaction8 = new GuidanceAction
			{
				Name = "Build and Run the Toolkit",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/BuildandRuntheToolkit.mht",
			};
			guidanceaction7.ConnectTo(guidanceaction8);
			var guidanceaction9 = new GuidanceAction
			{
				Name = "Create a New Solution",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/CreateaNewSolution.mht",
			};
			guidanceaction8.ConnectTo(guidanceaction9);
			var guidanceaction10 = new GuidanceAction
			{
				Name = "Add a new Widget Solution to Solution Builder",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/AddanewWidgetSolutiontoSolutionBuilder.mht",
			};
			guidanceaction9.ConnectTo(guidanceaction10);
			var guidanceaction11 = new GuidanceAction
			{
				Name = "Widget Solutions are added to Solution Builder",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/WidgetSolutionsareaddedtoSolutionBuilder.mht",
			};
			guidanceaction10.ConnectTo(guidanceaction11);
			var guidanceaction12 = new GuidanceAction
			{
				Name = "Add More Widgets to Solution Builder",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/AddMoreWidgetstoSolutionBuilder.mht",
			};
			guidanceaction11.ConnectTo(guidanceaction12);
			var guidanceaction13 = new GuidanceAction
			{
				Name = "What Have We Done?",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/WhatHaveWeDone.mht",
			};
			guidanceaction12.ConnectTo(guidanceaction13);
			var join14 = new Join
			{
				Name = "Join2",
			};
			guidanceaction13.ConnectTo(join14);
			var fork15 = new Fork
			{
				Name = "Part 2: Add Project and Item Templates",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/Part2AddProjectandItemTemplates.mht",
			};
			join14.ConnectTo(fork15);
			var guidanceaction16 = new GuidanceAction
			{
				Name = "Add a Project Template",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/AddaProjectTemplate.mht",
			};
			fork15.ConnectTo(guidanceaction16);
			var guidanceaction17 = new GuidanceAction
			{
				Name = "Create a VS Template Launch Point",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/CreateaVSTemplateLaunchPoint.mht",
			};
			guidanceaction16.ConnectTo(guidanceaction17);
			var guidanceaction18 = new GuidanceAction
			{
				Name = "Rename the Template Launch Point",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/RenametheTemplateLaunchPoint.mht",
			};
			guidanceaction17.ConnectTo(guidanceaction18);
			var guidanceaction19 = new GuidanceAction
			{
				Name = "Configure the Template Launch Point",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/ConfiguretheTemplateLaunchPoint.mht",
			};
			guidanceaction18.ConnectTo(guidanceaction19);
			var guidanceaction20 = new GuidanceAction
			{
				Name = "Build and Test",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/BuildandTest.mht",
			};
			guidanceaction19.ConnectTo(guidanceaction20);
			var guidanceaction21 = new GuidanceAction
			{
				Name = "Add an Item Template",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/AddanItemTemplate.mht",
			};
			guidanceaction20.ConnectTo(guidanceaction21);
			var guidanceaction22 = new GuidanceAction
			{
				Name = "Create a Command for Unfolding the Template",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/CreateaCommandforUnfoldingtheTemplate.mht",
			};
			guidanceaction21.ConnectTo(guidanceaction22);
			var guidanceaction23 = new GuidanceAction
			{
				Name = "Rename the Command",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/RenametheCommand.mht",
			};
			guidanceaction22.ConnectTo(guidanceaction23);
			var guidanceaction24 = new GuidanceAction
			{
				Name = "Configure the Command",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/ConfiguretheCommand.mht",
			};
			guidanceaction23.ConnectTo(guidanceaction24);
			var guidanceaction25 = new GuidanceAction
			{
				Name = "Create a Launch Point to trigger the Unfold Command",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/CreateaLaunchPointtotriggertheUnfoldCommand.mht",
			};
			guidanceaction24.ConnectTo(guidanceaction25);
			var guidanceaction26 = new GuidanceAction
			{
				Name = "Enable Navigation",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/EnableNavigation.mht",
			};
			guidanceaction25.ConnectTo(guidanceaction26);
			var guidanceaction27 = new GuidanceAction
			{
				Name = "Build, Run and Create Widget Classes",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/BuildRunandCreateWidgetClasses.mht",
			};
			guidanceaction26.ConnectTo(guidanceaction27);
			var guidanceaction28 = new GuidanceAction
			{
				Name = "Where Are We Now?",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/WhereAreWeNow.mht",
			};
			guidanceaction27.ConnectTo(guidanceaction28);
			var join29 = new Join
			{
				Name = "Join3",
			};
			guidanceaction28.ConnectTo(join29);
			var fork30 = new Fork
			{
				Name = "Part 3: Generating Code with T4 Templates",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/Part3GeneratingCodewithT4Templates.mht",
			};
			join29.ConnectTo(fork30);
			var guidanceaction31 = new GuidanceAction
			{
				Name = "Add a New T4 Text Template",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/AddaNewT4TextTemplate.mht",
			};
			fork30.ConnectTo(guidanceaction31);
			var guidanceaction32 = new GuidanceAction
			{
				Name = "Add a Command for Running the T4 Template",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/AddaCommandforRunningtheT4Template.mht",
			};
			guidanceaction31.ConnectTo(guidanceaction32);
			var guidanceaction33 = new GuidanceAction
			{
				Name = "Configure the Command for Running the T4 Template",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/ConfiguretheCommandforRunningtheT4Template.mht",
			};
			guidanceaction32.ConnectTo(guidanceaction33);
			var guidanceaction34 = new GuidanceAction
			{
				Name = "Create a Launch Point to Trigger Code Generation With a Menu",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/CreateaLaunchPointtoTriggerCodeGenerationWithaMenu.mht",
			};
			guidanceaction33.ConnectTo(guidanceaction34);
			var guidanceaction35 = new GuidanceAction
			{
				Name = "Build and Test the Context Menu Launch Point",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/BuildandTesttheContextMenuLaunchPoint.mht",
			};
			guidanceaction34.ConnectTo(guidanceaction35);
			var guidanceaction36 = new GuidanceAction
			{
				Name = "Create a Launch Point to Trigger Code Generation On Project Build",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/CreateaLaunchPointtoTriggerCodeGenerationOnProjectBuild.mht",
			};
			guidanceaction35.ConnectTo(guidanceaction36);
			var guidanceaction37 = new GuidanceAction
			{
				Name = "Build and Test the Build Event Launch Point",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/BuildandTesttheBuildEventLaunchPoint.mht",
			};
			guidanceaction36.ConnectTo(guidanceaction37);
			var guidanceaction38 = new GuidanceAction
			{
				Name = "What Have We Got Now?",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/WhatHaveWeGotNow.mht",
			};
			guidanceaction37.ConnectTo(guidanceaction38);
			var join39 = new Join
			{
				Name = "Join4",
			};
			guidanceaction38.ConnectTo(join39);
			var fork40 = new Fork
			{
				Name = "Part 4: Create Guidance",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/Part4CreateGuidance.mht",
			};
			join39.ConnectTo(fork40);
			var guidanceaction41 = new GuidanceAction
			{
				Name = "Examine the Guidance Document",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/ExaminetheGuidanceDocument.mht",
			};
			fork40.ConnectTo(guidanceaction41);
			var guidanceaction42 = new GuidanceAction
			{
				Name = "Edit the Guidance Document",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/EdittheGuidanceDocument.mht",
			};
			guidanceaction41.ConnectTo(guidanceaction42);
			var guidanceaction43 = new GuidanceAction
			{
				Name = "Build the Guidance",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/BuildtheGuidance.mht",
			};
			guidanceaction42.ConnectTo(guidanceaction43);
			var guidanceaction44 = new GuidanceAction
			{
				Name = "Associate the Guidance with the Pattern",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/AssociatetheGuidancewiththePattern.mht",
			};
			guidanceaction43.ConnectTo(guidanceaction44);
			var guidanceaction45 = new GuidanceAction
			{
				Name = "Build and Test the Guidance",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/BuildandTesttheGuidance.mht",
			};
			guidanceaction44.ConnectTo(guidanceaction45);
			var join46 = new Join
			{
				Name = "Join5",
			};
			guidanceaction45.ConnectTo(join46);
			var guidanceaction47 = new GuidanceAction
			{
				Name = "Hands-On Lab 1 Review",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/HandsOnLab1Review.mht",
			};
			join46.ConnectTo(guidanceaction47);
			var join48 = new Join
			{
				Name = "Join6",
			};
			guidanceaction47.ConnectTo(join48);
			var fork49 = new Fork
			{
				Name = "Hands-On Lab: Building Better Pattern Toolkits",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/HandsOnLabBuildingBetterPatternToolkits.mht",
			};
			join48.ConnectTo(fork49);
			var fork50 = new Fork
			{
				Name = "Part 1: Modeling Variability in a Pattern",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/Part1ModelingVariabilityinaPattern.mht",
			};
			fork49.ConnectTo(fork50);
			var guidanceaction51 = new GuidanceAction
			{
				Name = "The Difference between Modeling a Pattern and Modeling Variability",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/TheDifferencebetweenModelingaPatternandModelingVariability.mht",
			};
			fork50.ConnectTo(guidanceaction51);
			var guidanceaction52 = new GuidanceAction
			{
				Name = "Think About Variability",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/ThinkAboutVariability.mht",
			};
			guidanceaction51.ConnectTo(guidanceaction52);
			var guidanceaction53 = new GuidanceAction
			{
				Name = "Create a new Pattern Toolkit for ASP.NET MVC",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/CreateanewPatternToolkitforASPNETMVC.mht",
			};
			guidanceaction52.ConnectTo(guidanceaction53);
			var guidanceaction54 = new GuidanceAction
			{
				Name = "Verify the Development Environment",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/VerifytheDevelopmentEnvironment.mht",
			};
			guidanceaction53.ConnectTo(guidanceaction54);
			var guidanceaction55 = new GuidanceAction
			{
				Name = "Add Controllers to the Pattern Model",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/AddControllerstothePatternModel.mht",
			};
			guidanceaction54.ConnectTo(guidanceaction55);
			var guidanceaction56 = new GuidanceAction
			{
				Name = "Verify the User Experience",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/VerifytheUserExperience.mht",
			};
			guidanceaction55.ConnectTo(guidanceaction56);
			var guidanceaction57 = new GuidanceAction
			{
				Name = "Add Actions to the Pattern Model",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/AddActionstothePatternModel.mht",
			};
			guidanceaction56.ConnectTo(guidanceaction57);
			var guidanceaction58 = new GuidanceAction
			{
				Name = "Test the Action",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/TesttheAction.mht",
			};
			guidanceaction57.ConnectTo(guidanceaction58);
			var guidanceaction59 = new GuidanceAction
			{
				Name = "Add Data to an Action",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/AddDatatoanAction.mht",
			};
			guidanceaction58.ConnectTo(guidanceaction59);
			var join60 = new Join
			{
				Name = "Join7",
			};
			guidanceaction59.ConnectTo(join60);
			var fork61 = new Fork
			{
				Name = "Part 2: Extension Points",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/Part2ExtensionPoints.mht",
			};
			join60.ConnectTo(fork61);
			var guidanceaction62 = new GuidanceAction
			{
				Name = "Create an Extension Point for Views",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/CreateanExtensionPointforViews.mht",
			};
			fork61.ConnectTo(guidanceaction62);
			var guidanceaction63 = new GuidanceAction
			{
				Name = "Test the Extension Point",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/TesttheExtensionPoint.mht",
			};
			guidanceaction62.ConnectTo(guidanceaction63);
			var guidanceaction64 = new GuidanceAction
			{
				Name = "Register the Extension Point in Visual Studio",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/RegistertheExtensionPointinVisualStudio.mht",
			};
			guidanceaction63.ConnectTo(guidanceaction64);
			var guidanceaction65 = new GuidanceAction
			{
				Name = "Add a New Pattern Toolkit to the Solution",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/AddaNewPatternToolkittotheSolution.mht",
			};
			guidanceaction64.ConnectTo(guidanceaction65);
			var guidanceaction66 = new GuidanceAction
			{
				Name = "Designate the SearchView Pattern as Extending View",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/DesignatetheSearchViewPatternasExtendingView.mht",
			};
			guidanceaction65.ConnectTo(guidanceaction66);
			var guidanceaction67 = new GuidanceAction
			{
				Name = "Add Extension Properties to Search View",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/AddExtensionPropertiestoSearchView.mht",
			};
			guidanceaction66.ConnectTo(guidanceaction67);
			var guidanceaction68 = new GuidanceAction
			{
				Name = "Test the Implemented Extension Point",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/TesttheImplementedExtensionPoint.mht",
			};
			guidanceaction67.ConnectTo(guidanceaction68);
			var join69 = new Join
			{
				Name = "Join8",
			};
			guidanceaction68.ConnectTo(join69);
			var fork70 = new Fork
			{
				Name = "Part 3: Validation",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/Part3Validation.mht",
			};
			join69.ConnectTo(fork70);
			var guidanceaction71 = new GuidanceAction
			{
				Name = "Enable Validation of the Whole Pattern",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/EnableValidationoftheWholePattern.mht",
			};
			fork70.ConnectTo(guidanceaction71);
			var guidanceaction72 = new GuidanceAction
			{
				Name = "Test Validation",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/TestValidation.mht",
			};
			guidanceaction71.ConnectTo(guidanceaction72);
			var guidanceaction73 = new GuidanceAction
			{
				Name = "Using Built-In Cardinality Validation",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/UsingBuiltInCardinalityValidation.mht",
			};
			guidanceaction72.ConnectTo(guidanceaction73);
			var guidanceaction74 = new GuidanceAction
			{
				Name = "Test Cardinality Validation",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/TestCardinalityValidation.mht",
			};
			guidanceaction73.ConnectTo(guidanceaction74);
			var guidanceaction75 = new GuidanceAction
			{
				Name = "Add Validation to a Node in the Pattern Model",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/AddValidationtoaNodeinthePatternModel.mht",
			};
			guidanceaction74.ConnectTo(guidanceaction75);
			var guidanceaction76 = new GuidanceAction
			{
				Name = "Test Property Rule Validation",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/TestPropertyRuleValidation.mht",
			};
			guidanceaction75.ConnectTo(guidanceaction76);
			var join77 = new Join
			{
				Name = "Join9",
			};
			guidanceaction76.ConnectTo(join77);
			var fork78 = new Fork
			{
				Name = "Part 4: Custom Validation Commands",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/Part4CustomValidationCommands.mht",
			};
			join77.ConnectTo(fork78);
			var guidanceaction79 = new GuidanceAction
			{
				Name = "Add a New Custom-Coded Validation Rule",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/AddaNewCustomCodedValidationRule.mht",
			};
			fork78.ConnectTo(guidanceaction79);
			var guidanceaction80 = new GuidanceAction
			{
				Name = "Configure the Custom-Coded Validation Rule",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/ConfiguretheCustomCodedValidationRule.mht",
			};
			guidanceaction79.ConnectTo(guidanceaction80);
			var guidanceaction81 = new GuidanceAction
			{
				Name = "Test the Custom Validation",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/TesttheCustomValidation.mht",
			};
			guidanceaction80.ConnectTo(guidanceaction81);
			var join82 = new Join
			{
				Name = "Join10",
			};
			guidanceaction81.ConnectTo(join82);
			var fork83 = new Fork
			{
				Name = "Part 5: Wizards",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/Part5Wizards.mht",
			};
			join82.ConnectTo(fork83);
			var guidanceaction84 = new GuidanceAction
			{
				Name = "Configure Validation Rules",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/ConfigureValidationRules.mht",
			};
			fork83.ConnectTo(guidanceaction84);
			var guidanceaction85 = new GuidanceAction
			{
				Name = "Create a new Properties Page",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/CreateanewPropertiesPage.mht",
			};
			guidanceaction84.ConnectTo(guidanceaction85);
			var guidanceaction86 = new GuidanceAction
			{
				Name = "Create a New Configuration Wizard",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/CreateaNewConfigurationWizard.mht",
			};
			guidanceaction85.ConnectTo(guidanceaction86);
			var guidanceaction87 = new GuidanceAction
			{
				Name = "Configure the Wizard",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/ConfiguretheWizard.mht",
			};
			guidanceaction86.ConnectTo(guidanceaction87);
			var guidanceaction88 = new GuidanceAction
			{
				Name = "Configure the Launch Points",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/ConfiguretheLaunchPoints.mht",
			};
			guidanceaction87.ConnectTo(guidanceaction88);
			var guidanceaction89 = new GuidanceAction
			{
				Name = "Test the Wizard",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/TesttheWizard.mht",
			};
			guidanceaction88.ConnectTo(guidanceaction89);
			var join90 = new Join
			{
				Name = "Join11",
			};
			guidanceaction89.ConnectTo(join90);
			var fork91 = new Fork
			{
				Name = "Part 6: Working with Project Templates and Item Templates",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/Part6WorkingwithProjectTemplatesandItemTemplates.mht",
			};
			join90.ConnectTo(fork91);
			var guidanceaction92 = new GuidanceAction
			{
				Name = "Export a New MVC Project Template from Visual Studio",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/ExportaNewMVCProjectTemplatefromVisualStudio.mht",
			};
			fork91.ConnectTo(guidanceaction92);
			var guidanceaction93 = new GuidanceAction
			{
				Name = "Import the Project Template into the Toolkit Project",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/ImporttheProjectTemplateintotheToolkitProject.mht",
			};
			guidanceaction92.ConnectTo(guidanceaction93);
			var guidanceaction94 = new GuidanceAction
			{
				Name = "Configure the Project Template to Unfold with the Toolkit",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/ConfiguretheProjectTemplatetoUnfoldwiththeToolkit.mht",
			};
			guidanceaction93.ConnectTo(guidanceaction94);
			var guidanceaction95 = new GuidanceAction
			{
				Name = "Test Custom Project Template",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/TestCustomProjectTemplate.mht",
			};
			guidanceaction94.ConnectTo(guidanceaction95);
			var guidanceaction96 = new GuidanceAction
			{
				Name = "Create a New Controller Item Template",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/CreateaNewControllerItemTemplate.mht",
			};
			guidanceaction95.ConnectTo(guidanceaction96);
			var guidanceaction97 = new GuidanceAction
			{
				Name = "Export the Template",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/ExporttheTemplate.mht",
			};
			guidanceaction96.ConnectTo(guidanceaction97);
			var guidanceaction98 = new GuidanceAction
			{
				Name = "Import the Item Template into the Toolkit Project",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/ImporttheItemTemplateintotheToolkitProject.mht",
			};
			guidanceaction97.ConnectTo(guidanceaction98);
			var guidanceaction99 = new GuidanceAction
			{
				Name = "Configure the Item Template to Unfold with the Controller Node",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/ConfiguretheItemTemplatetoUnfoldwiththeControllerNode.mht",
			};
			guidanceaction98.ConnectTo(guidanceaction99);
			var guidanceaction100 = new GuidanceAction
			{
				Name = "Test Custom Item Template",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/TestCustomItemTemplate.mht",
			};
			guidanceaction99.ConnectTo(guidanceaction100);
			var join101 = new Join
			{
				Name = "Join12",
			};
			guidanceaction100.ConnectTo(join101);
			var join102 = new Join
			{
				Name = "Join13",
			};
			join101.ConnectTo(join102);
			var guidanceaction103 = new GuidanceAction
			{
				Name = "Feedback",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/Feedback.mht",
			};
			join102.ConnectTo(guidanceaction103);
			var guidanceaction104 = new GuidanceAction
			{
				Name = "Notes",
				Link = "content://5d64cfe6-a6ff-4e73-a000-c6a8832740ff/GeneratedCode/Guidance/Content/Notes.mht",
			};
			guidanceaction103.ConnectTo(guidanceaction104);
			var final105 = new Final
			{
				Name = "ActivityFinal1",
			};
			guidanceaction104.ConnectTo(final105);
			
			this.OnPostInitialize();
		}

		partial void OnPostInitialize();
	}

	/// <summary>
	/// Defines the guidance extension containing the guidance workflow.
	/// </summary>
	[GuidanceExtension("5d64cfe6-a6ff-4e73-a000-c6a8832740ff", DefaultName = "")]
	[Export(typeof(IGuidanceExtension))]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	[CLSCompliant(false)]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	[System.CodeDom.Compiler.GeneratedCode("NuPattern Toolkit Builder", "1.3.20.0")]
	public partial class GuidanceExtension : BlackboardGuidanceExtension<ProcessWorkflow>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GuidanceExtension"/> class.
		/// </summary>
		public GuidanceExtension() : base() { }

		/// <summary>
		/// Gets or sets the ServiceProvider.
		/// </summary>
		[Import]
		public SVsServiceProvider ServiceProvider { get; set; }
	}
}