<?xml version="1.0" encoding="utf-8"?>
<Dsl xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="04739abb-263d-488f-b865-fed569a7b766" Description="The store of the state of all products in the solution." Name="ProductStateStore" DisplayName="Product State Store" Namespace="NuPattern.Runtime.Store" MinorVersion="2" ProductName="Product State Store" CompanyName="NuPattern" PackageGuid="93373818-600f-414b-8181-3a0cb79fa785" PackageNamespace="NuPattern.Runtime.Store" xmlns="http://schemas.microsoft.com/VisualStudio/2005/DslTools/DslDefinitionModel">
  <Classes>
    <DomainClass Id="567410f3-5e44-4211-8623-bd9ee337dd82" Description="The state of all products in the solution." Name="ProductState" DisplayName="Product State" Namespace="NuPattern.Runtime.Store" HasCustomConstructor="true">
      <Notes>IsRoot</Notes>
      <ElementMergeDirectives>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="Product" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>ProductStateHasProducts.Products</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
      </ElementMergeDirectives>
    </DomainClass>
    <DomainClass Id="9d160ca6-f94d-4cb7-956d-e2f537a0e33d" Description="A property of an element." Name="Property" DisplayName="Property" Namespace="NuPattern.Runtime.Store" HasCustomConstructor="true">
      <BaseClass>
        <DomainClassMoniker Name="InstanceBase" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="e0bdc7da-54bd-4b50-a3df-4b26e569b88d" Description="The current serialized value of the property. Use Value to get the typed value." Name="RawValue" DisplayName="Raw Value" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="4d1f8038-151c-461d-97cc-3eaf779f50eb" Description="A container for elements in a view." Name="Collection" DisplayName="Solution Element" Namespace="NuPattern.Runtime.Store">
      <BaseClass>
        <DomainClassMoniker Name="AbstractElement" />
      </BaseClass>
    </DomainClass>
    <DomainClass Id="375050fd-908d-47b2-9494-7626fee46113" Description="An element of a view." Name="Element" DisplayName="Solution Element" Namespace="NuPattern.Runtime.Store">
      <BaseClass>
        <DomainClassMoniker Name="AbstractElement" />
      </BaseClass>
    </DomainClass>
    <DomainClass Id="ce0a63a4-80a0-4a07-b9d1-d97fd206a8f1" Description="An element within the product." Name="ProductElement" DisplayName="Product Element" InheritanceModifier="Abstract" Namespace="NuPattern.Runtime.Store" HasCustomConstructor="true">
      <Notes>SchemaTypeName=PatternElementSchema</Notes>
      <Attributes>
        <ClrAttribute Name="System.ComponentModel.TypeDescriptionProvider">
          <Parameters>
            <AttributeParameter Value="typeof(ProductElementTypeDescriptionProvider)" />
          </Parameters>
        </ClrAttribute>
      </Attributes>
      <BaseClass>
        <DomainClassMoniker Name="InstanceBase" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="5ed95450-9b27-4485-92c7-862e2c0721d1" Description="The name of this element instance." Name="InstanceName" DisplayName="Name" Category="General">
          <Attributes>
            <ClrAttribute Name="System.ComponentModel.ParenthesizePropertyNameAttribute">
              <Parameters>
                <AttributeParameter Value="true" />
              </Parameters>
            </ClrAttribute>
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="01523a35-435b-40a4-a24e-7656b71c1ad6" Description="The order of this element relative to its siblings." Name="InstanceOrder" DisplayName="Instance Order" Category="Appearance" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/Double" />
          </Type>
        </DomainProperty>
      </Properties>
      <ElementMergeDirectives>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="Reference" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>ProductElementHasReferences.References</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
      </ElementMergeDirectives>
    </DomainClass>
    <DomainClass Id="f7b1afaf-b90d-411b-9792-1b402d72dfaf" Description="A product instance in the solution." Name="Product" DisplayName="Product" Namespace="NuPattern.Runtime.Store">
      <Notes>SchemaTypeName=PatternSchema</Notes>
      <BaseClass>
        <DomainClassMoniker Name="ProductElement" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="8f8d39a6-537c-4b79-b9fe-49636a5a88bb" Description="The identifier of the Visual Studio extension deploying the product." Name="ExtensionId" DisplayName="Extension Id" Category="General" IsBrowsable="false">
          <Attributes>
            <ClrAttribute Name="Hidden" />
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="f69e7d29-03b1-41c8-ab04-f6137567f952" Description="The name of the Visual Studio extension that deploys this product." Name="ExtensionName" DisplayName="Extension Name" SetterAccessModifier="Assembly" IsBrowsable="false">
          <Attributes>
            <ClrAttribute Name="Hidden" />
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="7871da94-2979-4616-9a84-a0bc6a7e5b65" Description="The author of this product." Name="Author" DisplayName="Author" SetterAccessModifier="Assembly" IsBrowsable="false">
          <Attributes>
            <ClrAttribute Name="Hidden" />
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="b55b9568-ce0e-47cb-8110-543e5ad9c692" Description="The version of this product." Name="Version" DisplayName="Version" SetterAccessModifier="Assembly" IsBrowsable="false">
          <Attributes>
            <ClrAttribute Name="Hidden" />
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="f02be9bc-9733-4d6c-a35c-8460b2d6e310" Description="A view of a product instance." Name="View" DisplayName="View" Namespace="NuPattern.Runtime.Store">
      <BaseClass>
        <DomainClassMoniker Name="InstanceBase" />
      </BaseClass>
      <ElementMergeDirectives>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="Product" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>ViewHasExtensionProducts.ExtensionProducts</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
      </ElementMergeDirectives>
    </DomainClass>
    <DomainClass Id="fb175123-6a49-496b-88b4-4c82c6d2e6ca" Description="A child collection or element." Name="AbstractElement" DisplayName="Abstract Element" InheritanceModifier="Abstract" Namespace="NuPattern.Runtime.Store">
      <BaseClass>
        <DomainClassMoniker Name="ProductElement" />
      </BaseClass>
      <ElementMergeDirectives>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="Product" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>ElementHasExtensions.ExtensionProducts</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
      </ElementMergeDirectives>
    </DomainClass>
    <DomainClass Id="30b2207d-bb9a-456f-82ee-a116d74598ef" Description="An element instance." Name="InstanceBase" DisplayName="Instance Base" InheritanceModifier="Abstract" Namespace="NuPattern.Runtime.Store">
      <Notes>SchemaTypeName=NamedElement</Notes>
      <Properties>
        <DomainProperty Id="d029965b-4622-4b05-853b-95cc6d163e0b" Description="The model element identifier in the owning definition." Name="DefinitionId" DisplayName="Definition Id" IsBrowsable="false" IsUIReadOnly="true">
          <Attributes>
            <ClrAttribute Name="Hidden" />
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/Guid" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="5cbc79d5-4132-4486-8370-1e03bd033112" Description="Informational-only rendering of the defining element referenced by the DefinitionId property." Name="DefinitionName" DisplayName="Definition Name" Kind="CustomStorage" IsElementName="true" IsBrowsable="false">
          <Attributes>
            <ClrAttribute Name="Hidden" />
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="277f2180-34d8-49f7-a33b-0497da7c5cbc" Description="Notes for this element." Name="Notes" DisplayName="Notes" Category="Documentation">
          <Attributes>
            <ClrAttribute Name="System.ComponentModel.Editor">
              <Parameters>
                <AttributeParameter Value="typeof(System.ComponentModel.Design.MultilineStringEditor)" />
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
    <DomainClass Id="3fcc5eb5-c492-48ba-84a1-9172012c8f5f" Description="A reference to external data or service." Name="Reference" DisplayName="Reference" Namespace="NuPattern.Runtime.Store">
      <Notes>SkipInfoProperty=true</Notes>
      <Attributes>
        <ClrAttribute Name="System.ComponentModel.DefaultProperty">
          <Parameters>
            <AttributeParameter Value="&quot;Kind&quot;" />
          </Parameters>
        </ClrAttribute>
      </Attributes>
      <Properties>
        <DomainProperty Id="f7d5f1f0-6498-42a2-b0c4-622d3770af4d" Description="The value of the reference, having meaning to the kind of the reference." Name="Value" DisplayName="Value">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="5b404e8a-2ac5-4470-950f-203b4e8099a6" Description="The kind of the reference, used to classify the reference. If this is the full type name of a class, then the class is used to provide the display characteristics of this reference." Name="Kind" DisplayName="Kind" IsElementName="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="304e2b4e-b105-47dd-bb81-027a220c37e2" Description="Provides arbitrary annotations on a reference, typically used by automation." Name="Tag" DisplayName="Tag">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
  </Classes>
  <Relationships>
    <DomainRelationship Id="6ca5790c-e0a4-4393-ac78-85cc87abf147" Description="Description for NuPattern.Runtime.Store.ProductElementHasProperties" Name="ProductElementHasProperties" DisplayName="Product Element Has Properties" Namespace="NuPattern.Runtime.Store" IsEmbedding="true">
      <Source>
        <DomainRole Id="52b9fc10-fcc7-47d1-adb7-6dcbef1f4ce1" Description="The properties of this element." Name="ProductElement" DisplayName="Product Element" PropertyName="Properties" PropagatesCopy="PropagatesCopyToLinkAndOppositeRolePlayer" PropertyDisplayName="Properties">
          <Attributes>
            <ClrAttribute Name="Hidden" />
          </Attributes>
          <RolePlayer>
            <DomainClassMoniker Name="ProductElement" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="94353169-930a-4c7a-9a90-8e5304b89a23" Description="The owning element." Name="Property" DisplayName="Property" PropertyName="Owner" Multiplicity="One" PropagatesDelete="true" PropertyDisplayName="Owner">
          <Attributes>
            <ClrAttribute Name="Hidden" />
          </Attributes>
          <RolePlayer>
            <DomainClassMoniker Name="Property" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="a6909603-f601-479e-92ba-ee082713a25d" Description="Description for NuPattern.Runtime.Store.ProductHasViews" Name="ProductHasViews" DisplayName="Product Has Views" Namespace="NuPattern.Runtime.Store" IsEmbedding="true">
      <Source>
        <DomainRole Id="75f3cd0f-aee1-462e-a9eb-637c969dd140" Description="The views of this product." Name="Product" DisplayName="Product" PropertyName="Views" Multiplicity="OneMany" PropagatesCopy="PropagatesCopyToLinkAndOppositeRolePlayer" PropertyDisplayName="Views">
          <Attributes>
            <ClrAttribute Name="Hidden" />
          </Attributes>
          <RolePlayer>
            <DomainClassMoniker Name="Product" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="1d00ce20-2179-47be-8bfd-633015700726" Description="The owning product." Name="View" DisplayName="View" PropertyName="Product" Multiplicity="One" PropagatesDelete="true" PropertyDisplayName="Product">
          <Attributes>
            <ClrAttribute Name="Hidden" />
          </Attributes>
          <RolePlayer>
            <DomainClassMoniker Name="View" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="c9108ec6-51cd-4a7e-8570-cb53cdd8d09a" Description="Description for NuPattern.Runtime.Store.ViewHasElements" Name="ViewHasElements" DisplayName="View Has Elements" Namespace="NuPattern.Runtime.Store" IsEmbedding="true">
      <Source>
        <DomainRole Id="9caaf0c1-dd84-4ea8-bdec-1c01dc20f062" Description="The child elements of this view." Name="View" DisplayName="View" PropertyName="Elements" PropagatesCopy="PropagatesCopyToLinkAndOppositeRolePlayer" PropertyDisplayName="Elements">
          <Notes>Skip=true
