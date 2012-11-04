using System;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace Microsoft.VisualStudio.Patterning.Runtime
{
	/// <summary>
	/// The service that provides MEF composition and exports resolution for bound components.
	/// </summary>
	[CLSCompliant(false)]
	public interface IBindingCompositionService : IFeatureCompositionService
	{
		/// <summary>
		/// Creates the context for providing dynamic values for binding evaluation.
		/// </summary>
		IDynamicBindingContext CreateDynamicContext();
	}
}
