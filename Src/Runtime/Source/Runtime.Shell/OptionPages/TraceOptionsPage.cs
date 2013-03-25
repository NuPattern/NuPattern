using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using NuPattern.Runtime.Settings;
using NuPattern.VisualStudio;

namespace NuPattern.Runtime.Shell.OptionPages
{
    /// <summary>
    /// Trace Options Page
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1408:DoNotUseAutoDualClassInterfaceType"), CLSCompliant(false)]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("7F0DCEFD-FAE7-4600-8B31-83BA201499F9")]
    public class TraceOptionsPage : DialogPage
    {
        private TraceOptionsPageControl control;

        [Import]
        private ISettingsManager settingManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceOptionsPage"/> class.
        /// </summary>
        public TraceOptionsPage()
        {
            var componentModel = ServiceProvider.GlobalProvider.GetService<SComponentModel, IComponentModel>();

            componentModel.DefaultCompositionService.SatisfyImportsOnce(this);

            this.RuntimeSettings = this.settingManager.Read();
        }

        internal IRuntimeSettings RuntimeSettings { get; set; }

        /// <summary>
        /// Gets an <see cref="IWin32Window"/> for the dialog 
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected override IWin32Window Window
        {
            get
            {
                this.control = new TraceOptionsPageControl();
                this.control.Location = new Point(0, 0);
                this.control.TraceOptionsPage = this;

                this.control.BoundControls();

                return control;
            }
        }

        /// <summary>
        /// Raises the <see cref="OnApply"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Microsoft.VisualStudio.Shell.DialogPage.PageApplyEventArgs"/> instance containing the event data.</param>
        protected override void OnApply(PageApplyEventArgs e)
        {
            base.OnApply(e);

            this.settingManager.Save(this.RuntimeSettings);
        }
    }
}