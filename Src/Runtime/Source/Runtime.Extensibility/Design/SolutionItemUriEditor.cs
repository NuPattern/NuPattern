using System;
using System.ComponentModel;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Runtime.Design
{
    /// <summary>
    /// Representes the editor to pick a solution item and return its Uri.
    /// </summary>
    [CLSCompliant(false)]
    public class SolutionItemUriEditor : SolutionItemEditor
    {
        /// <summary>
        /// Converts the value to the destination type.
        /// </summary>
        protected override object ConvertValue(ITypeDescriptorContext context, IServiceProvider provider, IItemContainer value)
        {
            if (value != null)
            {
                var uriService = provider.GetService<IUriReferenceService>();
                return uriService.CreateUri(value, SolutionUri.UriScheme);
            }

            return null;
        }
    }
}