using System;
using System.ComponentModel;

namespace NuPattern.Runtime.IntegrationTests.SampleVsix
{
	/// <summary>
	/// Sample event publisher.
	/// </summary>
	public interface IEventPublisher : IObservable<IEvent<PropertyChangedEventArgs>>, IObservableEvent
	{
	}
}
