using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NuPattern.VisualStudio;
using NuPattern.VisualStudio.Shell;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Default implementation of <see cref="IShellEvents"/>.
    /// </summary>
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IShellEvents))]
    internal sealed class ShellEvents : IVsShellPropertyEvents, IShellEvents
    {
        private uint shellCookie;
        private IVsShell shellService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellEvents"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        [ImportingConstructor]
        public ShellEvents([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
        {
            Guard.NotNull(() => serviceProvider, serviceProvider);

            this.shellService = serviceProvider.GetService<SVsShell, IVsShell>();

            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(
                this.shellService.AdviseShellPropertyChanges(this, out this.shellCookie));
        }

        /// <summary>
        /// Occurs when the shell has finished initializing.
        /// </summary>
        public event EventHandler ShellInitialized = (sender, args) => { };

        /// <summary>
        /// Gets a value indicating whether the shell has been initialized.
        /// </summary>
        public bool IsInitialized
        {
            get
            {
                object zombied;
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(this.shellService.GetProperty((int)__VSSPROPID.VSSPROPID_Zombie, out zombied));
                return !(bool)zombied;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (this.shellCookie != 0)
                {
                    Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(this.shellService.UnadviseShellPropertyChanges(this.shellCookie));
                }
            }
            catch (Exception ex)
            {
                if (Microsoft.VisualStudio.ErrorHandler.IsCriticalException(ex))
                {
                    throw;
                }
            }

            this.shellCookie = 0;
        }

        /// <summary>
        /// Can't really unit or integration-test this behavior, as retrieving a component 
        /// from MEF would be too late for the event to raise, as well as accessing the DTE, etc.
        /// </summary>
        int IVsShellPropertyEvents.OnShellPropertyChange(int propid, object var)
        {
            if (propid == (int)__VSSPROPID.VSSPROPID_Zombie)
            {
                if ((bool)var == false)
                {
                    Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(this.shellService.UnadviseShellPropertyChanges(this.shellCookie));
                    this.shellCookie = 0;

                    this.ShellInitialized(this, EventArgs.Empty);
                }
            }

            return Microsoft.VisualStudio.VSConstants.S_OK;
        }
    }
}
