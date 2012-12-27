using System;
using System.ComponentModel.Composition;

namespace NuPattern.Runtime
{
	/// <summary>
	/// Provides a dynamic binding context to provide additional exports 
	/// for binding evaluation.
	/// </summary>
	public interface IDynamicBindingContext : IDisposable
	{
		/// <summary>
		/// Gets the composition service backing the context.
		/// </summary>
		ICompositionService CompositionService { get; }

		/// <summary>
		/// Adds the given instance with the contract specified.
		/// </summary>
		/// <typeparam name="T">The type of the contract to export the instance with.</typeparam>
		/// <param name="instance">The exported value.</param>
		void AddExport<T>(T instance)
			where T : class;

		/// <summary>
		/// Adds the given instance with the contract type and name specified.
		/// </summary>
		/// <typeparam name="T">The type of the contract to export the instance with.</typeparam>
		/// <param name="instance">The exported value.</param>
		/// <param name="contractName">Name of the contract.</param>
		void AddExport<T>(T instance, string contractName)
			where T : class;
	}
}
