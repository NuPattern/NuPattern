using System;
using System.ComponentModel.Composition;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NuPattern.IO;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.VisualStudio.Events
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IItemEvents))]
    internal class ItemEvents : IDisposable, IItemEvents
    {
        private bool isDisposed;
        private ProjectItemsEvents projectItemsEvents;

        public event EventHandler<FileEventArgs> ItemRemoved = (sender, args) => { };

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called by MEF.")]
        [ImportingConstructor]
        public ItemEvents([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
        {
            Guard.NotNull(() => serviceProvider, serviceProvider);

            var dte = serviceProvider.GetService<SDTE, DTE>();

            if (dte != null)
            {
                projectItemsEvents = dte.Events.GetObject(typeof(ProjectItemsEvents).Name) as ProjectItemsEvents;
                projectItemsEvents.ItemRemoved += new _dispProjectItemsEvents_ItemRemovedEventHandler(OnItemRemoved);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void OnItemRemoved(ProjectItem item)
        {
            this.ItemRemoved(this, new FileEventArgs(item.get_FileNames(1)));
        }

        private void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    try
                    {
                        projectItemsEvents.ItemRemoved -= OnItemRemoved;
                    }
                    catch (Exception ex)
                    {
                        if (Microsoft.VisualStudio.ErrorHandler.IsCriticalException(ex))
                        {
                            throw;
                        }
                    }
                }

                this.isDisposed = true;
            }
        }
    }
}