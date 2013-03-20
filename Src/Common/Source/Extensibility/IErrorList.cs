using System;

namespace NuPattern.Extensibility
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
        void AddMessage(string message, ErrorCategory errorCategory);

        /// <summary>
        /// Adds a message of the given document to the error window.
        /// </summary>
        void AddMessage(string message, string document, ErrorCategory errorCategory);

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