SkipCreate=true</Notes>
          <Attributes>
            <ClrAttribute Name="Hidden" />
          </Attributes>
          <RolePlayer>
            <DomainClassMoniker Name="View" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="afd7c74d-4157-43cb-bc14-cf6c33ae9965" Description="The owning view." Name="AbstractElement" DisplayName="Abstract Element" PropertyName="View" Multiplicity="ZeroOne" PropagatesDelete="true" PropertyDisplayName="View">
          <Attributes>
            <ClrAttribute Name="Hidden" />
          </Attributes>
          <RolePlayer>
            <DomainClassMoniker Name="AbstractElement" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="f403a072-caab-432e-b178-fac3cd111c01" Description="Description for NuPattern.Runtime.Store.ElementHasChildElements" Name="ElementHasChildElements" DisplayName="Element Has Child Elements" Namespace="NuPattern.Runtime.Store" IsEmbedding="true">
      <Source>
        <DomainRole Id="b978b2db-5e97-4331-a9fc-c32bb129d2ca" Description="The child elements of this element." Name="ParentElement" DisplayName="Parent Element" PropertyName="Elements" PropagatesCopy="PropagatesCopyToLinkAndOppositeRolePlayer" PropertyDisplayName="Elements">
          <Notes>Skip=true
SkipCreate=true</Notes>
          <Attributes>
            <ClrAttribute Name="Hidden" />
          </Attributes>
          <RolePlayer>
            <DomainClassMoniker Name="AbstractElement" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="8acfd773-86ab-49fa-a64b-856a7259d6b9" Description="The owning element." Name="ChildElement" DisplayName="Child Element" PropertyName="Owner" Multiplicity="ZeroOne" PropagatesDelete="true" PropertyDisplayName="Owner">
          <Attributes>
            <ClrAttribute Name="Hidden" />
          </Attributes>
          <RolePlayer>
            <DomainClassMoniker Name="AbstractElement" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="6f7b5a1d-97f0-4816-a525-8dae64aa347b" Description="Description for NuPattern.Runtime.Store.ProductStateHasProducts" Name="ProductStateHasProducts" DisplayName="Product State Has Products" Namespace="NuPattern.Runtime.Store" IsEmbedding="true">
      <Source>
        <DomainRole Id="20d6306b-1643-4f42-ad13-c5808af02833" Description="The products in this solution." Name="ProductState" DisplayName="Product State" PropertyName="Products" PropagatesCopy="PropagatesCopyToLinkAndOppositeRolePlayer" PropertyDisplayName="Products">
          <RolePlayer>
            <DomainClassMoniker Name="ProductState" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="bbff43e8-412a-4ffa-8034-88ae5dd3d024" Description="The owning state model." Name="Product" DisplayName="Product" PropertyName="ProductState" Multiplicity="ZeroOne" PropagatesDelete="true" PropertyDisplayName="Product State">
          <Attributes>
            <ClrAttribute Name="Hidden" />
          </Attributes>
          <RolePlayer>
            <DomainClassMoniker Name="Product" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="d33a9c43-9ba9-4e98-8841-72ec8355ee3a" Description="Description for NuPattern.Runtime.Store.ElementHasExtensions" Name="ElementHasExtensions" DisplayName="Element Has Extensions" Namespace="NuPattern.Runtime.Store" IsEmbedding="true">
      <Source>
        <DomainRole Id="a38cf9ed-b2aa-4114-8287-8c3f388b642a" Description="The child extension products of this element." Name="AbstractElement" DisplayName="Abstract Element" PropertyName="ExtensionProducts" PropagatesCopy="PropagatesCopyToLinkAndOppositeRolePlayer" PropertyDisplayName="Extension Products">
          <Notes>Skip=true
