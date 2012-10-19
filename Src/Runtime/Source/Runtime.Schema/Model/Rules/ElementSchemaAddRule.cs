using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Modeling;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema
{
	/// <summary>
	/// Triggers this notification rule whether a <see cref="ElementSchema"/> is added.
	/// </summary>
	[RuleOn(typeof(ElementSchema), FireTime = TimeToFire.TopLevelCommit)]
	public partial class ElementSchemaAddRule : AddRule
	{
		/// <summary>
		/// Triggers this notification rule whether a <see cref="ElementSchema"/> is added.
		/// </summary>
		/// <param name="e">The provided data for this event.</param>
		public override void ElementAdded(ElementAddedEventArgs e)
		{
			Guard.NotNull(() => e, e);

			var element = (ElementSchema)e.ModelElement;

			if (!element.Store.TransactionManager.CurrentTransaction.IsSerializing)
			{
				if (element.Owner == null)
				{
					var relationship = (ViewHasElements)DomainRelationshipInfo.FindEmbeddingElementLink(element);

					if (relationship != null)
					{
						relationship.WithTransaction(r =>
						{
							r.Cardinality = Runtime.Cardinality.ZeroToMany;
							r.AutoCreate = false;
						});
					}
				}
				else
				{
					var relationship = (ElementHasElements)DomainRelationshipInfo.FindEmbeddingElementLink(element);

					if (relationship != null)
					{
						relationship.WithTransaction(r =>
						{
							r.Cardinality = Runtime.Cardinality.ZeroToMany;
							r.AutoCreate = false;
						});
					}
				}
			}
		}
	}
}