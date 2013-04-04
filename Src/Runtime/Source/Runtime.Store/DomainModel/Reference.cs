using NuPattern.Modeling;

namespace NuPattern.Runtime.Store
{
    /// <summary>
    /// Customizations for the Reference class.
    /// </summary>
    partial class Reference
    {
        /// <summary>
        /// Returns whether the state containing this instance is currently 
        /// in a transaction or not.
        /// </summary>
        public bool InTransaction
        {
            get { return this.Store.TransactionActive; }
        }

        /// <summary>
        /// Returns whether the state containing this instance is currently 
        /// in a serialization transaction or not.
        /// </summary>
        public bool IsSerializing
        {
            get { return this.Store.InSerializationTransaction; }
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        public ITransaction BeginTransaction()
        {
            return new ModelingTransaction(this.Store.TransactionManager.BeginTransaction());
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        public ITransaction BeginTransaction(string name)
        {
            return new ModelingTransaction(this.Store.TransactionManager.BeginTransaction(name));
        }
    }
}