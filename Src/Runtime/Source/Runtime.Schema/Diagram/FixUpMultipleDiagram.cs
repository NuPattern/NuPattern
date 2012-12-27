using Microsoft.VisualStudio.Modeling;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Double derived implementation for the rule that initiates view fixup when an element that has an associated shape is added to the model.
    /// This now enables the DSL author to everride the SkipFixUp() method.
    /// </summary>
    internal sealed partial class FixUpMultipleDiagram
    {
        private static ModelElement GetParentForElementSchema(ElementSchema root)
        {
            return root.GetViewForAbstractElement().Pattern.PatternModel;
        }

        private static ModelElement GetParentForCollectionSchema(CollectionSchema root)
        {
            return root.GetViewForAbstractElement().Pattern.PatternModel;
        }

		private static ModelElement GetParentForExtensionPointSchema(ExtensionPointSchema root)
		{
			if(root.View != null)
			{
				return root.View.Pattern.PatternModel;
			}
			else
			{
				return root.Owner.GetViewForAbstractElement().Pattern.PatternModel;
			}
		}
    }
}