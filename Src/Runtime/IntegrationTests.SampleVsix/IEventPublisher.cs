using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Patterning.Runtime.IntegrationTests.SampleVsix
{
	/// <summary>
	/// Sample event publisher.
	/// </summary>
	public interface IEventPublisher : IObservable<IEvent<PropertyChangedEventArgs>>, IObservableEvent
	{
	}
}
