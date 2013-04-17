using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Runtime.Bindings;
using NuPattern.Runtime.Store.Properties;

namespace NuPattern.Runtime.Store.Design
{
    internal class PropertyPropertyDescriptor : PropertyDescriptor
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<PropertyPropertyDescriptor>();

        private Property property;
        private Type type;
        private IDynamicBindingContext bindingContext;
        private IDynamicBinding<IValueProvider> valueProvider;
        private bool bindingFailureReported;
        ITypeDescriptorContext descriptorContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyPropertyDescriptor"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="type">The type of the property</param>
        public PropertyPropertyDescriptor(Property property, Type type)
            : base(property.Info.Name, BuildAttributes(property.Info))
        {
            Guard.NotNull(() => property, property);
            Guard.NotNull(() => type, type);

            this.property = property;
            this.type = type;
            this.descriptorContext = new SimpleTypeDescriptorContext
            {
                Instance = this.property,
                PropertyDescriptor = this,
                ServiceProvider = this.property.Store,
            };
        }

        /// <summary>
        /// When overridden in a derived class, gets the type of the component this property is bound to.
        /// </summary>
        public override Type ComponentType
        {
            get
            {
                return typeof(ProductElement);
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether this property is read-only.
        /// </summary>
        public override bool IsReadOnly
        {
            get
            {
                return this.property.Info.IsReadOnly;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets the type of the property.
        /// </summary>
        public override Type PropertyType
        {
            get
            {
                return this.type;
            }
        }

        /// <summary>
        /// When overridden in a derived class, returns whether resetting an object changes its value.
        /// </summary>
        public override bool CanResetValue(object component)
        {
            return !Object.Equals(GetValue(component), GetDefaultValue(component));
            // return this.property.RawValue != this.property.Info.DefaultValue.Value;
        }

        /// <summary>
        /// When overridden in a derived class, gets the current value of the property on a component.
        /// </summary>
        public override object GetValue(object component)
        {
            // Note: this code has to be resilient to being called from the T4 appdomain. So 
            // we don't throw, we log warnings and have fallbacks.
            if (this.ValueProviderBinding != null)
            {
                if (this.ValueProviderBinding.Evaluate(this.bindingContext))
                {
                    bindingFailureReported = false;

                    return this.ValueProviderBinding.Value.Evaluate();
                }
                else if (!bindingFailureReported)
                {
                    tracer.TraceRecord(this, TraceEventType.Warning,
                        new
                        {
                            BindingResults = this.ValueProviderBinding.EvaluationResults
                        },
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Resources.PropertyPropertyDescriptor_ValueProviderBindingFailed,
                            this.property.Info.Parent.Name,
                            this.property.Info.Name,
                            this.property.Info.ValueProvider.TypeId));

                    bindingFailureReported = true;
                }
            }

            return this.Converter.ConvertFromString(this.property.RawValue);
        }

        /// <summary>
        /// When overridden in a derived class, resets the value for this property of the component to the default value.
        /// </summary>
        public override void ResetValue(object component)
        {
            // If we have a value provider, then we try to use that as the default value.
            if (this.property.Info != null && this.property.Info.DefaultValue != null)
            {
                if (this.property.Info.DefaultValue.ValueProvider != null)
                {
                    var bindingFactory = this.property.EnsureGetService<IBindingFactory>();
                    var binding = bindingFactory.CreateBinding<IValueProvider>(this.property.Info.DefaultValue.ValueProvider);
                    // Make the entire set of element interfaces available to VP bindings.
                    // We add the owner with its full interfaces. And we add the IProperty as well.
                    using (var context = binding.CreateDynamicContext())
                    {
                        context.AddExport<IProperty>(this.property);
                        context.AddExportsFromInterfaces(this.property.Owner);
                        if (binding.Evaluate(context))
                        {
                            // We go via the type descriptor, which performs any necessary type conversion.
                            var prop = TypeDescriptor.GetProperties(this.property.Owner)[this.property.Info.Name];
                            prop.SetValue(this.property.Owner, binding.Value.Evaluate());

                            return;
                        }
                        else
                        {
                            // TODO: should we trace the binding results or is it done already by the feature runtime?
                            tracer.TraceWarning(Resources.ElementMapper_DefaultValueProviderBindingFailed,
                                this.property.Owner.DefinitionName, this.property.DefinitionName, this.property.Info.DefaultValue.ValueProvider.TypeId);
                        }
                    }
                }

                // Either there was no value provider or we could not successfully evaluate it.
                // Set raw DefaultValue if any.
                // Could not evaluate binding, falling back to default value, if any.
                if (!string.IsNullOrEmpty(this.property.Info.DefaultValue.Value))
                    this.property.RawValue = ExpressionEvaluator.Evaluate(this.property.Owner, this.property.Info.DefaultValue.Value);
                else
                    SetDefaultForType();
            }
            else
            {
                SetDefaultForType();
            }

            this.OnValueChanged(component, EventArgs.Empty);
        }

        /// <summary>
        /// When overridden in a derived class, sets the value of the component to a different value.
        /// </summary>
        public override void SetValue(object component, object value)
        {
            if (value == null)
            {
                this.property.RawValue = string.Empty;
            }
            else
            {
                if (!IsValid(value))
                {
                    throw new ArgumentException(string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.PropertyPropertyDescriptor_InvalidPropertyValue,
                        value,
                        this.property.Owner.DefinitionName,
                        this.property.Info.Name,
                        this.PropertyType.FullName));
                }

                this.property.RawValue = this.Converter.ConvertToString(value);
            }

            this.OnValueChanged(component, EventArgs.Empty);
        }

        /// <summary>
        /// Always returns <see langword="true"/>.
        /// </summary>
        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }

        private object GetDefaultValue(object component)
        {
            // If we have a value provider, then we try to use that as the default value.
            if (this.property.Info != null && this.property.Info.DefaultValue != null)
            {
                if (this.property.Info.DefaultValue.ValueProvider != null)
                {
                    var bindingFactory = this.property.EnsureGetService<IBindingFactory>();
                    var binding = bindingFactory.CreateBinding<IValueProvider>(this.property.Info.DefaultValue.ValueProvider);
                    // Make the entire set of element interfaces available to VP bindings.
                    // We add the owner with its full interfaces. And we add the IProperty as well.
                    using (var context = binding.CreateDynamicContext())
                    {
                        context.AddExport<IProperty>(this.property);
                        context.AddExportsFromInterfaces(this.property.Owner);
                        if (binding.Evaluate(context))
                        {
                            return binding.Value.Evaluate();
                        }
                        else
                        {
                            // TODO: should we trace the binding results or is it done already by the feature runtime?
                            tracer.TraceWarning(Resources.ElementMapper_DefaultValueProviderBindingFailed,
                                this.property.Owner.DefinitionName, this.property.DefinitionName, this.property.Info.DefaultValue.ValueProvider.TypeId);
                        }
                    }
                }

                // Either there was no value provider or we could not successfully evaluate it.
                // Set raw DefaultValue if any.
                // Could not evaluate binding, falling back to default value, if any.
                if (!string.IsNullOrEmpty(this.property.Info.DefaultValue.Value))
                    return this.Converter.ConvertFromString(this.descriptorContext,
                        ExpressionEvaluator.Evaluate(this.property.Owner, this.property.Info.DefaultValue.Value));
                else
                    return GetDefaultForType();
            }
            else
            {
                return GetDefaultForType();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private bool IsValid(object value)
        {
            // If the value is directly assignable to the property type, 
            // no validation is performed. This is how the boolean converter 
            // works, for example. It will say IsValid == false for a boolean :S
            // So the guideline is: IsValid is called when conversion will be 
            // needed and only then. We have property validation rules to 
            // validate values of the type.
            if (!this.PropertyType.IsAssignableFrom(value.GetType()) &&
                !this.Converter.IsValid(value))
            {
                // Try to convert the value to workaround the bugs in a lot of converters
                // that don't implement the IsValid() method correctly. 
                // i.e. most of the basic System.ComponentModel.XXXConverters
                // This unfortunate workaround is not going to heed the IsValid() for converters
                // that are correctly implemented, where the value is truely invalid. We will
                // rely on Validation rules to verify the values.
                try
                {
                    this.Converter.ConvertToString(value);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return true;
        }

        private IDynamicBinding<IValueProvider> ValueProviderBinding
        {
            get
            {
                if (this.valueProvider == null &&
                    this.property.Info.ValueProvider != null &&
                    !string.IsNullOrEmpty(this.property.Info.ValueProvider.TypeId))
                {
                    var factory = this.property.Store.GetService<IBindingFactory>();
                    if (factory == null)
                    {
                        tracer.TraceWarning(string.Format(
                            CultureInfo.CurrentCulture,
                            Resources.Validation_RequiredServiceMissing,
                            typeof(IBindingFactory)));
                    }
                    else
                    {
                        this.valueProvider = factory.CreateBinding<IValueProvider>(this.property.Info.ValueProvider);
                        this.bindingContext = factory.CreateContext();
                        this.bindingContext.AddExport<IProperty>(this.property);
                        this.bindingContext.AddExportsFromInterfaces(this.property.Owner);
                    }
                }

                return this.valueProvider;
            }
        }

        private void SetDefaultForType()
        {
            if (this.PropertyType.IsNullable())
            {
                this.property.RawValue = string.Empty;
            }
            else if (this.PropertyType.IsNumber())
            {
                this.property.RawValue = "0";
            }
            else if (this.PropertyType.IsValueType)
            {
                // Instantiate a default value for the property type if it's a value type.
                // Going through the descriptor will do any necessary conversion to string.
                this.SetValue(this.property, Activator.CreateInstance(this.PropertyType));
            }
            else
            {
                // For reference types, we cannot specify null in the DSL storage, so we make it empty.
                this.property.RawValue = string.Empty;
            }
        }

        private object GetDefaultForType()
        {
            if (this.PropertyType.IsNullable())
            {
                return null;
            }
            else
            {
                try
                {
                    return Activator.CreateInstance(this.PropertyType);
                }
                catch (MissingMethodException)
                {
                    return null;
                }
            }
        }

        private static Attribute[] BuildAttributes(IPropertyInfo info)
        {
            var attributes = new List<Attribute>
            {
                new BrowsableAttribute(info.IsVisible),
                new ReadOnlyAttribute(info.IsReadOnly),
                new CategoryAttribute(info.Category),
                new DescriptionAttribute(info.Description),
                new DisplayNameAttribute(info.DisplayName), 
            };

            if (!string.IsNullOrEmpty(info.TypeConverterTypeName))
            {
                var type = FindType(info.TypeConverterTypeName);
                if (type != null)
                    attributes.Add(new TypeConverterAttribute(type.AssemblyQualifiedName));
                else
                    attributes.Add(new TypeConverterAttribute(info.TypeConverterTypeName));
            }

            if (!string.IsNullOrEmpty(info.EditorTypeName))
            {
                var type = FindType(info.EditorTypeName);
                if (type != null)
                    attributes.Add(new EditorAttribute(type.AssemblyQualifiedName, typeof(UITypeEditor)));
                else
                    attributes.Add(new EditorAttribute(info.EditorTypeName, typeof(UITypeEditor)));
            }

            return attributes.ToArray();
        }

        /// <summary>
        /// Attempts to find a type from loaded assemblies.
        /// </summary>
        private static Type FindType(string typeName)
        {
            var type = Type.GetType(typeName, false);
            if (type != null)
                return type;

            // If we don't have an assembly name after the type name, quit.
            if (typeName.IndexOf(',') == -1)
                return null;

            var asmName = typeName.Substring(typeName.IndexOf(',') + 1);
            // Remove potential version information from the assembly name.
            if (asmName.IndexOf(',') != -1)
                asmName = asmName.Substring(0, asmName.IndexOf(','));

            asmName = asmName.Trim();

            // Find by assembly simple name.
            var asm = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == asmName);
            if (asm == null)
                return null;

            // Grab type simple name (we already know we have an assembly name there), and 
            // try to match manually.
            var simpleName = typeName.Substring(0, typeName.IndexOf(',')).Trim();
            type = asm.GetTypes().FirstOrDefault(t => t.FullName == simpleName);

            return type;
        }
    }
}
