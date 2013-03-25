using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.Modeling;
using NuPattern.Modeling;
using NuPattern.Reflection;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Customizations to the CustomizableSettings domain class.
    /// </summary>
    partial class CustomizableSettingSchemaBase
    {
        private bool defaultValue = true;
        private bool value = true;

        /// <summary>
        /// Disables customization of the setting.
        /// </summary>
        public void Disable()
        {
            // Ensure not customized, by resetting value
            if (this.IsEnabled == true)
            {
                this.Value = this.DefaultValue;
            }

            // Disable.
            this.IsEnabled = false;
        }

        /// <summary>
        /// Populates the customization settings for the given customizable element, 
        /// from the attributes on attributed DomainProperties and DomainRoles in this domain model.
        /// </summary>
        internal static void EnsurePolicyPopulated(CustomizableElementSchema element)
        {
            if (element == null)
            {
                return;
            }

            // Populate settings from domain properties which are attributed.
            PopulateFromOwnedDomainProperties(element);

            // Populate settings from domain roles which are attributed.
            PopulateFromEmbeddedRelationshipDomainRoleProperties(element);
        }

        /// <summary>
        /// Returns the value of the DefaultValue property.
        /// </summary>
        internal bool GetDefaultValueValue()
        {
            return this.defaultValue;
        }

        /// <summary>
        /// Sets the value of the DefaultValue property.
        /// </summary>
        internal void SetDefaultValueValue(bool newValue)
        {
            if (this.IsEnabled == false)
            {
                // Can't change defaultvalue when disabled.
                return;
            }
            else
            {
                this.defaultValue = newValue;
            }
        }

        /// <summary>
        /// Gets the value of the DomainElementSettingType property.
        /// </summary>
        internal CustomizableDomainElementSettingType GetDomainElementSettingTypeValue()
        {
            // Determine whether the current name is a DomainProperty or a DomainRole
            CustomizableElementSchema element = this.Policy.Owner;
            if (element != null)
            {
                DomainPropertyInfo domainPropertyInfo = element.GetDomainClass().FindDomainProperty(this.PropertyId, true);
                if (domainPropertyInfo != null)
                {
                    return CustomizableDomainElementSettingType.DomainProperty;
                }
                else
                {
                    // Must be a domain role property
                    return CustomizableDomainElementSettingType.DomainRole;
                }
            }

            return CustomizableDomainElementSettingType.DomainProperty;
        }

        /// <summary>
        /// Returns the value of the Value property.
        /// </summary>
        internal bool GetValueValue()
        {
            if (this.IsEnabled == true)
            {
                return this.value;
            }
            else
            {
                // Always return false when disabled.
                return false;
            }
        }

        /// <summary>
        /// Sets the value of the Value property.
        /// </summary>
        internal void SetValueValue(bool newValue)
        {
            if (this.IsEnabled == false)
            {
                // Can't change value when disabled.
                return;
            }
            else
            {
                this.value = newValue;
            }
        }

        /// <summary>
        /// Returns the value of the IsModified property.
        /// </summary>
        internal bool GetIsModifiedValue()
        {
            // Never consider changes if the setting is not enabled
            if (!this.IsEnabled)
            {
                return false;
            }

            return this.Value != this.DefaultValue;
        }

        /// <summary>
        /// Returns the value of the Caption property.
        /// </summary>
        internal string GetCaptionValue()
        {
            string propertyDisplayName = this.GetPropertyIdDisplayName();
            string displayedName = (!string.IsNullOrEmpty(propertyDisplayName)) ? propertyDisplayName : Properties.Resources.CustomizableSetting_DefaultCaption;

            return string.Format(CultureInfo.CurrentCulture, this.CaptionFormatter, displayedName);
        }

        /// <summary>
        /// Returns the value of the Description property.
        /// </summary>
        internal string GetDescriptionValue()
        {
            string propertyDisplayName = this.GetPropertyIdDisplayName();
            string displayedName = (!string.IsNullOrEmpty(propertyDisplayName)) ? propertyDisplayName : Properties.Resources.CustomizableSetting_DefaultCaption;

            return string.Format(CultureInfo.CurrentCulture, this.DescriptionFormatter, displayedName);
        }

        /// <summary>
        /// Returns the display name for the asasociated propertyId.
        /// </summary>
        internal string GetPropertyIdDisplayName()
        {
            if (this.Policy != null
                && this.Policy.Owner != null)
            {
                // Get the display name of the associated property from this element (search public, internal, provate properties)
                PropertyInfo propertyInfo = this.Policy.Owner.GetDomainClass().ImplementationClass.GetProperty(this.PropertyId,
                    (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.IgnoreCase));
                string displayName = propertyInfo.DisplayName();
                return (!string.IsNullOrEmpty(displayName)) ? displayName : propertyInfo.Name;
            }
            else
            {
                return this.PropertyId;
            }
        }

        /// <summary>
        /// Populates the collection of settings by searching for <see cref="CustomizableDomainElementSettingAttribute"/> attributes 
        /// on the domain properties of the given element.
        /// </summary>
        private static void PopulateFromOwnedDomainProperties(CustomizableElementSchema element)
        {
            DomainClassInfo currentElementInfo = element.GetDomainClass();

            // Traverse all domain properties
            IEnumerable<DomainPropertyInfo> domainPropertyInfos = currentElementInfo.AllDomainProperties;
            foreach (DomainPropertyInfo domainPropertyInfo in domainPropertyInfos)
            {
                PropertyInfo domainPropertyType = domainPropertyInfo.PropertyInfo;

                // Create the setting, if domain property is attributed
                CreateSettingForAttribute(element, domainPropertyType, null);
            }
        }

        /// <summary>
        /// Populates the collection of settings by searching for <see cref="CustomizableDomainElementSettingAttribute"/> attributes 
        /// on the embedded relationships of the given element.
        /// </summary>
        private static void PopulateFromEmbeddedRelationshipDomainRoleProperties(CustomizableElementSchema element)
        {
            DomainClassInfo currentElementInfo = element.GetDomainClass();

            // Traverse all embedded domain relationships
            IEnumerable<DomainRoleInfo> sourceDomainRoleInfos = currentElementInfo.AllDomainRolesPlayed.Where(role => role.IsEmbedding);
            foreach (DomainRoleInfo sourceDomainRole in sourceDomainRoleInfos)
            {
                PropertyInfo sourceRolePropertyType = sourceDomainRole.RolePlayer.ImplementationClass.GetProperty(sourceDomainRole.PropertyName);
                PropertyInfo attributedRelationshipPropertyType = sourceDomainRole.DomainRelationship.ImplementationClass.GetProperty(sourceDomainRole.Name);
                if (attributedRelationshipPropertyType != null)
                {
                    // Create the setting, if the property of the relationsip on the opposite side is attributed
                    CreateSettingForAttribute(element, attributedRelationshipPropertyType, sourceRolePropertyType);
                }
            }
        }

        /// <summary>
        /// Searches for the customizable attribute and if found creates a new setting uisng the name of the given member.
        /// </summary>
        private static void CreateSettingForAttribute(CustomizableElementSchema element, MemberInfo attributedMember, MemberInfo namedMember)
        {
            // Query the attributes of the instance, for a customization attribute.
            CustomizableDomainElementSettingAttribute[] attributes = (CustomizableDomainElementSettingAttribute[])attributedMember.GetCustomAttributes(typeof(CustomizableDomainElementSettingAttribute), true);
            if (attributes.Count() > 0)
            {
                var propertyId = (namedMember != null) ? namedMember.Name : attributedMember.Name;

                // Create a new corresponding setting (for the first attribute only: AllowMultiple=false)
                CustomizableDomainElementSettingAttribute attribute = attributes[0];
                if (!element.Policy.Settings.Any(s => s.PropertyId == propertyId))
                {
                    element.Store.TransactionManager.DoWithinTransaction(() =>
                    {
                        var setting = element.Policy.Create<CustomizableSettingSchema>();
                        setting.PropertyId = propertyId;
                        setting.DefaultValue = attribute.DefaultValue;
                    });
                }
            }
        }
    }
}
