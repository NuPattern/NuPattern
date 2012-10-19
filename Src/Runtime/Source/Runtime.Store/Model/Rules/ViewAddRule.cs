using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;

namespace Microsoft.VisualStudio.Patterning.Runtime.Store
{
    /// <summary>
    /// Triggers this notification rule whether a <see cref="Product"/> is added.
    /// </summary>
    [RuleOn(typeof(View), FireTime = TimeToFire.LocalCommit)]
    public partial class ViewAddRule : AddRule
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<ViewAddRule>();

        /// <summary>
        /// Triggers this notification rule whether a <see cref="Product"/> is added.
        /// </summary>
        /// <param name="e">The provided data for this event.</param>
        public override void ElementAdded(ElementAddedEventArgs e)
        {
            Guard.NotNull(() => e, e);

            var view = (View)e.ModelElement;

            var info = FindInfo(view.Product, view.DefinitionId);
            if (info != null)
            {
                view.Info = info;

                // Create and cache the interface layer proxy.
                var layer = view.GetInterfaceLayer();
                var layerInit = layer as ISupportInitialize;
                if (layerInit != null)
                    layerInit.BeginInit();

                if (layer != null)
                {
                    var composition = e.ModelElement.Store.TryGetService<IBindingCompositionService>();

                    // In the T4 appdomain the service is unavailable
                    using (var context = composition != null ? composition.CreateDynamicContext() : NullDynamicBindingContext.Instance)
                    {
                        // Make the local current element available to layer, including events.
                        context.AddExport(view);
                        context.AddExportsFromInterfaces(view);

                        context.CompositionService.SatisfyImportsOnce(layer);
                    }
                }

                view.SyncElementsFrom(info.Elements);

                var patternManager = view.Store.GetService<IPatternManager>();
                if (patternManager != null)
                {
                    view.SyncExtensionPointsFrom(info.ExtensionPoints, patternManager);
                }

                if (layerInit != null)
                    layerInit.EndInit();
            }
            else
            {
                tracer.TraceWarning("DefinitionId not set for view '{0}', the schema information can not be loaded.", view.Id);
            }
        }

        private static IViewInfo FindInfo(Product owner, Guid id)
        {
            if (owner != null && owner.Info != null)
            {
                return owner.Info.Views.FirstOrDefault(v => v.Id == id);
            }

            return null;
        }
    }
}