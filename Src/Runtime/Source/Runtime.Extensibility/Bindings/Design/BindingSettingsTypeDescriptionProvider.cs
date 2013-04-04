using System;
using System.ComponentModel;

namespace NuPattern.Runtime.Bindings.Design
{
    /// <summary>
    /// Custom type description provider that can be associated with a binding settings class 
    /// to provide dynamic properties based on the binding type id.
    /// </summary>
    /// <typeparam name="TComponents">Type of the binding that will be discovered via the standard mechanisms of MEF and project types</typeparam>
    public class BindingSettingsTypeDescriptionProvider<TComponents> : TypeDescriptionProvider where TComponents : class
    {
        private static readonly TypeDescriptionProvider parent = TypeDescriptor.GetProvider(typeof(BindingSettings));

        /// <summary>
        /// Gets a custom type descriptor for the given type and object.
        /// </summary>
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return new BindingSettingsTypeDescriptor(parent.GetTypeDescriptor(objectType, instance), instance as BindingSettings);
        }

        private class BindingSettingsTypeDescriptor : DesignComponentTypeDescriptor<TComponents, BindingSettings>
        {
            private BindingSettings settings;

            public BindingSettingsTypeDescriptor(ICustomTypeDescriptor parent, BindingSettings settings)
                : base(parent)
            {
                this.settings = settings;
            }

            /// <summary>
            /// Returns the name of the current extension.
            /// </summary>
            /// <returns></returns>
            protected override string GetExtensionName()
            {
                return this.settings.TypeId;
            }
        }
    }
}
