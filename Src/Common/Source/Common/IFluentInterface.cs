using System;
using System.ComponentModel;

namespace NuPattern
{
    /// <summary>
    /// Helper interface used to hide the base <see cref="Object"/> 
    /// members from interfaces to make for cleaner Visual Studio intellisense 
    /// on fluent or marker interfaces.
    /// </summary>
    /// <devdoc>
    /// Applying this interface to any type hides the object members. 
    /// Typically used on interfaces rather than objects. 
    /// The EditorBrowsable attribute applied to all members 
    /// hides not only members but also this interface type 
    /// altogether from intellisense (so that consumers of the NuPattern 
    /// don't even know about this type's existence).
    /// </devdoc>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IFluentInterface
    {
        /// <summary/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "We're hiding Object.GetType")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "GetType", Justification = "We're hiding Object.GetType")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        Type GetType();

        /// <summary/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        int GetHashCode();

        /// <summary/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        string ToString();

        /// <summary/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool Equals(object other);
    }
}
