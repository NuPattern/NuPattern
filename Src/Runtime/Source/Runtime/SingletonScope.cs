using System;
using System.Runtime.Remoting.Messaging;

namespace NuPattern.Runtime
{
    /// <summary>
    /// A scope that can only be set as active by the 
    /// first caller that constructs an instance of the 
    /// scope in the current logical thread.
    /// </summary>
    /// <typeparam name="TKey">The type that will be used as the key 
    /// in the local thread to determine whether the scope is active.
    /// </typeparam>
    /// <remarks>
    /// Scopes are useful when code paths may be re-entrant. This 
    /// singleton scope only activates the scope if a previously active 
    /// scope doesn't already exist. Consumers can therefore safely 
    /// instantiate new scopes with C# <c>using</c> construct:
    /// <code>
    /// using (var scope = new MyTemplateScope())
    /// {
    ///    // Call some code that might need to be aware of the scope.
    /// }
    /// </code>
    /// The disposal of this scope will only cause the scope to 
    /// become inactive if it was activated when constructed. 
    /// Otherwise, the scope remains active. Another way of 
    /// saying this is that the scope is deactivated only by 
    /// whichever instance activated it in the first place.
    /// <para>
    /// Other code can check with the static <see cref="IsActive"/> 
    /// property whether a given scope is active at any time:
    /// <code>
    /// if (MyTemplateScope.IsActive)
    /// {
    ///    // ...
    /// }
    /// </code>
    /// </para>
    /// </remarks>
    internal abstract class SingletonScope<TKey> : IDisposable
    {
        private static readonly string TlsKey = typeof(TKey).FullName;
        private bool contextSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonScope&lt;TKey&gt;"/> class.
        /// </summary>
        protected SingletonScope()
        {
            if (!IsActive)
            {
                CallContext.LogicalSetData(TlsKey, new object());
                this.contextSet = true;
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="SingletonScope&lt;TKey&gt;"/> class.
        /// </summary>
        ~SingletonScope()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is active.
        /// </summary>
        /// <value>Returns <c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000", Justification = "This value is correct for each static derived class.")]
        public static bool IsActive
        {
            get { return CallContext.LogicalGetData(TlsKey) != null; }
        }

        /// <summary>
        /// Resets the <see cref="IsActive"/> state if necessary.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">Specify <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.contextSet)
                {
                    CallContext.LogicalSetData(TlsKey, null);
                }
            }
        }
    }
}
