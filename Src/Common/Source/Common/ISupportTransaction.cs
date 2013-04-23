
namespace NuPattern
{
    /// <summary>
    /// Interface implemented by components that support transactions.
    /// </summary>
    public interface ISupportTransaction
    {
        /// <summary>
        /// Returns whether the state containing this instance is currently 
        /// in a transaction or not.
        /// </summary>
        bool InTransaction { get; }

        /// <summary>
        /// Returns whether the state containing this instance is currently 
        /// in a serialization transaction or not.
        /// </summary>
        bool IsSerializing { get; }

        /// <summary>
        /// Begins a new transaction.
        /// </summary>
        ITransaction BeginTransaction();

        /// <summary>
        /// Begins a new transaction with the given name.
        /// </summary>
        ITransaction BeginTransaction(string name);
    }
}
