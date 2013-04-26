using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using Microsoft.VisualStudio.Modeling;
using NuPattern.Diagnostics;
using NuPattern.Runtime.Bindings;
using NuPattern.Runtime.Store.Properties;
using NuPattern.Runtime.ToolkitInterface;

namespace NuPattern.Runtime.Store
{
    /// <summary>
    /// Rule that automatically creates the runtime automation components from the 
    /// deserialized settings classes.
    /// </summary>
    [RuleOn(typeof(ProductElement), FireTime = TimeToFire.TopLevelCommit)]
    internal class ProductElementAddRule : AddRule
    {
        private static readonly ITracer tracer = Tracer.Get<ProductElementAddRule>();

        /// <summary>
        /// Adds the runtime automation extensions.
        /// </summary>
        public override void ElementAdded(ElementAddedEventArgs e)
        {
            Guard.NotNull(() => e, e);

            var owner = (ProductElement)e.ModelElement;

            if (owner.Info != null)
            {
                //tracer.TraceData(
                //    TraceEventType.Verbose,
                //    Resources.ProductElementAddRule_InitializingEventId,
                //    owner);

                // Try to retrieve the interface layer proxy.
                var layer = owner.GetInterfaceLayer();
                var composition = e.ModelElement.Store.TryGetService<IBindingCompositionService>();

                // In the T4 appdomain the service is unavailable
                using (var context = composition != null ? composition.CreateDynamicContext() : NullDynamicBindingContext.Instance)
                {
                    // Make the local current element available to automations, including events.
                    context.AddExport(owner);
                    context.AddExportsFromInterfaces(owner);
                    context.AddInterfaceLayer(owner);

                    try
                    {
                        var layerInit = layer as ISupportInitialize;
                        if (layerInit != null)
                            layerInit.BeginInit();

                        if (layer != null)
                            context.CompositionService.SatisfyImportsOnce(layer);

                        foreach (var setting in owner.Info.AutomationSettings)
                        {
                            var automation = setting.As<IAutomationSettings>().CreateAutomation(owner);
                            var initializable = automation as ISupportInitialize;

                            if (initializable != null)
                                initializable.BeginInit();

                            context.CompositionService.SatisfyImportsOnce(automation);

                            if (initializable != null)
                                initializable.EndInit();

                            owner.AutomationExtensions.Add(automation);
                        }

                        if (layerInit != null)
                            layerInit.EndInit();
                    }
                    catch (OperationCanceledException)
                    {
                        // Delete ourselves and rethrow.
                        owner.Delete();
                        throw;
                    }
                }

                //tracer.TraceData(
                //    TraceEventType.Verbose,
                //    Resources.ProductElementAddRule_InitializedEventId,
                //    owner);
            }

            base.ElementAdded(e);
        }
    }
}