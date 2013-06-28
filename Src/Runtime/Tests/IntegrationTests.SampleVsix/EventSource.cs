using System.ComponentModel;
using System.ComponentModel.Composition;

namespace NuPattern.Runtime.IntegrationTests.SampleVsix
{
	/// <summary>
	/// Sample event source.
	/// </summary>
	[Export]
	[Export(typeof(INotifyPropertyChanged))]
	public class EventSource : INotifyPropertyChanged
	{
		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged = (sender, args) => { };

		/// <summary>
		/// Raises the property changed event.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Intended for testing.")]
		public void RaisePropertyChanged(string propertyName)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
