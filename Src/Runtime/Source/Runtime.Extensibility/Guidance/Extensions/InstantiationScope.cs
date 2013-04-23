using System;
using System.Runtime.Remoting.Messaging;

namespace NuPattern.Runtime.Guidance.Extensions
{
    /// <summary>
    /// A scope for instantiating guidance extensions
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class InstantiationScope<T> : IDisposable
    {
        private readonly static string TlsKey = typeof(T).FullName;
        private bool contextSet;

        /// <summary>
        /// Creates a new instance of the <see cref="InstantiationScope{T}"/> class.
        /// </summary>
        protected InstantiationScope()
        {
            if (!IsActive)
            {
                CallContext.LogicalSetData(TlsKey, new object());
                contextSet = true;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the scope is active.
        /// </summary>
        public static bool IsActive
        {
            get { return CallContext.LogicalGetData(TlsKey) != null; }
        }

        /// <summary>
        /// Disposes this instance
        /// </summary>
        public void Dispose()
        {
            if (this.contextSet)
            {
                CallContext.LogicalSetData(TlsKey, null);
            }
        }
    }
}