using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using NuPattern.Library.Automation;
using NuPattern.Runtime.Bindings.Design;

namespace NuPattern.Library.Design
{
    /// <summary>
    /// Command references type converter
    /// </summary>
    internal class CommandReferencesConverter : DesignCollectionConverter<CommandReference>
    {
        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var baseResult = base.ConvertFrom(context, culture, value);
            var references = baseResult as Collection<CommandReference>;
            if (references != null)
            {
                // Assign commandSettings (design-time only)
                if (context != null && context.Instance != null)
                {
                    var settings = (ICommandSettings)context.Instance;

                    var values = (Collection<CommandReference>)baseResult;
                    return values.Select(val => new CommandReference(settings) { CommandId = val.CommandId });
                }

                return references;
            }

            return baseResult;
        }
    }
}