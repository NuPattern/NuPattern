
namespace NuPattern.Runtime
{
	/// <summary>
	/// Marker interface that can be used to import from MEF all 
	/// exported events.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "This is a required marker interface so we can get exports from MEF using it for events that are of disparate generic types.")]
	public interface IObservableEvent
	{
	}
}