SkipCreate=true</Notes>
          <Attributes>
            <ClrAttribute Name="Hidden" />
          </Attributes>
          <RolePlayer>
            <DomainClassMoniker Name="AbstractElement" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="760038e7-6f05-4da1-bdef-08291ed2153a" Description="The owning element." Name="Extension" DisplayName="Extension" PropertyName="Owner" Multiplicity="ZeroOne" PropagatesDelete="true" PropertyDisplayName="Owner">
          <Attributes>
            <ClrAttribute Name="Hidden" />
          </Attributes>
          <RolePlayer>
            <DomainClassMoniker Name="Product" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="857a8e39-d8a4-4eba-a51c-c502c131831e" Description="Description for NuPattern.Runtime.Store.ViewHasExtensionProducts" Name="ViewHasExtensionProducts" DisplayName="View Has Extension Products" Namespace="NuPattern.Runtime.Store" IsEmbedding="true">
      <Source>
        <DomainRole Id="f88f4ae1-e5be-4f4b-92fb-1c899966ee99" Description="The child extension products of this view." Name="View" DisplayName="View" PropertyName="ExtensionProducts" PropagatesCopy="PropagatesCopyToLinkAndOppositeRolePlayer" PropertyDisplayName="Extension Products">
          <Notes>Skip=true
