
namespace NuPattern.Runtime
{
	/// <summary>
	/// Represents the Sender and EventArg values of a .NET event.
	/// </summary>
	/// <devdoc>Taken from Rx extensions.</devdoc>
	public interface IEvent<out TEventArgs>
	{
		/// <summary>
		/// Gets the event arguments value of the event.
		/// </summary>
		TEventArgs EventArgs { get; }

		/// <summary>
		/// Gets the sender value of the event.
		/// </summary>
		object Sender { get; }
	}
}
