namespace Microsoft.VisualStudio.Patterning.Runtime
{
	/// <summary>
	/// Represents a runtime behavior of a wizard automation extension.
	/// </summary>
	public interface IWizardAutomationExtension : IAutomationExtension
	{
		/// <summary>
		/// Gets a value indicating whether the wizard was canceled.
		/// </summary>
		bool IsCanceled { get; }
	}
}