using System;
using System.Reflection;
using Microsoft.VisualStudio.Modeling;

namespace NuPattern.Modeling
{
    /// <summary>
    /// Provides helper methods over <see cref="ElementFactory"/>.
    /// </summary>
    public static class ElementFactoryExtensions
    {
        /// <summary>
        /// Creates a new instance of the given element.
        /// </summary>
        public static TModelElement CreateElement<TModelElement>(this ElementFactory factory) where TModelElement : ModelElement
        {
            return CreateElement<TModelElement>(factory, string.Empty);
        }

        /// <summary>
        /// Creates a new instance of the given element, with the given name.
        /// </summary>
        public static TModelElement CreateElement<TModelElement>(this ElementFactory factory, string name) where TModelElement : ModelElement
        {
            Guard.NotNull(() => factory, factory);

            var field = typeof(TModelElement).GetField(
                "DomainClassId",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            if (field != null)
            {
                var id = (Guid)field.GetValue(null);
                var modelElement = factory.CreateElement(id) as TModelElement;

                if (!string.IsNullOrEmpty(name))
                {
                    string tryName;
                    if (DomainClassInfo.TryGetName(modelElement, out tryName))
                    {
                        DomainClassInfo.SetName(modelElement, name);
                    }
                }

                return modelElement;
            }

            return default(TModelElement);
        }
    }
}