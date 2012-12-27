using System;

namespace NuPattern.Runtime
{
	/// <summary>
	/// Represents a transaction within the schema or the runtime state.
	/// </summary>
	public interface ITransaction : IDisposable
	{
		/// <summary>
		/// Commits the transaction.
		/// </summary>
		void Commit();

		/// <summary>
		/// Aborts the transaction.
		/// </summary>
		void Rollback();
	}
}
