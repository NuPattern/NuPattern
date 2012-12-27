using System;
using System.Globalization;
using Microsoft.VisualStudio.Modeling;

namespace NuPattern.Extensibility
{
    /// <summary>
    /// Defines extension methods related to DSL <see cref="TransactionManager"/>.
    /// </summary>
    public static class TransactionManagerExtensions
    {
        /// <summary>
        /// Executes a delegate inside a DSL transaction.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="action">The action.</param>
        public static void DoWithinTransaction(this TransactionManager manager, Action action)
        {
            DoWithinTransaction(manager, action, string.Empty, false);
        }

        /// <summary>
        /// Executes a delegate inside a DSL transaction.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="action">The action.</param>
        /// <param name="transactionName">Name of the transaction.</param>
        public static void DoWithinTransaction(this TransactionManager manager, Action action, string transactionName)
        {
            DoWithinTransaction(manager, action, transactionName, false);
        }

        /// <summary>
        /// Executes a delegate inside a DSL transaction.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="action">The action.</param>
        /// <param name="serializing">If set to <c>true</c> [is serializing].</param>
        public static void DoWithinTransaction(this TransactionManager manager, Action action, bool serializing)
        {
            DoWithinTransaction(manager, action, string.Empty, serializing);
        }

        /// <summary>
        /// Executes a delegate inside a DSL transaction.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="action">The action.</param>
        /// <param name="transactionName">Name of the transaction.</param>
        /// <param name="serializing">Whether the transaction is a serialization one.</param>
        public static void DoWithinTransaction(this TransactionManager manager, Action action, string transactionName, bool serializing)
        {
            Guard.NotNull(() => manager, manager);
            Guard.NotNull(() => action, action);

            var transacName = transactionName;

            if (string.IsNullOrEmpty(transacName))
            {
                transacName = string.Format(
                    CultureInfo.CurrentCulture,
                    "Executing {0} @ {1}",
                    action.Method.Name,
                    DateTime.Now.ToString());
            }

            if (manager.Store.InUndoRedoOrRollback || manager.InTransaction || (manager.CurrentTransaction != null && manager.CurrentTransaction.InRollback))
            {
                // Do not create nested transaction in rollback scenarios.
                action();
            }
            else
            {
                using (var tx = manager.BeginTransaction(transacName, serializing))
                {
                    action();
                    tx.Commit();
                }
            }
        }
    }
}