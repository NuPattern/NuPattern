using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace NuPattern.VisualStudio
{
    /// <summary>
    /// Class to interact with the VS status bar
    /// </summary>
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IStatusBar))]
    internal class StatusBar : IStatusBar
    {
        private IServiceProvider serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusBar"/> class.
        /// </summary>
        /// <param name="serviceProvider">An <see cref="IServiceProvider"/></param>
        [ImportingConstructor]
        public StatusBar([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        private IVsStatusbar bar;

        /// <summary>
        /// Gets the status bar instance.
        /// </summary>
        protected IVsStatusbar Bar
        {
            get
            {
                return this.bar ?? (this.bar = this.serviceProvider.GetService<SVsStatusbar, IVsStatusbar>());
            }
        }

        /// <summary>
        /// Displays the message.
        /// </summary>
        /// <param name="message">The message.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Microsoft.VisualStudio.Shell.Interop.IVsStatusbar.SetText(System.String)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Microsoft.VisualStudio.Shell.Interop.IVsStatusbar.IsFrozen(System.Int32@)")]
        public void DisplayMessage(string message)
        {
            int frozen;

            this.Bar.IsFrozen(out frozen);

            if (frozen == 0)
            {
                this.Bar.SetText(message);
            }
        }
    }
}