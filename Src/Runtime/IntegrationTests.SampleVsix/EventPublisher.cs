using System;
using System.ComponentModel;
using System.ComponentModel.Composition;

namespace NuPattern.Runtime.IntegrationTests.SampleVsix
{
	/// <summary>
	/// Sample publisher.
	/// </summary>
	[Category("Samples")]
	[Description("Raised when the source property changes")]
	[Event(typeof(IEventPublisher))]
	[Export(typeof(IEventPublisher))]
	public class EventPublisher : IEventPublisher
	{
		//// private INotifyPropertyChanged eventSource;
		private IObservable<IEvent<PropertyChangedEventArgs>> observable;

		/// <summary>
		/// Initializes a new instance of the <see cref="EventPublisher"/> class.
		/// </summary>
		/// <param name="eventSource">The event source.</param>
		[ImportingConstructor]
		public EventPublisher(INotifyPropertyChanged eventSource)
		{
			//// this.eventSource = eventSource;

			this.observable = WeakObservable.FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
				handler => new PropertyChangedEventHandler(handler.Invoke),
				handler => eventSource.PropertyChanged += handler,
				handler => eventSource.PropertyChanged -= handler);
		}

		/// <summary>
		/// Subscribes the specified observer.
		/// </summary>
		public IDisposable Subscribe(IObserver<IEvent<PropertyChangedEventArgs>> observer)
		{
			return this.observable.Subscribe(observer);
		}
	}
}
