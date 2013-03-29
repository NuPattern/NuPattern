using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using NuPattern.Runtime.ToolkitInterface;
using NuPattern.VisualStudio;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Implements the default <see cref="IPropertyEvaluator"/> behavior.
    /// </summary>
    public class PropertyEvaluator : IPropertyEvaluator
    {
        /// <summary>
        /// Evaluates the property for the given target object.
        /// </summary>
        /// <returns>
        /// The property value or <see langword="null"/> if the property is not found
        /// </returns>
        public object Evaluate(object target, string propertyName)
        {
            if (target == null)
                return null;

            var property = TypeDescriptor.GetProperties(target)[propertyName];
            if (property != null)
                return property.GetValue(target);

            // Try via toolkit interface.
            var element = target as IProductElement;
            if (element != null)
            {
                var interfaceService = element.Root.ProductState.TryGetService<IToolkitInterfaceService>();
                if (interfaceService != null)
                {
                    var definitionId = Guid.Empty;
                    var proxyType = interfaceService.AllInterfaces
                        .Where(export => !string.IsNullOrEmpty(export.Metadata.DefinitionId) && Guid.TryParse(export.Metadata.DefinitionId, out definitionId))
                        .Where(export =>
                            export.Metadata.ExtensionId == element.Info.GetProduct().ExtensionId &&
                            definitionId == element.DefinitionId)
                        .Select(export => export.Metadata.ProxyType)
                        .FirstOrDefault();

                    if (proxyType != null)
                    {
                        try
                        {
                            var proxy = Activator.CreateInstance(proxyType, element);
                            var proxyDesc = TypeDescriptor.GetProperties(target)[propertyName];
                            if (proxyDesc != null)
                                return proxyDesc.GetValue(proxy);

                            // Try via reflection.
                            var proxyProp = proxy.GetType().GetProperty(propertyName);
                            if (proxyProp != null)
                                return proxyProp.GetValue(proxy, null);
                        }
                        catch (TargetInvocationException) { }
                    }
                }
            }

            // Try via reflection.
            var propInfo = target.GetType().GetProperty(propertyName);
            if (propInfo != null)
                return propInfo.GetValue(target, null);

            // TODO: this is compatible with the previous behavior.
            // See if we can make it return null instead. Need to 
            // account for the fact that the DSL state cannot hold 
            // null value for properties (the Property.RawValue is 
            // a string).
            return string.Empty;
        }
    }
}
