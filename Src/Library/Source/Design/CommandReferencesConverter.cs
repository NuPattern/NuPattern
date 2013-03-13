using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using NuPattern.Extensibility.Binding;
using NuPattern.Library.Automation;

namespace NuPattern.Library.Design
{
    /// <summary>
    /// Command references type converter
    /// </summary>
    public class CommandReferencesConverter : DesignCollectionConverter<Collection<CommandReference>, CommandReference>
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
                var settings = (ICommandSettings)context.Instance;

                var values = (Collection<CommandReference>)baseResult;
                return values.Select(val => new CommandReference(settings) { CommandId = val.CommandId });
            }

            return baseResult;
        }
    }
}