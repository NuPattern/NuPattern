using System.Linq;
using Microsoft.VisualStudio.Modeling;
using NuPattern.Diagnostics;
using NuPattern.Runtime.Store.Properties;

namespace NuPattern.Runtime.Store
{
    /// <summary>
    /// Triggers this notification rule whether a <see cref="Product"/> is added.
    /// </summary>
    [RuleOn(typeof(Product), FireTime = TimeToFire.LocalCommit)]
    internal class ProductAddRule : AddRule
    {
        private static readonly ITracer tracer = Tracer.Get<ProductAddRule>();

        /// <summary>
        /// Triggers this notification rule whether a <see cref="Product"/> is added.
        /// </summary>
        /// <param name="e">The provided data for this event.</param>
        public override void ElementAdded(ElementAddedEventArgs e)
        {
            Guard.NotNull(() => e, e);

            var product = (Product)e.ModelElement;

            var productInfo = FindInfo(product);
            if (productInfo != null)
            {
                product.Info = productInfo;
                product.SyncPropertiesFrom(productInfo.Properties);
                product.SyncViewsFrom(productInfo.Views);
            }
            else
            {
                tracer.Warn(Properties.Resources.TracerWarning_ProductInfoNotFound, product.Id);
            }
        }

        private static IPatternInfo FindInfo(Product product)
        {
            var patternManager = product.Store.TryGetService<IPatternManager>();
            if (patternManager != null)
            {
                var toolkitInfo = patternManager.InstalledToolkits.FirstOrDefault(tk => tk.ContainsSchema(product));
                if (toolkitInfo != null)
                {
                    //TODO: Return unique instance (and cache) of info for parented products for same parent instance
                    return toolkitInfo.Schema.Pattern;
                }
            }

            tracer.Warn(Resources.TracerWarning_ToolkitInfoNotFound, product.ExtensionId, product.InstanceName);

            return null;
        }
    }
}