using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Design;
using NuPattern.Extensibility;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Deefines a type descriptor over a design property value.
    /// </summary>
    public class DesignPropertyValueDescriptor : PropertyDescriptor
    {
        private Type propertyType;
		private TypeConverter converter;

        /// <summary>
        /// Initializes a new instance of the <see cref="DesignPropertyValueDescriptor"/> class.
        /// </summary>
        /// <param name="name">The name of the property value.</param>
        /// <param name="propertyType">Type of the property.</param>
        /// <param name="converter">Type converter to use.</param>
        /// <param name="attributes">The attributes.</param>
        public DesignPropertyValueDescriptor(string name, Type propertyType, TypeConverter converter, Attribute[] attributes)
            : base(name, attributes)
        {
            this.propertyType = propertyType;
			this.converter = converter;
        }

        /// <summary>
        /// When overridden in a derived class, gets the type of the component this property is bound to.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.Type"/> that represents the type of component this property is bound to. When the <see cref="M:System.ComponentModel.PropertyDescriptor.GetValue(System.Object)"/> or <see cref="M:System.ComponentModel.PropertyDescriptor.SetValue(System.Object,System.Object)"/> methods are invoked, the object specified might be an instance of this type.</returns>
        public override Type ComponentType
        {
            get { return typeof(DesignProperty); }
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
				//Fallback to constructor converter
				return this.FindCustomTypeConverter() ?? this.converter;
            }
        }

        /// <summary>
        /// Gets a value indicating whether value change notifications for this property may originate from outside the property descriptor.
        /// </summary>
        /// <value></value>
        /// <returns>true if value change notifications may originate from outside the property descriptor; otherwise, false.</returns>
        public override bool SupportsChangeEvents
        {
            get { return true; }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether this property is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the property is read-only; otherwise, false.</returns>
        public override bool IsReadOnly
        {
            get { return false; }
        }


        /// <summary>
        /// Gets the description of the member, as specified in the <see cref="T:System.ComponentModel.DescriptionAttribute"/>.
        /// </summary>
        /// <value></value>
        /// <returns>The description of the member. If there is no <see cref="T:System.ComponentModel.DescriptionAttribute"/>, the property value is set to the default, which is an empty string ("").</returns>
        public override string Description
        {
            get
            {
                return NuPattern.Library.Properties.Resources.DesignValueProvider_ValueDescription;
            }
        }

        /// <summary>
        /// Gets the name that can be displayed in a window, such as a Properties window.
        /// </summary>
        /// <value></value>
        /// <returns>The name to display for the member.</returns>
        public override string DisplayName
        {
            get { return NuPattern.Library.Properties.Resources.DesignValueProvider_ValueDisplayName; }
        }

        /// <summary>
        /// When overridden in a derived class, gets the type of the property.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.Type"/> that represents the type of the property.</returns>
        public override Type PropertyType
        {
            get { return this.propertyType; }
        }

        /// <summary>
        /// When overridden in a derived class, returns whether resetting an object changes its value.
        /// </summary>
        /// <param name="component">The component to test for reset capability.</param>
        public override bool CanResetValue(object component)
        {
            return true;
        }

        /// <summary>
        /// Gets an editor of the specified type.
        /// </summary>
        /// <param name="editorBaseType">The base type of editor, which is used to differentiate between multiple editors that a property supports.</param>
        /// <returns>
        /// An instance of the requested editor type, or null if an editor cannot be found.
        /// </returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public override object GetEditor(Type editorBaseType)
        {
            var designEditorAttribute = this.Attributes.OfType<DesignEditorAttribute>().FirstOrDefault(attr => attr.BaseType == editorBaseType);

            if (designEditorAttribute != null)
            {
                return Activator.CreateInstance(designEditorAttribute.EditorType);
            }

            //Try  property type editor
            var editorAttribute = this.Attributes.OfType<EditorAttribute>().FirstOrDefault();

            if (editorAttribute != null)
            {
                var editorType = Type.GetType(editorAttribute.EditorTypeName);

                if (editorType != null)
                {
                    try
                    {
                        return (UITypeEditor)Activator.CreateInstance(editorType, this.PropertyType);
                    }
                    catch
                    {
                        if (editorType.GetConstructors().Any(c => c.GetParameters().Length == 0))
                        {
                            return (UITypeEditor)Activator.CreateInstance(editorType);
                        }

                        return null;
                    }
                }
            }

            //Fallback to property type
            return base.GetEditor(editorBaseType);
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
            var designProperty = (DesignProperty)component;
            return designProperty != null ? designProperty.Value : null;
        }

        /// <summary>
        /// When overridden in a derived class, sets the value of the component to a different value.
        /// </summary>
        /// <param name="component">The component with the property value that is to be set.</param>
        /// <param name="value">The new value.</param>
        public override void SetValue(object component, object value)
        {
            var designProperty = (DesignProperty)component;
            designProperty.Value = value;
        }

        /// <summary>
        /// When overridden in a derived class, determines a value indicating whether the value of this property needs to be persisted.
        /// </summary>
        /// <param name="component">The component with the property to be examined for persistence.</param>
        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }

        /// <summary>
        /// When overridden in a derived class, resets the value for this property of the component to the default value.
        /// </summary>
        /// <param name="component">The component with the property value that is to be reset to the default value.</param>
        public override void ResetValue(object component)
        {
        }
    }
}