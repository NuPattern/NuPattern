using System;
using System.Runtime.Serialization;

namespace NuPattern
{
    /// <summary>
    /// A weak reference to a static target, which is 
    /// therefore always alive and with a <see langword="null"/> <see cref="WeakReference.Target"/>.
    /// </summary>
    internal class StaticWeakReference : WeakReference
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StaticWeakReference"/> class.
        /// </summary>
        public StaticWeakReference()
            : base(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticWeakReference"/> class for serialization.
        /// </summary>
        protected StaticWeakReference(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets an indication whether the object referenced by the current <see cref="T:System.WeakReference"/> object has been garbage collected.
        /// </summary>
        /// <returns>Returns true if the object referenced by the current <see cref="T:System.WeakReference"/> object has not been garbage collected and is still accessible; otherwise, false.</returns>
        public override bool IsAlive
        {
            get
            {
                return true;
            }
        }
    }
}
