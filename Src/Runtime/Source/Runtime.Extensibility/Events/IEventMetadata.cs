using System;

namespace NuPattern.Runtime.Events
{
    /// <summary>
    /// Provides introspection metadata about exported events.
    /// </summary>
    public interface IEventMetadata
    {
        /// <summary>
        /// Gets the concrete type of <see cref="System.IObservable{T}"/> with an <see cref="IEvent{TEventArgs}"/> argument exported by the event.
        /// </summary>
        Type ObservableType { get; }

        /// <summary>
        /// Gets the type of publisher exporting the event.
        /// </summary>
        Type EventPublisherType { get; }

        /// <summary>
        /// Gets the type of the event args exposed by the event.
        /// </summary>
        Type EventArgsType { get; }

        /// <summary>
        /// Gets the name of the event declared by the <see cref="EventPublisherType"/>.
        /// </summary>
        string EventName { get; }
    }
}