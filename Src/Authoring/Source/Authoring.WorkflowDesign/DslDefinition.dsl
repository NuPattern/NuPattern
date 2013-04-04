<?xml version="1.0" encoding="utf-8"?>
<Dsl xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="c0712d75-bf72-45f5-9b7b-bf7ad51c42c8" Description="Description for NuPattern.Authoring.WorkflowDesign.WorkflowDesign" Name="WorkflowDesign" DisplayName="WorkflowDesign" Namespace="NuPattern.Authoring.WorkflowDesign" AccessModifier="Assembly" MinorVersion="2" ProductName="WorkflowDesign" CompanyName="NuPattern" PackageGuid="6370b1db-500b-44d0-82dd-a61eb6db992a" PackageNamespace="NuPattern.Authoring.WorkflowDesign" xmlns="http://schemas.microsoft.com/VisualStudio/2005/DslTools/DslDefinitionModel">
  <Classes>
    <DomainClass Id="19c7bdb0-d161-487c-bd32-38fb3f9bb023" Description="A requirement for a point of variability in the product line." Name="VariabilityRequirement" DisplayName="Variability Requirement" AccessModifier="Assembly" Namespace="NuPattern.Authoring.WorkflowDesign">
      <BaseClass>
        <DomainClassMoniker Name="DesignElement" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="0d2aea9a-7e9f-4ac3-9a59-53e050a1a9b0" Description="Determines whether the requirement is satisfied by one or more tools in the design." Name="IsSatisfiedByProductionTool" DisplayName="Is Satisfied By Production Tool" Kind="Calculated" Category="Design" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
      </Properties>
      <ElementMergeDirectives>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="VariabilityRequirement" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>ParentVariabilityRequirementHasParentVariabilityRequirements.VariabilityRequirements</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
      </ElementMergeDirectives>
    </DomainClass>
    <DomainClass Id="82cf2129-d420-41dc-8b95-12a99a5d20c9" Description="An asset that is produced by a production tool." Name="ProducedAsset" DisplayName="Produced Asset" AccessModifier="Assembly" Namespace="NuPattern.Authoring.WorkflowDesign">
      <BaseClass>
        <DomainClassMoniker Name="Asset" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="bfe0dc2d-98d8-4de6-a901-3bebc6f2ea7f" Description="Whether the asset is part of the final delivered product." Name="IsFinal" DisplayName="Is Deliverable" DefaultValue="false" Category="Design">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="67726e9a-a7da-4c26-b642-b4065710cee0" Description="A tool that is supplied assets that produces more assets." Name="ProductionTool" DisplayName="Production Tool" AccessModifier="Assembly" Namespace="NuPattern.Authoring.WorkflowDesign">
      <BaseClass>
        <DomainClassMoniker Name="DesignElement" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="86433396-f14f-4697-9a45-f0f7c95265ae" Description="The intended classification of this tool, defining how it processes and produces its assets." Name="Classification" DisplayName="Classification" DefaultValue="(undefined)" Category="Design">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="882ced98-c760-4937-a994-b5d229eb33e3" Description="Whether the tool satisfies any variability requirements." Name="IsSatisfyingVariability" DisplayName="Is Satisfying Variability" Kind="Calculated" Category="Design" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="a395e79e-1a6d-4132-9724-671c4413ddab" Description="An asset that is supplied to a production tool." Name="SuppliedAsset" DisplayName="Supplied Asset" AccessModifier="Assembly" Namespace="NuPattern.Authoring.WorkflowDesign">
      <BaseClass>
        <DomainClassMoniker Name="Asset" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="cb4d8e7b-c39a-4096-a1c3-b5f8b7be6765" Description="Whether this asset is supplied by the user as configuration to the product, or a supplied artifact as input to the product line." Name="IsUserSupplied" DisplayName="Is User Supplied" DefaultValue="false" Category="Design">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="245abfec-9096-464a-a73b-358748042100" Description="A supplied or fabricated asset in the product line." Name="Asset" DisplayName="Asset" InheritanceModifier="Abstract" AccessModifier="Assembly" Namespace="NuPattern.Authoring.WorkflowDesign">
      <BaseClass>
        <DomainClassMoniker Name="DesignElement" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="22610cd1-2a38-467c-8395-ccbe6f01d83e" Description="A reference to a known physical artifact that is represented by this asset." Name="SourceReference" DisplayName="Source Reference" Category="Design">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="24632330-dbd9-48f7-8535-230e3faafead" Description="Whether the asset is used as an input to a tool." Name="IsSuppliedToTool" DisplayName="Is Supplied To Tool" Kind="Calculated" Category="Design" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="fedd3e36-d9ec-4f70-85b9-16728700f201" Description="A production line design." Name="Design" DisplayName="Design" AccessModifier="Assembly" Namespace="NuPattern.Authoring.WorkflowDesign">
      <ElementMergeDirectives>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="ProductionTool" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>DesignHasProductionTools.ProductionTools</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="Asset" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>DesignHasAssets.Assets</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="VariabilityRequirement" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>DesignHasVariabilityRequirements.VariabilityRequirements</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
      </ElementMergeDirectives>
    </DomainClass>
    <DomainClass Id="6665aba9-d5b1-4412-9757-e5172874d63b" Description="An element that has a unique name." Name="NamedElementSchema" DisplayName="Named Element Schema" InheritanceModifier="Abstract" AccessModifier="Assembly" Namespace="NuPattern.Authoring.WorkflowDesign">
      <Properties>
        <DomainProperty Id="1264ecd0-db3c-4300-b15e-7cda16874ea4" Description="The well-known name of this item in this model." Name="Name" DisplayName="Name" Category="General" IsElementName="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="afaadae8-6ae4-43d6-afeb-5fb3968e78fe" Description="The identifier of the inherited variability model." Name="BaseId" DisplayName="Base Id" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="22a06ee5-1b83-40dd-8c3a-6b7dd9b29a62" Description="The name used for instances of this item, as seen by the user. Also used to name associated artifacts/configuration created for this item." Name="DisplayName" DisplayName="Display Name" Kind="CustomStorage" Category="General">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="c0d910ef-7fd4-4912-900f-729014f4afd4" Description="Used to track whether the user changed the display name manually." Name="IsDisplayNameTracking" DisplayName="Is Display Name Tracking" DefaultValue="true" GetterAccessModifier="Assembly" SetterAccessModifier="Private" IsBrowsable="false">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="62c5e1fa-d334-4235-9702-0af58b7f07e6" Description="The description of this item displayed to the user." Name="Description" DisplayName="Description" Category="General">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="3093952e-10f2-401d-a1c8-58b8b263ac3f" Description="Whether the element is derived from a base variability model definition." Name="IsInheritedFromBase" DisplayName="Is Inherited From Base" DefaultValue="false" Kind="Calculated" SetterAccessModifier="Private" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="865cf412-cfa1-4e9f-b6ec-bf02f69bea74" Description="An element used for realization." Name="DesignElement" DisplayName="Design Element" InheritanceModifier="Abstract" AccessModifier="Assembly" Namespace="NuPattern.Authoring.WorkflowDesign">
      <BaseClass>
        <DomainClassMoniker Name="NamedElementSchema" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="395aba29-4372-4244-8b5f-df13caa40432" Description="Design notes for this element." Name="DesignNotes" DisplayName="Design Notes" Category="Design">
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
        <DomainProperty Id="3c5e96bf-d2ae-4b91-9df1-00a4ead60049" Description="Whether to ignore this item. Ignored items will not be considered part of the current design." Name="IsIgnored" DisplayName="Ignore" DefaultValue="false" Category="Design">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
  </Classes>
  <Relationships>
    <DomainRelationship Id="91409b5f-6b3a-4a3d-a8b7-e347eccee8a6" Description="Description for NuPattern.Authoring.WorkflowDesign.ProductionToolReferencesVariabilityRequirements" Name="ProductionToolReferencesVariabilityRequirements" DisplayName="Production Tool References Variability Requirements" AccessModifier="Assembly" Namespace="NuPattern.Authoring.WorkflowDesign">
      <Source>
        <DomainRole Id="4c5bf749-6e50-4f84-9f9a-fd4329f1a2dc" Description="Description for NuPattern.Authoring.WorkflowDesign.ProductionToolReferencesVariabilityRequirements.ProductionTool" Name="ProductionTool" DisplayName="Production Tool" PropertyName="VariabilityRequirements" PropertyDisplayName="Variability Requirements">
          <RolePlayer>
            <DomainClassMoniker Name="ProductionTool" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="f42ef571-400e-416c-8e44-66a92e1a0afd" Description="Description for NuPattern.Authoring.WorkflowDesign.ProductionToolReferencesVariabilityRequirements.VariabilityRequirement" Name="VariabilityRequirement" DisplayName="Variability Requirement" PropertyName="ProductionTools" PropertyDisplayName="Production Tools">
          <RolePlayer>
            <DomainClassMoniker Name="VariabilityRequirement" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="4e0787f5-1b90-4962-af22-34b462ca1993" Description="Description for NuPattern.Authoring.WorkflowDesign.ProductionToolProducesProducedAssets" Name="ProductionToolProducesProducedAssets" DisplayName="Production Tool Produces Produced Assets" AccessModifier="Assembly" Namespace="NuPattern.Authoring.WorkflowDesign">
      <Source>
        <DomainRole Id="a410ff8e-d1ac-468f-927a-a97dbd2f981c" Description="Description for NuPattern.Authoring.WorkflowDesign.ProductionToolProducesProducedAssets.ProductionTool" Name="ProductionTool" DisplayName="Production Tool" PropertyName="ProducedProducedAssets" PropertyDisplayName="Produced Produced Assets">
          <RolePlayer>
            <DomainClassMoniker Name="ProductionTool" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="ea75f6de-6b2d-4b7b-9c5a-081104bf2fbd" Description="Description for NuPattern.Authoring.WorkflowDesign.ProductionToolProducesProducedAssets.ProducedAsset" Name="ProducedAsset" DisplayName="Produced Asset" PropertyName="ProducingProductionTools" PropertyDisplayName="Producing Production Tools">
          <RolePlayer>
            <DomainClassMoniker Name="ProducedAsset" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="b7c9e653-7d6f-4370-9ae1-d86964397b7d" Description="Description for NuPattern.Authoring.WorkflowDesign.ProducedAssetSuppliesProductionTools" Name="ProducedAssetSuppliesProductionTools" DisplayName="Produced Asset Supplies Production Tools" AccessModifier="Assembly" Namespace="NuPattern.Authoring.WorkflowDesign">
      <BaseRelationship>
        <DomainRelationshipMoniker Name="AssetSuppliesAllProducingTools" />
      </BaseRelationship>
      <Source>
        <DomainRole Id="41648a11-e24d-4726-a802-1e31e2e2d526" Description="Description for NuPattern.Authoring.WorkflowDesign.ProducedAssetSuppliesProductionTools.ProducedAsset" Name="ProducedAsset" DisplayName="Produced Asset" PropertyName="ProductionTools" PropertyDisplayName="Production Tools">
          <RolePlayer>
            <DomainClassMoniker Name="ProducedAsset" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="c7d26b7c-5f73-404f-9f97-dbe3e98bcf71" Description="Description for NuPattern.Authoring.WorkflowDesign.ProducedAssetSuppliesProductionTools.ProductionTool" Name="ProductionTool" DisplayName="Production Tool" PropertyName="SuppliedProducedAssets" PropertyDisplayName="Supplied Produced Assets">
          <RolePlayer>
            <DomainClassMoniker Name="ProductionTool" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="6103e751-0edc-4bde-9870-bcb0af30910b" Description="Description for NuPattern.Authoring.WorkflowDesign.SuppliedAssetCopiesToProducedAssets" Name="SuppliedAssetCopiesToProducedAssets" DisplayName="Supplied Asset Copies To Produced Assets" AccessModifier="Assembly" Namespace="NuPattern.Authoring.WorkflowDesign">
      <Source>
        <DomainRole Id="e16620b7-aae0-44a0-9b84-fcbc5a275f2d" Description="Description for NuPattern.Authoring.WorkflowDesign.SuppliedAssetCopiesToProducedAssets.SuppliedAsset" Name="SuppliedAsset" DisplayName="Supplied Asset" PropertyName="ProducedAssets" PropertyDisplayName="Produced Assets">
          <RolePlayer>
            <DomainClassMoniker Name="SuppliedAsset" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="202d77ab-7b4e-4139-b3d8-89167ee1b327" Description="Description for NuPattern.Authoring.WorkflowDesign.SuppliedAssetCopiesToProducedAssets.ProducedAsset" Name="ProducedAsset" DisplayName="Produced Asset" PropertyName="SuppliedAssets" PropertyDisplayName="Supplied Assets">
          <RolePlayer>
            <DomainClassMoniker Name="ProducedAsset" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="c884ef2a-69d3-4df7-9595-b270b3a54a5b" Description="Description for NuPattern.Authoring.WorkflowDesign.SuppliedAssetSuppliesProductionTools" Name="SuppliedAssetSuppliesProductionTools" DisplayName="Supplied Asset Supplies Production Tools" AccessModifier="Assembly" Namespace="NuPattern.Authoring.WorkflowDesign">
      <BaseRelationship>
        <DomainRelationshipMoniker Name="AssetSuppliesAllProducingTools" />
      </BaseRelationship>
      <Source>
        <DomainRole Id="9f26c80d-04ab-4b84-adfa-672fc88246d6" Description="Description for NuPattern.Authoring.WorkflowDesign.SuppliedAssetSuppliesProductionTools.SuppliedAsset" Name="SuppliedAsset" DisplayName="Supplied Asset" PropertyName="ProductionTools" PropertyDisplayName="Production Tools">
          <RolePlayer>
            <DomainClassMoniker Name="SuppliedAsset" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="8d8cfed6-33f9-45b1-8132-6b8791cfc218" Description="Description for NuPattern.Authoring.WorkflowDesign.SuppliedAssetSuppliesProductionTools.ProductionTool" Name="ProductionTool" DisplayName="Production Tool" PropertyName="SuppliedSuppliedAssets" PropertyDisplayName="Supplied Supplied Assets">
          <RolePlayer>
            <DomainClassMoniker Name="ProductionTool" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="dc20e99c-5f1d-4a9c-81af-eec621f4c3ec" Description="Description for NuPattern.Authoring.WorkflowDesign.AssetSuppliesAllProducingTools" Name="AssetSuppliesAllProducingTools" DisplayName="Asset Supplies All Producing Tools" InheritanceModifier="Abstract" AccessModifier="Assembly" Namespace="NuPattern.Authoring.WorkflowDesign">
      <Source>
        <DomainRole Id="6fe991a6-e1b0-4fc3-b36a-810bfefd791e" Description="Description for NuPattern.Authoring.WorkflowDesign.AssetSuppliesAllProducingTools.Asset" Name="Asset" DisplayName="Asset" PropertyName="AllProducingTools" PropertyDisplayName="All Producing Tools">
          <RolePlayer>
            <DomainClassMoniker Name="Asset" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="09210d13-2037-4f79-aeda-2e04e49ba3ca" Description="Description for NuPattern.Authoring.ComponentModel.AssetSuppliesAllProducingTools.ProductionTool" Name="ProductionTool" DisplayName="Production Tool" PropertyName="AllSuppliedAssets" PropertyDisplayName="All Supplied Assets">
          <RolePlayer>
            <DomainClassMoniker Name="ProductionTool" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="80b044c8-3478-4387-8ae4-b1f78fbd6475" Description="Description for NuPattern.Authoring.WorkflowDesign.DesignHasProductionTools" Name="DesignHasProductionTools" DisplayName="Design Has Production Tools" AccessModifier="Assembly" Namespace="NuPattern.Authoring.WorkflowDesign" IsEmbedding="true">
      <Source>
        <DomainRole Id="c5ee3106-0130-492a-86d8-0c3ef3532520" Description="Description for NuPattern.Authoring.WorkflowDesign.DesignHasProductionTools.Design" Name="Design" DisplayName="Design" PropertyName="ProductionTools" PropagatesCopy="PropagatesCopyToLinkAndOppositeRolePlayer" PropertyDisplayName="Production Tools">
          <RolePlayer>
            <DomainClassMoniker Name="Design" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="3f6e20bf-ff82-4b55-aafe-8f4930e47800" Description="Description for NuPattern.Authoring.WorkflowDesign.DesignHasProductionTools.ProductionTool" Name="ProductionTool" DisplayName="Production Tool" PropertyName="Design" Multiplicity="One" PropagatesDelete="true" PropertyDisplayName="Design">
          <RolePlayer>
            <DomainClassMoniker Name="ProductionTool" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="425ce70d-ec0b-4add-ab06-0368a05904e4" Description="Description for NuPattern.Authoring.WorkflowDesign.DesignHasAssets" Name="DesignHasAssets" DisplayName="Design Has Assets" AccessModifier="Assembly" Namespace="NuPattern.Authoring.WorkflowDesign" IsEmbedding="true">
      <Source>
        <DomainRole Id="915f6d0f-3ba1-4216-a0bf-37f21f50fcbf" Description="Description for NuPattern.Authoring.WorkflowDesign.DesignHasAssets.Design" Name="Design" DisplayName="Design" PropertyName="Assets" PropagatesCopy="PropagatesCopyToLinkAndOppositeRolePlayer" PropertyDisplayName="Assets">
          <RolePlayer>
            <DomainClassMoniker Name="Design" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="826f0f43-41df-4f3e-b574-faf6d13b69b6" Description="Description for NuPattern.Authoring.WorkflowDesign.DesignHasAssets.Asset" Name="Asset" DisplayName="Asset" PropertyName="Design" Multiplicity="One" PropagatesDelete="true" PropertyDisplayName="Design">
          <RolePlayer>
            <DomainClassMoniker Name="Asset" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="0c15cee8-4ab6-4fe8-8218-6ffca2d22dc9" Description="Description for NuPattern.Authoring.WorkflowDesign.DesignHasVariabilityRequirements" Name="DesignHasVariabilityRequirements" DisplayName="Design Has Variability Requirements" AccessModifier="Assembly" Namespace="NuPattern.Authoring.WorkflowDesign" IsEmbedding="true">
      <Source>
        <DomainRole Id="341c3a81-81ad-485d-82b4-17d5472f56e3" Description="Description for NuPattern.Authoring.WorkflowDesign.DesignHasVariabilityRequirements.Design" Name="Design" DisplayName="Design" PropertyName="VariabilityRequirements" PropagatesCopy="PropagatesCopyToLinkAndOppositeRolePlayer" PropertyDisplayName="Variability Requirements">
          <RolePlayer>
            <DomainClassMoniker Name="Design" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="bf7f9257-8bf9-470d-adef-a9922c3a93d5" Description="Description for NuPattern.Authoring.WorkflowDesign.DesignHasVariabilityRequirements.VariabilityRequirement" Name="VariabilityRequirement" DisplayName="Variability Requirement" PropertyName="Design" Multiplicity="ZeroOne" PropagatesDelete="true" PropertyDisplayName="Design">
          <RolePlayer>
            <DomainClassMoniker Name="VariabilityRequirement" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="60c1f1f1-e7cf-4ab5-927f-9864d9ba40e9" Description="Description for NuPattern.Authoring.WorkflowDesign.ParentVariabilityRequirementHasParentVariabilityRequirements" Name="ParentVariabilityRequirementHasParentVariabilityRequirements" DisplayName="Parent Variability Requirement Has Parent Variability Requirements" AccessModifier="Assembly" Namespace="NuPattern.Authoring.WorkflowDesign" IsEmbedding="true">
      <Source>
        <DomainRole Id="d938a687-573c-4f75-b7de-f8b2a6ecc53c" Description="Description for NuPattern.Authoring.WorkflowDesign.ParentVariabilityRequirementHasParentVariabilityRequirements.SourceVariabilityRequirement" Name="SourceVariabilityRequirement" DisplayName="Source Variability Requirement" PropertyName="VariabilityRequirements" PropagatesCopy="PropagatesCopyToLinkAndOppositeRolePlayer" PropertyDisplayName="Variability Requirements">
          <RolePlayer>
            <DomainClassMoniker Name="VariabilityRequirement" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="216e3b31-177b-40b6-b217-697beb5799f3" Description="Description for NuPattern.Authoring.WorkflowDesign.ParentVariabilityRequirementHasParentVariabilityRequirements.TargetVariabilityRequirement" Name="TargetVariabilityRequirement" DisplayName="Target Variability Requirement" PropertyName="ParentVariabilityRequirement" Multiplicity="ZeroOne" PropagatesDelete="true" PropertyDisplayName="Parent Variability Requirement">
          <RolePlayer>
            <DomainClassMoniker Name="VariabilityRequirement" />
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
    <ExternalType Name="Color" Namespace="System.Drawing" />
  </Types>
  <Shapes>
    <GeometryShape Id="38a6bf9c-4b83-43e7-b028-30eaa21d1ee8" Description="Description for NuPattern.Authoring.WorkflowDesign.ProductionToolShape" Name="ProductionToolShape" DisplayName="Production Tool Shape" AccessModifier="Assembly" Namespace="NuPattern.Authoring.WorkflowDesign" GeneratesDoubleDerived="true" FixedTooltipText="Production Tool" TextColor="White" FillColor="95, 167, 216" OutlineColor="White" InitialWidth="2" InitialHeight="0.6" OutlineThickness="0.03" FillGradientMode="None" Geometry="RoundedRectangle">
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0.6" VerticalOffset="0.2">
        <TextDecorator Name="StereotypeDecorator" DisplayName="Stereotype Decorator" DefaultText="Production Tool" FontSize="7" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0.6" VerticalOffset="0.05">
        <TextDecorator Name="NameDecorator" DisplayName="Name Decorator" DefaultText="NameDecorator" FontStyle="Bold" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0.34" VerticalOffset="0.3">
        <IconDecorator Name="IsSatisfyingVariabilityDecorator" DisplayName="Is Satisfying Variability Decorator" DefaultIcon="..\..\..\Images\BoundIndicator.png" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0.34" VerticalOffset="0.3">
        <IconDecorator Name="IsNotSatisfyingVariabilityDecorator" DisplayName="Is Not Satisfying Variability Decorator" DefaultIcon="..\..\..\Images\UnboundIndicator.png" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0.31" VerticalOffset="-0.01">
        <IconDecorator Name="IndicatorBackgroundDecorator" DisplayName="Indicator Background Decorator" DefaultIcon="..\..\..\Images\IndicatorBackground.png" />
      </ShapeHasDecorators>
    </GeometryShape>
    <GeometryShape Id="a2bcd210-b675-4fac-aa78-d57e942ad65d" Description="Description for NuPattern.Authoring.WorkflowDesign.ProducedAssetShape" Name="ProducedAssetShape" DisplayName="Produced Asset Shape" AccessModifier="Assembly" Namespace="NuPattern.Authoring.WorkflowDesign" GeneratesDoubleDerived="true" FixedTooltipText="Produced Asset" TextColor="White" FillColor="153, 102, 153" OutlineColor="White" InitialWidth="1.8" InitialHeight="0.6" OutlineThickness="0.03" FillGradientMode="None" Geometry="RoundedRectangle">
      <Properties>
        <DomainProperty Id="0ef25a03-abb8-49c9-a8bc-cd708055217c" Description="The color of a deliverable asset." Name="IsFinalColor" DisplayName="Is Final Color" DefaultValue="0, 52, 0" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
          <Notes>The color of the shape when final.</Notes>
          <Type>
            <ExternalTypeMoniker Name="/System.Drawing/Color" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="4c385efe-5352-4d20-9fd9-fa1059688bc6" Description="The color of a intermediate asset." Name="IsIntermediateColor" DisplayName="Is Intermediate Color" DefaultValue="153, 102, 153" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
          <Notes>The color of the shape when not final. </Notes>
          <Type>
            <ExternalTypeMoniker Name="/System.Drawing/Color" />
          </Type>
        </DomainProperty>
      </Properties>
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0.34" VerticalOffset="0.2">
        <TextDecorator Name="StereotypeDecorator" DisplayName="Stereotype Decorator" DefaultText="Produced Asset" FontSize="7" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0.34" VerticalOffset="0.05">
        <TextDecorator Name="NameDecorator" DisplayName="Name Decorator" DefaultText="NameDecorator" FontStyle="Bold" />
      </ShapeHasDecorators>
    </GeometryShape>
    <GeometryShape Id="b9afa446-5e10-4f35-9489-7bdd0cf307d7" Description="Description for NuPattern.Authoring.WorkflowDesign.SuppliedAssetShape" Name="SuppliedAssetShape" DisplayName="Supplied Asset Shape" AccessModifier="Assembly" Namespace="NuPattern.Authoring.WorkflowDesign" GeneratesDoubleDerived="true" FixedTooltipText="Supplied Asset" TextColor="White" FillColor="255, 105, 55" OutlineColor="White" InitialHeight="0.6" OutlineThickness="0.03" FillGradientMode="None" Geometry="RoundedRectangle">
      <Properties>
        <DomainProperty Id="65925450-d8e9-4144-a4c2-afa169b96fff" Description="The color of a user supplied asset." Name="IsUserSuppliedColor" DisplayName="Is User Supplied Color" DefaultValue="0, 51, 52" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
          <Notes>The color of the shape when final.</Notes>
          <Type>
            <ExternalTypeMoniker Name="/System.Drawing/Color" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="2a0d5d1c-43a9-4891-9468-a10fdb3876bd" Description="The color of a supplied material asset." Name="IsMaterialColor" DisplayName="Is Material Color" DefaultValue="255, 105, 55" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
          <Notes>The color of the shape when not final. </Notes>
          <Type>
            <ExternalTypeMoniker Name="/System.Drawing/Color" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="6154b2ef-a890-45bd-b187-415b472edb14" Description="The stereotype text of a user supplied asset." Name="IsUserSuppliedStereotypeText" DisplayName="Is User Supplied Stereotype Text" DefaultValue="User Supplied Asset" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="2ff72b1e-258e-4083-a18e-173c1dcc1f2a" Description="The stereotype text of a supplied material asset." Name="IsMaterialStereotypeText" DisplayName="Is Material Stereotype Text" DefaultValue="Supplied Material Asset" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="d3017221-0e65-4fb8-bc24-5e723ecf971c" Description="The text of the stereotype of this element." Name="StereotypeText" DisplayName="Stereotype Text" Kind="Calculated" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0.04" VerticalOffset="0.2">
        <TextDecorator Name="StereotypeDecorator" DisplayName="Stereotype Decorator" DefaultText="Supplied Asset" FontSize="7" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0.04" VerticalOffset="0.05">
        <TextDecorator Name="NameDecorator" DisplayName="Name Decorator" DefaultText="NameDecorator" FontStyle="Bold" />
      </ShapeHasDecorators>
    </GeometryShape>
  </Shapes>
  <Connectors>
    <Connector Id="1a58d220-af4d-465b-8aff-a62573131fd1" Description="Description for NuPattern.Authoring.WorkflowDesign.DesignConnector" Name="DesignConnector" DisplayName="Design Connector" InheritanceModifier="Abstract" AccessModifier="Assembly" Namespace="NuPattern.Authoring.WorkflowDesign" FixedTooltipText="Design Connector" TextColor="102, 102, 102" Color="102, 102, 102" TargetEndStyle="EmptyArrow" Thickness="0.03" />
    <Connector Id="a1da0e63-fd65-4168-b144-9e9d9ef08824" Description="Description for NuPattern.Authoring.WorkflowDesign.ToolInputConnector" Name="ToolInputConnector" DisplayName="Tool Input Connector" AccessModifier="Assembly" Namespace="NuPattern.Authoring.WorkflowDesign" FixedTooltipText="Tool Input Connector">
      <BaseConnector>
        <ConnectorMoniker Name="DesignConnector" />
      </BaseConnector>
      <ConnectorHasDecorators Position="SourceTop" OffsetFromShape="0.2" OffsetFromLine="0" isMoveable="true">
        <TextDecorator Name="ActionDecorator" DisplayName="Action Decorator" DefaultText="Supplies Input" />
      </ConnectorHasDecorators>
    </Connector>
    <Connector Id="bfe9b906-3fdc-4c62-9e64-27567c7a82a1" Description="Description for NuPattern.Authoring.WorkflowDesign.ToolOutputConnector" Name="ToolOutputConnector" DisplayName="Tool Output Connector" AccessModifier="Assembly" Namespace="NuPattern.Authoring.WorkflowDesign" FixedTooltipText="Tool Output Connector">
      <BaseConnector>
        <ConnectorMoniker Name="DesignConnector" />
      </BaseConnector>
      <ConnectorHasDecorators Position="TargetTop" OffsetFromShape="0.1" OffsetFromLine="0" isMoveable="true">
        <TextDecorator Name="ActionDecorator" DisplayName="Action Decorator" DefaultText="Produces" />
      </ConnectorHasDecorators>
    </Connector>
    <Connector Id="d5a4626d-0dd7-4d2b-8a95-7f4e9e14bec4" Description="Description for NuPattern.Authoring.WorkflowDesign.ToolCopyConnector" Name="ToolCopyConnector" DisplayName="Tool Copy Connector" AccessModifier="Assembly" Namespace="NuPattern.Authoring.WorkflowDesign" FixedTooltipText="Tool Copy Connector">
      <BaseConnector>
        <ConnectorMoniker Name="DesignConnector" />
      </BaseConnector>
      <ConnectorHasDecorators Position="SourceTop" OffsetFromShape="0.2" OffsetFromLine="0" isMoveable="true">
        <TextDecorator Name="ActionDecorator" DisplayName="Action Decorator" DefaultText="Copies To" />
      </ConnectorHasDecorators>
    </Connector>
  </Connectors>
  <XmlSerializationBehavior Name="WorkflowDesignSerializationBehavior" Namespace="NuPattern.Authoring.WorkflowDesign">
    <ClassData>
      <XmlClassData TypeName="WorkflowDesignDiagram" MonikerAttributeName="" SerializeId="true" MonikerElementName="workflowDesignDiagramMoniker" ElementName="workflowDesignDiagram" MonikerTypeName="WorkflowDesignDiagramMoniker">
        <DiagramMoniker Name="WorkflowDesignDiagram" />
        <ElementData>
          <XmlPropertyData XmlName="authoringGradientColor">
            <DomainPropertyMoniker Name="WorkflowDesignDiagram/AuthoringGradientColor" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="authoringBackgroundColor">
            <DomainPropertyMoniker Name="WorkflowDesignDiagram/AuthoringBackgroundColor" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="authoringTitleTextColor">
            <DomainPropertyMoniker Name="WorkflowDesignDiagram/AuthoringTitleTextColor" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="tailoringGradientColor">
            <DomainPropertyMoniker Name="WorkflowDesignDiagram/TailoringGradientColor" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="tailoringBackgroundColor">
            <DomainPropertyMoniker Name="WorkflowDesignDiagram/TailoringBackgroundColor" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="tailoringTitleTextColor">
            <DomainPropertyMoniker Name="WorkflowDesignDiagram/TailoringTitleTextColor" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="VariabilityRequirement" MonikerAttributeName="" SerializeId="true" MonikerElementName="variabilityRequirementMoniker" ElementName="variabilityRequirement" MonikerTypeName="VariabilityRequirementMoniker">
        <DomainClassMoniker Name="VariabilityRequirement" />
        <ElementData>
          <XmlPropertyData XmlName="isSatisfiedByProductionTool" Representation="Ignore">
            <DomainPropertyMoniker Name="VariabilityRequirement/IsSatisfiedByProductionTool" />
          </XmlPropertyData>
          <XmlRelationshipData UseFullForm="true" RoleElementName="variabilityRequirements">
            <DomainRelationshipMoniker Name="ParentVariabilityRequirementHasParentVariabilityRequirements" />
          </XmlRelationshipData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ProducedAsset" MonikerAttributeName="" SerializeId="true" MonikerElementName="producedAssetMoniker" ElementName="producedAsset" MonikerTypeName="ProducedAssetMoniker">
        <DomainClassMoniker Name="ProducedAsset" />
        <ElementData>
          <XmlPropertyData XmlName="isFinal">
            <DomainPropertyMoniker Name="ProducedAsset/IsFinal" />
          </XmlPropertyData>
          <XmlRelationshipData UseFullForm="true" RoleElementName="productionTools">
            <DomainRelationshipMoniker Name="ProducedAssetSuppliesProductionTools" />
          </XmlRelationshipData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ProductionTool" MonikerAttributeName="" SerializeId="true" MonikerElementName="productionToolMoniker" ElementName="productionTool" MonikerTypeName="ProductionToolMoniker">
        <DomainClassMoniker Name="ProductionTool" />
        <ElementData>
          <XmlPropertyData XmlName="classification">
            <DomainPropertyMoniker Name="ProductionTool/Classification" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="isSatisfyingVariability" Representation="Ignore">
            <DomainPropertyMoniker Name="ProductionTool/IsSatisfyingVariability" />
          </XmlPropertyData>
          <XmlRelationshipData UseFullForm="true" RoleElementName="variabilityRequirements">
            <DomainRelationshipMoniker Name="ProductionToolReferencesVariabilityRequirements" />
          </XmlRelationshipData>
          <XmlRelationshipData UseFullForm="true" RoleElementName="producedAssets">
            <DomainRelationshipMoniker Name="ProductionToolProducesProducedAssets" />
          </XmlRelationshipData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="SuppliedAsset" MonikerAttributeName="" SerializeId="true" MonikerElementName="suppliedAssetMoniker" ElementName="suppliedAsset" MonikerTypeName="SuppliedAssetMoniker">
        <DomainClassMoniker Name="SuppliedAsset" />
        <ElementData>
          <XmlPropertyData XmlName="isUserSupplied">
            <DomainPropertyMoniker Name="SuppliedAsset/IsUserSupplied" />
          </XmlPropertyData>
          <XmlRelationshipData UseFullForm="true" RoleElementName="producedAssets">
            <DomainRelationshipMoniker Name="SuppliedAssetCopiesToProducedAssets" />
          </XmlRelationshipData>
          <XmlRelationshipData UseFullForm="true" RoleElementName="productionTools">
            <DomainRelationshipMoniker Name="SuppliedAssetSuppliesProductionTools" />
          </XmlRelationshipData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="Asset" MonikerAttributeName="" SerializeId="true" MonikerElementName="assetMoniker" ElementName="asset" MonikerTypeName="AssetMoniker">
        <DomainClassMoniker Name="Asset" />
        <ElementData>
          <XmlPropertyData XmlName="sourceReference">
            <DomainPropertyMoniker Name="Asset/SourceReference" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="isSuppliedToTool" Representation="Ignore">
            <DomainPropertyMoniker Name="Asset/IsSuppliedToTool" />
          </XmlPropertyData>
          <XmlRelationshipData UseFullForm="true" RoleElementName="allProducingTools">
            <DomainRelationshipMoniker Name="AssetSuppliesAllProducingTools" />
          </XmlRelationshipData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ProductionToolReferencesVariabilityRequirements" MonikerAttributeName="" SerializeId="true" MonikerElementName="productionToolReferencesVariabilityRequirementsMoniker" ElementName="productionToolReferencesVariabilityRequirements" MonikerTypeName="ProductionToolReferencesVariabilityRequirementsMoniker">
        <DomainRelationshipMoniker Name="ProductionToolReferencesVariabilityRequirements" />
      </XmlClassData>
      <XmlClassData TypeName="ProductionToolProducesProducedAssets" MonikerAttributeName="" SerializeId="true" MonikerElementName="productionToolProducesProducedAssetsMoniker" ElementName="productionToolProducesProducedAssets" MonikerTypeName="ProductionToolProducesProducedAssetsMoniker">
        <DomainRelationshipMoniker Name="ProductionToolProducesProducedAssets" />
      </XmlClassData>
      <XmlClassData TypeName="Design" MonikerAttributeName="" SerializeId="true" MonikerElementName="designMoniker" ElementName="design" MonikerTypeName="DesignMoniker">
        <DomainClassMoniker Name="Design" />
        <ElementData>
          <XmlRelationshipData UseFullForm="true" RoleElementName="productionTools">
            <DomainRelationshipMoniker Name="DesignHasProductionTools" />
          </XmlRelationshipData>
          <XmlRelationshipData UseFullForm="true" RoleElementName="assets">
            <DomainRelationshipMoniker Name="DesignHasAssets" />
          </XmlRelationshipData>
          <XmlRelationshipData UseFullForm="true" RoleElementName="variabilityRequirements">
            <DomainRelationshipMoniker Name="DesignHasVariabilityRequirements" />
          </XmlRelationshipData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ProducedAssetSuppliesProductionTools" MonikerAttributeName="" SerializeId="true" MonikerElementName="producedAssetSuppliesProductionToolsMoniker" ElementName="producedAssetSuppliesProductionTools" MonikerTypeName="ProducedAssetSuppliesProductionToolsMoniker">
        <DomainRelationshipMoniker Name="ProducedAssetSuppliesProductionTools" />
      </XmlClassData>
      <XmlClassData TypeName="SuppliedAssetCopiesToProducedAssets" MonikerAttributeName="" SerializeId="true" MonikerElementName="suppliedAssetCopiesToProducedAssetsMoniker" ElementName="suppliedAssetCopiesToProducedAssets" MonikerTypeName="SuppliedAssetCopiesToProducedAssetsMoniker">
        <DomainRelationshipMoniker Name="SuppliedAssetCopiesToProducedAssets" />
      </XmlClassData>
      <XmlClassData TypeName="SuppliedAssetSuppliesProductionTools" MonikerAttributeName="" SerializeId="true" MonikerElementName="suppliedAssetSuppliesProductionToolsMoniker" ElementName="suppliedAssetSuppliesProductionTools" MonikerTypeName="SuppliedAssetSuppliesProductionToolsMoniker">
        <DomainRelationshipMoniker Name="SuppliedAssetSuppliesProductionTools" />
      </XmlClassData>
      <XmlClassData TypeName="NamedElementSchema" MonikerAttributeName="" SerializeId="true" MonikerElementName="namedElementSchemaMoniker" ElementName="namedElementSchema" MonikerTypeName="NamedElementSchemaMoniker">
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
          <XmlPropertyData XmlName="isDisplayNameTracking">
            <DomainPropertyMoniker Name="NamedElementSchema/IsDisplayNameTracking" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="description">
            <DomainPropertyMoniker Name="NamedElementSchema/Description" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="isInheritedFromBase" Representation="Ignore">
            <DomainPropertyMoniker Name="NamedElementSchema/IsInheritedFromBase" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="DesignElement" MonikerAttributeName="" SerializeId="true" MonikerElementName="designElementMoniker" ElementName="designElement" MonikerTypeName="DesignElementMoniker">
        <DomainClassMoniker Name="DesignElement" />
        <ElementData>
          <XmlPropertyData XmlName="designNotes">
            <DomainPropertyMoniker Name="DesignElement/DesignNotes" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="isIgnored">
            <DomainPropertyMoniker Name="DesignElement/IsIgnored" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="DesignConnector" MonikerAttributeName="" SerializeId="true" MonikerElementName="designConnectorMoniker" ElementName="designConnector" MonikerTypeName="DesignConnectorMoniker">
        <ConnectorMoniker Name="DesignConnector" />
      </XmlClassData>
      <XmlClassData TypeName="ToolInputConnector" MonikerAttributeName="" SerializeId="true" MonikerElementName="toolInputConnectorMoniker" ElementName="toolInputConnector" MonikerTypeName="ToolInputConnectorMoniker">
        <ConnectorMoniker Name="ToolInputConnector" />
      </XmlClassData>
      <XmlClassData TypeName="ToolOutputConnector" MonikerAttributeName="" SerializeId="true" MonikerElementName="toolOutputConnectorMoniker" ElementName="toolOutputConnector" MonikerTypeName="ToolOutputConnectorMoniker">
        <ConnectorMoniker Name="ToolOutputConnector" />
      </XmlClassData>
      <XmlClassData TypeName="ToolCopyConnector" MonikerAttributeName="" SerializeId="true" MonikerElementName="toolCopyConnectorMoniker" ElementName="toolCopyConnector" MonikerTypeName="ToolCopyConnectorMoniker">
        <ConnectorMoniker Name="ToolCopyConnector" />
      </XmlClassData>
      <XmlClassData TypeName="ProducedAssetShape" MonikerAttributeName="" SerializeId="true" MonikerElementName="producedAssetShapeMoniker" ElementName="producedAssetShape" MonikerTypeName="ProducedAssetShapeMoniker">
        <GeometryShapeMoniker Name="ProducedAssetShape" />
        <ElementData>
          <XmlPropertyData XmlName="isFinalColor">
            <DomainPropertyMoniker Name="ProducedAssetShape/IsFinalColor" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="isIntermediateColor">
            <DomainPropertyMoniker Name="ProducedAssetShape/IsIntermediateColor" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="SuppliedAssetShape" MonikerAttributeName="" SerializeId="true" MonikerElementName="suppliedAssetShapeMoniker" ElementName="suppliedAssetShape" MonikerTypeName="SuppliedAssetShapeMoniker">
        <GeometryShapeMoniker Name="SuppliedAssetShape" />
        <ElementData>
          <XmlPropertyData XmlName="isUserSuppliedColor">
            <DomainPropertyMoniker Name="SuppliedAssetShape/IsUserSuppliedColor" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="isMaterialColor">
            <DomainPropertyMoniker Name="SuppliedAssetShape/IsMaterialColor" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="isUserSuppliedStereotypeText">
            <DomainPropertyMoniker Name="SuppliedAssetShape/IsUserSuppliedStereotypeText" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="isMaterialStereotypeText">
            <DomainPropertyMoniker Name="SuppliedAssetShape/IsMaterialStereotypeText" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="stereotypeText" Representation="Ignore">
            <DomainPropertyMoniker Name="SuppliedAssetShape/StereotypeText" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ProductionToolShape" MonikerAttributeName="" SerializeId="true" MonikerElementName="productionToolShapeMoniker" ElementName="productionToolShape" MonikerTypeName="ProductionToolShapeMoniker">
        <GeometryShapeMoniker Name="ProductionToolShape" />
      </XmlClassData>
      <XmlClassData TypeName="AssetSuppliesAllProducingTools" MonikerAttributeName="" SerializeId="true" MonikerElementName="assetSuppliesAllProducingToolsMoniker" ElementName="assetSuppliesAllProducingTools" MonikerTypeName="AssetSuppliesAllProducingToolsMoniker">
        <DomainRelationshipMoniker Name="AssetSuppliesAllProducingTools" />
      </XmlClassData>
      <XmlClassData TypeName="DesignHasProductionTools" MonikerAttributeName="" SerializeId="true" MonikerElementName="designHasProductionToolsMoniker" ElementName="designHasProductionTools" MonikerTypeName="DesignHasProductionToolsMoniker">
        <DomainRelationshipMoniker Name="DesignHasProductionTools" />
      </XmlClassData>
      <XmlClassData TypeName="DesignHasAssets" MonikerAttributeName="" SerializeId="true" MonikerElementName="designHasAssetsMoniker" ElementName="designHasAssets" MonikerTypeName="DesignHasAssetsMoniker">
        <DomainRelationshipMoniker Name="DesignHasAssets" />
      </XmlClassData>
      <XmlClassData TypeName="DesignHasVariabilityRequirements" MonikerAttributeName="" SerializeId="true" MonikerElementName="designHasVariabilityRequirementsMoniker" ElementName="designHasVariabilityRequirements" MonikerTypeName="DesignHasVariabilityRequirementsMoniker">
        <DomainRelationshipMoniker Name="DesignHasVariabilityRequirements" />
      </XmlClassData>
      <XmlClassData TypeName="ParentVariabilityRequirementHasParentVariabilityRequirements" MonikerAttributeName="" SerializeId="true" MonikerElementName="parentVariabilityRequirementHasParentVariabilityRequirementsMoniker" ElementName="parentVariabilityRequirementHasParentVariabilityRequirements" MonikerTypeName="ParentVariabilityRequirementHasParentVariabilityRequirementsMoniker">
        <DomainRelationshipMoniker Name="ParentVariabilityRequirementHasParentVariabilityRequirements" />
      </XmlClassData>
    </ClassData>
  </XmlSerializationBehavior>
  <ExplorerBehavior Name="WorkflowDesignExplorer" />
  <ConnectionBuilders>
    <ConnectionBuilder Name="ProductionWorkflow">
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="ProducedAssetSuppliesProductionTools" />
        <SourceDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ProducedAsset" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </SourceDirectives>
        <TargetDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ProductionTool" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </TargetDirectives>
      </LinkConnectDirective>
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="ProductionToolReferencesVariabilityRequirements" />
        <SourceDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ProductionTool" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </SourceDirectives>
        <TargetDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="VariabilityRequirement" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </TargetDirectives>
      </LinkConnectDirective>
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="SuppliedAssetSuppliesProductionTools" />
        <SourceDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="SuppliedAsset" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </SourceDirectives>
        <TargetDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ProductionTool" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </TargetDirectives>
      </LinkConnectDirective>
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="SuppliedAssetCopiesToProducedAssets" />
        <SourceDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="SuppliedAsset" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </SourceDirectives>
        <TargetDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ProducedAsset" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </TargetDirectives>
      </LinkConnectDirective>
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="ProductionToolProducesProducedAssets" />
        <SourceDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ProductionTool" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </SourceDirectives>
        <TargetDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ProducedAsset" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </TargetDirectives>
      </LinkConnectDirective>
    </ConnectionBuilder>
  </ConnectionBuilders>
  <Diagram Id="1e053907-f125-4a0f-bc82-0cfe32cb32d6" Description="Description for NuPattern.Authoring.WorkflowDesign.WorkflowDesignDiagram" Name="WorkflowDesignDiagram" DisplayName="WorkflowDesignDiagram" AccessModifier="Assembly" Namespace="NuPattern.Authoring.WorkflowDesign" GeneratesDoubleDerived="true">
    <Properties>
      <DomainProperty Id="62daca27-748c-4918-9f33-938eefbf5361" Description="The color of the gradient in the title of the background." Name="AuthoringGradientColor" DisplayName="Authoring Gradient Color" DefaultValue="WhiteSmoke" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
        <Type>
          <ExternalTypeMoniker Name="/System.Drawing/Color" />
        </Type>
      </DomainProperty>
      <DomainProperty Id="5f31d4ab-e4f7-4a1e-a2fb-ccc547948537" Description="Description for NuPattern.Authoring.WorkflowDesign.WorkflowDesignDiagram.Authoring Background Color" Name="AuthoringBackgroundColor" DisplayName="Authoring Background Color" DefaultValue="White" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
        <Type>
          <ExternalTypeMoniker Name="/System.Drawing/Color" />
        </Type>
      </DomainProperty>
      <DomainProperty Id="49859ae8-64df-49be-aec0-0405d653e09a" Description="The color of the text of the diagram title." Name="AuthoringTitleTextColor" DisplayName="Authoring Title Text Color" DefaultValue="DarkGray" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
        <Type>
          <ExternalTypeMoniker Name="/System.Drawing/Color" />
        </Type>
      </DomainProperty>
      <DomainProperty Id="d4807ceb-9f5b-4d28-82bf-8ad5972c5249" Description="The color of the gradient in the title of the background." Name="TailoringGradientColor" DisplayName="Tailoring Gradient Color" DefaultValue="WhiteSmoke" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
        <Type>
          <ExternalTypeMoniker Name="/System.Drawing/Color" />
        </Type>
      </DomainProperty>
      <DomainProperty Id="7e3a2258-2ee4-49fc-9afe-96310c5ed58d" Description="Description for NuPattern.Authoring.WorkflowDesign.WorkflowDesignDiagram.Tailoring Background Color" Name="TailoringBackgroundColor" DisplayName="Tailoring Background Color" DefaultValue="204, 204, 204" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
        <Type>
          <ExternalTypeMoniker Name="/System.Drawing/Color" />
        </Type>
      </DomainProperty>
      <DomainProperty Id="310d7d2c-e3e2-4b53-a474-1e6bcc3f2747" Description="The color of the text of the diagram title." Name="TailoringTitleTextColor" DisplayName="Tailoring Title Text Color" DefaultValue="DarkGray" GetterAccessModifier="Assembly" SetterAccessModifier="Assembly" IsBrowsable="false" IsUIReadOnly="true">
        <Type>
          <ExternalTypeMoniker Name="/System.Drawing/Color" />
        </Type>
      </DomainProperty>
    </Properties>
    <Class>
      <DomainClassMoniker Name="Design" />
    </Class>
    <ShapeMaps>
      <ShapeMap>
        <DomainClassMoniker Name="ProductionTool" />
        <ParentElementPath>
          <DomainPath>DesignHasProductionTools.Design/!Design</DomainPath>
        </ParentElementPath>
        <DecoratorMap>
          <TextDecoratorMoniker Name="ProductionToolShape/NameDecorator" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElementSchema/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="ProductionToolShape/IsNotSatisfyingVariabilityDecorator" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="ProductionTool/IsSatisfyingVariability" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="False" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <DecoratorMap>
          <IconDecoratorMoniker Name="ProductionToolShape/IsSatisfyingVariabilityDecorator" />
          <VisibilityPropertyPath>
            <DomainPropertyMoniker Name="ProductionTool/IsSatisfyingVariability" />
            <PropertyFilters>
              <PropertyFilter FilteringValue="True" />
            </PropertyFilters>
          </VisibilityPropertyPath>
        </DecoratorMap>
        <GeometryShapeMoniker Name="ProductionToolShape" />
      </ShapeMap>
      <ShapeMap>
        <DomainClassMoniker Name="ProducedAsset" />
        <ParentElementPath>
          <DomainPath>DesignHasAssets.Design/!Design</DomainPath>
        </ParentElementPath>
        <DecoratorMap>
          <TextDecoratorMoniker Name="ProducedAssetShape/NameDecorator" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElementSchema/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <GeometryShapeMoniker Name="ProducedAssetShape" />
      </ShapeMap>
      <ShapeMap>
        <DomainClassMoniker Name="SuppliedAsset" />
        <ParentElementPath>
          <DomainPath>DesignHasAssets.Design/!Design</DomainPath>
        </ParentElementPath>
        <DecoratorMap>
          <TextDecoratorMoniker Name="ProducedAssetShape/NameDecorator" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElementSchema/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <TextDecoratorMoniker Name="ProducedAssetShape/StereotypeDecorator" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="SuppliedAssetShape/StereotypeText" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <TextDecoratorMoniker Name="SuppliedAssetShape/NameDecorator" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElementSchema/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <TextDecoratorMoniker Name="SuppliedAssetShape/StereotypeDecorator" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="SuppliedAssetShape/StereotypeText" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <GeometryShapeMoniker Name="SuppliedAssetShape" />
      </ShapeMap>
    </ShapeMaps>
    <ConnectorMaps>
      <ConnectorMap>
        <ConnectorMoniker Name="ToolCopyConnector" />
        <DomainRelationshipMoniker Name="SuppliedAssetCopiesToProducedAssets" />
      </ConnectorMap>
      <ConnectorMap>
        <ConnectorMoniker Name="ToolInputConnector" />
        <DomainRelationshipMoniker Name="SuppliedAssetSuppliesProductionTools" />
      </ConnectorMap>
      <ConnectorMap>
        <ConnectorMoniker Name="ToolInputConnector" />
        <DomainRelationshipMoniker Name="ProducedAssetSuppliesProductionTools" />
      </ConnectorMap>
      <ConnectorMap>
        <ConnectorMoniker Name="ToolOutputConnector" />
        <DomainRelationshipMoniker Name="ProductionToolProducesProducedAssets" />
      </ConnectorMap>
    </ConnectorMaps>
  </Diagram>
  <Designer CopyPasteGeneration="CopyPasteOnly" FileExtension="toolingdesign" EditorGuid="1bce7fd4-586e-42e0-a37f-c34b3a60c7e7" usesStickyToolboxItems="true">
    <RootClass>
      <DomainClassMoniker Name="Design" />
    </RootClass>
    <XmlSerializationDefinition RootNamespace="http://schemas.microsoft.com/visualstudio/patterning/authoring/workflowdesign" CustomPostLoad="false">
      <XmlSerializationBehaviorMoniker Name="WorkflowDesignSerializationBehavior" />
    </XmlSerializationDefinition>
    <ToolboxTab TabText="Production Tooling">
      <ElementTool Name="SuppliedAsset" ToolboxIcon="..\..\..\Images\SuppliedAssetShapeToolBitmap.bmp" Caption="Supplied Asset" Tooltip="Create a fixed or variable material asset or user supplied information, that is supplied as input to a production tool." HelpKeyword="SuppliedAsset">
        <DomainClassMoniker Name="SuppliedAsset" />
      </ElementTool>
      <ElementTool Name="ProductionTool" ToolboxIcon="..\..\..\Images\ProductionToolShapeToolBitmap.bmp" Caption="Production Tool" Tooltip="Create a production tool that accepts user supplied or material input assets, and produces intermediate or final assets." HelpKeyword="ProductionTool">
        <DomainClassMoniker Name="ProductionTool" />
      </ElementTool>
      <ElementTool Name="ProducedAsset" ToolboxIcon="..\..\..\Images\ProducedAssetShapeToolBitmap.bmp" Caption="Produced Asset" Tooltip="Produced Asset" HelpKeyword="ProducedAsset">
        <DomainClassMoniker Name="ProducedAsset" />
      </ElementTool>
      <ConnectionTool Name="ProductionWorkflowConnector" ToolboxIcon="..\..\..\Images\DesignConnectorToolBitmap.bmp" Caption="Connector" Tooltip="Connects supplied assets, produced assets and production tools." HelpKeyword="ProductionWorkflowConnector">
        <ConnectionBuilderMoniker Name="WorkflowDesign/ProductionWorkflow" />
      </ConnectionTool>
    </ToolboxTab>
    <Validation UsesMenu="true" UsesOpen="false" UsesSave="true" UsesLoad="false" />
    <DiagramMoniker Name="WorkflowDesignDiagram" />
  </Designer>
</Dsl>