SkipCreate=true</Notes>
          <Attributes>
            <ClrAttribute Name="Hidden" />
          </Attributes>
          <RolePlayer>
            <DomainClassMoniker Name="View" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="829004e5-3211-4e48-8667-07cb1acad22c" Description="The owning view." Name="Extension" DisplayName="Extension" PropertyName="View" Multiplicity="ZeroOne" PropagatesDelete="true" PropertyDisplayName="View">
          <Attributes>
            <ClrAttribute Name="Hidden" />
          </Attributes>
          <RolePlayer>
            <DomainClassMoniker Name="Product" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="3cf98e81-5f66-450b-a2f7-60e75e96a40a" Description="Description for NuPattern.Runtime.Store.ProductElementHasReferences" Name="ProductElementHasReferences" DisplayName="Product Element Has References" Namespace="NuPattern.Runtime.Store" IsEmbedding="true">
      <Source>
        <DomainRole Id="5f1b080b-17dc-4d4a-9330-c289ebc0a510" Description="The references of this element." Name="ProductElement" DisplayName="Product Element" PropertyName="References" PropagatesCopy="PropagatesCopyToLinkAndOppositeRolePlayer" PropertyDisplayName="References">
          <RolePlayer>
            <DomainClassMoniker Name="ProductElement" />
          </RolePlayer>
          <propertyAttributes>
            <ClrAttribute Name="System.ComponentModel.Editor">
              <Parameters>
                <AttributeParameter Value="typeof(System.Drawing.Design.UITypeEditor)" />
                <AttributeParameter Value="typeof(System.Drawing.Design.UITypeEditor)" />
              </Parameters>
            </ClrAttribute>
            <ClrAttribute Name="DslDesign::DisplayNameResource">
              <Parameters>
                <AttributeParameter Value="&quot;ProductElement_ReferencesDisplayName&quot;" />
                <AttributeParameter Value="typeof(Properties.Resources)" />
              </Parameters>
            </ClrAttribute>
            <ClrAttribute Name="DslDesign::DescriptionResource">
              <Parameters>
                <AttributeParameter Value="&quot;ProductElement_ReferencesDescription&quot;" />
                <AttributeParameter Value="typeof(Properties.Resources)" />
              </Parameters>
            </ClrAttribute>
            <ClrAttribute Name="System.ComponentModel.TypeConverter">
              <Parameters>
                <AttributeParameter Value="typeof(ProductElementTypeDescriptor.ReferencesTypeConverter)" />
              </Parameters>
            </ClrAttribute>
          </propertyAttributes>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="7b4099c8-aa85-46f6-9d34-38513bd61317" Description="The owning element." Name="Reference" DisplayName="Reference" PropertyName="Owner" Multiplicity="One" PropagatesDelete="true" PropertyDisplayName="Owner">
          <RolePlayer>
            <DomainClassMoniker Name="Reference" />
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
    <ExternalType Name="Uri" Namespace="System" />
  </Types>
  <XmlSerializationBehavior Name="ProductStateStoreSerializationBehavior" Namespace="NuPattern.Runtime.Store">
    <ClassData>
      <XmlClassData TypeName="ProductState" MonikerAttributeName="" SerializeId="true" MonikerElementName="productStateMoniker" ElementName="productState" MonikerTypeName="ProductStateMoniker">
        <DomainClassMoniker Name="ProductState" />
        <ElementData>
          <XmlRelationshipData RoleElementName="products">
            <DomainRelationshipMoniker Name="ProductStateHasProducts" />
          </XmlRelationshipData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ViewHasElements" MonikerAttributeName="" MonikerElementName="viewHasElementsMoniker" ElementName="viewHasElements" MonikerTypeName="ViewHasElementsMoniker">
        <DomainRelationshipMoniker Name="ViewHasElements" />
      </XmlClassData>
      <XmlClassData TypeName="ElementHasChildElements" MonikerAttributeName="" MonikerElementName="elementHasChildElementsMoniker" ElementName="elementHasChildElements" MonikerTypeName="ElementHasChildElementsMoniker">
        <DomainRelationshipMoniker Name="ElementHasChildElements" />
      </XmlClassData>
      <XmlClassData TypeName="Property" MonikerAttributeName="" SerializeId="true" MonikerElementName="propertyMoniker" ElementName="property" MonikerTypeName="PropertyMoniker">
        <DomainClassMoniker Name="Property" />
        <ElementData>
          <XmlPropertyData XmlName="rawValue">
            <DomainPropertyMoniker Name="Property/RawValue" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="Element" MonikerAttributeName="" SerializeId="true" MonikerElementName="elementMoniker" ElementName="element" MonikerTypeName="ElementMoniker">
        <DomainClassMoniker Name="Element" />
      </XmlClassData>
      <XmlClassData TypeName="Product" MonikerAttributeName="" SerializeId="true" MonikerElementName="productMoniker" ElementName="product" MonikerTypeName="ProductMoniker">
        <DomainClassMoniker Name="Product" />
        <ElementData>
          <XmlRelationshipData RoleElementName="views">
            <DomainRelationshipMoniker Name="ProductHasViews" />
          </XmlRelationshipData>
          <XmlPropertyData XmlName="extensionId">
            <DomainPropertyMoniker Name="Product/ExtensionId" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="extensionName">
            <DomainPropertyMoniker Name="Product/ExtensionName" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="author">
            <DomainPropertyMoniker Name="Product/Author" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="version">
            <DomainPropertyMoniker Name="Product/Version" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="View" MonikerAttributeName="" SerializeId="true" MonikerElementName="viewMoniker" ElementName="view" MonikerTypeName="ViewMoniker">
        <DomainClassMoniker Name="View" />
        <ElementData>
          <XmlRelationshipData RoleElementName="elements">
            <DomainRelationshipMoniker Name="ViewHasElements" />
          </XmlRelationshipData>
          <XmlRelationshipData RoleElementName="extensionProducts">
            <DomainRelationshipMoniker Name="ViewHasExtensionProducts" />
          </XmlRelationshipData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="AbstractElement" MonikerAttributeName="" SerializeId="true" MonikerElementName="abstractElementMoniker" ElementName="abstractElement" MonikerTypeName="AbstractElementMoniker">
        <DomainClassMoniker Name="AbstractElement" />
        <ElementData>
          <XmlRelationshipData RoleElementName="elements">
            <DomainRelationshipMoniker Name="ElementHasChildElements" />
          </XmlRelationshipData>
          <XmlRelationshipData RoleElementName="extensionProducts">
            <DomainRelationshipMoniker Name="ElementHasExtensions" />
          </XmlRelationshipData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="InstanceBase" MonikerAttributeName="" SerializeId="true" MonikerElementName="instanceBaseMoniker" ElementName="instanceBase" MonikerTypeName="InstanceBaseMoniker">
        <DomainClassMoniker Name="InstanceBase" />
        <ElementData>
          <XmlPropertyData XmlName="definitionId">
            <DomainPropertyMoniker Name="InstanceBase/DefinitionId" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="definitionName">
            <DomainPropertyMoniker Name="InstanceBase/DefinitionName" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="notes">
            <DomainPropertyMoniker Name="InstanceBase/Notes" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ProductElementHasProperties" MonikerAttributeName="" MonikerElementName="productElementHasPropertiesMoniker" ElementName="productElementHasProperties" MonikerTypeName="ProductElementHasPropertiesMoniker">
        <DomainRelationshipMoniker Name="ProductElementHasProperties" />
      </XmlClassData>
      <XmlClassData TypeName="ProductElement" MonikerAttributeName="" SerializeId="true" MonikerElementName="productElementMoniker" ElementName="productElement" MonikerTypeName="ProductElementMoniker">
        <DomainClassMoniker Name="ProductElement" />
        <ElementData>
          <XmlRelationshipData RoleElementName="properties">
            <DomainRelationshipMoniker Name="ProductElementHasProperties" />
          </XmlRelationshipData>
          <XmlPropertyData XmlName="instanceName">
            <DomainPropertyMoniker Name="ProductElement/InstanceName" />
          </XmlPropertyData>
          <XmlRelationshipData RoleElementName="references">
            <DomainRelationshipMoniker Name="ProductElementHasReferences" />
          </XmlRelationshipData>
          <XmlPropertyData XmlName="instanceOrder" Representation="Ignore">
            <DomainPropertyMoniker Name="ProductElement/InstanceOrder" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ProductHasViews" MonikerAttributeName="" MonikerElementName="productHasViewsMoniker" ElementName="productHasViews" MonikerTypeName="ProductHasViewsMoniker">
        <DomainRelationshipMoniker Name="ProductHasViews" />
      </XmlClassData>
      <XmlClassData TypeName="Collection" MonikerAttributeName="" SerializeId="true" MonikerElementName="collectionMoniker" ElementName="collection" MonikerTypeName="CollectionMoniker">
        <DomainClassMoniker Name="Collection" />
      </XmlClassData>
      <XmlClassData TypeName="ProductStateHasProducts" MonikerAttributeName="" MonikerElementName="productStateHasProductsMoniker" ElementName="productStateHasProducts" MonikerTypeName="ProductStateHasProductsMoniker">
        <DomainRelationshipMoniker Name="ProductStateHasProducts" />
      </XmlClassData>
      <XmlClassData TypeName="ElementHasExtensions" MonikerAttributeName="" MonikerElementName="elementHasExtensionsMoniker" ElementName="elementHasExtensions" MonikerTypeName="ElementHasExtensionsMoniker">
        <DomainRelationshipMoniker Name="ElementHasExtensions" />
      </XmlClassData>
      <XmlClassData TypeName="ViewHasExtensionProducts" MonikerAttributeName="" MonikerElementName="viewHasExtensionProductsMoniker" ElementName="viewHasExtensionProducts" MonikerTypeName="ViewHasExtensionProductsMoniker">
        <DomainRelationshipMoniker Name="ViewHasExtensionProducts" />
      </XmlClassData>
      <XmlClassData TypeName="Reference" MonikerAttributeName="" SerializeId="true" MonikerElementName="referenceMoniker" ElementName="reference" MonikerTypeName="ReferenceMoniker">
        <DomainClassMoniker Name="Reference" />
        <ElementData>
          <XmlPropertyData XmlName="value">
            <DomainPropertyMoniker Name="Reference/Value" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="kind">
            <DomainPropertyMoniker Name="Reference/Kind" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="tag">
            <DomainPropertyMoniker Name="Reference/Tag" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ProductElementHasReferences" MonikerAttributeName="" SerializeId="true" MonikerElementName="productElementHasReferencesMoniker" ElementName="productElementHasReferences" MonikerTypeName="ProductElementHasReferencesMoniker">
        <DomainRelationshipMoniker Name="ProductElementHasReferences" />
      </XmlClassData>
    </ClassData>
  </XmlSerializationBehavior>
  <ExplorerBehavior Name="ProductStateExplorer" />
  <CustomEditor FileExtension="slnbldr" EditorGuid="56cc9112-4d34-4b72-a951-0be43e5afeb4">
    <RootClass>
      <DomainClassMoniker Name="ProductState" />
    </RootClass>
    <XmlSerializationDefinition RootNamespace="http://schemas.microsoft.com/visualstudio/patterning/runtime/productstate" CustomPostLoad="false">
      <XmlSerializationBehaviorMoniker Name="ProductStateStoreSerializationBehavior" />
    </XmlSerializationDefinition>
    <Validation UsesMenu="false" UsesOpen="false" UsesSave="false" UsesLoad="false" />
  </CustomEditor>
  <Explorer ExplorerGuid="9e094c7f-3548-4b90-83ed-6d449bc6a53e" Title="Product State Explorer">
    <ExplorerBehaviorMoniker Name="ProductStateStore/ProductStateExplorer" />
  </Explorer>
</Dsl>