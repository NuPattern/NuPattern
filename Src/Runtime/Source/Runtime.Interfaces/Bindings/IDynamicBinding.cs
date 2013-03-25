using System;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace NuPattern.Runtime.Bindings
{
	/// <summary>
	/// A binding that allows passing dynamic values at runtime.
	/// </summary>
	[CLSCompliant(false)]
	public interface IDynamicBinding<T> : IBinding<T>
		where T : class
	{
		/// <summary>
		/// Creates the context for providing dynamic values for binding evaluation.
		/// </summary>
		IDynamicBindingContext CreateDynamicContext();

		/// <summary>
		/// Evaluates the binding with the additional exports provided in the given context.
		/// </summary>
		bool Evaluate(IDynamicBindingContext context);
	}
}
