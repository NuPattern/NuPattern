using System;
using System.ComponentModel;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Design;
using Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ComponentModel.Editors;
using NuPattern.ComponentModel.Design;
using NuPattern.ComponentModel.Design.UI;
using NuPattern.Reflection;

namespace NuPattern.Extensibility.Design
{
    /// <summary>
    /// Defines a picker editor for selecting types discoverd in the current solution.
    /// </summary>
    public class TypePicker : TypeSelectionEditor
    {
        /// <summary>
        /// Edits the value of the picker.
        /// </summary>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            Guard.NotNull(() => context, context);

            ITypeDescriptorContext delegatedContext;
            var attribute = ReflectionExtensions.GetCustomAttribute<EditorBaseTypeAttribute>(this.GetType(), true);

            if (attribute != null)
            {
                delegatedContext = new DelegatingTypeDescriptorContext(context, new DelegatingPropertyDescriptor(context.PropertyDescriptor, new[] { new BaseTypeAttribute(attribute.BaseType) }));
            }
            else
            {
                delegatedContext = new DelegatingTypeDescriptorContext(context);
            }

            return base.EditValue(delegatedContext, new AssemblyDiscoveryServiceProvider(provider, delegatedContext), value);
        }
    }
}
