﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using DslModeling = global::Microsoft.VisualStudio.Modeling;
using DslDesign = global::Microsoft.VisualStudio.Modeling.Design;
namespace NuPattern.Library.Automation
{
	/// <summary>
	/// DomainModel LibraryDomainModel
	/// Library
	/// </summary>
	[DslDesign::DisplayNameResource("NuPattern.Library.Automation.LibraryDomainModel.DisplayName", typeof(global::NuPattern.Library.Automation.LibraryDomainModel), "NuPattern.Library.GeneratedCode.DomainModelResx.gen")]
	[DslDesign::DescriptionResource("NuPattern.Library.Automation.LibraryDomainModel.Description", typeof(global::NuPattern.Library.Automation.LibraryDomainModel), "NuPattern.Library.GeneratedCode.DomainModelResx.gen")]
	[DslModeling::DependsOnDomainModel(typeof(global::Microsoft.VisualStudio.Modeling.CoreDomainModel))]
	[global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
	[DslModeling::DomainObjectId("47c59fea-739b-4a85-8f6f-e9e260340928")]
	internal partial class LibraryDomainModel : DslModeling::DomainModel
	{
		#region Constructor, domain model Id
	
		/// <summary>
		/// LibraryDomainModel domain model Id.
		/// </summary>
		public static readonly global::System.Guid DomainModelId = new global::System.Guid(0x47c59fea, 0x739b, 0x4a85, 0x8f, 0x6f, 0xe9, 0xe2, 0x60, 0x34, 0x09, 0x28);
	
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="store">Store containing the domain model.</param>
		public LibraryDomainModel(DslModeling::Store store)
			: base(store, DomainModelId)
		{
			// Call the partial method to allow any required serialization setup to be done.
			this.InitializeSerialization(store);		
		}
		
	
		///<Summary>
		/// Defines a partial method that will be called from the constructor to
		/// allow any necessary serialization setup to be done.
		///</Summary>
		///<remarks>
		/// For a DSL created with the DSL Designer wizard, an implementation of this 
		/// method will be generated in the GeneratedCode\SerializationHelper.cs class.
		///</remarks>
		partial void InitializeSerialization(DslModeling::Store store);
	
	
		#endregion
		#region Domain model reflection
			
		/// <summary>
		/// Gets the list of generated domain model types (classes, rules, relationships).
		/// </summary>
		/// <returns>List of types.</returns>
		[global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]	
		protected sealed override global::System.Type[] GetGeneratedDomainModelTypes()
		{
			return new global::System.Type[]
			{
				typeof(TemplateSettings),
				typeof(EventSettings),
				typeof(CommandSettings),
				typeof(MenuSettings),
				typeof(GuidanceExtension),
				typeof(WizardSettings),
				typeof(ArtifactExtension),
				typeof(ValidationExtension),
				typeof(DragDropSettings),
			};
		}
		/// <summary>
		/// Gets the list of generated domain properties.
		/// </summary>
		/// <returns>List of property data.</returns>
		[global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]	
		protected sealed override DomainMemberInfo[] GetGeneratedDomainProperties()
		{
			return new DomainMemberInfo[]
			{
				new DomainMemberInfo(typeof(TemplateSettings), "TemplateUri", TemplateSettings.TemplateUriDomainPropertyId, typeof(TemplateSettings.TemplateUriPropertyHandler)),
				new DomainMemberInfo(typeof(TemplateSettings), "CreateElementOnUnfold", TemplateSettings.CreateElementOnUnfoldDomainPropertyId, typeof(TemplateSettings.CreateElementOnUnfoldPropertyHandler)),
				new DomainMemberInfo(typeof(TemplateSettings), "UnfoldOnElementCreated", TemplateSettings.UnfoldOnElementCreatedDomainPropertyId, typeof(TemplateSettings.UnfoldOnElementCreatedPropertyHandler)),
				new DomainMemberInfo(typeof(TemplateSettings), "CommandId", TemplateSettings.CommandIdDomainPropertyId, typeof(TemplateSettings.CommandIdPropertyHandler)),
				new DomainMemberInfo(typeof(TemplateSettings), "WizardId", TemplateSettings.WizardIdDomainPropertyId, typeof(TemplateSettings.WizardIdPropertyHandler)),
				new DomainMemberInfo(typeof(TemplateSettings), "TemplateAuthoringUri", TemplateSettings.TemplateAuthoringUriDomainPropertyId, typeof(TemplateSettings.TemplateAuthoringUriPropertyHandler)),
				new DomainMemberInfo(typeof(TemplateSettings), "SyncName", TemplateSettings.SyncNameDomainPropertyId, typeof(TemplateSettings.SyncNamePropertyHandler)),
				new DomainMemberInfo(typeof(TemplateSettings), "SanitizeName", TemplateSettings.SanitizeNameDomainPropertyId, typeof(TemplateSettings.SanitizeNamePropertyHandler)),
				new DomainMemberInfo(typeof(TemplateSettings), "RawTargetFileName", TemplateSettings.RawTargetFileNameDomainPropertyId, typeof(TemplateSettings.RawTargetFileNamePropertyHandler)),
				new DomainMemberInfo(typeof(TemplateSettings), "RawTargetPath", TemplateSettings.RawTargetPathDomainPropertyId, typeof(TemplateSettings.RawTargetPathPropertyHandler)),
				new DomainMemberInfo(typeof(TemplateSettings), "Tag", TemplateSettings.TagDomainPropertyId, typeof(TemplateSettings.TagPropertyHandler)),
				new DomainMemberInfo(typeof(EventSettings), "EventId", EventSettings.EventIdDomainPropertyId, typeof(EventSettings.EventIdPropertyHandler)),
				new DomainMemberInfo(typeof(EventSettings), "CommandId", EventSettings.CommandIdDomainPropertyId, typeof(EventSettings.CommandIdPropertyHandler)),
				new DomainMemberInfo(typeof(EventSettings), "Conditions", EventSettings.ConditionsDomainPropertyId, typeof(EventSettings.ConditionsPropertyHandler)),
				new DomainMemberInfo(typeof(EventSettings), "WizardId", EventSettings.WizardIdDomainPropertyId, typeof(EventSettings.WizardIdPropertyHandler)),
				new DomainMemberInfo(typeof(CommandSettings), "TypeId", CommandSettings.TypeIdDomainPropertyId, typeof(CommandSettings.TypeIdPropertyHandler)),
				new DomainMemberInfo(typeof(CommandSettings), "Properties", CommandSettings.PropertiesDomainPropertyId, typeof(CommandSettings.PropertiesPropertyHandler)),
				new DomainMemberInfo(typeof(MenuSettings), "Conditions", MenuSettings.ConditionsDomainPropertyId, typeof(MenuSettings.ConditionsPropertyHandler)),
				new DomainMemberInfo(typeof(MenuSettings), "Text", MenuSettings.TextDomainPropertyId, typeof(MenuSettings.TextPropertyHandler)),
				new DomainMemberInfo(typeof(MenuSettings), "Icon", MenuSettings.IconDomainPropertyId, typeof(MenuSettings.IconPropertyHandler)),
				new DomainMemberInfo(typeof(MenuSettings), "CommandId", MenuSettings.CommandIdDomainPropertyId, typeof(MenuSettings.CommandIdPropertyHandler)),
				new DomainMemberInfo(typeof(MenuSettings), "CustomStatus", MenuSettings.CustomStatusDomainPropertyId, typeof(MenuSettings.CustomStatusPropertyHandler)),
				new DomainMemberInfo(typeof(MenuSettings), "WizardId", MenuSettings.WizardIdDomainPropertyId, typeof(MenuSettings.WizardIdPropertyHandler)),
				new DomainMemberInfo(typeof(MenuSettings), "SortOrder", MenuSettings.SortOrderDomainPropertyId, typeof(MenuSettings.SortOrderPropertyHandler)),
				new DomainMemberInfo(typeof(GuidanceExtension), "AssociatedGuidance", GuidanceExtension.AssociatedGuidanceDomainPropertyId, typeof(GuidanceExtension.AssociatedGuidancePropertyHandler)),
				new DomainMemberInfo(typeof(GuidanceExtension), "GuidanceInstanceName", GuidanceExtension.GuidanceInstanceNameDomainPropertyId, typeof(GuidanceExtension.GuidanceInstanceNamePropertyHandler)),
				new DomainMemberInfo(typeof(GuidanceExtension), "GuidanceActivateOnCreation", GuidanceExtension.GuidanceActivateOnCreationDomainPropertyId, typeof(GuidanceExtension.GuidanceActivateOnCreationPropertyHandler)),
				new DomainMemberInfo(typeof(GuidanceExtension), "ExtensionId", GuidanceExtension.ExtensionIdDomainPropertyId, typeof(GuidanceExtension.ExtensionIdPropertyHandler)),
				new DomainMemberInfo(typeof(GuidanceExtension), "GuidanceSharedInstance", GuidanceExtension.GuidanceSharedInstanceDomainPropertyId, typeof(GuidanceExtension.GuidanceSharedInstancePropertyHandler)),
				new DomainMemberInfo(typeof(WizardSettings), "TypeName", WizardSettings.TypeNameDomainPropertyId, typeof(WizardSettings.TypeNamePropertyHandler)),
				new DomainMemberInfo(typeof(ArtifactExtension), "AssociatedArtifacts", ArtifactExtension.AssociatedArtifactsDomainPropertyId, typeof(ArtifactExtension.AssociatedArtifactsPropertyHandler)),
				new DomainMemberInfo(typeof(ArtifactExtension), "OnArtifactActivation", ArtifactExtension.OnArtifactActivationDomainPropertyId, typeof(ArtifactExtension.OnArtifactActivationPropertyHandler)),
				new DomainMemberInfo(typeof(ArtifactExtension), "OnArtifactDeletion", ArtifactExtension.OnArtifactDeletionDomainPropertyId, typeof(ArtifactExtension.OnArtifactDeletionPropertyHandler)),
				new DomainMemberInfo(typeof(ValidationExtension), "ValidationExecution", ValidationExtension.ValidationExecutionDomainPropertyId, typeof(ValidationExtension.ValidationExecutionPropertyHandler)),
				new DomainMemberInfo(typeof(ValidationExtension), "ValidationOnBuild", ValidationExtension.ValidationOnBuildDomainPropertyId, typeof(ValidationExtension.ValidationOnBuildPropertyHandler)),
				new DomainMemberInfo(typeof(ValidationExtension), "ValidationOnSave", ValidationExtension.ValidationOnSaveDomainPropertyId, typeof(ValidationExtension.ValidationOnSavePropertyHandler)),
				new DomainMemberInfo(typeof(ValidationExtension), "ValidationOnMenu", ValidationExtension.ValidationOnMenuDomainPropertyId, typeof(ValidationExtension.ValidationOnMenuPropertyHandler)),
				new DomainMemberInfo(typeof(ValidationExtension), "ValidationOnCustomEvent", ValidationExtension.ValidationOnCustomEventDomainPropertyId, typeof(ValidationExtension.ValidationOnCustomEventPropertyHandler)),
				new DomainMemberInfo(typeof(DragDropSettings), "CommandId", DragDropSettings.CommandIdDomainPropertyId, typeof(DragDropSettings.CommandIdPropertyHandler)),
				new DomainMemberInfo(typeof(DragDropSettings), "DropConditions", DragDropSettings.DropConditionsDomainPropertyId, typeof(DragDropSettings.DropConditionsPropertyHandler)),
				new DomainMemberInfo(typeof(DragDropSettings), "WizardId", DragDropSettings.WizardIdDomainPropertyId, typeof(DragDropSettings.WizardIdPropertyHandler)),
				new DomainMemberInfo(typeof(DragDropSettings), "StatusText", DragDropSettings.StatusTextDomainPropertyId, typeof(DragDropSettings.StatusTextPropertyHandler)),
			};
		}
		#endregion
		#region Factory methods
		private static global::System.Collections.Generic.Dictionary<global::System.Type, int> createElementMap;
	
		/// <summary>
		/// Creates an element of specified type.
		/// </summary>
		/// <param name="partition">Partition where element is to be created.</param>
		/// <param name="elementType">Element type which belongs to this domain model.</param>
		/// <param name="propertyAssignments">New element property assignments.</param>
		/// <returns>Created element.</returns>
		[global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		[global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]	
		public sealed override DslModeling::ModelElement CreateElement(DslModeling::Partition partition, global::System.Type elementType, DslModeling::PropertyAssignment[] propertyAssignments)
		{
			if (elementType == null) throw new global::System.ArgumentNullException("elementType");
	
			if (createElementMap == null)
			{
				createElementMap = new global::System.Collections.Generic.Dictionary<global::System.Type, int>(9);
				createElementMap.Add(typeof(TemplateSettings), 0);
				createElementMap.Add(typeof(EventSettings), 1);
				createElementMap.Add(typeof(CommandSettings), 2);
				createElementMap.Add(typeof(MenuSettings), 3);
				createElementMap.Add(typeof(GuidanceExtension), 4);
				createElementMap.Add(typeof(WizardSettings), 5);
				createElementMap.Add(typeof(ArtifactExtension), 6);
				createElementMap.Add(typeof(ValidationExtension), 7);
				createElementMap.Add(typeof(DragDropSettings), 8);
			}
			int index;
			if (!createElementMap.TryGetValue(elementType, out index))
			{
				// construct exception error message		
				string exceptionError = string.Format(
								global::System.Globalization.CultureInfo.CurrentCulture,
								global::NuPattern.Library.Automation.LibraryDomainModel.SingletonResourceManager.GetString("UnrecognizedElementType"),
								elementType.Name);
				throw new global::System.ArgumentException(exceptionError, "elementType");
			}
			switch (index)
			{
				case 0: return new TemplateSettings(partition, propertyAssignments);
				case 1: return new EventSettings(partition, propertyAssignments);
				case 2: return new CommandSettings(partition, propertyAssignments);
				case 3: return new MenuSettings(partition, propertyAssignments);
				case 4: return new GuidanceExtension(partition, propertyAssignments);
				case 5: return new WizardSettings(partition, propertyAssignments);
				case 6: return new ArtifactExtension(partition, propertyAssignments);
				case 7: return new ValidationExtension(partition, propertyAssignments);
				case 8: return new DragDropSettings(partition, propertyAssignments);
				default: return null;
			}
		}
	
		private static global::System.Collections.Generic.Dictionary<global::System.Type, int> createElementLinkMap;
	
		/// <summary>
		/// Creates an element link of specified type.
		/// </summary>
		/// <param name="partition">Partition where element is to be created.</param>
		/// <param name="elementLinkType">Element link type which belongs to this domain model.</param>
		/// <param name="roleAssignments">List of relationship role assignments for the new link.</param>
		/// <param name="propertyAssignments">New element property assignments.</param>
		/// <returns>Created element link.</returns>
		[global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		public sealed override DslModeling::ElementLink CreateElementLink(DslModeling::Partition partition, global::System.Type elementLinkType, DslModeling::RoleAssignment[] roleAssignments, DslModeling::PropertyAssignment[] propertyAssignments)
		{
			if (elementLinkType == null) throw new global::System.ArgumentNullException("elementLinkType");
			if (roleAssignments == null) throw new global::System.ArgumentNullException("roleAssignments");
	
			if (createElementLinkMap == null)
			{
				createElementLinkMap = new global::System.Collections.Generic.Dictionary<global::System.Type, int>(0);
			}
			int index;
			if (!createElementLinkMap.TryGetValue(elementLinkType, out index))
			{
				// construct exception error message
				string exceptionError = string.Format(
								global::System.Globalization.CultureInfo.CurrentCulture,
								global::NuPattern.Library.Automation.LibraryDomainModel.SingletonResourceManager.GetString("UnrecognizedElementLinkType"),
								elementLinkType.Name);
				throw new global::System.ArgumentException(exceptionError, "elementLinkType");
			
			}
			switch (index)
			{
				default: return null;
			}
		}
		#endregion
		#region Resource manager
		
		private static global::System.Resources.ResourceManager resourceManager;
		
		/// <summary>
		/// The base name of this model's resources.
		/// </summary>
		public const string ResourceBaseName = "NuPattern.Library.GeneratedCode.DomainModelResx.gen";
		
		/// <summary>
		/// Gets the DomainModel's ResourceManager. If the ResourceManager does not already exist, then it is created.
		/// </summary>
		public override global::System.Resources.ResourceManager ResourceManager
		{
			[global::System.Diagnostics.DebuggerStepThrough]
			get
			{
				return LibraryDomainModel.SingletonResourceManager;
			}
		}
	
		/// <summary>
		/// Gets the Singleton ResourceManager for this domain model.
		/// </summary>
		public static global::System.Resources.ResourceManager SingletonResourceManager
		{
			[global::System.Diagnostics.DebuggerStepThrough]
			get
			{
				if (LibraryDomainModel.resourceManager == null)
				{
					LibraryDomainModel.resourceManager = new global::System.Resources.ResourceManager(ResourceBaseName, typeof(LibraryDomainModel).Assembly);
				}
				return LibraryDomainModel.resourceManager;
			}
		}
		#endregion
		#region Copy/Remove closures
		/// <summary>
		/// CopyClosure cache
		/// </summary>
		private static DslModeling::IElementVisitorFilter copyClosure;
		/// <summary>
		/// DeleteClosure cache
		/// </summary>
		private static DslModeling::IElementVisitorFilter removeClosure;
		/// <summary>
		/// Returns an IElementVisitorFilter that corresponds to the ClosureType.
		/// </summary>
		/// <param name="type">closure type</param>
		/// <param name="rootElements">collection of root elements</param>
		/// <returns>IElementVisitorFilter or null</returns>
		public override DslModeling::IElementVisitorFilter GetClosureFilter(DslModeling::ClosureType type, global::System.Collections.Generic.ICollection<DslModeling::ModelElement> rootElements)
		{
			switch (type)
			{
				case DslModeling::ClosureType.CopyClosure:
					return LibraryDomainModel.CopyClosure;
				case DslModeling::ClosureType.DeleteClosure:
					return LibraryDomainModel.DeleteClosure;
			}
			return base.GetClosureFilter(type, rootElements);
		}
		/// <summary>
		/// CopyClosure cache
		/// </summary>
		private static DslModeling::IElementVisitorFilter CopyClosure
		{
			get
			{
				// Incorporate all of the closures from the models we extend
				if (LibraryDomainModel.copyClosure == null)
				{
					DslModeling::ChainingElementVisitorFilter copyFilter = new DslModeling::ChainingElementVisitorFilter();
					copyFilter.AddFilter(new LibraryCopyClosure());
					copyFilter.AddFilter(new DslModeling::CoreCopyClosure());
					
					LibraryDomainModel.copyClosure = copyFilter;
				}
				return LibraryDomainModel.copyClosure;
			}
		}
		/// <summary>
		/// DeleteClosure cache
		/// </summary>
		private static DslModeling::IElementVisitorFilter DeleteClosure
		{
			get
			{
				// Incorporate all of the closures from the models we extend
				if (LibraryDomainModel.removeClosure == null)
				{
					DslModeling::ChainingElementVisitorFilter removeFilter = new DslModeling::ChainingElementVisitorFilter();
					removeFilter.AddFilter(new LibraryDeleteClosure());
					removeFilter.AddFilter(new DslModeling::CoreDeleteClosure());
		
					LibraryDomainModel.removeClosure = removeFilter;
				}
				return LibraryDomainModel.removeClosure;
			}
		}
		#endregion
	}
		
	#region Copy/Remove closure classes
	/// <summary>
	/// Remove closure visitor filter
	/// </summary>
	internal partial class LibraryDeleteClosure : LibraryDeleteClosureBase, DslModeling::IElementVisitorFilter
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public LibraryDeleteClosure() : base()
		{
		}
	}
	
	/// <summary>
	/// Base class for remove closure visitor filter
	/// </summary>
	internal partial class LibraryDeleteClosureBase : DslModeling::IElementVisitorFilter
	{
		/// <summary>
		/// DomainRoles
		/// </summary>
		private global::System.Collections.Specialized.HybridDictionary domainRoles;
		/// <summary>
		/// Constructor
		/// </summary>
		public LibraryDeleteClosureBase()
		{
			#region Initialize DomainData Table
			#endregion
		}
		/// <summary>
		/// Called to ask the filter if a particular relationship from a source element should be included in the traversal
		/// </summary>
		/// <param name="walker">ElementWalker that is traversing the model</param>
		/// <param name="sourceElement">Model Element playing the source role</param>
		/// <param name="sourceRoleInfo">DomainRoleInfo of the role that the source element is playing in the relationship</param>
		/// <param name="domainRelationshipInfo">DomainRelationshipInfo for the ElementLink in question</param>
		/// <param name="targetRelationship">Relationship in question</param>
		/// <returns>Yes if the relationship should be traversed</returns>
		public virtual DslModeling::VisitorFilterResult ShouldVisitRelationship(DslModeling::ElementWalker walker, DslModeling::ModelElement sourceElement, DslModeling::DomainRoleInfo sourceRoleInfo, DslModeling::DomainRelationshipInfo domainRelationshipInfo, DslModeling::ElementLink targetRelationship)
		{
			return DslModeling::VisitorFilterResult.Yes;
		}
		/// <summary>
		/// Called to ask the filter if a particular role player should be Visited during traversal
		/// </summary>
		/// <param name="walker">ElementWalker that is traversing the model</param>
		/// <param name="sourceElement">Model Element playing the source role</param>
		/// <param name="elementLink">Element Link that forms the relationship to the role player in question</param>
		/// <param name="targetDomainRole">DomainRoleInfo of the target role</param>
		/// <param name="targetRolePlayer">Model Element that plays the target role in the relationship</param>
		/// <returns></returns>
		public virtual DslModeling::VisitorFilterResult ShouldVisitRolePlayer(DslModeling::ElementWalker walker, DslModeling::ModelElement sourceElement, DslModeling::ElementLink elementLink, DslModeling::DomainRoleInfo targetDomainRole, DslModeling::ModelElement targetRolePlayer)
		{
			if (targetDomainRole == null) throw new global::System.ArgumentNullException("targetDomainRole");
			return this.DomainRoles.Contains(targetDomainRole.Id) ? DslModeling::VisitorFilterResult.Yes : DslModeling::VisitorFilterResult.DoNotCare;
		}
		/// <summary>
		/// DomainRoles
		/// </summary>
		private global::System.Collections.Specialized.HybridDictionary DomainRoles
		{
			get
			{
				if (this.domainRoles == null)
				{
					this.domainRoles = new global::System.Collections.Specialized.HybridDictionary();
				}
				return this.domainRoles;
			}
		}
	
	}
	/// <summary>
	/// Copy closure visitor filter
	/// </summary>
	internal partial class LibraryCopyClosure : LibraryCopyClosureBase, DslModeling::IElementVisitorFilter
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public LibraryCopyClosure() : base()
		{
		}
	}
	/// <summary>
	/// Base class for copy closure visitor filter
	/// </summary>
	internal partial class LibraryCopyClosureBase : DslModeling::CopyClosureFilter, DslModeling::IElementVisitorFilter
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public LibraryCopyClosureBase():base()
		{
		}
	}
	#endregion
		
}
namespace NuPattern.Library.Automation
{
	/// <summary>
	/// DomainEnumeration: ArtifactActivatedAction
	/// Description for NuPattern.Library.Automation.ArtifactActivatedAction
	/// </summary>
	[global::System.CLSCompliant(true)]
	public enum ArtifactActivatedAction
	{
		/// <summary>
		/// None
		/// No action is performed, activated items are neither opened nor selected.
		/// </summary>
		[DslDesign::DescriptionResource("NuPattern.Library.Automation.ArtifactActivatedAction/None.Description", typeof(global::NuPattern.Library.Automation.LibraryDomainModel), "NuPattern.Library.GeneratedCode.DomainModelResx.gen")]
		None,
		/// <summary>
		/// Open
		/// Associated artifacts are opened, in their default view.
		/// </summary>
		[DslDesign::DescriptionResource("NuPattern.Library.Automation.ArtifactActivatedAction/Open.Description", typeof(global::NuPattern.Library.Automation.LibraryDomainModel), "NuPattern.Library.GeneratedCode.DomainModelResx.gen")]
		Open,
		/// <summary>
		/// Select
		/// Associated artifacts are selected in Solution Explorer.
		/// </summary>
		[DslDesign::DescriptionResource("NuPattern.Library.Automation.ArtifactActivatedAction/Select.Description", typeof(global::NuPattern.Library.Automation.LibraryDomainModel), "NuPattern.Library.GeneratedCode.DomainModelResx.gen")]
		Select,
	}
}
namespace NuPattern.Library.Automation
{
	/// <summary>
	/// DomainEnumeration: ArtifactDeletedAction
	/// Description for NuPattern.Library.Automation.ArtifactDeletedAction
	/// </summary>
	[global::System.CLSCompliant(true)]
	public enum ArtifactDeletedAction
	{
		/// <summary>
		/// None
		/// No action is performed, associated solution items are not deleted from the
		/// solution.
		/// </summary>
		[DslDesign::DescriptionResource("NuPattern.Library.Automation.ArtifactDeletedAction/None.Description", typeof(global::NuPattern.Library.Automation.LibraryDomainModel), "NuPattern.Library.GeneratedCode.DomainModelResx.gen")]
		None,
		/// <summary>
		/// DeleteAll
		/// All associated solution items are deleted automatically.
		/// </summary>
		[DslDesign::DescriptionResource("NuPattern.Library.Automation.ArtifactDeletedAction/DeleteAll.Description", typeof(global::NuPattern.Library.Automation.LibraryDomainModel), "NuPattern.Library.GeneratedCode.DomainModelResx.gen")]
		DeleteAll,
		/// <summary>
		/// PromptUser
		/// The user is prompted to select which associated solution items to delete.
		/// </summary>
		[DslDesign::DescriptionResource("NuPattern.Library.Automation.ArtifactDeletedAction/PromptUser.Description", typeof(global::NuPattern.Library.Automation.LibraryDomainModel), "NuPattern.Library.GeneratedCode.DomainModelResx.gen")]
		PromptUser,
	}
}

