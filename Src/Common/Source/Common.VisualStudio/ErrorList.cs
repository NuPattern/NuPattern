using System;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Shell;

namespace NuPattern.VisualStudio
{
    /// <summary>
    /// Provides access to the Visual Studio Error List window.
    /// </summary>
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IErrorList))]
    internal class ErrorList : IErrorList, IDisposable
    {
        private ErrorListProvider errorListProvider;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the ErrorList class.
        /// </summary>
        public ErrorList()
        {
        }

        /// <summary>
        /// Destructs this class
        /// </summary>
        ~ErrorList()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets or sets the IServiceProvider instance. 
        /// </summary>
        [Import(typeof(SVsServiceProvider), AllowDefault = true)]
        internal IServiceProvider ServiceProvider
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ErrorListProvider instance.
        /// </summary>
        internal ErrorListProvider ErrorListProvider
        {
            get
            {
                if (this.errorListProvider == null)
                {
                    this.errorListProvider = new ErrorListProvider(this.ServiceProvider);
                }

                return this.errorListProvider;
            }

            set
            {
                this.errorListProvider = value;
            }
        }

        /// <summary>
        /// Adds a message to the error list.
        /// </summary>
        public void AddMessage(string message, ErrorCategory errorCategory)
        {
            AddMessage(message, string.Empty, errorCategory);
        }

        /// <summary>
        /// Adds a message of the given document to the error list.
        /// </summary>
        public void AddMessage(string message, string document, ErrorCategory errorCategory)
        {
            var errorTask = new ErrorTask
                {
                    ErrorCategory = (TaskErrorCategory)errorCategory,
                    Text = message,
                    Document = document,
                };

            this.ErrorListProvider.Tasks.Add(errorTask);
            this.ErrorListProvider.Show();
            this.ErrorListProvider.BringToFront();
        }

        /// <summary>
        /// Clears all errors in the list for the given document.
        /// </summary>
        public void Clear(string document)
        {
            if (string.IsNullOrEmpty(document))
            {
                ClearAll();
            }
            else
            {
                // Clear only tasks of this document
                this.ErrorListProvider.Tasks.Cast<Task>()
                    .Where(t => t.Document.Equals(document, StringComparison.OrdinalIgnoreCase)).ToList<Task>()
                    .ForEach(t => this.ErrorListProvider.Tasks.Remove(t));
            }
        }

        /// <summary>
        /// Clears all errors in the list.
        /// </summary>
        public void ClearAll()
        {
            this.ErrorListProvider.Tasks.Clear();
        }

        /// <summary>
        /// Disploses the class.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.errorListProvider != null)
                    {
                        this.errorListProvider.Dispose();
                    }
                }

                this.disposed = true;
            }
        }
    }
}