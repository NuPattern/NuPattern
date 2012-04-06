<?xml version="1.0" encoding="utf-8"?>
<Dsl xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="15a342fd-f046-4b7a-9dc8-73b0a8eec119" Description="The design of a pattern in a pattern toolkit" Name="PatternModel" DisplayName="Pattern Model Designer" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema" MinorVersion="2" ProductName="Pattern Model Designer" CompanyName="Microsoft" PackageGuid="95d8dad7-be4a-42ae-9cb1-a48bc0eb77c0" PackageNamespace="Microsoft.VisualStudio.Patterning.Runtime.Schema" xmlns="http://schemas.microsoft.com/VisualStudio/2005/DslTools/DslDefinitionModel">
  <Classes>
    <DomainClass Id="76e57dce-a400-4ed1-bbba-00e7d337f5ad" Description="The definition of the pattern in this toolkit." Name="PatternModelSchema" DisplayName="Pattern Model" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema" GeneratesDoubleDerived="true">
      <Notes>IsRoot</Notes>
      <Properties>
        <DomainProperty Id="adc27259-7812-4979-b334-3b4923fb7763" Description="The version of the base pattern line that this pattern line derives from." Name="BaseVersion" DisplayName="Base Version" Category="General" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="d02ca418-3c63-46b4-a3ed-1cc36322ffcd" Description="The unique identifier of the base pattern line that this pattern line derives from." Name="BaseId" DisplayName="Base Identifier" Category="General" IsBrowsable="false">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
      <ElementMergeDirectives>
        <ElementMergeDirective>
          <Notes>Creates an embedding link when an element is dropped onto a model. </Notes>
          <Index>
            <DomainClassMoniker Name="PatternSchema" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>PatternModelHasPattern.Pattern</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
      </ElementMergeDirectives>
    </DomainClass>
    <DomainClass Id="15a27251-8feb-4fae-b6d8-703a5697d3eb" Description="The definition of the pattern." Name="PatternSchema" DisplayName="Pattern" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema">
      <BaseClass>
        <DomainClassMoniker Name="PatternElementSchema" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="1ac193fd-0b29-439b-8009-fc6d6f50ec0c" Description="The identifier of the Visual Studio extension that deploys this pattern." Name="ExtensionId" DisplayName="Extension Id" Kind="CustomStorage" Category="General" SetterAccessModifier="Private" IsBrowsable="false">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="1678920d-c2ac-4160-8ae6-bb643e52f2cd" Description="Gets the currently opened diagram identifier." Name="CurrentDiagramId" DisplayName="Current Diagram Id" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="97c8fce3-87a7-45d6-8c3b-dbb0d68b0f1f" Description="The identifier of the instance of this pattern." Name="PatternLink" DisplayName="Pattern Link" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
      <ElementMergeDirectives>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="ViewSchema" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>PatternHasViews.Views</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="ProvidedExtensionPointSchema" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>PatternHasProvidedExtensionPoints.ProvidedExtensionPoints</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
      </ElementMergeDirectives>
    </DomainClass>
    <DomainClass Id="50bd80bb-6516-4bfc-a5e5-8ce26fc23224" Description="An element that has a unique name." Name="NamedElementSchema" DisplayName="Named Element" InheritanceModifier="Abstract" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema">
      <Properties>
        <DomainProperty Id="3e893411-e795-45d4-89b4-1d6d8beef0ec" Description="The well-known name of this item in this model." Name="Name" DisplayName="Name" Category="General" IsElementName="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
          <ElementNameProvider>
            <ExternalTypeMoniker Name="NamedElementSchemaNameProvider" />
          </ElementNameProvider>
        </DomainProperty>
        <DomainProperty Id="143d5d67-86e6-4506-9370-e9f05fbcfd9f" Description="The identifier of the inherited variability model." Name="BaseId" DisplayName="Base Id" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="3cf4da77-3984-4048-913f-1014118cee9f" Description="The name used for instances of this item, as seen by the user. Also used to name associated artifacts/configuration created for this item." Name="DisplayName" DisplayName="Display Name" Kind="CustomStorage" Category="Appearance">
          <Attributes>
            <ClrAttribute Name="Microsoft.VisualStudio.Patterning.Runtime.Schema.CustomizableDomainElementSettingAttribute" />
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="8e1985e3-334e-4e0c-9b12-78260eb8a403" Description="Used to track whether the user changed the display name manually." Name="IsDisplayNameTracking" DisplayName="Is Display Name Tracking" DefaultValue="true" Category="Appearance" GetterAccessModifier="Assembly" SetterAccessModifier="Private" IsBrowsable="false">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="d60b22b2-540e-4024-9a15-6a66fc3719ba" Description="The description of this item displayed to the user." Name="Description" DisplayName="Description" Kind="CustomStorage" Category="Appearance">
          <Attributes>
            <ClrAttribute Name="Microsoft.VisualStudio.Patterning.Runtime.Schema.CustomizableDomainElementSettingAttribute" />
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="465a9853-9f76-45d1-8196-10ca1c3f3ed3" Description="Used to track whether the user changed the description manually." Name="IsDescriptionTracking" DisplayName="Is Description Tracking" DefaultValue="true" Category="Appearance" GetterAccessModifier="Assembly" SetterAccessModifier="Private" IsBrowsable="false">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="eabb729a-4396-4bfb-a420-cca5524bf83c" Description="Whether the element is derived from a base variability model definition." Name="IsInheritedFromBase" DisplayName="Is Inherited From Base" DefaultValue="false" Kind="Calculated" SetterAccessModifier="Private" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="08aa5eae-38da-48fe-8106-022bb0533190" Description="The path of this element in the variability model." Name="SchemaPath" DisplayName="Schema Path" Kind="Calculated" Category="General" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="d749fd5a-8b9d-4925-acb6-2eea03c3b6d0" Description="Whether this element is hidden from the design-view. Used by automation extensions." Name="IsSystem" DisplayName="Is Hidden" DefaultValue="false" Category="General" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="048d7d20-3602-4ada-857c-3709e6701460" Description="The identifier used for naming generating code artifacts that represent this element. This identifier must be unique across the whole model." Name="CodeIdentifier" DisplayName="Code Identifier" Kind="CustomStorage" Category="Generation">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="f1112098-59a1-467d-b034-dc1f358b5e3b" Description="Used to track whether the user changed the  code identifier manually." Name="IsCodeIdentifierTracking" DisplayName="Is Code Identifier Tracking" DefaultValue="true" GetterAccessModifier="Assembly" SetterAccessModifier="Private" IsBrowsable="false">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="dbe13a31-7dcd-4fbd-a601-18ca765e264e" Description="A container of properties and automation." Name="PatternElementSchema" DisplayName="Pattern Element" InheritanceModifier="Abstract" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema">
      <BaseClass>
        <DomainClassMoniker Name="CustomizableElementSchema" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="009e8fbb-1e3a-4fd4-98ba-3e20e2507428" Description="The validation rules applied to this element." Name="ValidationRules" DisplayName="Validation Rules" Category="Validation">
          <Notes>Skip=true</Notes>
          <Attributes>
            <ClrAttribute Name="Microsoft.VisualStudio.Patterning.Runtime.Schema.CustomizableDomainElementSettingAttribute" />
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="036d59f2-87d1-49f3-aee4-4e73b3566b2e" Description="The icon for this item displayed to the user." Name="Icon" DisplayName="Icon" Category="Appearance">
          <Attributes>
            <ClrAttribute Name="Microsoft.VisualStudio.Patterning.Extensibility.ImageFilter">
              <Parameters>
                <AttributeParameter Value="Microsoft.VisualStudio.Patterning.Extensibility.ImageKind.Image " />
              </Parameters>
            </ClrAttribute>
            <ClrAttribute Name="System.ComponentModel.Editor">
              <Parameters>
                <AttributeParameter Value="typeof(Microsoft.VisualStudio.Patterning.Extensibility.ImageUriEditor)" />
                <AttributeParameter Value="typeof(System.Drawing.Design.UITypeEditor)" />
              </Parameters>
            </ClrAttribute>
            <ClrAttribute Name="Microsoft.VisualStudio.Patterning.Runtime.Schema.CustomizableDomainElementSettingAttribute" />
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
      <ElementMergeDirectives>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="PropertySchema" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>PatternElementHasProperties.Properties</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="AutomationSettingsSchema" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>PatternElementHasAutomationSettings.AutomationSettings</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
      </ElementMergeDirectives>
    </DomainClass>
    <DomainClass Id="079a705a-0fc2-4c40-b0ba-c8b76fc60f7c" Description="A property of an element." Name="PropertySchema" DisplayName="Property" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema">
      <BaseClass>
        <DomainClassMoniker Name="CustomizableElementSchema" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="df11ede4-3da1-4416-9f7d-cfb840e28bb3" Description="The initial value of this property when created." Name="RawDefaultValue" DisplayName="Default Value" Category="Data" SetterAccessModifier="Assembly">
          <Attributes>
            <ClrAttribute Name="Microsoft.VisualStudio.Patterning.Runtime.Schema.CustomizableDomainElementSettingAttribute" />
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="112c82d2-866f-4f9f-8f32-9cb0cc636fac" Description="The data type of this property, which determines the type of its value." Name="Type" DisplayName="Type" DefaultValue="System.String" Category="General">
          <Attributes>
            <ClrAttribute Name="System.ComponentModel.RefreshProperties">
              <Parameters>
                <AttributeParameter Value="System.ComponentModel.RefreshProperties.All" />
              </Parameters>
            </ClrAttribute>
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="97ced30c-61bc-44ad-9041-00945076d37d" Description="Whether this property is shown to the user." Name="IsVisible" DisplayName="Is Visible" DefaultValue="true" Category="Appearance">
          <Attributes>
            <ClrAttribute Name="Microsoft.VisualStudio.Patterning.Runtime.Schema.CustomizableDomainElementSettingAttribute" />
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="452d1740-162b-4911-874a-a45ca1018e97" Description="Whether this property is read-only to the user." Name="IsReadOnly" DisplayName="Is Read Only" DefaultValue="false" Category="Appearance">
          <Attributes>
            <ClrAttribute Name="Microsoft.VisualStudio.Patterning.Runtime.Schema.CustomizableDomainElementSettingAttribute" />
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="93af01ae-05ca-44fd-9e8b-8230c5662415" Description="The category for this property, used to organize similar properties shown in the Properties Window." Name="Category" DisplayName="Category" DefaultValue="General" Category="Appearance">
          <Attributes>
            <ClrAttribute Name="Microsoft.VisualStudio.Patterning.Runtime.Schema.CustomizableDomainElementSettingAttribute" />
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="b6c1c888-82c5-4619-8e2b-e69e1cf975aa" Description="The primary usage of this property," Name="PropertyUsage" DisplayName="Property Usage" DefaultValue="General" Category="General" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/Microsoft.VisualStudio.Patterning.Runtime/PropertyUsages" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="19df1177-f0a6-4540-bd5b-7b350441bee6" Description="A System.ComponentModel.TypeConverter that converts from the string value of this property, that the user enters, to an instance of the Type of this property (and visa-versa). This Type Converter can also be used to provide a list of acceptable values." Name="TypeConverterTypeName" DisplayName="Type Converter" Category="Appearance">
          <Attributes>
            <ClrAttribute Name="System.ComponentModel.Editor">
              <Parameters>
                <AttributeParameter Value="typeof(Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Design.StandardValuesEditor)" />
                <AttributeParameter Value="typeof(System.Drawing.Design.UITypeEditor)" />
              </Parameters>
            </ClrAttribute>
            <ClrAttribute Name="System.ComponentModel.TypeConverter">
              <Parameters>
                <AttributeParameter Value="typeof(Microsoft.VisualStudio.Patterning.Extensibility.FullTypeTypeConverter&lt;System.ComponentModel.TypeConverter&gt;)" />
              </Parameters>
            </ClrAttribute>
            <ClrAttribute Name="Microsoft.VisualStudio.Patterning.Runtime.Schema.CustomizableDomainElementSettingAttribute" />
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="377771ed-e31c-4d13-bd13-920a33d7c0b0" Description="A System.Drawing.Design.UITypeEditor that provides a custom UI for editing the value of this property." Name="EditorTypeName" DisplayName="Type Editor" Category="Appearance">
          <Attributes>
            <ClrAttribute Name="System.ComponentModel.Editor">
              <Parameters>
                <AttributeParameter Value="typeof(Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Design.StandardValuesEditor)" />
                <AttributeParameter Value="typeof(System.Drawing.Design.UITypeEditor)" />
              </Parameters>
            </ClrAttribute>
            <ClrAttribute Name="System.ComponentModel.TypeConverter">
              <Parameters>
                <AttributeParameter Value="typeof(Microsoft.VisualStudio.Patterning.Extensibility.FullTypeTypeConverter&lt;System.Drawing.Design.UITypeEditor&gt;)" />
              </Parameters>
            </ClrAttribute>
            <ClrAttribute Name="Microsoft.VisualStudio.Patterning.Runtime.Schema.CustomizableDomainElementSettingAttribute" />
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="e9070cd0-3aa5-4e07-8504-a735f40a8be4" Description="The validation rules applied to this element." Name="RawValidationRules" DisplayName="Validation Rules" Category="Validation">
          <Notes>Skip=true</Notes>
          <Attributes>
            <ClrAttribute Name="Microsoft.VisualStudio.Patterning.Runtime.Schema.CustomizableDomainElementSettingAttribute" />
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="e6d6779f-b6b9-4db4-9bf3-04e864a4c58e" Description="A value provider that calculates the value of this property dynamically." Name="RawValueProvider" DisplayName="Value Provider" Category="Data">
          <Attributes>
            <ClrAttribute Name="Microsoft.VisualStudio.Patterning.Extensibility.PropertyDescriptor">
              <Parameters>
                <AttributeParameter Value="typeof(Microsoft.VisualStudio.Patterning.Extensibility.Binding.BindingPropertyDescriptor&lt;Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.IValueProvider&gt;)" />
              </Parameters>
            </ClrAttribute>
            <ClrAttribute Name="Microsoft.VisualStudio.Patterning.Runtime.Schema.CustomizableDomainElementSettingAttribute" />
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="14b6f5ec-0468-4380-9210-07c2fdbda012" Description="A distinct view of the pattern." Name="ViewSchema" DisplayName="View" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema" GeneratesDoubleDerived="true">
      <BaseClass>
        <DomainClassMoniker Name="CustomizableElementSchema" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="5812d2f0-206f-4356-a12f-b2419cc11082" Description="Whether this view is shown to the user." Name="IsVisible" DisplayName="Is Visible" DefaultValue="true" Category="Appearance">
          <Attributes>
            <ClrAttribute Name="Microsoft.VisualStudio.Patterning.Runtime.Schema.CustomizableDomainElementSettingAttribute" />
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="0e03dbb8-03d7-44b6-9583-6ce62b8df519" Description="Whether this is the default view" Name="IsDefault" DisplayName="Is Default" DefaultValue="false" Category="General">
          <Attributes>
            <ClrAttribute Name="Microsoft.VisualStudio.Patterning.Runtime.Schema.CustomizableDomainElementSettingAttribute" />
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="2eb831f0-0ef8-46a7-937b-0c6c6d64a77b" Description="" Name="DiagramId" DisplayName="Diagram Id" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="8d050866-c6e3-4231-b5d5-f1e9cc8b1edb" Description="" Name="Caption" DisplayName="Caption" Kind="Calculated" Category="Appearance" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
      <ElementMergeDirectives>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="AbstractElementSchema" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>ViewHasElements.Elements</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="ExtensionPointSchema" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>ViewHasExtensionPoints.ExtensionPoints</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
      </ElementMergeDirectives>
    </DomainClass>
    <DomainClass Id="5c399883-85a3-4863-90dd-470b20576f61" Description="A child collection element." Name="CollectionSchema" DisplayName="Collection" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema">
      <BaseClass>
        <DomainClassMoniker Name="AbstractElementSchema" />
      </BaseClass>
    </DomainClass>
    <DomainClass Id="18de646e-e001-4e6b-b78b-59086f74d429" Description="A child element." Name="ElementSchema" DisplayName="Variable Element" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema">
      <BaseClass>
        <DomainClassMoniker Name="AbstractElementSchema" />
      </BaseClass>
    </DomainClass>
    <DomainClass Id="129120e9-f7e2-4154-805e-16bf6890f67a" Description="An element that supports customization of its properties." Name="CustomizableElementSchema" DisplayName="Customizable Element" InheritanceModifier="Abstract" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema" GeneratesDoubleDerived="true">
      <BaseClass>
        <DomainClassMoniker Name="NamedElementSchema" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="0bc1eb0c-b224-4d81-871c-bfda75e94ba5" Description="Whether customization is permitted for this element, all its policy settings, and any child elements." Name="IsCustomizable" DisplayName="Is Customizable" DefaultValue="Inherited" Kind="CustomStorage" Category="Customization">
          <Type>
            <ExternalTypeMoniker Name="/Microsoft.VisualStudio.Patterning.Runtime/CustomizationState" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="f6025928-bb14-41d8-86fc-af24d6f05424" Description="Whether customization is enabled for the tailor." Name="IsCustomizationEnabled" DisplayName="Is Customization Enabled" DefaultValue="true" Category="Customization" SetterAccessModifier="Private" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="e6037ef6-b825-4b9d-82cd-e851c41122a2" Description="Whether the policy can be modified." Name="IsCustomizationPolicyModifyable" DisplayName="Is Policy Modifyable" Kind="Calculated" Category="Customization" SetterAccessModifier="Private" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="8c6962aa-df4f-4bf9-baf0-d15c0cf6099f" Description="The combined state of IsEnabled and IsCustomizable state of the element." Name="IsCustomizationEnabledState" DisplayName="Is Customization Enabled State" Kind="Calculated" Category="Customization" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <DomainEnumerationMoniker Name="CustomizationEnabledState" />
          </Type>
        </DomainProperty>
      </Properties>
      <ElementMergeDirectives>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="CustomizationPolicySchema" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>CustomizableElementHasPolicy.Policy</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
      </ElementMergeDirectives>
    </DomainClass>
    <DomainClass Id="a022a552-666a-4f23-b829-4edf1cea0971" Description="A child element or collection of the pattern." Name="AbstractElementSchema" DisplayName="Abstract Element" InheritanceModifier="Abstract" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema">
      <BaseClass>
        <DomainClassMoniker Name="PatternElementSchema" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="18d96656-8bab-4ce0-b8fa-2eb9edaf26cc" Description="Whether this item is shown to the user." Name="IsVisible" DisplayName="Is Visible" DefaultValue="true" Category="Appearance">
          <Attributes>
            <ClrAttribute Name="Microsoft.VisualStudio.Patterning.Runtime.Schema.CustomizableDomainElementSettingAttribute" />
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
      </Properties>
      <ElementMergeDirectives>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="AbstractElementSchema" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>ElementHasElements.Elements</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="ExtensionPointSchema" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>ElementHasExtensionPoints.ExtensionPoints</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
      </ElementMergeDirectives>
    </DomainClass>
    <DomainClass Id="c9fbbae3-628d-46f1-860e-5e80fc1b6211" Description="The policy that controls what properties are customizable on an element." Name="CustomizationPolicySchema" DisplayName="Customization Policy" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema" GeneratesDoubleDerived="true">
      <Attributes>
        <ClrAttribute Name="Microsoft.VisualStudio.Patterning.Extensibility.CategoryResource">
          <Parameters>
            <AttributeParameter Value="&quot;CustomizationCategory&quot;" />
            <AttributeParameter Value="typeof(Microsoft.VisualStudio.Patterning.Runtime.Schema.Properties.Resources)" />
          </Parameters>
        </ClrAttribute>
      </Attributes>
      <Properties>
        <DomainProperty Id="b59b2925-1a2c-44bd-ba9e-903ff5fd45b2" Description="Whether any of the settings in the policy have been modified from their default values." Name="IsModified" DisplayName="Is Modified" Kind="Calculated" Category="Customization" SetterAccessModifier="Private" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="9c519261-d0f9-4433-bcb4-7593c4ca6548" Description="The extent to which settings have been customized." Name="CustomizationLevel" DisplayName="Customization Level" DefaultValue="None" Kind="Calculated" Category="Customization" SetterAccessModifier="Private" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/Microsoft.VisualStudio.Patterning.Runtime/CustomizedLevel" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="29a60577-5540-4a0b-a1d2-e5496fd9ecd5" Description="Description for Microsoft.VisualStudio.Patterning.Runtime.Schema.CustomizationPolicySchema.Name" Name="Name" DisplayName="Name" GetterAccessModifier="Private" SetterAccessModifier="Private" IsElementName="true" IsBrowsable="false">
          <Notes>Used by custom type descriptors.</Notes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
      <ElementMergeDirectives>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="CustomizableSettingSchema" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>PolicyHasSettings.Settings</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
      </ElementMergeDirectives>
    </DomainClass>
    <DomainClass Id="a8991222-7d76-4467-b4d4-663daec5050d" Description="The settings for a customizable property." Name="CustomizableSettingSchema" DisplayName="Customizable Setting" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema" GeneratesDoubleDerived="true">
      <Properties>
        <DomainProperty Id="c85e8df3-5cfc-4f0e-8798-9f9efac899e1" Description="Whether this setting can be further customized by a tailor." Name="IsEnabled" DisplayName="Is Enabled" DefaultValue="true" Category="Customization" SetterAccessModifier="Private" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="9cf11c58-d5a3-4791-b28a-6d3678705cc9" Description="The displayed caption shown to the user." Name="Caption" DisplayName="Caption" Kind="Calculated" Category="Customization" SetterAccessModifier="Private" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="5b0bba18-b966-4cc1-9e51-58f72242be36" Description="The formatter used for the caption." Name="CaptionFormatter" DisplayName="Caption Formatter" DefaultValue="Modify '{0}'" Category="Customization" SetterAccessModifier="Private" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="ba6a5b29-9ac7-4280-98aa-04d8bab7cdb7" Description="Whether the settings has been modified from its default value." Name="IsModified" DisplayName="Is Modified" DefaultValue="" Kind="Calculated" Category="Customization" SetterAccessModifier="Private" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="14621c1b-4dd9-4a7f-9913-2fa61e0e41fe" Description="Whether this setting can be customized by a tailor by default." Name="DefaultValue" DisplayName="Default Value" DefaultValue="true" Kind="CustomStorage" Category="Customization" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="6a4707d5-4843-412f-9fd2-141916c04e91" Description="Whether this setting can be customized by a tailor." Name="Value" DisplayName="Value" DefaultValue="true" Kind="CustomStorage" Category="Customization">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="f098de8d-21bc-4375-9dae-9a8e21fbfc21" Description="The associated property name for the setting." Name="PropertyId" DisplayName="Property Id" DefaultValue="" Category="Customization" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="8e232562-aca2-4bf7-a016-d30e0c9960a4" Description="The formatter used for the description." Name="DescriptionFormatter" DisplayName="Description Formatter" DefaultValue="Whether the '{0}' property of this element is customizable or not." Category="Customization" SetterAccessModifier="Private" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="2f4e56f5-78e4-4f49-9186-8ec03ca0d897" Description="The displayed description shown to the user." Name="Description" DisplayName="Description" Kind="Calculated" Category="Customization" SetterAccessModifier="Private" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="39fd83fe-037f-4e26-88a5-bf43cdbb97c7" Description="The type of domain element that this setting applies to." Name="DomainElementSettingType" DisplayName="Domain Element Setting Type" DefaultValue="" Kind="Calculated" Category="Customization" GetterAccessModifier="Assembly" SetterAccessModifier="Private" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <DomainEnumerationMoniker Name="CustomizableDomainElementSettingType" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="a997feca-f406-49ad-82ae-fc1f53d31527" Description="The settings for an automation extension." Name="AutomationSettingsSchema" DisplayName="Automation Settings" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema">
      <BaseClass>
        <DomainClassMoniker Name="CustomizableElementSchema" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="8f400f8b-1c99-4793-8713-4739e1c13afb" Description="The name of this type of automation." Name="AutomationType" DisplayName="Automation Type" Category="Automation" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="f7ec96b2-44eb-4a19-8fd2-77a695f34e08" Description="The specific settings for this Automation Type." Name="Settings" DisplayName="Settings" Category="Automation" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly">
          <Notes>The property is the placeholder for the type descriptor to provide reflected properties of the extender settings.</Notes>
          <Attributes>
            <ClrAttribute Name="Microsoft.VisualStudio.Patterning.Runtime.Schema.CustomizableDomainElementSettingAttribute" />
            <ClrAttribute Name="System.ComponentModel.TypeConverter">
              <Parameters>
                <AttributeParameter Value="typeof(AutomationSettingsTypeConverter)" />
              </Parameters>
            </ClrAttribute>
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="a1d81216-30bc-43a9-aa6d-ae06a58f5490" Description="The classification of this automation." Name="Classification" DisplayName="Classification" DefaultValue="General" Category="Automation" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/Microsoft.VisualStudio.Patterning.Runtime/AutomationSettingsClassification" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="14956bbf-ded7-4762-9ade-ced0cc89683c" Description="The extension points that this pattern provides." Name="ProvidedExtensionPointSchema" DisplayName="Provided Extension Point Schema" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema">
      <Properties>
        <DomainProperty Id="9e6b27e9-f683-444d-a1ab-2b3b384f51de" Description="The extension point provided by this pattern." Name="ExtensionPointId" DisplayName="Extension Point Id" Category="Extensibility" IsElementName="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="889fcf39-249f-4484-a7e8-2bf14320962f" Description="A child extension to the pattern, provided by a pattern of another toolkit." Name="ExtensionPointSchema" DisplayName="Extension Point" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema">
      <BaseClass>
        <DomainClassMoniker Name="PatternElementSchema" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="828924e1-deaf-4cca-bcc7-47889b27ac06" Description="The unique type of this extension point, that other patterns would provide extensions to." Name="RequiredExtensionPointId" DisplayName="Extension Type Id" Kind="Calculated" Category="Extensibility" SetterAccessModifier="Assembly" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="a0a863a2-8c90-474f-b2ee-0a7f34037241" Description="The constraints that determine which patterns from other toolkits can extend this model." Name="Conditions" DisplayName="Constraints" Category="Extensibility">
          <Notes>Skip=true</Notes>
          <Attributes>
            <ClrAttribute Name="Microsoft.VisualStudio.Patterning.Runtime.Schema.CustomizableDomainElementSettingAttribute" />
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="26995a2e-7740-4fe4-9721-78db15026552" Description="An extension point of a pattern represented by this extension point." Name="RepresentedExtensionPointId" DisplayName="Representation Of" Category="Extensibility" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
  </Classes>
  <Relationships>
    <DomainRelationship Id="ef11f513-2a94-4473-8b92-e23ad76801f3" Description="Description for Microsoft.VisualStudio.Patterning.Runtime.Schema.PatternModelHasPattern" Name="PatternModelHasPattern" DisplayName="Pattern Model Has Pattern" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema" IsEmbedding="true">
      <Source>
        <DomainRole Id="ba78db30-bb4c-4613-b749-2c4112135770" Description="The pattern in this definition." Name="PatternModelSchema" DisplayName="Pattern Model Schema" PropertyName="Pattern" Multiplicity="One" PropagatesCopy="PropagatesCopyToLinkAndOppositeRolePlayer" PropertyDisplayName="Pattern">
          <RolePlayer>
            <DomainClassMoniker Name="PatternModelSchema" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="a173d11e-d3aa-41c0-800c-8a90c8bfdf2b" Description="The definition of the pattern." Name="PatternSchema" DisplayName="Pattern Schema" PropertyName="PatternModel" Multiplicity="One" PropagatesDelete="true" PropertyDisplayName="Pattern Model">
          <RolePlayer>
            <DomainClassMoniker Name="PatternSchema" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="4b1811b8-c0fe-451a-9715-881e3af1eb2f" Description="Description for Microsoft.VisualStudio.Patterning.Runtime.Schema.PatternElementHasProperties" Name="PatternElementHasProperties" DisplayName="Pattern Element Has Properties" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema" IsEmbedding="true">
      <Source>
        <DomainRole Id="047b91aa-4ea9-4dfe-9f20-b5c2ff8ae6df" Description="The properties of this element." Name="PatternElementSchema" DisplayName="Pattern Element Schema" PropertyName="Properties" PropagatesCopy="PropagatesCopyToLinkAndOppositeRolePlayer" PropertyDisplayName="Properties">
          <RolePlayer>
            <DomainClassMoniker Name="PatternElementSchema" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="a36dc941-fb07-4d65-847f-09fe8ee64497" Description="The owning element." Name="PropertySchema" DisplayName="Property Schema" PropertyName="Owner" Multiplicity="One" PropagatesDelete="true" PropertyDisplayName="Owner">
          <Notes>Skip=true</Notes>
          <RolePlayer>
            <DomainClassMoniker Name="PropertySchema" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="aad70656-c8af-4d31-bd93-e288096ee274" Description="Description for Microsoft.VisualStudio.Patterning.Runtime.Schema.PatternHasViews" Name="PatternHasViews" DisplayName="Pattern Has Views" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema" IsEmbedding="true">
      <Source>
        <DomainRole Id="9b7b2ee3-1c1f-48f2-a873-07774f4dd02d" Description="The views of this pattern." Name="PatternSchema" DisplayName="Pattern Schema" PropertyName="Views" Multiplicity="OneMany" PropagatesCopy="PropagatesCopyToLinkAndOppositeRolePlayer" PropertyDisplayName="Views">
          <RolePlayer>
            <DomainClassMoniker Name="PatternSchema" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="413f3607-99ef-4762-8093-eab85559d32d" Description="The owning pattern." Name="ViewSchema" DisplayName="View Schema" PropertyName="Pattern" Multiplicity="One" PropagatesDelete="true" PropertyDisplayName="Pattern">
          <RolePlayer>
            <DomainClassMoniker Name="ViewSchema" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="3bfd2e51-26d4-48dd-89a2-7e7a715b2bfd" Description="Description for Microsoft.VisualStudio.Patterning.Runtime.Schema.ViewHasElements" Name="ViewHasElements" DisplayName="View Has Elements" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema" IsEmbedding="true">
      <Notes>Skip=true</Notes>
      <Properties>
        <DomainProperty Id="dfc4aa83-b438-459f-bb11-b470e82cbcc2" Description="The number of instances of this collection/element, for this view." Name="Cardinality" DisplayName="Cardinality" DefaultValue="OneToOne" Category="Structure">
          <Type>
            <ExternalTypeMoniker Name="/Microsoft.VisualStudio.Patterning.Runtime/Cardinality" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="1e1d2e7f-fc69-484e-a9c8-f695e00b9b3d" Description="The displayed caption for the cardinality." Name="CardinalityCaption" DisplayName="Cardinality Caption" Kind="Calculated" Category="Structure" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="a805afd4-df27-410e-b467-1f5e7fcdb78e" Description="Whether to automatically create the first instance of this collection/element when the view is created." Name="AutoCreate" DisplayName="Auto Create" DefaultValue="true" Category="Structure">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="c19f4398-079f-4f7d-9d83-57995ad2a715" Description="Whether to allow UI (i.e. menus) for adding new instances of this collection/element manually." Name="AllowAddNew" DisplayName="Allow Add New" DefaultValue="true" Category="Appearance">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="b4bdc451-c23d-4ad0-8d54-2f0b1b5ff25c" Description="The group in which instances of the element/collection will be ordered together, relative to instances of sibling elements/collections. By default, all instances of all sibling elements/collections are ordered together alphabetically by their 'InstanceName' property." Name="OrderGroup" DisplayName="Order Group" DefaultValue="1" Category="Structure">
          <Type>
            <ExternalTypeMoniker Name="/System/Int32" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="c619605a-9fed-4372-be5c-dca910fc1061" Description="A System.Collections.IComparer class that determines how instances of all elements/collections in this 'Order Group' are ordered together. If left blank, then instances of all elements/collections are ordered together alphabetically by their 'InstanceName' property." Name="OrderGroupComparerTypeName" DisplayName="Order Group Comparer" Category="Structure">
          <Attributes>
            <ClrAttribute Name="System.ComponentModel.Editor">
              <Parameters>
                <AttributeParameter Value="typeof(Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Design.StandardValuesEditor)" />
                <AttributeParameter Value="typeof(System.Drawing.Design.UITypeEditor)" />
              </Parameters>
            </ClrAttribute>
            <ClrAttribute Name="System.ComponentModel.TypeConverter">
              <Parameters>
                <AttributeParameter Value="typeof(Microsoft.VisualStudio.Patterning.Extensibility.FullTypeTypeConverter&lt;System.Collections.IComparer&gt;)" />
              </Parameters>
            </ClrAttribute>
            <ClrAttribute Name="Microsoft.VisualStudio.Patterning.Runtime.Schema.CustomizableDomainElementSettingAttribute" />
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
      <Source>
        <DomainRole Id="f8ee9bb4-3327-41db-84c6-10f81425e0b1" Description="The child elements of the view." Name="ViewSchema" DisplayName="View Schema" PropertyName="Elements" PropagatesCopy="PropagatesCopyToLinkAndOppositeRolePlayer" PropertyDisplayName="Elements">
          <Notes>Skip=true</Notes>
          <RolePlayer>
            <DomainClassMoniker Name="ViewSchema" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="9144c828-2674-4da6-b7cd-02ba2caa48b0" Description="The owning view." Name="AbstractElementSchema" DisplayName="Abstract Element Schema" PropertyName="View" Multiplicity="ZeroOne" PropagatesDelete="true" PropertyDisplayName="View">
          <Notes>Skip=true</Notes>
          <RolePlayer>
            <DomainClassMoniker Name="AbstractElementSchema" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="c30fb6ed-9e58-473d-a596-e52f20c88224" Description="Description for Microsoft.VisualStudio.Patterning.Runtime.Schema.PolicyHasSettings" Name="PolicyHasSettings" DisplayName="Policy Has Settings" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema" IsEmbedding="true">
      <Source>
        <DomainRole Id="eb849f5a-92b0-4bd1-8b0d-05663de599cd" Description="The individual settings of the customization policy" Name="CustomizationPolicySchema" DisplayName="Customization Policy Schema" PropertyName="Settings" PropagatesCopy="PropagatesCopyToLinkAndOppositeRolePlayer" PropertyDisplayName="Settings">
          <RolePlayer>
            <DomainClassMoniker Name="CustomizationPolicySchema" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="ab2c4075-57db-4e21-9b1f-da59fa3e5d3d" Description="The owning policy." Name="CustomizableSettingSchema" DisplayName="Customizable Setting Schema" PropertyName="Policy" Multiplicity="One" PropagatesDelete="true" PropertyDisplayName="Policy">
          <RolePlayer>
            <DomainClassMoniker Name="CustomizableSettingSchema" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="b060fe61-7009-47eb-a1ad-5267426e6afd" Description="Description for Microsoft.VisualStudio.Patterning.Runtime.Schema.CustomizableElementHasPolicy" Name="CustomizableElementHasPolicy" DisplayName="Customizable Element Has Policy" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema" IsEmbedding="true">
      <Source>
        <DomainRole Id="873ce323-5608-4607-870b-1419d5a4c88d" Description="The customization policy that applies to the element." Name="CustomizableElementSchema" DisplayName="Customizable Element Schema" PropertyName="Policy" Multiplicity="One" PropagatesCopy="PropagatesCopyToLinkAndOppositeRolePlayer" PropertyDisplayName="Policy">
          <RolePlayer>
            <DomainClassMoniker Name="CustomizableElementSchema" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="fdc3a233-a290-4fa9-b891-b5f9ae2425f7" Description="The owning element." Name="CustomizationPolicySchema" DisplayName="Customization Policy Schema" PropertyName="Owner" Multiplicity="One" PropagatesDelete="true" PropertyDisplayName="Owner">
          <RolePlayer>
            <DomainClassMoniker Name="CustomizationPolicySchema" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="b0afa4dc-233e-4c6c-a058-5b2f2989b751" Description="Description for Microsoft.VisualStudio.Patterning.Runtime.Schema.ElementHasElements" Name="ElementHasElements" DisplayName="Element Has Elements" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema" IsEmbedding="true">
      <Notes>Skip=true</Notes>
      <Properties>
        <DomainProperty Id="c67de03d-9482-4c9a-a6b9-e619f5aef380" Description="The number of instances of this element/collection, for each parent element." Name="Cardinality" DisplayName="Cardinality" DefaultValue="OneToOne" Category="Structure">
          <Type>
            <ExternalTypeMoniker Name="/Microsoft.VisualStudio.Patterning.Runtime/Cardinality" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="b8f5717a-75e0-4afe-be12-dda9dc292df3" Description="The displayed caption for the cardinality." Name="CardinalityCaption" DisplayName="Cardinality Caption" Kind="Calculated" Category="Structure" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="03cf9a2c-aa6b-4d4f-a657-c4dd8a85e6f5" Description="Whether to automatically create the first instance of this collection/element when the parent element is created." Name="AutoCreate" DisplayName="Auto Create" DefaultValue="true" Category="Structure">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="f8af50a7-3fa3-4904-8365-4b7bcb05e0f2" Description="Whether to allow UI (i.e. menus) for adding new instances of this collection/element manually." Name="AllowAddNew" DisplayName="Allow Add New" DefaultValue="true" Category="Appearance">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="23dd652d-01cb-4d67-9ba8-12c5432e2021" Description="The group in which instances of the element/collection will be ordered together, relative to instances of sibling elements/collections. By default, all instances of all sibling elements/collections are ordered together alphabetically by their 'InstanceName' property." Name="OrderGroup" DisplayName="Order Group" DefaultValue="1" Category="Structure">
          <Type>
            <ExternalTypeMoniker Name="/System/Int32" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="790467d4-8890-464e-a449-5eca7313a082" Description="A System.Collections.IComparer class that determines how instances of all elements/collections in this 'Order Group' are ordered together. If left blank, then instances of all elements/collections are ordered together alphabetically by their 'InstanceName' property." Name="OrderGroupComparerTypeName" DisplayName="Order Group Comparer" Category="Structure">
          <Attributes>
            <ClrAttribute Name="System.ComponentModel.Editor">
              <Parameters>
                <AttributeParameter Value="typeof(Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Design.StandardValuesEditor)" />
                <AttributeParameter Value="typeof(System.Drawing.Design.UITypeEditor)" />
              </Parameters>
            </ClrAttribute>
            <ClrAttribute Name="System.ComponentModel.TypeConverter">
              <Parameters>
                <AttributeParameter Value="typeof(Microsoft.VisualStudio.Patterning.Extensibility.FullTypeTypeConverter&lt;System.Collections.IComparer&gt;)" />
              </Parameters>
            </ClrAttribute>
            <ClrAttribute Name="Microsoft.VisualStudio.Patterning.Runtime.Schema.CustomizableDomainElementSettingAttribute" />
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
      <Source>
        <DomainRole Id="8a909b18-fbd3-48c1-adf9-a6fbeae26aa6" Description="The child elemets of this element." Name="ParentElement" DisplayName="Parent Element" PropertyName="Elements" PropagatesCopy="PropagatesCopyToLinkAndOppositeRolePlayer" PropertyDisplayName="Elements">
          <Notes>Skip=true</Notes>
          <RolePlayer>
            <DomainClassMoniker Name="AbstractElementSchema" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="f9fecc66-b0f5-4f50-9ab8-30fffb30f00e" Description="The owning element." Name="ChildElement" DisplayName="Child Element" PropertyName="Owner" Multiplicity="ZeroOne" PropagatesDelete="true" PropertyDisplayName="Owner">
          <Notes>Skip=true</Notes>
          <RolePlayer>
            <DomainClassMoniker Name="AbstractElementSchema" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="932e4f6e-8c17-4cdd-923e-409a627ba93c" Description="Description for Microsoft.VisualStudio.Patterning.Runtime.Schema.PatternHasProvidedExtensionPoints" Name="PatternHasProvidedExtensionPoints" DisplayName="Pattern Has Provided Extension Points" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema" IsEmbedding="true">
      <Source>
        <DomainRole Id="ac61bcdd-4aa4-44a2-b7a1-d71a7f46999a" Description="The extension points of other patterns that this pattern extends." Name="PatternSchema" DisplayName="Pattern Schema" PropertyName="ProvidedExtensionPoints" PropagatesCopy="PropagatesCopyToLinkAndOppositeRolePlayer" PropertyDisplayName="Provided Extension Points">
          <RolePlayer>
            <DomainClassMoniker Name="PatternSchema" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="28e7dfd9-4a29-473c-a0cb-74bd9b3b3c6a" Description="The owning pattern." Name="ProvidedExtensionPointSchema" DisplayName="Provided Extension Point Schema" PropertyName="Pattern" Multiplicity="One" PropagatesDelete="true" PropertyDisplayName="Pattern">
          <RolePlayer>
            <DomainClassMoniker Name="ProvidedExtensionPointSchema" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="936a92cc-4eec-4e05-84b5-4dca5b2845bd" Description="Description for Microsoft.VisualStudio.Patterning.Runtime.Schema.ElementHasExtensionPoints" Name="ElementHasExtensionPoints" DisplayName="Element Has Extension Points" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema" IsEmbedding="true">
      <Notes>Skip=true</Notes>
      <Properties>
        <DomainProperty Id="5852a2c3-b9c7-4833-9d7d-1374e473bd2d" Description="The number of instances of this extension, for each parent element." Name="Cardinality" DisplayName="Cardinality" DefaultValue="OneToOne" Category="Structure">
          <Type>
            <ExternalTypeMoniker Name="/Microsoft.VisualStudio.Patterning.Runtime/Cardinality" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="ad438cd3-81c7-45aa-ad4f-8199952a0831" Description="The displayed caption for the cardinality." Name="CardinalityCaption" DisplayName="Cardinality Caption" Kind="Calculated" Category="Structure" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="f0c1d459-a2fb-4ad1-87b2-af89eeecb9fc" Description="The group in which instances of the element/collection will be ordered together, relative to instances of sibling elements/collections. By default, all instances of all sibling elements/collections are ordered together alphabetically by their 'InstanceName' property." Name="OrderGroup" DisplayName="Order Group" DefaultValue="1" Category="Structure">
          <Type>
            <ExternalTypeMoniker Name="/System/Int32" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="3291230f-5c4d-41af-b574-0ddf27a169f2" Description="A System.Collections.IComparer class that determines how instances of all elements/collections in this 'Order Group' are ordered together. If left blank, then instances of all elements/collections are ordered together alphabetically by their 'InstanceName' property." Name="OrderGroupComparerTypeName" DisplayName="Order Group Comparer" Category="Structure">
          <Attributes>
            <ClrAttribute Name="System.ComponentModel.Editor">
              <Parameters>
                <AttributeParameter Value="typeof(Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Design.StandardValuesEditor)" />
                <AttributeParameter Value="typeof(System.Drawing.Design.UITypeEditor)" />
              </Parameters>
            </ClrAttribute>
            <ClrAttribute Name="System.ComponentModel.TypeConverter">
              <Parameters>
                <AttributeParameter Value="typeof(Microsoft.VisualStudio.Patterning.Extensibility.FullTypeTypeConverter&lt;System.Collections.IComparer&gt;)" />
              </Parameters>
            </ClrAttribute>
            <ClrAttribute Name="Microsoft.VisualStudio.Patterning.Runtime.Schema.CustomizableDomainElementSettingAttribute" />
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
      <Source>
        <DomainRole Id="398dc337-36b1-4259-a3b2-1a1a61db928a" Description="The child extension points of this element." Name="ParentElement" DisplayName="Parent Element" PropertyName="ExtensionPoints" PropagatesCopy="PropagatesCopyToLinkAndOppositeRolePlayer" PropertyDisplayName="Extension Points">
          <Notes>Skip=true</Notes>
          <RolePlayer>
            <DomainClassMoniker Name="AbstractElementSchema" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="1e701701-d288-458a-a579-10e1474d9b2a" Description="The owning element." Name="ChildElement" DisplayName="Child Element" PropertyName="Owner" Multiplicity="ZeroOne" PropagatesDelete="true" PropertyDisplayName="Owner">
          <RolePlayer>
            <DomainClassMoniker Name="ExtensionPointSchema" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="990a7838-e437-42ab-8bd6-2accfad82fc7" Description="Description for Microsoft.VisualStudio.Patterning.Runtime.Schema.ViewHasExtensionPoints" Name="ViewHasExtensionPoints" DisplayName="View Has Extension Points" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema" IsEmbedding="true">
      <Notes>Skip=true</Notes>
      <Properties>
        <DomainProperty Id="d3949f1f-1041-4a92-80b6-c17f6051119e" Description="The number of instances of this extension, for this view." Name="Cardinality" DisplayName="Cardinality" DefaultValue="OneToOne" Category="Structure">
          <Type>
            <ExternalTypeMoniker Name="/Microsoft.VisualStudio.Patterning.Runtime/Cardinality" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="fb7c5df4-ebb1-4680-8bf2-581947a61524" Description="The displayed caption for the cardinality." Name="CardinalityCaption" DisplayName="Cardinality Caption" Kind="Calculated" Category="Structure" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="5d525bd9-7baa-4d99-9d04-ddfbd58e70fe" Description="The group in which instances of the element/collection will be ordered together, relative to instances of sibling elements/collections. By default, all instances of all sibling elements/collections are ordered together alphabetically by their 'InstanceName' property." Name="OrderGroup" DisplayName="Order Group" DefaultValue="1" Category="Structure">
          <Type>
            <ExternalTypeMoniker Name="/System/Int32" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="98134d80-f742-4671-87db-e5c9380cca7e" Description="A System.Collections.IComparer class that determines how instances of all elements/collections in this 'Order Group' are ordered together. If left blank, then instances of all elements/collections are ordered together alphabetically by their 'InstanceName' property." Name="OrderGroupComparerTypeName" DisplayName="Order Group Comparer" Category="Structure">
          <Attributes>
            <ClrAttribute Name="System.ComponentModel.Editor">
              <Parameters>
                <AttributeParameter Value="typeof(Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Design.StandardValuesEditor)" />
                <AttributeParameter Value="typeof(System.Drawing.Design.UITypeEditor)" />
              </Parameters>
            </ClrAttribute>
            <ClrAttribute Name="System.ComponentModel.TypeConverter">
              <Parameters>
                <AttributeParameter Value="typeof(Microsoft.VisualStudio.Patterning.Extensibility.FullTypeTypeConverter&lt;System.Collections.IComparer&gt;)" />
              </Parameters>
            </ClrAttribute>
            <ClrAttribute Name="Microsoft.VisualStudio.Patterning.Runtime.Schema.CustomizableDomainElementSettingAttribute" />
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
      <Source>
        <DomainRole Id="f48796b2-8c83-41b9-beb8-8c98b4d99521" Description="The child extension points of the view." Name="ViewSchema" DisplayName="View Schema" PropertyName="ExtensionPoints" PropagatesCopy="PropagatesCopyToLinkAndOppositeRolePlayer" PropertyDisplayName="Extension Points">
          <Notes>Skip=true</Notes>
          <RolePlayer>
            <DomainClassMoniker Name="ViewSchema" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="4322ef66-06a1-48f4-a863-4f61fc52d4de" Description="The owning view." Name="ExtensionPointSchema" DisplayName="Extension Point Schema" PropertyName="View" Multiplicity="ZeroOne" PropagatesDelete="true" PropertyDisplayName="View">
          <Notes>Skip=true</Notes>
          <RolePlayer>
            <DomainClassMoniker Name="ExtensionPointSchema" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="66cfcfbd-f570-48a2-a9e7-a317c9779ca0" Description="Description for Microsoft.VisualStudio.Patterning.Runtime.Schema.PatternElementHasAutomationSettings" Name="PatternElementHasAutomationSettings" DisplayName="Pattern Element Has Automation Settings" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema" IsEmbedding="true">
      <Source>
        <DomainRole Id="5f06d1b0-aba7-4689-8c74-e92735685207" Description="The automation settings of this element." Name="PatternElementSchema" DisplayName="Pattern Element Schema" PropertyName="AutomationSettings" PropagatesCopy="PropagatesCopyToLinkAndOppositeRolePlayer" PropertyDisplayName="Automation Settings">
          <RolePlayer>
            <DomainClassMoniker Name="PatternElementSchema" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="1740ff91-ccb8-4f9f-a970-88ea0cae783c" Description="The owning element." Name="AutomationSettingsSchema" DisplayName="Automation Settings Schema" PropertyName="Owner" Multiplicity="One" PropagatesDelete="true" PropertyDisplayName="Owner">
          <Notes>Skip=true</Notes>
          <RolePlayer>
            <DomainClassMoniker Name="AutomationSettingsSchema" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
  </Relationships>
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
    <ExternalType Name="CustomizationState" Namespace="Microsoft.VisualStudio.Patterning.Runtime" />
    <ExternalType Name="CustomizedLevel" Namespace="Microsoft.VisualStudio.Patterning.Runtime" />
    <ExternalType Name="Color" Namespace="System.Drawing" />
    <ExternalType Name="Cardinality" Namespace="Microsoft.VisualStudio.Patterning.Runtime" />
    <ExternalType Name="Image" Namespace="System.Drawing" />
    <DomainEnumeration Name="CustomizationEnabledState" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema" AccessModifier="Assembly" Description="The combined state of IsEnabled and IsCustomizable.">
      <Literals>
        <EnumerationLiteral Description="Description for Microsoft.VisualStudio.Patterning.Runtime.Schema.CustomizationEnabledState.FalseEnabled" Name="FalseEnabled" Value="" />
        <EnumerationLiteral Description="Description for Microsoft.VisualStudio.Patterning.Runtime.Schema.CustomizationEnabledState.FalseDisabled" Name="FalseDisabled" Value="" />
        <EnumerationLiteral Description="Description for Microsoft.VisualStudio.Patterning.Runtime.Schema.CustomizationEnabledState.InheritedEnabled" Name="InheritedEnabled" Value="" />
        <EnumerationLiteral Description="Description for Microsoft.VisualStudio.Patterning.Runtime.Schema.CustomizationEnabledState.TrueDisabled" Name="TrueDisabled" Value="" />
        <EnumerationLiteral Description="Description for Microsoft.VisualStudio.Patterning.Runtime.Schema.CustomizationEnabledState.TrueEnabled" Name="TrueEnabled" Value="" />
        <EnumerationLiteral Description="Description for Microsoft.VisualStudio.Patterning.Runtime.Schema.CustomizationEnabledState.InheritedDisabled" Name="InheritedDisabled" Value="" />
      </Literals>
    </DomainEnumeration>
    <DomainEnumeration Name="CustomizableDomainElementSettingType" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema" AccessModifier="Assembly" Description="The kinds of domain element that has been marked as customizable.">
      <Literals>
        <EnumerationLiteral Description="The domain property is customizable as a whole." Name="DomainProperty" Value="" />
        <EnumerationLiteral Description="The domain role, and all its child elements are customizable as a whole." Name="DomainRole" Value="" />
      </Literals>
    </DomainEnumeration>
    <ExternalType Name="AutomationSettingsClassification" Namespace="Microsoft.VisualStudio.Patterning.Runtime" />
    <ExternalType Name="PropertyUsages" Namespace="Microsoft.VisualStudio.Patterning.Runtime" />
    <ExternalType Name="IBindingSettings" Namespace="Microsoft.VisualStudio.Patterning.Runtime" />
    <ExternalType Name="NamedElementSchemaNameProvider" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema" />
  </Types>
  <Shapes>
    <CompartmentShape Id="6d4c3263-f5e1-49e0-b1be-d7188c1157eb" Description="Description for Microsoft.VisualStudio.Patterning.Runtime.Schema.PatternShape" Name="PatternShape" DisplayName="Pattern Shape" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema" GeneratesDoubleDerived="true" FixedTooltipText="Pattern" OutlineColor="DarkGray" InitialWidth="2" InitialHeight="0.5" OutlineThickness="0.01" FillGradientMode="None" Geometry="Rectangle">
      <BaseCompartmentShape>
        <CompartmentShapeMoniker Name="CustomizableElementShape" />
      </BaseCompartmentShape>
      <Properties>
        <DomainProperty Id="c4273198-78d4-4def-8027-41a30b87fc8c" Description="The color of the shape fill, in tailoring mode." Name="TailoringFillColor" DisplayName="Tailoring Fill Color" DefaultValue="White" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System.Drawing/Color" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="9bc3bbc5-06c2-4609-add2-f84dbfce9e9e" Description="The color of the shape text, in tailoring mode." Name="TailoringTextColor" DisplayName="Tailoring Text Color" DefaultValue="DimGray" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System.Drawing/Color" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="8cdfb709-3a79-4a53-a1f8-db4bc6e78ecb" Description="The color of the shape outline, in tailoring mode." Name="TailoringOutlineColor" DisplayName="Tailoring Outline Color" DefaultValue="DarkGray" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System.Drawing/Color" />
          </Type>
        </DomainProperty>
      </Properties>
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0.225" VerticalOffset="0">
        <TextDecorator Name="NameDecorator" DisplayName="Name Decorator" DefaultText="NameDecorator" FontStyle="Bold" FontSize="9" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0.225" VerticalOffset="0.2">
        <TextDecorator Name="StereotypeDecorator" DisplayName="Stereotype Decorator" DefaultText="The Pattern" FontStyle="Italic" FontSize="7" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopRight" HorizontalOffset="0" VerticalOffset="0">
        <ExpandCollapseDecorator Name="ExpandCollapseDecorator" DisplayName="Expand Collapse Decorator" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0.0225" VerticalOffset="-0.025">
        <IconDecorator Name="InheritedFromBaseDecorator" DisplayName="Inherited From Base Decorator" DefaultIcon="..\..\..\Authoring\Source\ToolkitDesign.Shell\Resources\Inherited.png" />
      </ShapeHasDecorators>
      <Compartment Name="Properties" Title="Variable Properties" />
      <Compartment TitleFillColor="234, 234, 234" Name="LaunchPoints" Title="Launch Points" />
      <Compartment TitleFillColor="234, 234, 234" Name="Automation" Title="Automation" />
    </CompartmentShape>
    <CompartmentShape Id="c5328ad8-6b7c-4688-97aa-dd2e25d8b1fc" Description="Description for Microsoft.VisualStudio.Patterning.Runtime.Schema.ElementShape" Name="ElementShape" DisplayName="Element Shape" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema" GeneratesDoubleDerived="true" FixedTooltipText="Variability Element" FillColor="165, 198, 165" OutlineColor="White" InitialWidth="1.6" InitialHeight="0.5" OutlineThickness="0.03" FillGradientMode="None" Geometry="RoundedRectangle">
      <BaseCompartmentShape>
        <CompartmentShapeMoniker Name="CustomizableElementShape" />
      </BaseCompartmentShape>
      <Properties>
        <DomainProperty Id="2d92e1cc-b8c0-4ade-a8ec-23876c40a6f3" Description="The color of the shape fill, in tailoring mode." Name="TailoringFillColor" DisplayName="Tailoring Fill Color" DefaultValue="211, 221, 210" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System.Drawing/Color" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="de1702e6-2253-4e30-b2b1-e9d164232a8b" Description="The color of the shape text, in tailoring mode." Name="TailoringTextColor" DisplayName="Tailoring Text Color" DefaultValue="DimGray" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System.Drawing/Color" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="85c5f12a-156c-4274-8355-0d30bfaa71e8" Description="The color of the shape outline, in tailoring mode." Name="TailoringOutlineColor" DisplayName="Tailoring Outline Color" DefaultValue="226, 226, 226" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System.Drawing/Color" />
          </Type>
        </DomainProperty>
      </Properties>
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0.225" VerticalOffset="0.05">
        <TextDecorator Name="NameDecorator" DisplayName="Name Decorator" DefaultText="NameDecorator" FontStyle="Bold" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0.225" VerticalOffset="0.2">
        <TextDecorator Name="StereotypeDecorator" DisplayName="Stereotype Decorator" DefaultText="Element" FontSize="7" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0.0225" VerticalOffset="0">
        <IconDecorator Name="InheritedFromBaseDecorator" DisplayName="Inherited From Base Decorator" DefaultIcon="..\..\..\Authoring\Source\ToolkitDesign.Shell\Resources\Inherited.png" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopRight" HorizontalOffset="0" VerticalOffset="0">
        <ExpandCollapseDecorator Name="ExpandCollapseDecorator" DisplayName="Expand Collapse Decorator" />
      </ShapeHasDecorators>
      <Compartment Name="Properties" Title="Variable Properties" />
      <Compartment TitleFillColor="234, 234, 234" Name="LaunchPoints" Title="Launch Points" />
      <Compartment TitleFillColor="234, 234, 234" Name="Automation" Title="Automation" />
    </CompartmentShape>
    <Port Id="bc4bc41e-9104-45be-b98b-ee0caf55331f" Description="Description for Microsoft.VisualStudio.Patterning.Runtime.Schema.ViewShape" Name="ViewShape" DisplayName="View Shape" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema" GeneratesDoubleDerived="true" FixedTooltipText="View Shape" FillColor="WhiteSmoke" OutlineColor="DarkGray" InitialWidth="2" InitialHeight="0.25" OutlineThickness="0.01" FillGradientMode="ForwardDiagonal" Geometry="Rectangle">
      <Properties>
        <DomainProperty Id="ee96117e-2f48-4493-9c53-b7bc52fb7dbe" Description="The color of the shape fill, in tailoring mode." Name="TailoringFillColor" DisplayName="Tailoring Fill Color" DefaultValue="WhiteSmoke" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System.Drawing/Color" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="e7349707-c712-4bef-a81a-0e926ecb925f" Description="The color of the shape text, in tailoring mode." Name="TailoringTextColor" DisplayName="Tailoring Text Color" DefaultValue="DarkGray" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System.Drawing/Color" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="fa2d5c11-1a80-438d-b757-9bb14200d098" Description="The color of the shape outline, in tailoring mode." Name="TailoringOutlineColor" DisplayName="Tailoring Outline Color" DefaultValue="DarkGray" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System.Drawing/Color" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="d8291028-a419-457b-aa23-6b3244ed9f5f" Description="Whether the layout of the view initialized the very first time." Name="IsLayoutInitialized" DisplayName="Is Layout Initialized" DefaultValue="False" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
      </Properties>
      <ShapeHasDecorators Position="Center" HorizontalOffset="0" VerticalOffset="0">
        <TextDecorator Name="NameDecorator" DisplayName="Name Decorator" DefaultText="NameDecorator" FontStyle="Bold, Italic" FontSize="10" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0.0225" VerticalOffset="-0.025">
        <IconDecorator Name="InheritedFromBaseDecorator" DisplayName="Inherited From Base Decorator" DefaultIcon="..\..\..\Authoring\Source\ToolkitDesign.Shell\Resources\Inherited.png" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0.1925" VerticalOffset="-0.025">
        <IconDecorator Name="CustomizationTrueEnabledDecorator" DisplayName="Customization True Enabled Decorator" DefaultIcon="..\..\..\Authoring\Source\ToolkitDesign.Shell\Resources\CustomizationTrueEnabled.png" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0.1925" VerticalOffset="-0.025">
        <IconDecorator Name="CustomizationTrueDisabledDecorator" DisplayName="Customization True Disabled Decorator" DefaultIcon="..\..\..\Authoring\Source\ToolkitDesign.Shell\Resources\CustomizationTrueDisabled.png" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0.1925" VerticalOffset="-0.025">
        <IconDecorator Name="CustomizationFalseEnabledDecorator" DisplayName="Customization False Enabled Decorator" DefaultIcon="..\..\..\Authoring\Source\ToolkitDesign.Shell\Resources\CustomizationFalseEnabled.png" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0.1925" VerticalOffset="-0.025">
        <IconDecorator Name="CustomizationFalseDisabledDecorator" DisplayName="Customization False Disabled Decorator" DefaultIcon="..\..\..\Authoring\Source\ToolkitDesign.Shell\Resources\CustomizationFalseDisabled.png" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0.1925" VerticalOffset="-0.025">
        <IconDecorator Name="CustomizationInheritedEnabledDecorator" DisplayName="Customization Inherited Enabled Decorator" DefaultIcon="..\..\..\Authoring\Source\ToolkitDesign.Shell\Resources\CustomizationInheritedEnabled.png" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0.1925" VerticalOffset="-0.025">
        <IconDecorator Name="CustomizationInheritedDisabledDecorator" DisplayName="Customization Inherited Disabled Decorator" DefaultIcon="..\..\..\Authoring\Source\ToolkitDesign.Shell\Resources\CustomizationInheritedDisabled.png" />
      </ShapeHasDecorators>
    </Port>
    <CompartmentShape Id="33005d46-3de2-402a-bca3-9b7e89fa8486" Description="Description for Microsoft.VisualStudio.Patterning.Runtime.Schema.CollectionShape" Name="CollectionShape" DisplayName="Collection Shape" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema" GeneratesDoubleDerived="true" FixedTooltipText="Variable Collection" FillColor="255, 216, 98" OutlineColor="White" InitialWidth="1.6" InitialHeight="0.5" OutlineThickness="0.03" FillGradientMode="None" Geometry="RoundedRectangle" DefaultExpandCollapseState="Collapsed">
      <BaseCompartmentShape>
        <CompartmentShapeMoniker Name="CustomizableElementShape" />
      </BaseCompartmentShape>
      <Properties>
        <DomainProperty Id="cbe9c413-2d93-4670-ae4b-39e073219eba" Description="The color of the shape fill, in tailoring mode." Name="TailoringFillColor" DisplayName="Tailoring Fill Color" DefaultValue="255, 233, 164" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System.Drawing/Color" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="20c46e3f-c7bf-496b-a6f5-d21805324326" Description="The color of the shape text, in tailoring mode." Name="TailoringTextColor" DisplayName="Tailoring Text Color" DefaultValue="DimGray" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System.Drawing/Color" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="b16dad84-64a6-4685-99bc-aa63a58b42f0" Description="The color of the shape outline, in tailoring mode." Name="TailoringOutlineColor" DisplayName="Tailoring Outline Color" DefaultValue="234, 234, 234" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System.Drawing/Color" />
          </Type>
        </DomainProperty>
      </Properties>
      <ShapeHasDecorators Position="InnerTopRight" HorizontalOffset="0" VerticalOffset="0.15">
        <ExpandCollapseDecorator Name="ExpandCollapseDecorator" DisplayName="Expand Collapse Decorator" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0.225" VerticalOffset="0.175">
        <TextDecorator Name="NameDecorator" DisplayName="Name Decorator" DefaultText="NameDecorator" FontStyle="Bold" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0.225" VerticalOffset="0.02">
        <TextDecorator Name="StereotypeDecorator" DisplayName="Stereotype Decorator" DefaultText="Collection" FontSize="7" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0.0225" VerticalOffset="-0.025">
        <IconDecorator Name="InheritedFromBaseDecorator" DisplayName="Inherited From Base Decorator" DefaultIcon="..\..\..\Authoring\Source\ToolkitDesign.Shell\Resources\Inherited.png" />
      </ShapeHasDecorators>
      <Compartment TitleFillColor="234, 234, 234" Name="Properties" Title="Variable Properties" />
      <Compartment TitleFillColor="234, 234, 234" Name="LaunchPoints" Title="Launch Points" />
      <Compartment TitleFillColor="234, 234, 234" Name="Automation" Title="Automation" />
    </CompartmentShape>
    <CompartmentShape Id="c7199c01-4aad-496c-b124-f7016fba3749" Description="Description for Microsoft.VisualStudio.Patterning.Runtime.Schema.CustomizableElementShape" Name="CustomizableElementShape" DisplayName="Customizable Element Shape" InheritanceModifier="Abstract" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema" FixedTooltipText="Customizable Element Shape" InitialHeight="1.1" Geometry="Rectangle">
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0.0225" VerticalOffset="0.1475">
        <IconDecorator Name="CustomizationTrueEnabledDecorator" DisplayName="Customization True Enabled Decorator" DefaultIcon="..\..\..\Authoring\Source\ToolkitDesign.Shell\Resources\CustomizationTrueEnabled.png" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0.0225" VerticalOffset="0.1475">
        <IconDecorator Name="CustomizationTrueDisabledDecorator" DisplayName="Customization True Disabled Decorator" DefaultIcon="..\..\..\Authoring\Source\ToolkitDesign.Shell\Resources\CustomizationTrueDisabled.png" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0.0225" VerticalOffset="0.1475">
        <IconDecorator Name="CustomizationFalseEnabledDecorator" DisplayName="Customization False Enabled Decorator" DefaultIcon="..\..\..\Authoring\Source\ToolkitDesign.Shell\Resources\CustomizationFalseEnabled.png" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0.0225" VerticalOffset="0.1475">
        <IconDecorator Name="CustomizationFalseDisabledDecorator" DisplayName="Customization False Disabled Decorator" DefaultIcon="..\..\..\Authoring\Source\ToolkitDesign.Shell\Resources\CustomizationFalseDisabled.png" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0.0225" VerticalOffset="0.1475">
        <IconDecorator Name="CustomizationInheritedEnabledDecorator" DisplayName="Customization Inherited Enabled Decorator" DefaultIcon="..\..\..\Authoring\Source\ToolkitDesign.Shell\Resources\CustomizationInheritedEnabled.png" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0.0225" VerticalOffset="0.1475">
        <IconDecorator Name="CustomizationInheritedDisabledDecorator" DisplayName="Customization Inherited Disabled Decorator" DefaultIcon="..\..\..\Authoring\Source\ToolkitDesign.Shell\Resources\CustomizationInheritedDisabled.png" />
      </ShapeHasDecorators>
    </CompartmentShape>
    <CompartmentShape Id="268f9015-d395-4b5c-bc4f-2f671b82e04d" Description="Description for Microsoft.VisualStudio.Patterning.Runtime.Schema.ExtensionPointShape" Name="ExtensionPointShape" DisplayName="Extension Point Shape" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema" GeneratesDoubleDerived="true" FixedTooltipText="Extension Point" FillColor="153, 204, 205" OutlineColor="White" InitialWidth="1.6" InitialHeight="0.5" OutlineThickness="0.03" FillGradientMode="None" Geometry="RoundedRectangle">
      <BaseCompartmentShape>
        <CompartmentShapeMoniker Name="CustomizableElementShape" />
      </BaseCompartmentShape>
      <Properties>
        <DomainProperty Id="17d080a8-2ae9-429f-92d1-318296729bb1" Description="The color of the shape fill, in tailoring mode." Name="TailoringFillColor" DisplayName="Tailoring Fill Color" DefaultValue="168, 186, 186" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System.Drawing/Color" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="b2e98661-8bf0-4ef7-b40f-9f778e98d854" Description="The color of the shape text, in tailoring mode." Name="TailoringTextColor" DisplayName="Tailoring Text Color" DefaultValue="DimGray" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System.Drawing/Color" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="2e36dbc2-8ae7-4abd-b5e2-9b77243b62d2" Description="The color of the shape outline, in tailoring mode." Name="TailoringOutlineColor" DisplayName="Tailoring Outline Color" DefaultValue="226, 226, 226" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System.Drawing/Color" />
          </Type>
        </DomainProperty>
      </Properties>
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0.225" VerticalOffset="0.05">
        <TextDecorator Name="NameDecorator" DisplayName="Name Decorator" DefaultText="NameDecorator" FontStyle="Bold" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0.225" VerticalOffset="0.2">
        <TextDecorator Name="StereotypeDecorator" DisplayName="Stereotype Decorator" DefaultText="Extension Point" FontSize="7" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0.0225" VerticalOffset="0">
        <IconDecorator Name="InheritedFromBaseDecorator" DisplayName="Inherited From Base Decorator" DefaultIcon="..\..\..\Authoring\Source\ToolkitDesign.Shell\Resources\Inherited.png" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopRight" HorizontalOffset="0" VerticalOffset="0">
        <ExpandCollapseDecorator Name="ExpandCollapseDecorator" DisplayName="Expand Collapse Decorator" />
      </ShapeHasDecorators>
      <Compartment Name="Properties" Title="Variable Properties" />
    </CompartmentShape>
  </Shapes>
  <Connectors>
    <Connector Id="4ab6824f-a9cf-45a6-9ac6-5a65b60fe5a0" Description="Description for Microsoft.VisualStudio.Patterning.Runtime.Schema.ViewHasElementsConnector" Name="ViewHasElementsConnector" DisplayName="View Has Elements Connector" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema" FixedTooltipText="View Has Elements Connector">
      <BaseConnector>
        <ConnectorMoniker Name="PatternElementConnector" />
      </BaseConnector>
      <ConnectorHasDecorators Position="TargetTop" OffsetFromShape="0" OffsetFromLine="0" isMoveable="true">
        <TextDecorator Name="CardinalityDecorator" DisplayName="Cardinality Decorator" DefaultText="" />
      </ConnectorHasDecorators>
    </Connector>
    <Connector Id="d880db25-406b-41e8-b8ec-0a0de1e3ac5d" Description="Description for Microsoft.VisualStudio.Patterning.Runtime.Schema.ElementHasElementsConnector" Name="ElementHasElementsConnector" DisplayName="Element Has Elements Connector" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema" FixedTooltipText="Element Has Elements Connector">
      <BaseConnector>
        <ConnectorMoniker Name="PatternElementConnector" />
      </BaseConnector>
      <ConnectorHasDecorators Position="TargetTop" OffsetFromShape="0" OffsetFromLine="0" isMoveable="true">
        <TextDecorator Name="CardinalityDecorator" DisplayName="Cardinality Decorator" DefaultText="" />
      </ConnectorHasDecorators>
    </Connector>
    <Connector Id="147c9db8-7ff8-4426-a8f7-2ffa38cc6bad" Description="Description for Microsoft.VisualStudio.Patterning.Runtime.Schema.PatternElementConnector" Name="PatternElementConnector" DisplayName="Pattern Element Connector" InheritanceModifier="Abstract" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema" GeneratesDoubleDerived="true" FixedTooltipText="Pattern Element Connector" TextColor="102, 102, 102" Color="102, 102, 102" SourceEndStyle="FilledDiamond" Thickness="0.03">
      <Properties>
        <DomainProperty Id="7b3d245c-8d9d-4130-be33-bce008d2d047" Description="The color of the connector, in tailoring mode." Name="TailoringOutlineColor" DisplayName="Tailoring Outline Color" DefaultValue="178,178,178" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System.Drawing/Color" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="ef26b1f4-2a67-4898-bd96-46891b8eb35f" Description="The color of the connector text, in tailoring mode." Name="TailoringTextColor" DisplayName="Tailoring Text Color" DefaultValue="137,137,137" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System.Drawing/Color" />
          </Type>
        </DomainProperty>
      </Properties>
    </Connector>
    <Connector Id="5ff64aa7-63bd-4ae6-8395-fc955f077c42" Description="Description for Microsoft.VisualStudio.Patterning.Runtime.Schema.ElementHasExtensionPointsConnector" Name="ElementHasExtensionPointsConnector" DisplayName="Element Has Extension Points Connector" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema" FixedTooltipText="Element Has Extension Points Connector">
      <BaseConnector>
        <ConnectorMoniker Name="PatternElementConnector" />
      </BaseConnector>
      <ConnectorHasDecorators Position="TargetTop" OffsetFromShape="0" OffsetFromLine="0" isMoveable="true">
        <TextDecorator Name="CardinalityDecorator" DisplayName="Cardinality Decorator" DefaultText="" />
      </ConnectorHasDecorators>
    </Connector>
    <Connector Id="54c171e1-89d9-4023-89a6-8cddb13bda44" Description="Description for Microsoft.VisualStudio.Patterning.Runtime.Schema.ViewHasExtensionPointsConnector" Name="ViewHasExtensionPointsConnector" DisplayName="View Has Extension Points Connector" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema" FixedTooltipText="View Has Extension Points Connector">
      <BaseConnector>
        <ConnectorMoniker Name="PatternElementConnector" />
      </BaseConnector>
      <ConnectorHasDecorators Position="TargetTop" OffsetFromShape="0" OffsetFromLine="0" isMoveable="true">
        <TextDecorator Name="CardinalityDecorator" DisplayName="Cardinality Decorator" DefaultText="" />
      </ConnectorHasDecorators>
    </Connector>
  </Connectors>
  <XmlSerializationBehavior Name="PatternModelSerializationBehavior" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema">
    <ClassData>
      <XmlClassData TypeName="PatternModelSchema" MonikerAttributeName="" SerializeId="true" MonikerElementName="patternModelSchemaMoniker" ElementName="patternModel" MonikerTypeName="PatternModelSchemaMoniker">
        <DomainClassMoniker Name="PatternModelSchema" />
        <ElementData>
          <XmlRelationshipData OmitElement="true" RoleElementName="pattern">
            <DomainRelationshipMoniker Name="PatternModelHasPattern" />
          </XmlRelationshipData>
          <XmlPropertyData XmlName="baseVersion">
            <DomainPropertyMoniker Name="PatternModelSchema/BaseVersion" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="baseId">
            <DomainPropertyMoniker Name="PatternModelSchema/BaseId" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="PatternSchema" MonikerAttributeName="" SerializeId="true" MonikerElementName="patternSchemaMoniker" ElementName="pattern" MonikerTypeName="PatternSchemaMoniker">
        <DomainClassMoniker Name="PatternSchema" />
        <ElementData>
          <XmlPropertyData XmlName="extensionId" Representation="Ignore">
            <DomainPropertyMoniker Name="PatternSchema/ExtensionId" />
          </XmlPropertyData>
          <XmlRelationshipData RoleElementName="views">
            <DomainRelationshipMoniker Name="PatternHasViews" />
          </XmlRelationshipData>
          <XmlPropertyData XmlName="currentDiagramId">
            <DomainPropertyMoniker Name="PatternSchema/CurrentDiagramId" />
          </XmlPropertyData>
          <XmlRelationshipData RoleElementName="providedExtensionPoints">
            <DomainRelationshipMoniker Name="PatternHasProvidedExtensionPoints" />
          </XmlRelationshipData>
          <XmlPropertyData XmlName="patternLink">
            <DomainPropertyMoniker Name="PatternSchema/PatternLink" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="PatternModelHasPattern" MonikerAttributeName="" MonikerElementName="patternModelHasPatternMoniker" ElementName="patternModelHasPattern" MonikerTypeName="PatternModelHasPatternMoniker">
        <DomainRelationshipMoniker Name="PatternModelHasPattern" />
      </XmlClassData>
      <XmlClassData TypeName="PatternModelSchemaDiagram" MonikerAttributeName="" MonikerElementName="patternModelSchemaDiagramMoniker" ElementName="patternModelDiagram" MonikerTypeName="PatternModelSchemaDiagramMoniker">
        <DiagramMoniker Name="PatternModelSchemaDiagram" />
        <ElementData>
          <XmlPropertyData XmlName="authoringBackgroundColor">
            <DomainPropertyMoniker Name="PatternModelSchemaDiagram/AuthoringBackgroundColor" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="tailoringBackgroundColor">
            <DomainPropertyMoniker Name="PatternModelSchemaDiagram/TailoringBackgroundColor" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="authoringGradientColor">
            <DomainPropertyMoniker Name="PatternModelSchemaDiagram/AuthoringGradientColor" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="tailoringGradientColor">
            <DomainPropertyMoniker Name="PatternModelSchemaDiagram/TailoringGradientColor" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="authoringTitleTextColor">
            <DomainPropertyMoniker Name="PatternModelSchemaDiagram/AuthoringTitleTextColor" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="tailoringTitleTextColor">
            <DomainPropertyMoniker Name="PatternModelSchemaDiagram/TailoringTitleTextColor" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="showHiddenEntries">
            <DomainPropertyMoniker Name="PatternModelSchemaDiagram/ShowHiddenEntries" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="NamedElementSchema" MonikerAttributeName="" MonikerElementName="namedElementSchemaMoniker" ElementName="namedElementSchema" MonikerTypeName="NamedElementSchemaMoniker">
        <DomainClassMoniker Name="NamedElementSchema" />
        <ElementData>
          <XmlPropertyData XmlName="name">
            <DomainPropertyMoniker Name="NamedElementSchema/Name" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="baseId">
            <DomainPropertyMoniker Name="NamedElementSchema/BaseId" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="displayName">
            <DomainPropertyMoniker Name="NamedElementSchema/DisplayName" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="description">
            <DomainPropertyMoniker Name="NamedElementSchema/Description" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="isInheritedFromBase" Representation="Ignore">
            <DomainPropertyMoniker Name="NamedElementSchema/IsInheritedFromBase" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="isDisplayNameTracking" Representation="Ignore">
            <DomainPropertyMoniker Name="NamedElementSchema/IsDisplayNameTracking" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="schemaPath" Representation="Ignore">
            <DomainPropertyMoniker Name="NamedElementSchema/SchemaPath" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="isDescriptionTracking" Representation="Ignore">
            <DomainPropertyMoniker Name="NamedElementSchema/IsDescriptionTracking" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="isSystem">
            <DomainPropertyMoniker Name="NamedElementSchema/IsSystem" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="codeIdentifier">
            <DomainPropertyMoniker Name="NamedElementSchema/CodeIdentifier" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="isCodeIdentifierTracking" Representation="Ignore">
            <DomainPropertyMoniker Name="NamedElementSchema/IsCodeIdentifierTracking" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="PatternElementSchema" MonikerAttributeName="" MonikerElementName="patternElementSchemaMoniker" ElementName="patternElementSchema" MonikerTypeName="PatternElementSchemaMoniker">
        <DomainClassMoniker Name="PatternElementSchema" />
        <ElementData>
          <XmlRelationshipData RoleElementName="properties">
            <DomainRelationshipMoniker Name="PatternElementHasProperties" />
          </XmlRelationshipData>
          <XmlPropertyData XmlName="validationRules" Representation="Element">
            <DomainPropertyMoniker Name="PatternElementSchema/ValidationRules" />
          </XmlPropertyData>
          <XmlRelationshipData RoleElementName="automationSettings">
            <DomainRelationshipMoniker Name="PatternElementHasAutomationSettings" />
          </XmlRelationshipData>
          <XmlPropertyData XmlName="icon">
            <DomainPropertyMoniker Name="PatternElementSchema/Icon" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="PropertySchema" MonikerAttributeName="" SerializeId="true" MonikerElementName="propertySchemaMoniker" ElementName="property" MonikerTypeName="PropertySchemaMoniker">
        <DomainClassMoniker Name="PropertySchema" />
        <ElementData>
          <XmlPropertyData XmlName="defaultValue" Representation="Element">
            <DomainPropertyMoniker Name="PropertySchema/RawDefaultValue" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="type">
            <DomainPropertyMoniker Name="PropertySchema/Type" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="isVisible">
            <DomainPropertyMoniker Name="PropertySchema/IsVisible" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="isReadOnly">
            <DomainPropertyMoniker Name="PropertySchema/IsReadOnly" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="category">
            <DomainPropertyMoniker Name="PropertySchema/Category" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="propertyUsage">
            <DomainPropertyMoniker Name="PropertySchema/PropertyUsage" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="typeConverterTypeName">
            <DomainPropertyMoniker Name="PropertySchema/TypeConverterTypeName" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="editorTypeName">
            <DomainPropertyMoniker Name="PropertySchema/EditorTypeName" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="validationRules" Representation="Element">
            <DomainPropertyMoniker Name="PropertySchema/RawValidationRules" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="valueProvider" Representation="Element">
            <DomainPropertyMoniker Name="PropertySchema/RawValueProvider" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="PatternElementHasProperties" MonikerAttributeName="" MonikerElementName="patternElementHasPropertiesMoniker" ElementName="patternElementHasProperties" MonikerTypeName="PatternElementHasPropertiesMoniker">
        <DomainRelationshipMoniker Name="PatternElementHasProperties" />
      </XmlClassData>
      <XmlClassData TypeName="PatternShape" MonikerAttributeName="" MonikerElementName="patternShapeMoniker" ElementName="patternShape" MonikerTypeName="PatternShapeMoniker">
        <CompartmentShapeMoniker Name="PatternShape" />
        <ElementData>
          <XmlPropertyData XmlName="tailoringFillColor">
            <DomainPropertyMoniker Name="PatternShape/TailoringFillColor" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="tailoringTextColor">
            <DomainPropertyMoniker Name="PatternShape/TailoringTextColor" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="tailoringOutlineColor">
            <DomainPropertyMoniker Name="PatternShape/TailoringOutlineColor" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ViewSchema" MonikerAttributeName="" SerializeId="true" MonikerElementName="viewSchemaMoniker" ElementName="view" MonikerTypeName="ViewSchemaMoniker">
        <DomainClassMoniker Name="ViewSchema" />
        <ElementData>
          <XmlPropertyData XmlName="isVisible">
            <DomainPropertyMoniker Name="ViewSchema/IsVisible" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="isDefault">
            <DomainPropertyMoniker Name="ViewSchema/IsDefault" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="diagramId">
            <DomainPropertyMoniker Name="ViewSchema/DiagramId" />
          </XmlPropertyData>
          <XmlRelationshipData OmitElement="true" UseFullForm="true" RoleElementName="elements">
            <DomainRelationshipMoniker Name="ViewHasElements" />
          </XmlRelationshipData>
          <XmlPropertyData XmlName="caption" Representation="Ignore">
            <DomainPropertyMoniker Name="ViewSchema/Caption" />
          </XmlPropertyData>
          <XmlRelationshipData OmitElement="true" UseFullForm="true" RoleElementName="extensionPoints">
            <DomainRelationshipMoniker Name="ViewHasExtensionPoints" />
          </XmlRelationshipData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="PatternHasViews" MonikerAttributeName="" MonikerElementName="patternHasViewsMoniker" ElementName="patternHasViews" MonikerTypeName="PatternHasViewsMoniker">
        <DomainRelationshipMoniker Name="PatternHasViews" />
      </XmlClassData>
      <XmlClassData TypeName="CollectionSchema" MonikerAttributeName="" SerializeId="true" MonikerElementName="collectionSchemaMoniker" ElementName="collection" MonikerTypeName="CollectionSchemaMoniker">
        <DomainClassMoniker Name="CollectionSchema" />
      </XmlClassData>
      <XmlClassData TypeName="ElementSchema" MonikerAttributeName="" SerializeId="true" MonikerElementName="elementSchemaMoniker" ElementName="element" MonikerTypeName="ElementSchemaMoniker">
        <DomainClassMoniker Name="ElementSchema" />
      </XmlClassData>
      <XmlClassData TypeName="ElementShape" MonikerAttributeName="" MonikerElementName="elementShapeMoniker" ElementName="elementShape" MonikerTypeName="ElementShapeMoniker">
        <CompartmentShapeMoniker Name="ElementShape" />
        <ElementData>
          <XmlPropertyData XmlName="tailoringFillColor">
            <DomainPropertyMoniker Name="ElementShape/TailoringFillColor" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="tailoringTextColor">
            <DomainPropertyMoniker Name="ElementShape/TailoringTextColor" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="tailoringOutlineColor">
            <DomainPropertyMoniker Name="ElementShape/TailoringOutlineColor" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="CustomizableElementSchema" MonikerAttributeName="" MonikerElementName="customizableElementSchemaMoniker" ElementName="customizableElementSchema" MonikerTypeName="CustomizableElementSchemaMoniker">
        <DomainClassMoniker Name="CustomizableElementSchema" />
        <ElementData>
          <XmlRelationshipData OmitElement="true" RoleElementName="policy">
            <DomainRelationshipMoniker Name="CustomizableElementHasPolicy" />
          </XmlRelationshipData>
          <XmlPropertyData XmlName="isCustomizable">
            <DomainPropertyMoniker Name="CustomizableElementSchema/IsCustomizable" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="isCustomizationEnabled">
            <DomainPropertyMoniker Name="CustomizableElementSchema/IsCustomizationEnabled" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="isCustomizationPolicyModifyable" Representation="Ignore">
            <DomainPropertyMoniker Name="CustomizableElementSchema/IsCustomizationPolicyModifyable" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="isCustomizationEnabledState" Representation="Ignore">
            <DomainPropertyMoniker Name="CustomizableElementSchema/IsCustomizationEnabledState" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ViewHasElementsConnector" MonikerAttributeName="" MonikerElementName="viewHasElementsConnectorMoniker" ElementName="viewHasElementsConnector" MonikerTypeName="ViewHasElementsConnectorMoniker">
        <ConnectorMoniker Name="ViewHasElementsConnector" />
      </XmlClassData>
      <XmlClassData TypeName="ViewShape" MonikerAttributeName="" MonikerElementName="viewShapeMoniker" ElementName="viewShape" MonikerTypeName="ViewShapeMoniker">
        <PortMoniker Name="ViewShape" />
        <ElementData>
          <XmlPropertyData XmlName="tailoringFillColor">
            <DomainPropertyMoniker Name="ViewShape/TailoringFillColor" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="tailoringTextColor">
            <DomainPropertyMoniker Name="ViewShape/TailoringTextColor" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="tailoringOutlineColor">
            <DomainPropertyMoniker Name="ViewShape/TailoringOutlineColor" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="isLayoutInitialized">
            <DomainPropertyMoniker Name="ViewShape/IsLayoutInitialized" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="AbstractElementSchema" MonikerAttributeName="" MonikerElementName="abstractElementSchemaMoniker" ElementName="abstractElement" MonikerTypeName="AbstractElementSchemaMoniker">
        <DomainClassMoniker Name="AbstractElementSchema" />
        <ElementData>
          <XmlPropertyData XmlName="isVisible">
            <DomainPropertyMoniker Name="AbstractElementSchema/IsVisible" />
          </XmlPropertyData>
          <XmlRelationshipData OmitElement="true" UseFullForm="true" RoleElementName="elements">
            <DomainRelationshipMoniker Name="ElementHasElements" />
          </XmlRelationshipData>
          <XmlRelationshipData OmitElement="true" UseFullForm="true" RoleElementName="extensionPoints">
            <DomainRelationshipMoniker Name="ElementHasExtensionPoints" />
          </XmlRelationshipData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ViewHasElements" MonikerAttributeName="" MonikerElementName="viewHasElementsMoniker" ElementName="elements" MonikerTypeName="ViewHasElementsMoniker">
        <DomainRelationshipMoniker Name="ViewHasElements" />
        <ElementData>
          <XmlPropertyData XmlName="cardinality">
            <DomainPropertyMoniker Name="ViewHasElements/Cardinality" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="cardinalityCaption" Representation="Ignore">
            <DomainPropertyMoniker Name="ViewHasElements/CardinalityCaption" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="autoCreate">
            <DomainPropertyMoniker Name="ViewHasElements/AutoCreate" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="allowAddNew">
            <DomainPropertyMoniker Name="ViewHasElements/AllowAddNew" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="orderGroup">
            <DomainPropertyMoniker Name="ViewHasElements/OrderGroup" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="orderGroupComparerTypeName">
            <DomainPropertyMoniker Name="ViewHasElements/OrderGroupComparerTypeName" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ElementHasElementsConnector" MonikerAttributeName="" MonikerElementName="elementHasElementsConnectorMoniker" ElementName="elementHasElementsConnector" MonikerTypeName="ElementHasElementsConnectorMoniker">
        <ConnectorMoniker Name="ElementHasElementsConnector" />
      </XmlClassData>
      <XmlClassData TypeName="CustomizationPolicySchema" MonikerAttributeName="" MonikerElementName="customizationPolicySchemaMoniker" ElementName="customizationPolicy" MonikerTypeName="CustomizationPolicySchemaMoniker">
        <DomainClassMoniker Name="CustomizationPolicySchema" />
        <ElementData>
          <XmlRelationshipData RoleElementName="settings">
            <DomainRelationshipMoniker Name="PolicyHasSettings" />
          </XmlRelationshipData>
          <XmlPropertyData XmlName="isModified" Representation="Ignore">
            <DomainPropertyMoniker Name="CustomizationPolicySchema/IsModified" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="customizationLevel" Representation="Ignore">
            <DomainPropertyMoniker Name="CustomizationPolicySchema/CustomizationLevel" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="name">
            <DomainPropertyMoniker Name="CustomizationPolicySchema/Name" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="CustomizableSettingSchema" MonikerAttributeName="" MonikerElementName="customizableSettingSchemaMoniker" ElementName="setting" MonikerTypeName="CustomizableSettingSchemaMoniker">
        <DomainClassMoniker Name="CustomizableSettingSchema" />
        <ElementData>
          <XmlPropertyData XmlName="isEnabled">
            <DomainPropertyMoniker Name="CustomizableSettingSchema/IsEnabled" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="caption" Representation="Ignore">
            <DomainPropertyMoniker Name="CustomizableSettingSchema/Caption" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="captionFormatter" Representation="Ignore">
            <DomainPropertyMoniker Name="CustomizableSettingSchema/CaptionFormatter" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="isModified" Representation="Ignore">
            <DomainPropertyMoniker Name="CustomizableSettingSchema/IsModified" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="defaultValue">
            <DomainPropertyMoniker Name="CustomizableSettingSchema/DefaultValue" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="value">
            <DomainPropertyMoniker Name="CustomizableSettingSchema/Value" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="propertyId">
            <DomainPropertyMoniker Name="CustomizableSettingSchema/PropertyId" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="descriptionFormatter" Representation="Ignore">
            <DomainPropertyMoniker Name="CustomizableSettingSchema/DescriptionFormatter" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="description" Representation="Ignore">
            <DomainPropertyMoniker Name="CustomizableSettingSchema/Description" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="domainElementSettingType" Representation="Ignore">
            <DomainPropertyMoniker Name="CustomizableSettingSchema/DomainElementSettingType" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="PolicyHasSettings" MonikerAttributeName="" MonikerElementName="policyHasSettingsMoniker" ElementName="policyHasSettings" MonikerTypeName="PolicyHasSettingsMoniker">
        <DomainRelationshipMoniker Name="PolicyHasSettings" />
      </XmlClassData>
      <XmlClassData TypeName="CustomizableElementHasPolicy" MonikerAttributeName="" MonikerElementName="customizableElementHasPolicyMoniker" ElementName="customizableElementHasPolicy" MonikerTypeName="CustomizableElementHasPolicyMoniker">
        <DomainRelationshipMoniker Name="CustomizableElementHasPolicy" />
      </XmlClassData>
      <XmlClassData TypeName="ElementHasElements" MonikerAttributeName="" MonikerElementName="elementHasElementsMoniker" ElementName="childElement" MonikerTypeName="ElementHasElementsMoniker">
        <DomainRelationshipMoniker Name="ElementHasElements" />
        <ElementData>
          <XmlPropertyData XmlName="cardinality">
            <DomainPropertyMoniker Name="ElementHasElements/Cardinality" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="cardinalityCaption" Representation="Ignore">
            <DomainPropertyMoniker Name="ElementHasElements/CardinalityCaption" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="autoCreate">
            <DomainPropertyMoniker Name="ElementHasElements/AutoCreate" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="allowAddNew">
            <DomainPropertyMoniker Name="ElementHasElements/AllowAddNew" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="orderGroup">
            <DomainPropertyMoniker Name="ElementHasElements/OrderGroup" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="orderGroupComparerTypeName">
            <DomainPropertyMoniker Name="ElementHasElements/OrderGroupComparerTypeName" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="AutomationSettingsSchema" MonikerAttributeName="" SerializeId="true" MonikerElementName="automationSettingsSchemaMoniker" ElementName="automationSettings" MonikerTypeName="AutomationSettingsSchemaMoniker">
        <DomainClassMoniker Name="AutomationSettingsSchema" />
        <ElementData>
          <XmlPropertyData XmlName="automationType">
            <DomainPropertyMoniker Name="AutomationSettingsSchema/AutomationType" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="settings">
            <DomainPropertyMoniker Name="AutomationSettingsSchema/Settings" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="classification">
            <DomainPropertyMoniker Name="AutomationSettingsSchema/Classification" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="CollectionShape" MonikerAttributeName="" MonikerElementName="collectionShapeMoniker" ElementName="collectionShape" MonikerTypeName="CollectionShapeMoniker">
        <CompartmentShapeMoniker Name="CollectionShape" />
        <ElementData>
          <XmlPropertyData XmlName="tailoringFillColor">
            <DomainPropertyMoniker Name="CollectionShape/TailoringFillColor" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="tailoringTextColor">
            <DomainPropertyMoniker Name="CollectionShape/TailoringTextColor" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="tailoringOutlineColor">
            <DomainPropertyMoniker Name="CollectionShape/TailoringOutlineColor" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="CustomizableElementShape" MonikerAttributeName="" MonikerElementName="customizableElementShapeMoniker" ElementName="customizableElementShape" MonikerTypeName="CustomizableElementShapeMoniker">
        <CompartmentShapeMoniker Name="CustomizableElementShape" />
      </XmlClassData>
      <XmlClassData TypeName="PatternElementConnector" MonikerAttributeName="" MonikerElementName="patternElementConnectorMoniker" ElementName="patternElementConnector" MonikerTypeName="PatternElementConnectorMoniker">
        <ConnectorMoniker Name="PatternElementConnector" />
        <ElementData>
          <XmlPropertyData XmlName="tailoringOutlineColor">
            <DomainPropertyMoniker Name="PatternElementConnector/TailoringOutlineColor" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="tailoringTextColor">
            <DomainPropertyMoniker Name="PatternElementConnector/TailoringTextColor" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ProvidedExtensionPointSchema" MonikerAttributeName="" SerializeId="true" MonikerElementName="providedExtensionPointSchemaMoniker" ElementName="providedExtensionPoint" MonikerTypeName="ProvidedExtensionPointSchemaMoniker">
        <DomainClassMoniker Name="ProvidedExtensionPointSchema" />
        <ElementData>
          <XmlPropertyData XmlName="extensionPointId">
            <DomainPropertyMoniker Name="ProvidedExtensionPointSchema/ExtensionPointId" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="PatternHasProvidedExtensionPoints" MonikerAttributeName="" SerializeId="true" MonikerElementName="patternHasProvidedExtensionPointsMoniker" ElementName="patternHasProvidedExtensionPoints" MonikerTypeName="PatternHasProvidedExtensionPointsMoniker">
        <DomainRelationshipMoniker Name="PatternHasProvidedExtensionPoints" />
      </XmlClassData>
      <XmlClassData TypeName="ExtensionPointSchema" MonikerAttributeName="" SerializeId="true" MonikerElementName="extensionPointSchemaMoniker" ElementName="extensionPoint" MonikerTypeName="ExtensionPointSchemaMoniker">
        <DomainClassMoniker Name="ExtensionPointSchema" />
        <ElementData>
          <XmlPropertyData XmlName="requiredExtensionPointId" Representation="Ignore">
            <DomainPropertyMoniker Name="ExtensionPointSchema/RequiredExtensionPointId" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="conditions" Representation="Element">
            <DomainPropertyMoniker Name="ExtensionPointSchema/Conditions" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="representedExtensionPointId">
            <DomainPropertyMoniker Name="ExtensionPointSchema/RepresentedExtensionPointId" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ElementHasExtensionPoints" MonikerAttributeName="" SerializeId="true" MonikerElementName="elementHasExtensionPointsMoniker" ElementName="childExtensionPoint" MonikerTypeName="ElementHasExtensionPointsMoniker">
        <DomainRelationshipMoniker Name="ElementHasExtensionPoints" />
        <ElementData>
          <XmlPropertyData XmlName="cardinality">
            <DomainPropertyMoniker Name="ElementHasExtensionPoints/Cardinality" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="cardinalityCaption" Representation="Ignore">
            <DomainPropertyMoniker Name="ElementHasExtensionPoints/CardinalityCaption" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="orderGroup">
            <DomainPropertyMoniker Name="ElementHasExtensionPoints/OrderGroup" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="orderGroupComparerTypeName">
            <DomainPropertyMoniker Name="ElementHasExtensionPoints/OrderGroupComparerTypeName" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ViewHasExtensionPoints" MonikerAttributeName="" SerializeId="true" MonikerElementName="viewHasExtensionPointsMoniker" ElementName="extensionPoints" MonikerTypeName="ViewHasExtensionPointsMoniker">
        <DomainRelationshipMoniker Name="ViewHasExtensionPoints" />
        <ElementData>
          <XmlPropertyData XmlName="cardinality">
            <DomainPropertyMoniker Name="ViewHasExtensionPoints/Cardinality" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="cardinalityCaption" Representation="Ignore">
            <DomainPropertyMoniker Name="ViewHasExtensionPoints/CardinalityCaption" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="orderGroup">
            <DomainPropertyMoniker Name="ViewHasExtensionPoints/OrderGroup" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="orderGroupComparerTypeName">
            <DomainPropertyMoniker Name="ViewHasExtensionPoints/OrderGroupComparerTypeName" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ExtensionPointShape" MonikerAttributeName="" MonikerElementName="extensionPointShapeMoniker" ElementName="extensionPointShape" MonikerTypeName="ExtensionPointShapeMoniker">
        <CompartmentShapeMoniker Name="ExtensionPointShape" />
        <ElementData>
          <XmlPropertyData XmlName="tailoringFillColor">
            <DomainPropertyMoniker Name="ExtensionPointShape/TailoringFillColor" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="tailoringTextColor">
            <DomainPropertyMoniker Name="ExtensionPointShape/TailoringTextColor" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="tailoringOutlineColor">
            <DomainPropertyMoniker Name="ExtensionPointShape/TailoringOutlineColor" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ElementHasExtensionPointsConnector" MonikerAttributeName="" MonikerElementName="elementHasExtensionPointsConnectorMoniker" ElementName="elementHasExtensionPointsConnector" MonikerTypeName="ElementHasExtensionPointsConnectorMoniker">
        <ConnectorMoniker Name="ElementHasExtensionPointsConnector" />
      </XmlClassData>
      <XmlClassData TypeName="ViewHasExtensionPointsConnector" MonikerAttributeName="" MonikerElementName="viewHasExtensionPointsConnectorMoniker" ElementName="viewHasExtensionPointsConnector" MonikerTypeName="ViewHasExtensionPointsConnectorMoniker">
        <ConnectorMoniker Name="ViewHasExtensionPointsConnector" />
      </XmlClassData>
      <XmlClassData TypeName="PatternElementHasAutomationSettings" MonikerAttributeName="" SerializeId="true" MonikerElementName="patternElementHasAutomationSettingsMoniker" ElementName="patternElementHasAutomationSettings" MonikerTypeName="PatternElementHasAutomationSettingsMoniker">
        <DomainRelationshipMoniker Name="PatternElementHasAutomationSettings" />
      </XmlClassData>
    </ClassData>
  </XmlSerializationBehavior>
  <ExplorerBehavior Name="PatternModelExplorer" />
  <Diagram Id="bbbccbe0-cf42-4307-b8a3-d9ac6b7169ff" Description="Description for Microsoft.VisualStudio.Patterning.Runtime.Schema.PatternModelSchemaDiagram" Name="PatternModelSchemaDiagram" DisplayName="Pattern Model Schema Diagram" Namespace="Microsoft.VisualStudio.Patterning.Runtime.Schema" GeneratesDoubleDerived="true" FillColor="Silver">
    <Properties>
      <DomainProperty Id="dcf24a57-f408-4ebb-a7e9-ba4abd2ef52e" Description="The color of the gradient in the title of the background." Name="AuthoringGradientColor" DisplayName="Authoring Gradient Color" DefaultValue="WhiteSmoke" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
        <Type>
          <ExternalTypeMoniker Name="/System.Drawing/Color" />
        </Type>
      </DomainProperty>
      <DomainProperty Id="0e90079d-12aa-433b-9e16-3bb229a6d5ab" Description="Description for Microsoft.VisualStudio.Patterning.Runtime.Schema.PatternModelSchemaDiagram.Authoring Background Color" Name="AuthoringBackgroundColor" DisplayName="Authoring Background Color" DefaultValue="White" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
        <Type>
          <ExternalTypeMoniker Name="/System.Drawing/Color" />
        </Type>
      </DomainProperty>
      <DomainProperty Id="e0c29222-1979-499b-8b18-ff6cc44d854a" Description="The color of the text of the diagram title." Name="AuthoringTitleTextColor" DisplayName="Authoring Title Text Color" DefaultValue="DarkGray" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
        <Type>
          <ExternalTypeMoniker Name="/System.Drawing/Color" />
        </Type>
      </DomainProperty>
      <DomainProperty Id="e457ea9f-5553-4121-9a97-ebbf44b2a9ef" Description="The color of the gradient in the title of the background." Name="TailoringGradientColor" DisplayName="Tailoring Gradient Color" DefaultValue="WhiteSmoke" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
        <Type>
          <ExternalTypeMoniker Name="/System.Drawing/Color" />
        </Type>
      </DomainProperty>
      <DomainProperty Id="42510b7f-442b-45fe-9932-dae575609522" Description="Description for Microsoft.VisualStudio.Patterning.Runtime.Schema.PatternModelSchemaDiagram.Tailoring Background Color" Name="TailoringBackgroundColor" DisplayName="Tailoring Background Color" DefaultValue="204, 204, 204" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
        <Type>
          <ExternalTypeMoniker Name="/System.Drawing/Color" />
        </Type>
      </DomainProperty>
      <DomainProperty Id="0c72187e-4118-4888-b3e7-75dbd923537e" Description="The color of the text of the diagram title." Name="TailoringTitleTextColor" DisplayName="Tailoring Title Text Color" DefaultValue="DarkGray" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
        <Type>
          <ExternalTypeMoniker Name="/System.Drawing/Color" />
        </Type>
      </DomainProperty>
      <DomainProperty Id="6c89c15a-5ef2-4026-9c1f-28b5c415cfe1" Description="Whether to display the hidden items on this model." Name="ShowHiddenEntries" DisplayName="Show Hidden Items" DefaultValue="false" Category="Design">
        <Type>
          <ExternalTypeMoniker Name="/System/Boolean" />
        </Type>
      </DomainProperty>
    </Properties>
    <Class>
      <DomainClassMoniker Name="PatternModelSchema" />
    </Class>
    <ShapeMaps>
      <CompartmentShapeMap>
        <DomainClassMoniker Name="PatternSchema" />
        <ParentElementPath>
          <DomainPath>PatternModelHasPattern.PatternModel/!PatternModelSchema</DomainPath>
        </ParentElementPath>
        <DecoratorMap>
          <TextDecoratorMoniker Name="PatternShape/NameDecorator" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElementSchema/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="PatternShape/InheritedFromBaseDecorator" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="NamedElementSchema/IsInheritedFromBase" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="True" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="CustomizableElementShape/CustomizationFalseDisabledDecorator" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="CustomizableElementSchema/IsCustomizationEnabledState" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="FalseDisabled" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="CustomizableElementShape/CustomizationFalseEnabledDecorator" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="CustomizableElementSchema/IsCustomizationEnabledState" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="FalseEnabled" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="CustomizableElementShape/CustomizationInheritedDisabledDecorator" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="CustomizableElementSchema/IsCustomizationEnabledState" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="InheritedDisabled" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="CustomizableElementShape/CustomizationInheritedEnabledDecorator" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="CustomizableElementSchema/IsCustomizationEnabledState" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="InheritedEnabled" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="CustomizableElementShape/CustomizationTrueDisabledDecorator" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="CustomizableElementSchema/IsCustomizationEnabledState" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="TrueDisabled" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="CustomizableElementShape/CustomizationTrueEnabledDecorator" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="CustomizableElementSchema/IsCustomizationEnabledState" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="TrueEnabled" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <CompartmentShapeMoniker Name="PatternShape" />
        <CompartmentMap UsesCustomFilter="true">
          <CompartmentMoniker Name="PatternShape/Properties" />
          <ElementsDisplayed>
            <DomainPath>PatternElementHasProperties.Properties/!PropertySchema</DomainPath>
          </ElementsDisplayed>
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElementSchema/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </CompartmentMap>
        <CompartmentMap UsesCustomFilter="true">
          <CompartmentMoniker Name="PatternShape/Automation" />
          <ElementsDisplayed>
            <DomainPath>PatternElementHasAutomationSettings.AutomationSettings/!AutomationSettingsSchema</DomainPath>
          </ElementsDisplayed>
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElementSchema/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </CompartmentMap>
        <CompartmentMap UsesCustomFilter="true">
          <CompartmentMoniker Name="PatternShape/LaunchPoints" />
          <ElementsDisplayed>
            <DomainPath>PatternElementHasAutomationSettings.AutomationSettings/!AutomationSettingsSchema</DomainPath>
          </ElementsDisplayed>
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElementSchema/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </CompartmentMap>
      </CompartmentShapeMap>
      <CompartmentShapeMap HasCustomParentElement="true">
        <DomainClassMoniker Name="ElementSchema" />
        <ParentElementPath>
          <DomainPath />
        </ParentElementPath>
        <DecoratorMap>
          <TextDecoratorMoniker Name="ElementShape/NameDecorator" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElementSchema/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="ElementShape/InheritedFromBaseDecorator" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="NamedElementSchema/IsInheritedFromBase" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="True" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="CustomizableElementShape/CustomizationFalseDisabledDecorator" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="CustomizableElementSchema/IsCustomizationEnabledState" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="FalseDisabled" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="CustomizableElementShape/CustomizationFalseEnabledDecorator" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="CustomizableElementSchema/IsCustomizationEnabledState" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="FalseEnabled" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="CustomizableElementShape/CustomizationInheritedDisabledDecorator" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="CustomizableElementSchema/IsCustomizationEnabledState" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="InheritedDisabled" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="CustomizableElementShape/CustomizationInheritedEnabledDecorator" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="CustomizableElementSchema/IsCustomizationEnabledState" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="InheritedEnabled" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="CustomizableElementShape/CustomizationTrueDisabledDecorator" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="CustomizableElementSchema/IsCustomizationEnabledState" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="TrueDisabled" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="CustomizableElementShape/CustomizationTrueEnabledDecorator" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="CustomizableElementSchema/IsCustomizationEnabledState" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="TrueEnabled" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <CompartmentShapeMoniker Name="ElementShape" />
        <CompartmentMap UsesCustomFilter="true">
          <CompartmentMoniker Name="ElementShape/Properties" />
          <ElementsDisplayed>
            <DomainPath>PatternElementHasProperties.Properties/!PropertySchema</DomainPath>
          </ElementsDisplayed>
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElementSchema/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </CompartmentMap>
        <CompartmentMap UsesCustomFilter="true">
          <CompartmentMoniker Name="ElementShape/Automation" />
          <ElementsDisplayed>
            <DomainPath>PatternElementHasAutomationSettings.AutomationSettings/!AutomationSettingsSchema</DomainPath>
          </ElementsDisplayed>
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElementSchema/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </CompartmentMap>
        <CompartmentMap UsesCustomFilter="true">
          <CompartmentMoniker Name="ElementShape/LaunchPoints" />
          <ElementsDisplayed>
            <DomainPath>PatternElementHasAutomationSettings.AutomationSettings/!AutomationSettingsSchema</DomainPath>
          </ElementsDisplayed>
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElementSchema/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </CompartmentMap>
      </CompartmentShapeMap>
      <ShapeMap>
        <DomainClassMoniker Name="ViewSchema" />
        <ParentElementPath>
          <DomainPath>PatternHasViews.Pattern/!PatternSchema</DomainPath>
        </ParentElementPath>
        <DecoratorMap>
          <TextDecoratorMoniker Name="ViewShape/NameDecorator" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElementSchema/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="ViewShape/InheritedFromBaseDecorator" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="NamedElementSchema/IsInheritedFromBase" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="True" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="ViewShape/CustomizationFalseDisabledDecorator" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="CustomizableElementSchema/IsCustomizationEnabledState" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="FalseDisabled" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="ViewShape/CustomizationFalseEnabledDecorator" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="CustomizableElementSchema/IsCustomizationEnabledState" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="FalseEnabled" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="ViewShape/CustomizationInheritedDisabledDecorator" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="CustomizableElementSchema/IsCustomizationEnabledState" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="InheritedDisabled" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="ViewShape/CustomizationInheritedEnabledDecorator" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="CustomizableElementSchema/IsCustomizationEnabledState" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="InheritedEnabled" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="ViewShape/CustomizationTrueDisabledDecorator" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="CustomizableElementSchema/IsCustomizationEnabledState" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="TrueDisabled" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="ViewShape/CustomizationTrueEnabledDecorator" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="CustomizableElementSchema/IsCustomizationEnabledState" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="TrueEnabled" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <PortMoniker Name="ViewShape" />
      </ShapeMap>
      <CompartmentShapeMap HasCustomParentElement="true">
        <Notes>Skip=true</Notes>
        <DomainClassMoniker Name="CollectionSchema" />
        <ParentElementPath>
          <DomainPath />
        </ParentElementPath>
        <DecoratorMap>
          <TextDecoratorMoniker Name="CollectionShape/NameDecorator" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElementSchema/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="CollectionShape/InheritedFromBaseDecorator" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="NamedElementSchema/IsInheritedFromBase" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="True" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="CustomizableElementShape/CustomizationFalseDisabledDecorator" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="CustomizableElementSchema/IsCustomizationEnabledState" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="FalseDisabled" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="CustomizableElementShape/CustomizationFalseEnabledDecorator" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="CustomizableElementSchema/IsCustomizationEnabledState" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="FalseEnabled" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="CustomizableElementShape/CustomizationInheritedDisabledDecorator" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="CustomizableElementSchema/IsCustomizationEnabledState" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="InheritedDisabled" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="CustomizableElementShape/CustomizationInheritedEnabledDecorator" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="CustomizableElementSchema/IsCustomizationEnabledState" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="InheritedEnabled" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="CustomizableElementShape/CustomizationTrueDisabledDecorator" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="CustomizableElementSchema/IsCustomizationEnabledState" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="TrueDisabled" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="CustomizableElementShape/CustomizationTrueEnabledDecorator" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="CustomizableElementSchema/IsCustomizationEnabledState" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="TrueEnabled" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <CompartmentShapeMoniker Name="CollectionShape" />
        <CompartmentMap UsesCustomFilter="true">
          <CompartmentMoniker Name="CollectionShape/LaunchPoints" />
          <ElementsDisplayed>
            <DomainPath>PatternElementHasAutomationSettings.AutomationSettings/!AutomationSettingsSchema</DomainPath>
          </ElementsDisplayed>
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElementSchema/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </CompartmentMap>
        <CompartmentMap UsesCustomFilter="true">
          <CompartmentMoniker Name="CollectionShape/Properties" />
          <ElementsDisplayed>
            <DomainPath>PatternElementHasProperties.Properties/!PropertySchema</DomainPath>
          </ElementsDisplayed>
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElementSchema/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </CompartmentMap>
        <CompartmentMap UsesCustomFilter="true">
          <CompartmentMoniker Name="CollectionShape/Automation" />
          <ElementsDisplayed>
            <DomainPath>PatternElementHasAutomationSettings.AutomationSettings/!AutomationSettingsSchema</DomainPath>
          </ElementsDisplayed>
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElementSchema/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </CompartmentMap>
      </CompartmentShapeMap>
      <CompartmentShapeMap HasCustomParentElement="true">
        <DomainClassMoniker Name="ExtensionPointSchema" />
        <ParentElementPath>
          <DomainPath />
        </ParentElementPath>
        <DecoratorMap>
          <TextDecoratorMoniker Name="ExtensionPointShape/NameDecorator" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElementSchema/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="CustomizableElementShape/CustomizationFalseDisabledDecorator" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="CustomizableElementSchema/IsCustomizationEnabledState" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="FalseDisabled" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="CustomizableElementShape/CustomizationFalseEnabledDecorator" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="CustomizableElementSchema/IsCustomizationEnabledState" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="FalseEnabled" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="ExtensionPointShape/InheritedFromBaseDecorator" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="NamedElementSchema/IsInheritedFromBase" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="True" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="CustomizableElementShape/CustomizationInheritedDisabledDecorator" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="CustomizableElementSchema/IsCustomizationEnabledState" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="InheritedDisabled" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="CustomizableElementShape/CustomizationTrueDisabledDecorator" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="CustomizableElementSchema/IsCustomizationEnabledState" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="TrueDisabled" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="CustomizableElementShape/CustomizationTrueEnabledDecorator" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="CustomizableElementSchema/IsCustomizationEnabledState" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="TrueEnabled" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="CustomizableElementShape/CustomizationInheritedEnabledDecorator" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="CustomizableElementSchema/IsCustomizationEnabledState" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="InheritedEnabled" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <CompartmentShapeMoniker Name="ExtensionPointShape" />
        <CompartmentMap UsesCustomFilter="true">
          <CompartmentMoniker Name="ExtensionPointShape/Properties" />
          <ElementsDisplayed>
            <DomainPath>PatternElementHasProperties.Properties/!PropertySchema</DomainPath>
          </ElementsDisplayed>
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElementSchema/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </CompartmentMap>
      </CompartmentShapeMap>
    </ShapeMaps>
    <ConnectorMaps>
      <ConnectorMap>
        <ConnectorMoniker Name="ViewHasElementsConnector" />
        <DomainRelationshipMoniker Name="ViewHasElements" />
        <DecoratorMap>
          <TextDecoratorMoniker Name="ElementHasElementsConnector/CardinalityDecorator" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="ViewHasElements/CardinalityCaption" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <TextDecoratorMoniker Name="ViewHasElementsConnector/CardinalityDecorator" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="ViewHasElements/CardinalityCaption" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <TextDecoratorMoniker Name="ElementHasExtensionPointsConnector/CardinalityDecorator" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="ViewHasElements/CardinalityCaption" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <TextDecoratorMoniker Name="ViewHasExtensionPointsConnector/CardinalityDecorator" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="ViewHasElements/CardinalityCaption" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
      </ConnectorMap>
      <ConnectorMap>
        <ConnectorMoniker Name="ElementHasElementsConnector" />
        <DomainRelationshipMoniker Name="ElementHasElements" />
        <DecoratorMap>
          <TextDecoratorMoniker Name="ElementHasElementsConnector/CardinalityDecorator" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="ElementHasElements/CardinalityCaption" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <TextDecoratorMoniker Name="ViewHasElementsConnector/CardinalityDecorator" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="ElementHasElements/CardinalityCaption" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
      </ConnectorMap>
      <ConnectorMap>
        <ConnectorMoniker Name="ElementHasExtensionPointsConnector" />
        <DomainRelationshipMoniker Name="ElementHasExtensionPoints" />
        <DecoratorMap>
          <TextDecoratorMoniker Name="ElementHasExtensionPointsConnector/CardinalityDecorator" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="ElementHasExtensionPoints/CardinalityCaption" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <TextDecoratorMoniker Name="ViewHasElementsConnector/CardinalityDecorator" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="ElementHasExtensionPoints/CardinalityCaption" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <TextDecoratorMoniker Name="ElementHasElementsConnector/CardinalityDecorator" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="ElementHasExtensionPoints/CardinalityCaption" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <TextDecoratorMoniker Name="ViewHasExtensionPointsConnector/CardinalityDecorator" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="ElementHasExtensionPoints/CardinalityCaption" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
      </ConnectorMap>
      <ConnectorMap>
        <ConnectorMoniker Name="ViewHasExtensionPointsConnector" />
        <DomainRelationshipMoniker Name="ViewHasExtensionPoints" />
        <DecoratorMap>
          <TextDecoratorMoniker Name="ViewHasExtensionPointsConnector/CardinalityDecorator" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="ViewHasExtensionPoints/CardinalityCaption" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <TextDecoratorMoniker Name="ElementHasElementsConnector/CardinalityDecorator" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="ViewHasExtensionPoints/CardinalityCaption" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <TextDecoratorMoniker Name="ViewHasElementsConnector/CardinalityDecorator" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="ViewHasExtensionPoints/CardinalityCaption" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
      </ConnectorMap>
    </ConnectorMaps>
  </Diagram>
  <Designer CopyPasteGeneration="CopyPasteOnly" FileExtension="patterndefinition" EditorGuid="506d6f77-9b65-41c8-b69c-8e7e78c8dce0" usesStickyToolboxItems="true">
    <RootClass>
      <DomainClassMoniker Name="PatternModelSchema" />
    </RootClass>
    <XmlSerializationDefinition RootNamespace="http://schemas.microsoft.com/visualstudio/patterning/runtime/patternmodel" CustomPostLoad="false">
      <XmlSerializationBehaviorMoniker Name="PatternModelSerializationBehavior" />
    </XmlSerializationDefinition>
    <ToolboxTab TabText="Pattern Model Designer">
      <ElementTool Name="Collection" ToolboxIcon="..\..\..\Authoring\Source\ToolkitDesign.Shell\Resources\CollectionShapeToolBitmap.bmp" Caption="Collection" Tooltip="Add a Collection to add structure or grouping of other elements in the pattern." HelpKeyword="Collection">
        <DomainClassMoniker Name="CollectionSchema" />
      </ElementTool>
      <ElementTool Name="Element" ToolboxIcon="..\..\..\Authoring\Source\ToolkitDesign.Shell\Resources\ElementShapeToolBitmap.bmp" Caption="Element" Tooltip="Add an Element to define a set of variable elements in the pattern." HelpKeyword="Element">
        <DomainClassMoniker Name="ElementSchema" />
      </ElementTool>
      <ElementTool Name="ExtensionPoint" ToolboxIcon="..\..\..\Authoring\Source\ToolkitDesign.Shell\Resources\ExtensionPointToolBitmap.bmp" Caption="Extension Point" Tooltip="Add an Extension Point to define an extension of your pattern, that will be provided by other pattern toolkits." HelpKeyword="ExtensionPoint">
        <DomainClassMoniker Name="ExtensionPointSchema" />
      </ElementTool>
    </ToolboxTab>
    <Validation UsesMenu="true" UsesOpen="false" UsesSave="true" UsesLoad="false" />
    <DiagramMoniker Name="PatternModelSchemaDiagram" />
  </Designer>
</Dsl>