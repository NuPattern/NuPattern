using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Modeling.Shell;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema
{
	/// <summary>
	/// Double-derived class to allow easier code customization.
	/// </summary>
	internal abstract partial class PatternModelPackageBase
	{
		partial void InitializeExtensions()
		{
			var commandExtensionRegistrar = new AuthoringCommandExtensionRegistrar();

			if (ModelingCompositionContainer.CompositionService != null)
			{
				ModelingCompositionContainer.CompositionService.SatisfyImportsOnce(commandExtensionRegistrar);

				commandExtensionRegistrar.Initialize(this);
			}
		}
	}
}