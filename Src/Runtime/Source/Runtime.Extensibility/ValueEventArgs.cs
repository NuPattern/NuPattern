using System;

namespace NuPattern.Runtime
{
	/// <summary>
	/// Provides a static factory method for creating the arguments
	/// class for events that publish a single value.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Static factory for typed args class.")]
	public static class ValueEventArgs
	{
		/// <summary>
		/// Creates an arguments class for the given value.
		/// </summary>
		/// <remarks>
		/// This helper static method makes it possible to avoid specifying 
		/// the type of the event args and just pass its value.
		/// </remarks>
		public static ValueEventArgs<TValue> Create<TValue>(TValue value)
		{
			return new ValueEventArgs<TValue>(value);
		}
	}

	/// <summary>
	/// Arguments for events that publish a single value.
	/// </summary>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	public class ValueEventArgs<TValue> : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ValueEventArgs{TValue}"/> class.
		/// </summary>
		/// <param name="value">The value.</param>
		public ValueEventArgs(TValue value)
		{
			this.Value = value;
		}

		/// <summary>
		/// Gets the value.
		/// </summary>
		public TValue Value { get; private set; }
	}
}
