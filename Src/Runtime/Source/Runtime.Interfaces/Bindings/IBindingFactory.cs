using System;

namespace Microsoft.VisualStudio.Patterning.Runtime
{
	/// <summary>
	/// Defines a factory to create <see cref="IDynamicBinding{T}"/>.
	/// </summary>
	[CLSCompliant(false)]
	public interface IBindingFactory
	{
		/// <summary>
		/// Creates a runtime binding from the given settings.
		/// </summary>
		IDynamicBinding<T> CreateBinding<T>(IBindingSettings settings) where T : class;

		/// <summary>
		/// Creates the dynamic context.
		/// </summary>
		IDynamicBindingContext CreateContext();
	}
}