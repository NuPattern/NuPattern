namespace Microsoft.VisualStudio.Patterning.Runtime
{
	/// <summary>
	/// Defines the interface for a type that provides reference kind information.
	/// </summary>
	public interface IReferenceKindProvider
	{
	}

	/// <summary>
	/// Defines the value typed interface for a type that provides reference kind information.
	/// </summary>
	/// <typeparam name="TValue">The type of the value being provided.</typeparam>
	public interface IReferenceKindProvider<TValue> : IReferenceKindProvider
	{
	}
}