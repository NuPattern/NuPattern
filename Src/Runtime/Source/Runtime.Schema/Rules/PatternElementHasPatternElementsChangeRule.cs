using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Handles property change events for the connectors.
    /// </summary>
    [RuleOn(typeof(ViewHasElements), FireTime = TimeToFire.TopLevelCommit)]
    [RuleOn(typeof(ViewHasExtensionPoints), FireTime = TimeToFire.TopLevelCommit)]
    [RuleOn(typeof(ElementHasElements), FireTime = TimeToFire.TopLevelCommit)]
    [RuleOn(typeof(ElementHasExtensionPoints), FireTime = TimeToFire.TopLevelCommit)]
    internal class ElementHasElementsChangeRule : ChangeRule
    {
        /// <summary>
        /// Handles property change events for the listed classes of this rule.
        /// </summary>
        /// <param name="e">The event args.</param>
        public override void ElementPropertyChanged(ElementPropertyChangedEventArgs e)
        {
            Guard.NotNull(() => e, e);

            var element = e.ModelElement;
            if (element != null)
            {
                // Repaint the connector (to update decorators)
                if (!element.Store.TransactionManager.CurrentTransaction.IsSerializing)
                {
                    if (e.DomainProperty.Id == ViewHasElements.CardinalityDomainPropertyId)
                    {
                        RepaintRelationshipConnectors<ViewHasElements>(element);
                    }

                    if (e.DomainProperty.Id == ViewHasExtensionPoints.CardinalityDomainPropertyId)
                    {
                        RepaintRelationshipConnectors<ViewHasExtensionPoints>(element);
                    }

                    if (e.DomainProperty.Id == ElementHasElements.CardinalityDomainPropertyId)
                    {
                        RepaintRelationshipConnectors<ElementHasElements>(element);
                    }

                    if (e.DomainProperty.Id == ElementHasExtensionPoints.CardinalityDomainPropertyId)
                    {
                        RepaintRelationshipConnectors<ElementHasExtensionPoints>(element);
                    }
                }
            }
        }

        /// <summary>
        /// Repaints the connector shapes for given links.
        /// </summary>
        private static void RepaintRelationshipConnectors<T>(ModelElement element) where T : ElementLink
        {
            var link = element as T;
            if (link != null)
            {
                foreach (var shape in PresentationViewsSubject.GetPresentation(link))
                {
                    BinaryLinkShape connector = shape as BinaryLinkShape;
                    if (connector != null)
                    {
                        connector.Invalidate();
                    }
                }
            }
        }
    }
}