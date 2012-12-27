using System;
using System.ComponentModel;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace NuPattern.Common.Presentation.Vs
{
    /// <summary>
    /// The VS EnvironmentRenderCapabilities control.
    /// </summary>
    /// <remarks>This class is a direct copy of the <see cref="EnvironmentRenderCapabilities"/>, and is embedded here to avoid 
    /// referencing Microsoft.VisualStudio.Shell.1X.0 in any xaml files that use this class.</remarks>
    public class VsEnvironmentRenderCapabilities : DisposableObject, INotifyPropertyChanged, IVsShellPropertyEvents
    {
        private bool areAnimationsAllowed;
        private bool areGradientsAllowed;
        private static VsEnvironmentRenderCapabilities current;
        private uint shellPropertyChangesCookie;
        private int visualEffectsAllowed;

        /// <summary>
        /// Property changed event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Handler for property changed event.
        /// </summary>
        public event EventHandler RenderCapabilitiesChanged;

        /// <summary>
        /// Creates anew instance of the <see cref="VsEnvironmentRenderCapabilities"/> class.
        /// </summary>
        private VsEnvironmentRenderCapabilities()
        {
            IVsShell service = ServiceProvider.GlobalProvider.GetService(typeof(SVsShell)) as IVsShell;
            if (service != null)
            {
                object obj2;
                if (Microsoft.VisualStudio.ErrorHandler.Succeeded(service.GetProperty(-9061, out obj2)))
                {
                    this.VisualEffectsAllowed = (int)obj2;
                }
                service.AdviseShellPropertyChanges(this, out this.shellPropertyChangesCookie);
            }
        }

        /// <summary>
        /// Disposes managed resources
        /// </summary>
        protected override void DisposeManagedResources()
        {
            if (this.shellPropertyChangesCookie != 0)
            {
                IVsShell service = ServiceProvider.GlobalProvider.GetService(typeof(SVsShell)) as IVsShell;
                if (service != null)
                {
                    service.UnadviseShellPropertyChanges(this.shellPropertyChangesCookie);
                }
                this.shellPropertyChangesCookie = 0;
            }
            base.DisposeManagedResources();
        }

        /// <summary>
        /// Handles a property change of the shell.
        /// </summary>
        public int OnShellPropertyChange(int propid, object var)
        {
            base.ThrowIfDisposed();
            if (propid == -9061)
            {
                this.VisualEffectsAllowed = (int)var;
            }
            return 0;
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Gets whether animations are allowed.
        /// </summary>
        public bool AreAnimationsAllowed
        {
            get
            {
                return this.areAnimationsAllowed;
            }
            private set
            {
                if (this.areAnimationsAllowed != value)
                {
                    this.areAnimationsAllowed = value;
                    this.RaisePropertyChanged("AreAnimationsAllowed");
                }
            }
        }

        /// <summary>
        /// Gets whether gradients are allowed.
        /// </summary>
        public bool AreGradientsAllowed
        {
            get
            {
                return this.areGradientsAllowed;
            }
            private set
            {
                if (this.areGradientsAllowed != value)
                {
                    this.areGradientsAllowed = value;
                    this.RaisePropertyChanged("AreGradientsAllowed");
                }
            }
        }

        /// <summary>
        /// Gets the current environment rendering capabilities.
        /// </summary>
        public static VsEnvironmentRenderCapabilities Current
        {
            get
            {
                return (current ?? (current = new VsEnvironmentRenderCapabilities()));
            }
        }

        /// <summary>
        /// Gets or sets the visual effects that are allowed.
        /// </summary>
        public int VisualEffectsAllowed
        {
            get
            {
                return this.visualEffectsAllowed;
            }
            set
            {
                if (this.visualEffectsAllowed != value)
                {
                    this.visualEffectsAllowed = value;
                    this.AreGradientsAllowed = (value & 2) == 2;
                    this.AreAnimationsAllowed = (value & 1) == 1;
                    this.RaisePropertyChanged("VisualEffectsAllowed");
                    this.RenderCapabilitiesChanged.RaiseEvent(this, EventArgs.Empty);
                }
            }
        }
    }
}
