using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Patterning.Extensibility;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema
{
    /// <summary>
    /// A name provider for all variable elements
    /// </summary>
    internal class NamedElementSchemaNameProvider : ElementNameProvider
    {
        public override void SetUniqueName(ModelElement element, ModelElement container, DomainRoleInfo embeddedDomainRole, string baseName)
        {
            Guard.NotNull(() => element, element);
            Guard.NotNull(() => embeddedDomainRole, embeddedDomainRole);
            Guard.NotNull(() => baseName, baseName);

            if (this.DomainProperty.PropertyType == typeof(string))
            {
                // Ensure name is unique across whole model for this kind of element
                base.SetUniqueNameCore(element, baseName, GetElementNames(element));
            }
            else
            {
                base.SetUniqueName(element, container, embeddedDomainRole, baseName);
            }
        }

        private IDictionary<string, ModelElement> GetElementNames(ModelElement element)
        {
            var names = new Dictionary<string, ModelElement>();
            element.Store.ElementDirectory.AllElements
                .Where(e => e.GetDomainClass().ImplementationClass == element.GetDomainClass().ImplementationClass)
                .Where(e => !e.Equals(element))
                .ForEach(e =>
                {
                    var name = (string)this.DomainProperty.GetValue(e);
                    if (!names.ContainsKey(name))
                    {
                        names.Add(name, e);
                    }
                });

            return names;
        }
    }
}
