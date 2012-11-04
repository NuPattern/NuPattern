using System;

namespace Microsoft.VisualStudio.Patterning.Extensibility.Binding
{
	/// <summary>
	/// Exception caused when a deserialization exception occurs.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors")]
	public class BindingSerializationException : Exception
	{
		/// <summary>
		/// Creates a new BindingSerializationException
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="inner">The exception that is the cause of the current exception, or a null reference if no inner exception is specified</param>
		public BindingSerializationException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
