using System;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.VisualStudio.Events
{
    /// <summary>
    /// Provides a listener to general events in the solution.
    /// </summary>
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(ISolutionEvents))]
    internal sealed class SolutionEvents : IDisposable, ISolutionEvents
    {
        private static volatile object mutex = new object();
        private bool disposed;
        private uint solutionEventsCookie;
        private IServiceProvider serviceProvider;
        private IVsSolution solution;
        private VsSolutionEvents solutionEvents;

        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionEvents"/> class.
        /// </summary>
        [ImportingConstructor]
        public SolutionEvents([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
        {
            Guard.NotNull(() => serviceProvider, serviceProvider);

            this.serviceProvider = serviceProvider;
            this.solution = serviceProvider.GetService<SVsSolution, IVsSolution>();
            this.solutionEvents = new VsSolutionEvents(this);
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(this.solution.AdviseSolutionEvents(this.solutionEvents, out this.solutionEventsCookie));
        }

        /// <summary>
        /// Occurs after the solution is closed.
        /// </summary>
        public event EventHandler<SolutionEventArgs> SolutionClosing = (sender, args) => { };

        /// <summary>
        /// Occurs after the solution is closed.
        /// </summary>
        public event EventHandler<SolutionEventArgs> SolutionClosed = (sender, args) => { };

        /// <summary>
        /// Occurs after the solution is opened.
        /// </summary>
        public event EventHandler<SolutionEventArgs> SolutionOpened = (sender, args) => { };

        /// <summary>
        /// Gets a value indicating whether this instance is solution opened.
        /// </summary>
        /// <value>
        /// Return <c>true</c> if this instance is solution opened; otherwise, <c>false</c>.
        /// </value>
        //// TODO should be replaced by state with an Enum
        public bool IsSolutionOpened { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
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
                lock (mutex)
                {
                    if (disposing && (this.solutionEventsCookie != 0))
                    {
                        var result = this.solution.UnadviseSolutionEvents(this.solutionEventsCookie);
                        if (result != Microsoft.VisualStudio.VSConstants.S_OK)
                        {
                            Marshal.ThrowExceptionForHR(result);
                        }

                        this.solutionEventsCookie = 0;
                    }

                    this.disposed = true;
                }
            }
        }

        /// <summary>
        /// Solution events class.
        /// </summary>
        private class VsSolutionEvents : IVsSolutionEvents
        {
            private SolutionEvents events;

            /// <summary>
            /// Initializes a new instance of the <see cref="VsSolutionEvents"/> class.
            /// </summary>
            public VsSolutionEvents(SolutionEvents events)
            {
                this.events = events;
            }

            /// <summary>
            /// Called when after the solution is close.
            /// </summary>
            /// <param name="unkReserved">The unk reserved.</param>
            /// <returns>The error result.</returns>
            int IVsSolutionEvents.OnAfterCloseSolution(object unkReserved)
            {
                this.events.IsSolutionOpened = false;
                this.events.SolutionClosed(this.events, new SolutionEventArgs(null));
                return Microsoft.VisualStudio.VSConstants.S_OK;
            }

            /// <summary>
            /// Called when [after load project].
            /// </summary>
            /// <param name="stubHierarchy">The stub hierarchy.</param>
            /// <param name="realHierarchy">The real hierarchy.</param>
            /// <returns>Not implemented.</returns>
            int IVsSolutionEvents.OnAfterLoadProject(IVsHierarchy stubHierarchy, IVsHierarchy realHierarchy)
            {
                return Microsoft.VisualStudio.VSConstants.S_OK;
            }

            /// <summary>
            /// Called when [after open project].
            /// </summary>
            /// <param name="hierarchy">The hierarchy.</param>
            /// <param name="added">The added.</param>
            /// <returns>Not implemented.</returns>
            int IVsSolutionEvents.OnAfterOpenProject(IVsHierarchy hierarchy, int added)
            {
                return Microsoft.VisualStudio.VSConstants.S_OK;
            }

            /// <summary>
            /// Called when [after open solution].
            /// </summary>
            /// <param name="unkReserved">The unk reserved.</param>
            /// <param name="newSolution">The new solution.</param>
            /// <returns>Not implemented.</returns>
            int IVsSolutionEvents.OnAfterOpenSolution(object unkReserved, int newSolution)
            {
                this.events.IsSolutionOpened = true;
                this.events.SolutionOpened(this.events, new SolutionEventArgs(this.events.serviceProvider.GetService<ISolution>()));
                return Microsoft.VisualStudio.VSConstants.S_OK;
            }

            /// <summary>
            /// Called when [before close project].
            /// </summary>
            /// <param name="hierarchy">The hierarchy.</param>
            /// <param name="removed">The removed.</param>
            /// <returns>Not implemented.</returns>
            int IVsSolutionEvents.OnBeforeCloseProject(IVsHierarchy hierarchy, int removed)
            {
                return Microsoft.VisualStudio.VSConstants.S_OK;
            }

            /// <summary>
            /// Called when [before close solution].
            /// </summary>
            /// <param name="unkReserved">The unk reserved.</param>
            /// <returns>Not implemented.</returns>
            int IVsSolutionEvents.OnBeforeCloseSolution(object unkReserved)
            {
                this.events.SolutionClosing(this.events, new SolutionEventArgs(null));

                return Microsoft.VisualStudio.VSConstants.S_OK;
            }

            /// <summary>
            /// Called when [before unload project].
            /// </summary>
            /// <param name="realHierarchy">The real hierarchy.</param>
            /// <param name="stubHierarchy">The stub hierarchy.</param>
            /// <returns>Not implemented.</returns>
            int IVsSolutionEvents.OnBeforeUnloadProject(IVsHierarchy realHierarchy, IVsHierarchy stubHierarchy)
            {
                return Microsoft.VisualStudio.VSConstants.S_OK;
            }

            /// <summary>
            /// Called when [query close project].
            /// </summary>
            /// <param name="hierarchy">The hierarchy.</param>
            /// <param name="removing">The removing.</param>
            /// <param name="cancel">The cancel.</param>
            /// <returns>Not implemented.</returns>
            int IVsSolutionEvents.OnQueryCloseProject(IVsHierarchy hierarchy, int removing, ref int cancel)
            {
                return Microsoft.VisualStudio.VSConstants.S_OK;
            }

            /// <summary>
            /// Called when [query close solution].
            /// </summary>
            /// <param name="unkReserved">The unk reserved.</param>
            /// <param name="cancel">The cancel.</param>
            /// <returns>Not implemented.</returns>
            int IVsSolutionEvents.OnQueryCloseSolution(object unkReserved, ref int cancel)
            {
                return Microsoft.VisualStudio.VSConstants.S_OK;
            }

            /// <summary>
            /// Called when [query unload project].
            /// </summary>
            /// <param name="realHierarchy">The real hierarchy.</param>
            /// <param name="cancel">The cancel.</param>
            /// <returns>Not implemented.</returns>
            int IVsSolutionEvents.OnQueryUnloadProject(IVsHierarchy realHierarchy, ref int cancel)
            {
                return Microsoft.VisualStudio.VSConstants.S_OK;
            }
        }
    }
}
