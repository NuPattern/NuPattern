using System;

namespace NuPattern
{
    /// <summary>
    /// Represents a transaction.
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
