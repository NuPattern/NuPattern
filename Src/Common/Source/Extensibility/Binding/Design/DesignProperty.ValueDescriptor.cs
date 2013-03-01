using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Design;
using NuPattern.Extensibility.Properties;

namespace NuPattern.Extensibility.Binding
{
    /// <summary>
    /// Property descriptor for the Value property of a <see cref="DesignProperty"/>.
    /// </summary>
    internal class DesignPropertyValueDescriptor : PropertyDescriptor
    {
        private Type propertyType;
        private TypeConverter converter;

        public DesignPropertyValueDescriptor(string name, Type propertyType, TypeConverter converter, Attribute[] attrs)
            : base(name, attrs)
        {
            this.propertyType = TypeDescriptor.GetProvider(propertyType).GetRuntimeType(propertyType);
            this.converter = converter;
        }

        public override bool CanResetValue(object component)
        {
            var value = this.GetValue(component);
            return (value == null) ? false : (!string.IsNullOrEmpty(value.ToString()));
        }

        public override Type ComponentType
        {
            get { return typeof(DesignProperty); }
        }

        public override TypeConverter Converter
        {
            get
            {
                //Fallback to constructor converter
                return this.FindCustomTypeConverter() ?? this.converter;
            }
        }

        public override object GetEditor(Type editorBaseType)
        {
            // Use design attribute first
            var designEditorAttribute = base.Attributes.OfType<DesignEditorAttribute>()
                .FirstOrDefault(attr => attr.BaseType == editorBaseType);
            if (designEditorAttribute != null)
            {
                return Activator.CreateInstance(designEditorAttribute.EditorType);
            }

            var editorAttribute = this.Attributes.OfType<EditorAttribute>().FirstOrDefault();
            if (editorAttribute != null)
            {
                var editorType = Type.GetType(editorAttribute.EditorTypeName);
                if (editorType != null)
                {
                    try
                    {
                        // Try editor for the type
                        return Activator.CreateInstance(editorType, this.PropertyType);
                    }
                    catch
                    {
                        if (editorType.GetConstructors().Any(c => c.GetParameters().Length == 0))
                        {
                            return Activator.CreateInstance(editorType);
                        }

                        return null;
                    }
                }
            }

            return base.GetEditor(editorBaseType);
        }

        public override bool SupportsChangeEvents
        {
            get { return true; }
        }

        public override object GetValue(object component)
        {
            var designProperty = (DesignProperty)component;
            return designProperty != null ? designProperty.GetValue() : null;
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override string Description
        {
            get { return Resources.DesignPropertyValueDescriptor_ValueDescription; }
        }

        public override string DisplayName
        {
            get { return Resources.DesignPropertyValueDescriptor_ValueDisplayName; }
        }

        public override Type PropertyType
        {
            get { return this.propertyType; }
        }

        public override void SetValue(object component, object value)
        {
            var designProperty = (DesignProperty)component;
            designProperty.SetValue(value);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }

        public override void ResetValue(object component)
        {
            SetValue(component, BindingSettings.Empty);
        }
    }
}