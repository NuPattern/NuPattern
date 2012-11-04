using System;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.VisualStudio.Patterning.Extensibility
{
    /// <summary>
    /// Interface for interacting with the VS Error List window.
    /// </summary>
    [CLSCompliant(false)]
    public interface IErrorList
    {
        /// <summary>
        /// Adds a message to the error window.
        /// </summary>
        void AddMessage(string message, TaskErrorCategory errorCategory);

        /// <summary>
        /// Adds a message of the given document to the error window.
        /// </summary>
        void AddMessage(string message, string document, TaskErrorCategory errorCategory);

        /// <summary>
        /// Clears all errors in the error list.
        /// </summary>
        void ClearAll();

        /// <summary>
        /// Clears all errors for given document.
        /// </summary>
        void Clear(string document);
    }
}