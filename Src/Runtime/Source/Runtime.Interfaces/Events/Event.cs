
namespace NuPattern.Runtime
{
	/// <summary>
	/// Factory for event instances.
	/// </summary>
	/// <devdoc>Taken from Rx extensions.</devdoc>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Event", Justification = "From Rx extensions.")]
	public static class Event
	{
		/// <summary>
		/// Creates an instance of the IEvent interface.
		/// </summary>
		public static IEvent<TEventArgs> Create<TEventArgs>(object sender, TEventArgs eventArgs)
		{
			Guard.NotNull(() => sender, sender);
			Guard.NotNull(() => eventArgs, eventArgs);

			return new EventImpl<TEventArgs>(sender, eventArgs);
		}

		/// <summary>
		/// DEfault implementation of <see cref="IEvent{TEventArgs}"/>.
		/// </summary>
		private class EventImpl<TEventArgs> : IEvent<TEventArgs>
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="EventImpl{TEventArgs}"/> class.
			/// </summary>
			public EventImpl(object sender, TEventArgs eventArgs)
			{
				this.Sender = sender;
				this.EventArgs = eventArgs;
			}

			/// <summary>
			/// Gets the event sender.
			/// </summary>
			public object Sender { get; private set; }

			/// <summary>
			/// Gets the event arguments.
			/// </summary>
			public TEventArgs EventArgs { get; private set; }
		}
	}
}
