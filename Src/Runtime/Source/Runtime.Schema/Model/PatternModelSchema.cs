using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Runtime.Schema.Properties;
using Microsoft.VisualStudio.Modeling;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema
{
    /// <summary>
    /// DomainClass PatternModelSchema
    /// The root in which all other elements are embedded. Appears as a diagram.
    /// </summary>
    public partial class PatternModelSchema
    {
        internal Func<PatternModelSchema, Version, PatternModelSchema, PatternModelCloner> ClonerFactory = (b, v, t) => new PatternModelCloner(b, v, t);

        /// <summary>
        /// Gets instances of a given type.
        /// </summary>
        public IEnumerable<T> FindAll<T>()
        {
            return this.Store.ElementDirectory.AllElements.OfType<T>();
        }

        /// <summary>
        /// Determines whether pattern line is being tailored or authored.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if in tailor mode; otherwise, <c>false</c>.
        /// </returns>
        public bool IsInTailorMode()
        {
            return !string.IsNullOrEmpty(this.BaseId);
        }

        /// <summary>
        /// Tailors the given pattern line, making this pattern line tailored.
        /// </summary>
        internal void Tailor(PatternModelSchema basePatternModel, Version baseVersion)
        {
            this.ClonerFactory(basePatternModel, baseVersion, this).Clone();
            this.ApplyTailoringCustomizationRules();
        }

        /// <summary>
        /// Applies the tailoring policies related to element customization based on the 
        /// existing policies set on the model, typically invoked after using the 
        /// <see cref="PatternModelCloner"/> to clone a base model for tailoring purposes.
        /// </summary>
        private void ApplyTailoringCustomizationRules()
        {
            if (!this.IsInTailorMode())
            {
                throw new InvalidOperationException(Resources.CannotApplyTailorRulesToNonTailored);
            }

            this.ApplyDisablePolicyRule();
            this.ApplyDisableSettingRule();
            this.ApplyFixupForValidationWarnings();
        }

        /// <summary>
        /// Returns a value indicating whether the source element represented by the
        /// specified root ProtoElement can be added to this element.
        /// </summary>
        /// <param name="rootElement">The root ProtoElement representing a source element.  This can be null,
        /// in which case the ElementGroupPrototype does not contain an ProtoElements
        /// and the code should inspect the ElementGroupPrototype context information.</param>
        /// <param name="elementGroupPrototype">The ElementGroupPrototype that contains the root ProtoElement.</param>
        /// <returns>
        /// True if the source element represented by the ProtoElement can be added to this target element.
        /// </returns>
        protected override bool CanMerge(ProtoElementBase rootElement, ElementGroupPrototype elementGroupPrototype)
        {
            Guard.NotNull(() => rootElement, rootElement);
            Guard.NotNull(() => elementGroupPrototype, elementGroupPrototype);

            if (rootElement != null)
            {
                var rootElementDomainInfo = this.Partition.DomainDataDirectory.GetDomainClass(rootElement.DomainClassId);

                if (rootElementDomainInfo.IsDerivedFrom(ElementSchema.DomainClassId) ||
                    rootElementDomainInfo.IsDerivedFrom(CollectionSchema.DomainClassId) ||
                    rootElementDomainInfo.IsDerivedFrom(ExtensionPointSchema.DomainClassId))
                {
                    return true;
                }
            }

            return base.CanMerge(rootElement, elementGroupPrototype);
        }

        /// <summary>
        /// Called by the Merge process to create a relationship between
        /// this target element and the specified source element.
        /// Typically, a parent-child relationship is established
        /// between the target element (the parent) and the source element
        /// (the child), but any relationship can be established.
        /// </summary>
        /// <param name="sourceElement">The element that is to be related to this model element.</param>
        /// <param name="elementGroup">The group of source ModelElements that have been rehydrated into the target state.</param>
        /// <remarks>
        /// This method is overriden to create the relationship between the target element and the specified source element.
        /// The base method does nothing.
        /// </remarks>
        protected override void MergeRelate(ModelElement sourceElement, ElementGroup elementGroup)
        {
            Guard.NotNull(() => sourceElement, sourceElement);
            Guard.NotNull(() => elementGroup, elementGroup);

            if (sourceElement is AbstractElementSchema)
            {
                var component = sourceElement as AbstractElementSchema;

                this.Store.GetCurrentDiagram().GetRepresentedView().Elements.Add(component);

                return;
            }
            else if (sourceElement is ExtensionPointSchema)
            {
                var component = sourceElement as ExtensionPointSchema;

                this.Store.GetCurrentDiagram().GetRepresentedView().ExtensionPoints.Add(component);

                return;
            }
            else if (sourceElement is ExtensionElement)
            {
                ////TODO: \o/ Remove this once DSL team fixes merging of MEXes
                return;
            }

            base.MergeRelate(sourceElement, elementGroup);
        }

        private void ApplyDisablePolicyRule()
        {
            /* 	Rule 1: Disable Policy rule
                § If, Element.Policy.IsCustomizable=false 
                § OR (Element.IsCustomizable=inherit
                § AND YoungestNonInheritingAncestor.IsCustomizable=false)
                § Then, Element.IsEnabled = false, AND all policy  settings.IsEnabled = false
            */
            var elementsToDisableCustomization = from element in this.Store.ElementDirectory.FindElements<CustomizableElementSchema>(true)
                                                 let relevantAncestor = element.FindYoungestNonInheritingAncestor()
                                                 where element.IsCustomizable == CustomizationState.False ||
                                                  (element.IsCustomizable == CustomizationState.Inherited &&
                                                      relevantAncestor != null &&
                                                      relevantAncestor.IsCustomizable == CustomizationState.False)
                                                 select element;

            foreach (var element in elementsToDisableCustomization)
            {
                element.DisableCustomization();
            }
        }

        private void ApplyDisableSettingRule()
        {
            /* 	Rule 2: Disable Setting rule
                    § If, (BaseToolkit).Element.Policy.Setting.Value =  false
                    § Then, (TailoredToolkit).Element.Policy.Setting.IsEnabled = false
            */
            var settingsToDisable = this.Store.ElementDirectory
                .FindElements<CustomizableSettingSchema>(true)
                .Where(setting => setting.Value == false);

            foreach (var setting in settingsToDisable)
            {
                setting.Disable();
            }
        }

        private void ApplyFixupForValidationWarnings()
        {
            this.ApplyFixupForCustomizedSettingsOverriddenByCustomizableState();
            this.ApplyFixupForCustomizableStateRedundant();
        }

        /// <summary>
        /// In sync with <see cref="CustomizableElementSchemaBase.ValidateCustomizedSettingsOverriddenByCustomizableState"/>.
        /// </summary>
        private void ApplyFixupForCustomizedSettingsOverriddenByCustomizableState()
        {
            var settingsToReset = from element in this.Store.ElementDirectory.FindElements<CustomizableElementSchema>(true)
                                  where element.IsCustomizationEnabled &&
                                        element.IsCustomizable == CustomizationState.False &&
                                        element.Policy.IsModified
                                  from setting in element.Policy.Settings
                                  where setting.IsModified
                                  select setting;

            foreach (var setting in settingsToReset)
            {
                setting.Value = setting.DefaultValue;
            }
        }

        /// <summary>
        /// In sync with <see cref="CustomizableElementSchemaBase.ValidateCustomizableStateRedundant"/>.
        /// </summary>
        private void ApplyFixupForCustomizableStateRedundant()
        {
            var elementsToReset = from element in this.Store.ElementDirectory.FindElements<CustomizableElementSchema>(true)
                                  where element.IsCustomizationEnabled &&
                                        element.IsCustomizable == CustomizationState.True
                                  let parent = element.GetParentCustomizationElement()
                                  where parent != null && parent.IsCustomizationPolicyModifyable
                                  select element;

            foreach (var element in elementsToReset)
            {
                element.IsCustomizable = CustomizationState.Inherited;
            }
        }
    }
}