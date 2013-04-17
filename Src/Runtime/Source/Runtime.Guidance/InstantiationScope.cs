using System;
using System.Runtime.Remoting.Messaging;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    internal abstract class InstantiationScope<T> : IDisposable
    {
        private readonly static string TlsKey = typeof(T).FullName;
        private bool contextSet;

        public InstantiationScope()
        {
            if (!IsActive)
            {
                CallContext.LogicalSetData(TlsKey, new object());
                contextSet = true;
            }
        }

        public static bool IsActive
        {
            get { return CallContext.LogicalGetData(TlsKey) != null; }
        }

        public void Dispose()
        {
            if (this.contextSet)
            {
                CallContext.LogicalSetData(TlsKey, null);
            }
        }
    }
}