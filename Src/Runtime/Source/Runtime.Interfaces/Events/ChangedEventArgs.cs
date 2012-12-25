using System;

namespace NuPattern.Runtime
{
	/// <summary>
	/// Arguments for events that publish old and new values for changes.
	/// </summary>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	public class ChangedEventArgs<TValue> : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ChangedEventArgs{TValue}"/> class.
		/// </summary>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		public ChangedEventArgs(TValue oldValue, TValue newValue)
		{
			this.OldValue = oldValue;
			this.NewValue = newValue;
		}

		/// <summary>
		/// Gets the old value.
		/// </summary>
		public TValue OldValue { get; private set; }

		/// <summary>
		/// Gets the new value.
		/// </summary>
		public TValue NewValue { get; private set; }
	}
}
