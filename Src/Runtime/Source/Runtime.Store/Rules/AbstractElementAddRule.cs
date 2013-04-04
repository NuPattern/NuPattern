using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Runtime.Store.Properties;

namespace NuPattern.Runtime.Store
{
    /// <summary>
    /// Triggers this notification rule whether a <see cref="AbstractElement"/> is added.
    /// </summary>
    [RuleOn(typeof(AbstractElement), FireTime = TimeToFire.LocalCommit)]
    internal class AbstractElementAddRule : AddRule
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<AbstractElementAddRule>();

        /// <summary>
        /// Triggers this notification rule whether a <see cref="AbstractElement"/> is added.
        /// </summary>
        /// <param name="e">The provided data for this event.</param>
        public override void ElementAdded(ElementAddedEventArgs e)
        {
            Guard.NotNull(() => e, e);

            var element = (AbstractElement)e.ModelElement;

            var info = FindInfo(element);
            if (info != null)
            {
                element.Info = info;

                if (string.IsNullOrEmpty(element.InstanceName))
                {
                    element.InstanceName = info.DisplayName;
                }

                element.SyncPropertiesFrom(info.Properties);
                element.SyncElementsFrom(info.Elements);

                var patternManager = element.Store.GetService<IPatternManager>();
                if (patternManager != null)
                {
                    element.SyncExtensionPointsFrom(info.ExtensionPoints, patternManager);
                }
            }
            else
            {
                tracer.TraceWarning(Resources.TracerWarning_ElementInfoNotFound, element.Id);
            }
        }

        private static IAbstractElementInfo FindInfo(AbstractElement element)
        {
            if (element.Parent != null)
            {
                var info = element.Parent.Info as IElementInfoContainer;
                if (info != null)
                {
                    return info.Elements.FirstOrDefault(v => v.Id == element.DefinitionId);
                }
            }

            return null;
        }
    }
}