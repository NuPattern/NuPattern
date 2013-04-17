using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.OLE.Interop;
using NuPattern.Diagnostics;
using NuPattern.Runtime.Shell.Properties;

namespace NuPattern.Runtime.Shell
{
    internal class VsIdleTaskHost : IDisposable
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<VsIdleTaskHost>();
        private IOleComponentManager componentManager;
        private HostComponent host;
        private uint componentId;

        public VsIdleTaskHost(System.IServiceProvider serviceProvider, Func<bool> task, TimeSpan updateDelay)
        {
            Guard.NotNull(() => serviceProvider, serviceProvider);
            Guard.NotNull(() => task, task);

            this.componentManager = (IOleComponentManager)serviceProvider.GetService(typeof(SOleComponentManager));
            this.host = new HostComponent(task, updateDelay);
        }

        public void Start(TimeSpan timeout)
        {
            var info = new[]
            {
                new OLECRINFO
                {
                    cbSize = (uint)Marshal.SizeOf(typeof(OLECRINFO)),
                    grfcrf = (uint)(_OLECRF.olecrfNeedIdleTime | _OLECRF.olecrfNeedPeriodicIdleTime),
                    grfcadvf = (uint)(_OLECADVF.olecadvfModal | _OLECADVF.olecadvfRedrawOff | _OLECADVF.olecadvfWarningsOff),
                    uIdleTimeInterval = (uint)timeout.TotalMilliseconds
                }
            };

            if (this.componentManager.FRegisterComponent(this.host, info, out this.componentId) != 1)
            {
                tracer.TraceError(Resources.VsIdleTaskHost_ErrorRegistration);
                throw new InvalidOperationException(Resources.VsIdleTaskHost_ErrorRegistration);
            }
        }

        public void Dispose()
        {
            if (this.componentId != 0 && this.componentManager != null)
            {
                this.componentManager.FRevokeComponent(this.componentId);
            }

            this.componentId = 0;
        }

        private class HostComponent : IOleComponent
        {
            private Func<bool> task;
            private TimeSpan updateDelay;
            private DateTime finishExecTime;

            public HostComponent(Func<bool> task, TimeSpan updateDelay)
            {
                this.task = task;
                this.updateDelay = updateDelay;
            }

            public int FContinueMessageLoop(uint uReason, IntPtr pvLoopData, MSG[] pMsgPeeked)
            {
                return 1;
            }

            public int FDoIdle(uint grfidlef)
            {
                if (DateTime.Now - this.finishExecTime >= updateDelay)
                {
                    if (!this.task())
                    {
                        this.finishExecTime = DateTime.Now;
                    }
                }

                return 0;
            }

            public int FPreTranslateMessage(MSG[] pMsg)
            {
                return 0;
            }

            public int FQueryTerminate(int fPromptUser)
            {
                return 1;
            }

            public int FReserved1(uint dwReserved, uint message, IntPtr wParam, IntPtr lParam)
            {
                return 1;
            }

            public IntPtr HwndGetWindow(uint dwWhich, uint dwReserved)
            {
                return IntPtr.Zero;
            }

            public void OnActivationChange(IOleComponent pic, int fSameComponent, OLECRINFO[] pcrinfo, int fHostIsActivating, OLECHOSTINFO[] pchostinfo, uint dwReserved)
            {
            }

            public void OnAppActivate(int fActive, uint dwOtherThreadID)
            {
            }

            public void OnEnterState(uint uStateID, int fEnter)
            {
            }

            public void OnLoseActivation()
            {
            }

            public void Terminate()
            {
            }
        }
    }
}