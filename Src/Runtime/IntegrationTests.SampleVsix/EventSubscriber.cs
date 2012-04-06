using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Patterning.Runtime.IntegrationTests.SampleVsix
{
	/// <summary>
	/// Sample event subscriber.
	/// </summary>
	[Export]
	public class EventSubscriber
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EventSubscriber"/> class.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Interfaces under design.")]
		[ImportingConstructor]
		public EventSubscriber(
			[Import(typeof(IEventPublisher))]
			IEventPublisher propertyChanged)
		{
			this.ChangedProperties = new List<string>();
			propertyChanged.Subscribe(this.OnChanged);
		}

		private void OnChanged(IEvent<PropertyChangedEventArgs> @event)
		{
			this.ChangedProperties.Add(@event.EventArgs.PropertyName);
		}

		/// <summary>
		/// Gets the changed properties.
		/// </summary>
		public IList<string> ChangedProperties { get; private set; }
	}
}
