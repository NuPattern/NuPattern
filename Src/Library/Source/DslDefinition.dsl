<?xml version="1.0" encoding="utf-8"?>
<DslLibrary xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="47c59fea-739b-4a85-8f6f-e9e260340928" Description="Library" Name="Library" DisplayName="Library" Namespace="NuPattern.Library.Automation" AccessModifier="Assembly" MinorVersion="3" xmlns="http://schemas.microsoft.com/VisualStudio/2005/DslTools/DslDefinitionModel">
  <Classes>
    <DomainClass Id="1d522a01-5d23-4d87-8493-dfcbe3d05a9b" Description="Configures the settings for adding a project or item template to unfold, and execute other automation on this element." Name="TemplateSettings" DisplayName="VS Template Launch Point" AccessModifier="Assembly" Namespace="NuPattern.Library.Automation">
      <Notes>IsAutomationExtension=True</Notes>
      <BaseClass>
        <DomainClassMoniker Name="/Microsoft.VisualStudio.Modeling/ExtensionElement" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="cf977d69-f13b-4deb-a6c7-2a2e954637dc" Description="The project or item template to unfold." Name="TemplateUri" DisplayName="Template">
          <Attributes>
            <ClrAttribute Name="System.ComponentModel.Editor">
              <Parameters>
                <AttributeParameter Value="typeof(NuPattern.Library.TypeEditors.VsTemplateUriEditor)" />
                <AttributeParameter Value="typeof(System.Drawing.Design.UITypeEditor)" />
              </Parameters>
            </ClrAttribute>
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="caa696ae-7c70-404b-8b39-5dadf7197713" Description="Whether to create the current element when the template is unfolded from the New Project/Item dialog in Visual Studio." Name="CreateElementOnUnfold" DisplayName="Create When Unfolded" DefaultValue="true">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="fc2057b9-cfab-4dc7-89dc-15f0a6835b47" Description="Whether to unfold the template when the element is created from the Solution Builder." Name="UnfoldOnElementCreated" DisplayName="Unfold When Created" DefaultValue="true">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="dd6b1c06-fd31-4685-aaa4-59f4b65ed7ca" Description="The command to execute after the template is unfolded." Name="CommandId" DisplayName="Command">
          <Type>
            <ExternalTypeMoniker Name="/System/Guid" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="e3761cb0-ad30-4c13-99ec-f5d860382831" Description="A wizard to gather input from the user, that configures the properties of this element, before the template is unfolded." Name="WizardId" DisplayName="Wizard">
          <Type>
            <ExternalTypeMoniker Name="/System/Guid" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="56192f42-cbaf-48e9-a2eb-f449ec10e8ff" Description="The Uri to resolve the referenced template at authoring time." Name="TemplateAuthoringUri" DisplayName="Template Authoring Uri" IsBrowsable="false">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="663a4280-0b38-457e-a40c-a69a70c4f438" Description="Whether to keep in sync the name of the unfolded artifact with the name of the current element." Name="SyncName" DisplayName="Sync Name" DefaultValue="false">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="6e63b903-3320-406e-a307-696e4bb48fa8" Description="Whether to remove spaces and other illegal characters from the name of the unfolded artifact." Name="SanitizeName" DisplayName="Sanitize Name" DefaultValue="true">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="d8c350bd-ff83-4000-95c4-961befdb289f" Description="The name of the unfolded artifact. For an item template, if no extension is provided, it is taken from the vstemplate file. This property supports property value substitution from properties on the current element. (e.g. {InstanceName} or {VariablePropertyName}). See guidance documentation for details." Name="RawTargetFileName" DisplayName="Target File Name">
          <Attributes>
            <ClrAttribute Name="NuPattern.ComponentModel.Design.PropertyDescriptor">
              <Parameters>
                <AttributeParameter Value="typeof(NuPattern.Runtime.Bindings.PropertyBindingDescriptor)" />
              </Parameters>
            </ClrAttribute>
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="e258cc61-f5b7-43fe-a9a9-e08c733c5c40" Description="The path to the target file, project, folder or solution folder in the current solution where the artifact is to be unfolded. Folders that don't currently exist are created automatically. i.e. ..\~\GeneratedCode, navigates up to the parent element, and traverses the first artifact link found on the parent element and into the 'GeneratedCode' sub-folder of that container (project or folder).  See guidance documentation for more example paths and details." Name="RawTargetPath" DisplayName="Target Path">
          <Attributes>
            <ClrAttribute Name="NuPattern.ComponentModel.Design.PropertyDescriptor">
              <Parameters>
                <AttributeParameter Value="typeof(NuPattern.Runtime.Bindings.PropertyBindingDescriptor)" />
              </Parameters>
            </ClrAttribute>
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="7b2582b2-ce44-470b-b179-a2ab92fd0e3b" Description="An optional arbitrary text value to tag the generated solution item, that is used as an aid in resolving the artifact reference to the solution item." Name="Tag" DisplayName="Tag">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="d7fce310-a0b0-424c-b439-6e94222544e6" Description="Configures the settings for handling an event for executing other automation on this element." Name="EventSettings" DisplayName="Event Launch Point" AccessModifier="Assembly" Namespace="NuPattern.Library.Automation">
      <Notes>IsAutomationExtension=True</Notes>
      <BaseClass>
        <DomainClassMoniker Name="/Microsoft.VisualStudio.Modeling/ExtensionElement" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="4564d86b-cf9d-4fa2-9a11-339596bdf65c" Description="The type of event which triggers the command to execute when all conditions are met." Name="EventId" DisplayName="Event Type">
          <Attributes>
            <ClrAttribute Name="System.ComponentModel.TypeConverter">
              <Parameters>
                <AttributeParameter Value="typeof(NuPattern.Runtime.Design.FeatureComponentTypeConverter&lt;NuPattern.IObservableEvent&gt;)" />
              </Parameters>
            </ClrAttribute>
            <ClrAttribute Name="System.ComponentModel.Editor">
              <Parameters>
                <AttributeParameter Value="typeof(Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Design.StandardValuesEditor)" />
                <AttributeParameter Value="typeof(System.Drawing.Design.UITypeEditor)" />
              </Parameters>
            </ClrAttribute>
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="518bf1de-2542-4099-9896-c7a3b4c5c824" Description="The command to execute on this event." Name="CommandId" DisplayName="Command">
          <Type>
            <ExternalTypeMoniker Name="/System/Guid" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="bf026eb8-785b-468a-ae93-3a41b152aad0" Description="The conditions that must be satisfied when the event is raised in order to execute the command." Name="Conditions" DisplayName="Conditions">
          <Notes>Skip=true</Notes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="ce20a268-439b-46cc-8021-fd9b261cd15d" Description="Whether to add a condition that filters the events to the current element only. Typically required for element instantiation and property change events types as examples." Name="FilterForCurrentElement" DisplayName="Current Element Only" DefaultValue="false" IsBrowsable="false">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="dfaa6346-fd67-4861-8814-a1731142aee5" Description="A wizard to gather input from the user, that configures the properties of this element, when the event is raised." Name="WizardId" DisplayName="Wizard">
          <Type>
            <ExternalTypeMoniker Name="/System/Guid" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="63e8eaf8-6f53-45ea-96f9-b6d8af7276e2" Description="Configures the settings for adding a command that can be executed on this element." Name="CommandSettings" DisplayName="Command" AccessModifier="Assembly" Namespace="NuPattern.Library.Automation">
      <Notes>IsAutomationExtension=True</Notes>
      <BaseClass>
        <DomainClassMoniker Name="/Microsoft.VisualStudio.Modeling/ExtensionElement" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="dc701988-b2bd-4576-97b6-93813069620f" Description="The type of the command. Once selected, the specific properties of the selected command can be configured." Name="TypeId" DisplayName="Command Type">
          <Attributes>
            <ClrAttribute Name="System.ComponentModel.TypeConverter">
              <Parameters>
                <AttributeParameter Value="typeof(NuPattern.Runtime.Design.FeatureComponentTypeConverter&lt;Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.IFeatureCommand&gt;)" />
              </Parameters>
            </ClrAttribute>
            <ClrAttribute Name="System.ComponentModel.Editor">
              <Parameters>
                <AttributeParameter Value="typeof(Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Design.StandardValuesEditor)" />
                <AttributeParameter Value="typeof(System.Drawing.Design.UITypeEditor)" />
              </Parameters>
            </ClrAttribute>
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="ec625787-ee50-4604-9b1e-de6fb588598e" Description="The design-time properties of the command." Name="Properties" DisplayName="Properties">
          <Notes>Skip=true</Notes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="29b8619f-8cba-4cdc-98ec-7035d9d2c3f8" Description="Configures the settings for adding a context menu item for executing other automation on this element." Name="MenuSettings" DisplayName="ContextMenu Launch Point" AccessModifier="Assembly" Namespace="NuPattern.Library.Automation">
      <Notes>IsAutomationExtension=True</Notes>
      <BaseClass>
        <DomainClassMoniker Name="/Microsoft.VisualStudio.Modeling/ExtensionElement" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="42fb25c3-8992-450f-b764-70bf4dc2c8e5" Description="The conditions that must be satisfied in order to display the menu." Name="Conditions" DisplayName="Conditions">
          <Notes>Skip=true</Notes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="37c61fda-acfd-4129-af47-ff4010b98752" Description="The text that is displayed on the menu to the user." Name="Text" DisplayName="Menu Text">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="4b76d211-4350-4cc8-8bde-c495be238113" Description="The icon that is displayed on the menu to the user." Name="Icon" DisplayName="Icon">
          <Attributes>
            <ClrAttribute Name="NuPattern.Runtime.Design.ImageFilter">
              <Parameters>
                <AttributeParameter Value="NuPattern.Runtime.Design.ImageKind.Image " />
              </Parameters>
            </ClrAttribute>
            <ClrAttribute Name="System.ComponentModel.Editor">
              <Parameters>
                <AttributeParameter Value="typeof(NuPattern.Runtime.Design.ImageUriEditor)" />
                <AttributeParameter Value="typeof(System.Drawing.Design.UITypeEditor)" />
              </Parameters>
            </ClrAttribute>
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="0e284967-1ca7-4da6-81a6-d846a87c67e4" Description="The command to execute when the menu is clicked." Name="CommandId" DisplayName="Command">
          <Type>
            <ExternalTypeMoniker Name="/System/Guid" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="cabcb4f4-e363-403e-96da-d0b695b67a6b" Description="An optional type that provides dynamic menu status updates for this menu." Name="CustomStatus" DisplayName="Status Provider" IsBrowsable="false">
          <Attributes>
            <ClrAttribute Name="System.ComponentModel.TypeConverter">
              <Parameters>
                <AttributeParameter Value="typeof(NuPattern.Runtime.Design.FeatureComponentTypeConverter&lt;NuPattern.Runtime.UI.ICommandStatus&gt;)" />
              </Parameters>
            </ClrAttribute>
            <ClrAttribute Name="System.ComponentModel.Editor">
              <Parameters>
                <AttributeParameter Value="typeof(Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Design.StandardValuesEditor)" />
                <AttributeParameter Value="typeof(System.Drawing.Design.UITypeEditor)" />
              </Parameters>
            </ClrAttribute>
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="e6f5c565-4e97-4e4b-9054-49ea14339379" Description="A wizard to gather input from the user, that configures the properties of this element, when the menu is clicked." Name="WizardId" DisplayName="Wizard">
          <Type>
            <ExternalTypeMoniker Name="/System/Guid" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="0ec79da4-eb1e-4935-8141-6683f453cb41" Description="A number that orders this menu item with others on the context menu." Name="SortOrder" DisplayName="Menu Order" DefaultValue="100">
          <Type>
            <ExternalTypeMoniker Name="/System/Int32" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="99460d09-2d87-47d5-a6b5-56cbf5d299fd" Description="Configures the settings for associating guidance to this element." Name="GuidanceExtension" DisplayName="Guidance Extension" AccessModifier="Assembly" Namespace="NuPattern.Library.Automation">
      <BaseClass>
        <DomainClassMoniker Name="/Microsoft.VisualStudio.Modeling/ExtensionElement" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="f53044c0-f655-4bed-9177-4ee71c65b4ca" Description="Configures guidance associated to this element. Expand this property to configure." Name="AssociatedGuidance" DisplayName="Associated Guidance" Category="Guidance">
          <Attributes>
            <ClrAttribute Name="System.ComponentModel.TypeConverter">
              <Parameters>
                <AttributeParameter Value="typeof(NuPattern.Library.Design.AssociatedGuidanceTypeConverter)" />
              </Parameters>
            </ClrAttribute>
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="86fdc7a3-d35e-4934-8bc1-e574376836bc" Description="The name of the created guidance workflow in the Guidance Explorer. If left blank, the instance name is calculated from the configured default name of the feature extension." Name="GuidanceInstanceName" DisplayName="Instance Name" Category="Guidance" IsBrowsable="false">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="3c4bfd70-5fb4-40f8-a50b-93d652efb322" Description="Whether to make this guidance the currently selected guidance in Guidance Explorer, when this element is created." Name="GuidanceActivateOnCreation" DisplayName="Activate On Creation" DefaultValue="true" Category="Guidance" IsBrowsable="false">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="c305b11e-9e20-4250-ac67-1b25a5caab41" Description="The identifier of the feature extension associated to this element. This is the VSIX ID of the Feature Extension, found in the source.extension.vsixmanifest file in the feature extension project." Name="GuidanceFeatureId" DisplayName="Feature Id" Category="Guidance" IsBrowsable="false">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="6b911b72-746b-426d-af1d-de4a2b5d2f38" Description="Whether a single guidance instance is shared between all instances of this element, or a separate instance of guidance is created for each instance of this element." Name="GuidanceSharedInstance" DisplayName="Shared Instance" DefaultValue="false" Category="Guidance" IsBrowsable="false">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="765329ab-9384-4a2c-b5c2-e2c4b60912f5" Description="Configures the settings for adding a wizard to gather and initialize data for properties on this element." Name="WizardSettings" DisplayName="Wizard" AccessModifier="Assembly" Namespace="NuPattern.Library.Automation">
      <Notes>IsAutomationExtension=True</Notes>
      <BaseClass>
        <DomainClassMoniker Name="/Microsoft.VisualStudio.Modeling/ExtensionElement" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="a0d15274-7041-4b71-9e11-2bce1b7ac841" Description="The type name of the wizard. " Name="TypeName" DisplayName="Wizard Type">
          <Attributes>
            <ClrAttribute Name="System.ComponentModel.Editor">
              <Parameters>
                <AttributeParameter Value="typeof(Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Design.StandardValuesEditor)" />
                <AttributeParameter Value="typeof(System.Drawing.Design.UITypeEditor)" />
              </Parameters>
            </ClrAttribute>
            <ClrAttribute Name="System.ComponentModel.TypeConverter">
              <Parameters>
                <AttributeParameter Value="typeof(NuPattern.Runtime.Design.FullTypeTypeConverter&lt;System.Windows.Window&gt;)" />
              </Parameters>
            </ClrAttribute>
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="18f556f4-beef-476c-89dc-f7e63f9dbf48" Description="Configures settings for managing associated artifacts to this element." Name="ArtifactExtension" DisplayName="Artifact Extension" AccessModifier="Assembly" Namespace="NuPattern.Library.Automation">
      <BaseClass>
        <DomainClassMoniker Name="/Microsoft.VisualStudio.Modeling/ExtensionElement" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="41cbff0c-4b29-4540-b9fd-af2e7c93a185" Description="Configures actions for working with solution items associated with this element. Expand this property to configure." Name="AssociatedArtifacts" DisplayName="Associated Solution Items" Category="Solution Items">
          <Attributes>
            <ClrAttribute Name="System.ComponentModel.TypeConverter">
              <Parameters>
                <AttributeParameter Value="typeof(NuPattern.Library.Design.AssociatedArtifactsTypeConverter)" />
              </Parameters>
            </ClrAttribute>
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="593016b6-aa8b-49ff-a5e9-26c7cfafbbfb" Description="The action to perform on associated solution items, when this element is 'activated' by the user (i.e. double-clicked). A value of 'Open' will open the solution item in its default view, a value of 'Select' will select the item in 'Solution Explorer'." Name="OnArtifactActivation" DisplayName="On Activation" DefaultValue="None" Category="Solution Items" IsBrowsable="false">
          <Type>
            <DomainEnumerationMoniker Name="ArtifactActivatedAction" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="7bcdb4f8-c554-4355-9ae6-6cbb0b97d818" Description="The action to perform on associated solution items, when this element is 'deleted' by the user. A value of 'DeleteAll' will automatically delete all associated solution items, a value of 'PromptUser' prompt the user to select which solution items to delete." Name="OnArtifactDeletion" DisplayName="On Deletion" DefaultValue="None" Category="Solution Items" IsBrowsable="false">
          <Type>
            <DomainEnumerationMoniker Name="ArtifactDeletedAction" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="8aceddb6-1e3a-4d8b-9703-43348f695c8e" Description="Configures settings for managing validation of this element." Name="ValidationExtension" DisplayName="Validation Extension" AccessModifier="Assembly" Namespace="NuPattern.Library.Automation">
      <BaseClass>
        <DomainClassMoniker Name="/Microsoft.VisualStudio.Modeling/ExtensionElement" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="8b912a8f-e192-43a5-8e7b-61668ba3944c" Description="Configures actions for validating this element. Expand this property to configure." Name="ValidationExecution" DisplayName="Validation Execution" Category="Validation">
          <Attributes>
            <ClrAttribute Name="System.ComponentModel.TypeConverter">
              <Parameters>
                <AttributeParameter Value="typeof(NuPattern.Library.Design.ValidationExecutionTypeConverter)" />
              </Parameters>
            </ClrAttribute>
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="074469a3-dddc-49dc-a554-10c6a66a699a" Description="Whether to validate the current element and all its descendants on build of the solution. (OnBuildStarted event)." Name="ValidationOnBuild" DisplayName="On Build" DefaultValue="false" Category="Validation" IsBrowsable="false">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="10a3b48a-0e79-4672-978d-f2d66a66a7ad" Description="Whether to validate the current element and all its descendants on save of the product. (OnProductStoreSaved event)." Name="ValidationOnSave" DisplayName="On Save" DefaultValue="false" Category="Validation" IsBrowsable="false">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="6adb9635-7468-4574-bd40-c175f3f3c611" Description="Whether to provide a menu to execute validation on this element and all its descendants. ('Validate All' menu item)." Name="ValidationOnMenu" DisplayName="On Menu" DefaultValue="false" Category="Validation" IsBrowsable="false">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="ec6131a2-68e6-40d2-af71-6e055945e967" Description="A custom event to execute validation on the current element and all its descendants." Name="ValidationOnCustomEvent" DisplayName="On Custom Event" DefaultValue="" Category="Validation" IsBrowsable="false">
          <Attributes>
            <ClrAttribute Name="System.ComponentModel.TypeConverter">
              <Parameters>
                <AttributeParameter Value="typeof(NuPattern.Runtime.Design.FeatureComponentTypeConverter&lt;NuPattern.IObservableEvent&gt;)" />
              </Parameters>
            </ClrAttribute>
            <ClrAttribute Name="System.ComponentModel.Editor">
              <Parameters>
                <AttributeParameter Value="typeof(Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Design.StandardValuesEditor)" />
                <AttributeParameter Value="typeof(System.Drawing.Design.UITypeEditor)" />
              </Parameters>
            </ClrAttribute>
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="4aed72c0-e518-4835-9d56-04f3d4e7bc58" Description="Configures settings for handling a drag drop operations on this element." Name="DragDropSettings" DisplayName="Drag Drop Launch Point" AccessModifier="Assembly" Namespace="NuPattern.Library.Automation">
      <Notes>IsAutomationExtension=True</Notes>
      <BaseClass>
        <DomainClassMoniker Name="/Microsoft.VisualStudio.Modeling/ExtensionElement" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="71e3a52c-0b1d-40e5-b29c-c3f2892a1f79" Description="The command to execute when valid data is dropped on instances of this element." Name="CommandId" DisplayName="Drop Command">
          <Type>
            <ExternalTypeMoniker Name="/System/Guid" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="ae02b40d-4128-41fd-b802-2c6deecc65e2" Description="The conditions that determine whether the data being dragged over instances of this element is permitted for dropping." Name="DropConditions" DisplayName="Drop Conditions">
          <Notes>Skip=true</Notes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="ac8c7a32-694c-4304-a5b2-2ee3efe1c922" Description="A wizard to gather input from the user, that configures the properties of this element, when dragged data is dropped on instances of this element." Name="WizardId" DisplayName="Wizard">
          <Type>
            <ExternalTypeMoniker Name="/System/Guid" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="b660dd15-3b2c-4542-837e-279785071ce9" Description="Informative message to display in Visual Studio's status bar when data is being dragged over instances of this element." Name="StatusText" DisplayName="Status Text">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
  </Classes>
  <Types>
    <ExternalType Name="DateTime" Namespace="System" />
    <ExternalType Name="String" Namespace="System" />
    <ExternalType Name="Int16" Namespace="System" />
    <ExternalType Name="Int32" Namespace="System" />
    <ExternalType Name="Int64" Namespace="System" />
    <ExternalType Name="UInt16" Namespace="System" />
    <ExternalType Name="UInt32" Namespace="System" />
    <ExternalType Name="UInt64" Namespace="System" />
    <ExternalType Name="SByte" Namespace="System" />
    <ExternalType Name="Byte" Namespace="System" />
    <ExternalType Name="Double" Namespace="System" />
    <ExternalType Name="Single" Namespace="System" />
    <ExternalType Name="Guid" Namespace="System" />
    <ExternalType Name="Boolean" Namespace="System" />
    <ExternalType Name="Char" Namespace="System" />
    <DomainEnumeration Name="ArtifactActivatedAction" Namespace="NuPattern.Library.Automation" Description="Description for NuPattern.Library.Automation.ArtifactActivatedAction">
      <Literals>
        <EnumerationLiteral Description="No action is performed, activated items are neither opened nor selected." Name="None" Value="" />
        <EnumerationLiteral Description="Associated artifacts are opened, in their default view." Name="Open" Value="" />
        <EnumerationLiteral Description="Associated artifacts are selected in Solution Explorer." Name="Select" Value="" />
      </Literals>
    </DomainEnumeration>
    <DomainEnumeration Name="ArtifactDeletedAction" Namespace="NuPattern.Library.Automation" Description="Description for NuPattern.Library.Automation.ArtifactDeletedAction">
      <Literals>
        <EnumerationLiteral Description="No action is performed, associated solution items are not deleted from the solution." Name="None" Value="" />
        <EnumerationLiteral Description="All associated solution items are deleted automatically." Name="DeleteAll" Value="" />
        <EnumerationLiteral Description="The user is prompted to select which associated solution items to delete." Name="PromptUser" Value="" />
      </Literals>
    </DomainEnumeration>
  </Types>
  <XmlSerializationBehavior Name="LibrarySerializationBehavior" Namespace="NuPattern.Library.Automation">
    <ClassData>
      <XmlClassData TypeName="TemplateSettings" MonikerAttributeName="" SerializeId="true" MonikerElementName="templateSettingsMoniker" ElementName="templateSettings" MonikerTypeName="TemplateSettingsMoniker">
        <DomainClassMoniker Name="TemplateSettings" />
        <ElementData>
          <XmlPropertyData XmlName="templateUri">
            <DomainPropertyMoniker Name="TemplateSettings/TemplateUri" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="createElementOnUnfold">
            <DomainPropertyMoniker Name="TemplateSettings/CreateElementOnUnfold" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="unfoldOnElementCreated">
            <DomainPropertyMoniker Name="TemplateSettings/UnfoldOnElementCreated" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="commandId">
            <DomainPropertyMoniker Name="TemplateSettings/CommandId" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="wizardId">
            <DomainPropertyMoniker Name="TemplateSettings/WizardId" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="templateAuthoringUri">
            <DomainPropertyMoniker Name="TemplateSettings/TemplateAuthoringUri" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="syncName">
            <DomainPropertyMoniker Name="TemplateSettings/SyncName" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sanitizeName">
            <DomainPropertyMoniker Name="TemplateSettings/SanitizeName" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetFileName" Representation="Element">
            <DomainPropertyMoniker Name="TemplateSettings/RawTargetFileName" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetPath" Representation="Element">
            <DomainPropertyMoniker Name="TemplateSettings/RawTargetPath" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="tag">
            <DomainPropertyMoniker Name="TemplateSettings/Tag" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="EventSettings" MonikerAttributeName="" SerializeId="true" MonikerElementName="eventSettingsMoniker" ElementName="eventSettings" MonikerTypeName="EventSettingsMoniker">
        <DomainClassMoniker Name="EventSettings" />
        <ElementData>
          <XmlPropertyData XmlName="commandId">
            <DomainPropertyMoniker Name="EventSettings/CommandId" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="eventId">
            <DomainPropertyMoniker Name="EventSettings/EventId" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="conditions" Representation="Element">
            <DomainPropertyMoniker Name="EventSettings/Conditions" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="filterForCurrentElement">
            <DomainPropertyMoniker Name="EventSettings/FilterForCurrentElement" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="wizardId">
            <DomainPropertyMoniker Name="EventSettings/WizardId" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="CommandSettings" MonikerAttributeName="" SerializeId="true" MonikerElementName="commandSettingsMoniker" ElementName="commandSettings" MonikerTypeName="CommandSettingsMoniker">
        <DomainClassMoniker Name="CommandSettings" />
        <ElementData>
          <XmlPropertyData XmlName="typeId">
            <DomainPropertyMoniker Name="CommandSettings/TypeId" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="properties" Representation="Element">
            <DomainPropertyMoniker Name="CommandSettings/Properties" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="MenuSettings" MonikerAttributeName="" SerializeId="true" MonikerElementName="menuSettingsMoniker" ElementName="menuSettings" MonikerTypeName="MenuSettingsMoniker">
        <DomainClassMoniker Name="MenuSettings" />
        <ElementData>
          <XmlPropertyData XmlName="conditions" Representation="Element">
            <DomainPropertyMoniker Name="MenuSettings/Conditions" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="text">
            <DomainPropertyMoniker Name="MenuSettings/Text" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="icon">
            <DomainPropertyMoniker Name="MenuSettings/Icon" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="commandId">
            <DomainPropertyMoniker Name="MenuSettings/CommandId" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="customStatus">
            <DomainPropertyMoniker Name="MenuSettings/CustomStatus" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="wizardId">
            <DomainPropertyMoniker Name="MenuSettings/WizardId" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sortOrder">
            <DomainPropertyMoniker Name="MenuSettings/SortOrder" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="GuidanceExtension" MonikerAttributeName="" SerializeId="true" MonikerElementName="guidanceExtensionMoniker" ElementName="guidanceExtension" MonikerTypeName="GuidanceExtensionMoniker">
        <DomainClassMoniker Name="GuidanceExtension" />
        <ElementData>
          <XmlPropertyData XmlName="associatedGuidance">
            <DomainPropertyMoniker Name="GuidanceExtension/AssociatedGuidance" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="guidanceInstanceName">
            <DomainPropertyMoniker Name="GuidanceExtension/GuidanceInstanceName" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="guidanceActivateOnCreation">
            <DomainPropertyMoniker Name="GuidanceExtension/GuidanceActivateOnCreation" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="guidanceFeatureId">
            <DomainPropertyMoniker Name="GuidanceExtension/GuidanceFeatureId" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="guidanceSharedInstance">
            <DomainPropertyMoniker Name="GuidanceExtension/GuidanceSharedInstance" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="WizardSettings" MonikerAttributeName="" SerializeId="true" MonikerElementName="wizardSettingsMoniker" ElementName="wizardSettings" MonikerTypeName="WizardSettingsMoniker">
        <DomainClassMoniker Name="WizardSettings" />
        <ElementData>
          <XmlPropertyData XmlName="typeName">
            <DomainPropertyMoniker Name="WizardSettings/TypeName" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ArtifactExtension" MonikerAttributeName="" SerializeId="true" MonikerElementName="artifactExtensionMoniker" ElementName="artifactExtension" MonikerTypeName="ArtifactExtensionMoniker">
        <DomainClassMoniker Name="ArtifactExtension" />
        <ElementData>
          <XmlPropertyData XmlName="onArtifactActivation">
            <DomainPropertyMoniker Name="ArtifactExtension/OnArtifactActivation" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="associatedArtifacts">
            <DomainPropertyMoniker Name="ArtifactExtension/AssociatedArtifacts" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="onArtifactDeletion">
            <DomainPropertyMoniker Name="ArtifactExtension/OnArtifactDeletion" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ValidationExtension" MonikerAttributeName="" SerializeId="true" MonikerElementName="validationExtensionMoniker" ElementName="validationExtension" MonikerTypeName="ValidationExtensionMoniker">
        <DomainClassMoniker Name="ValidationExtension" />
        <ElementData>
          <XmlPropertyData XmlName="validationExecution">
            <DomainPropertyMoniker Name="ValidationExtension/ValidationExecution" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="validationOnBuild">
            <DomainPropertyMoniker Name="ValidationExtension/ValidationOnBuild" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="validationOnSave">
            <DomainPropertyMoniker Name="ValidationExtension/ValidationOnSave" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="validationOnMenu">
            <DomainPropertyMoniker Name="ValidationExtension/ValidationOnMenu" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="validationOnCustomEvent">
            <DomainPropertyMoniker Name="ValidationExtension/ValidationOnCustomEvent" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="DragDropSettings" MonikerAttributeName="" SerializeId="true" MonikerElementName="dragDropSettingsMoniker" ElementName="dragDropSettings" MonikerTypeName="DragDropSettingsMoniker">
        <DomainClassMoniker Name="DragDropSettings" />
        <ElementData>
          <XmlPropertyData XmlName="commandId">
            <DomainPropertyMoniker Name="DragDropSettings/CommandId" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="dropConditions" Representation="Element">
            <DomainPropertyMoniker Name="DragDropSettings/DropConditions" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="wizardId">
            <DomainPropertyMoniker Name="DragDropSettings/WizardId" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="statusText">
            <DomainPropertyMoniker Name="DragDropSettings/StatusText" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
    </ClassData>
  </XmlSerializationBehavior>
</DslLibrary>