using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Design;
using NuPattern.Modeling;
using NuPattern.Runtime.Schema.Properties;
using NuPattern.VisualStudio;

namespace NuPattern.Runtime.Schema.Design
{
    internal class HostedExtensionPointPropertyDescriptor : PropertyDescriptor
    {
        private ExtensionPointSchema extensionPoint;
        private IUserMessageService messageService;

        public HostedExtensionPointPropertyDescriptor(PropertyDescriptor descriptor, ExtensionPointSchema extensionPoint, IUserMessageService messageService, params Attribute[] attributes)
            : base(descriptor, attributes)
        {
            this.extensionPoint = extensionPoint;
            this.messageService = messageService;
        }

        /// <summary>
        /// When overridden in a derived class, gets the type of the component this property is bound to.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.Type"/> that represents the type of component this property is bound to. When the <see cref="M:System.ComponentModel.PropertyDescriptor.GetValue(System.Object)"/> or <see cref="M:System.ComponentModel.PropertyDescriptor.SetValue(System.Object,System.Object)"/> methods are invoked, the object specified might be an instance of this type.</returns>
        public override Type ComponentType
        {
            get { return typeof(ExtensionPointSchema); }
        }

        /// <summary>
        /// When overridden in a derived class, gets the type of the property.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.Type"/> that represents the type of the property.</returns>
        public override Type PropertyType
        {
            get { return typeof(string); }
        }

        /// <summary>
        /// Gets an editor of the specified type.
        /// </summary>
        /// <param name="editorBaseType">The base type of editor, which is used to differentiate between multiple editors that a property supports.</param>
        /// <returns>
        /// An instance of the requested editor type, or null if an editor cannot be found.
        /// </returns>
        public override object GetEditor(Type editorBaseType)
        {
            return new StandardValuesEditor();
        }

        /// <summary>
        /// Gets the type converter for this property.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.ComponentModel.TypeConverter"/> that is used to convert the <see cref="T:System.Type"/> of this property.</returns>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode"/>
        /// </PermissionSet>
        public override TypeConverter Converter
        {
            get
            {
                // Filter out this extension point
                return new ExtensionPointConverter(
                    ext => !ext.RequiredExtensionPointId.Equals(this.extensionPoint.RequiredExtensionPointId));
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether this property is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the property is read-only; otherwise, false.</returns>
        public override bool IsReadOnly
        {
            get
            {
                return this.extensionPoint.IsInheritedFromBase;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets the current value of the property on a component.
        /// </summary>
        /// <param name="component">The component with the property for which to retrieve the value.</param>
        /// <returns>
        /// The value of a property for a given component.
        /// </returns>
        public override object GetValue(object component)
        {
            return this.extensionPoint.RepresentedExtensionPointId;
        }

        /// <summary>
        /// When overridden in a derived class, returns whether resetting an object changes its value.
        /// </summary>
        /// <param name="component">The component to test for reset capability.</param>
        /// <returns>
        /// True if resetting the component changes its value; otherwise, false.
        /// </returns>
        public override bool CanResetValue(object component)
        {
            return !this.extensionPoint.IsInheritedFromBase;
        }

        /// <summary>
        /// When overridden in a derived class, resets the value for this property of the component to the default value.
        /// </summary>
        /// <param name="component">The component with the property value that is to be reset to the default value.</param>
        public override void ResetValue(object component)
        {
            this.extensionPoint.Store.TransactionManager.DoWithinTransaction(() =>
            {
                this.extensionPoint.ClearAllProperties();
                this.extensionPoint.RepresentedExtensionPointId = string.Empty;
            });
        }

        /// <summary>
        /// When overridden in a derived class, sets the value of the component to a different value.
        /// </summary>
        /// <param name="component">The component with the property value that is to be set.</param>
        /// <param name="value">The new value.</param>
        public override void SetValue(object component, object value)
        {
            var extensionPointToHost = value as IExtensionPointSchema;
            if (extensionPointToHost == null)
            {
                throw new ArgumentException(Resources.HostedExtensionPointPropertyDescriptor_InvalidValue, "value");
            }

            // Warn user that they cannot host the self-extension point
            if (extensionPointToHost.RequiredExtensionPointId == this.extensionPoint.RequiredExtensionPointId)
            {
                this.messageService.ShowWarning(
                    string.Format(CultureInfo.InvariantCulture, Properties.Resources.HostedExtensionPointPropertyDescriptor_CantSelfHost));
                return;
            }

            // Warn user that they will lose any regular variable properties
            if (this.extensionPoint.Properties.Any(p => !p.IsInheritedFromBase))
            {
                var resume = this.messageService.PromptWarning(
                    string.Format(CultureInfo.InvariantCulture, Properties.Resources.HostedExtensionPointPropertyDescriptor_PropertiesWillBeDeleted));
                if (!resume)
                {
                    return;
                }
            }

            // Add contract properties
            this.extensionPoint.Store.TransactionManager.DoWithinTransaction(() =>
            {
                this.extensionPoint.ClearAllProperties();
                this.extensionPoint.CopyHostedContractProperties(extensionPointToHost);
                this.extensionPoint.RepresentedExtensionPointId = extensionPointToHost.RequiredExtensionPointId;
            });
        }

        /// <summary>
        /// When overridden in a derived class, determines a value indicating whether the value of this property needs to be persisted.
        /// </summary>
        /// <param name="component">The component with the property to be examined for persistence.</param>
        /// <returns>
        /// True if the property should be persisted; otherwise, false.
        /// </returns>
        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }
    }
}