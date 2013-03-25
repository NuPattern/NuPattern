using System;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace NuPattern.Runtime.Events
{
    /// <summary>
    /// Marks a class or interface as an event. The <see cref="System.ComponentModel.Composition.ExportAttribute"/> must 
    /// still be applied to the type in order to make it available under the right 
    /// contract for consumers.
    /// </summary>
    /// <remarks>
    /// This attribute automatically exports the annotated type as an <see cref="IObservableEvent"/>.
    /// Consumers would typically use the actual concrete type/interface when importing events.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1018:MarkAttributesWithAttributeUsage", Justification = "Base class provides supported usage.")]
    [CLSCompliant(false)]
    public sealed class EventAttribute : FeatureComponentAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventAttribute"/> class.
        /// </summary>
        /// <param name="exportedEventType">Type of the exported event, which must match 
        /// a corresponding <see cref="System.ComponentModel.Composition.ExportAttribute"/> on the class, and which is used 
        /// as the event identifier.</param>
        public EventAttribute(Type exportedEventType)
            : base(typeof(IObservableEvent))
        {
            Guard.NotNull(() => exportedEventType, exportedEventType);

            this.ExportedEventType = exportedEventType;
            this.Id = exportedEventType.FullName;
        }

        /// <summary>
        /// Gets the type of the exported event.
        /// </summary>
        public Type ExportedEventType { get; private set; }
    }
}
