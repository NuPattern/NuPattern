using System;
using System.ComponentModel;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace NuPattern.Extensibility.Binding
{
    /// <summary>
    /// Defines a type description provider over <see cref="DesignValueProvider"/>.
    /// </summary>
    internal class DesignValueProviderTypeDescriptionProvider : TypeDescriptionProvider
    {
        private static TypeDescriptionProvider parent = TypeDescriptor.GetProvider(typeof(DesignValueProvider));

        ///// <summary>
        ///// Creates a new instance of the <see cref="DesignValueProviderTypeDescriptionProvider"/> class.
        ///// </summary>
        //public DesignValueProviderTypeDescriptionProvider()
        //    : base(parent)
        //{
        //}

        /// <summary>
        /// Returns the type descriptor for the object type.
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return new DesignValueProviderTypeDescriptor(parent.GetTypeDescriptor(objectType, instance), instance as DesignValueProvider);
        }

        private class DesignValueProviderTypeDescriptor : DesignComponentTypeDescriptor<IValueProvider, DesignValueProvider>
        {
            private DesignValueProvider designValueProvider;

            /// <summary>
            /// Creates a new instance of the <see cref="DesignValueProviderTypeDescriptor"/> class.
            /// </summary>
            /// <param name="parent">The parent.</param>
            /// <param name="valueProvider">The value provider.</param>
            public DesignValueProviderTypeDescriptor(ICustomTypeDescriptor parent, DesignValueProvider valueProvider)
                : base(parent)
            {
                this.designValueProvider = valueProvider;
            }

            /// <summary>
            /// Returns the name of the current extension.
            /// </summary>
            /// <returns></returns>
            protected override string GetExtensionName()
            {
                return this.designValueProvider.Name;
            }
        }
    }
}