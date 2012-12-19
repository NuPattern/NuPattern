using Microsoft.VisualStudio.Modeling;

namespace NuPattern.Extensibility
{
	/// <summary>
	/// Add rule that automation extensions can use to automatically create a 1..1 related 
	/// settings class whenever the extension is created.
	/// </summary>
	/// <typeparam name="TAutomationExtension">The type of the automation extension.</typeparam>
	/// <typeparam name="TAutomationSettings">The type of the automation extension settings.</typeparam>
	public abstract class AutomationExtensionAddRule<TAutomationExtension, TAutomationSettings> : AddRule
		where TAutomationExtension : ExtensionElement
		where TAutomationSettings : ModelElement
	{
		/// <summary>
		/// Public virtual method for the client to have his own user-defined add rule class.
		/// </summary>
		/// <param name="e">The event argument data.</param>
		public override void ElementAdded(ElementAddedEventArgs e)
		{
			Guard.NotNull(() => e, e);

			var element = (TAutomationExtension)e.ModelElement;

			if (!element.Store.TransactionManager.CurrentTransaction.IsSerializing)
			{
				element.WithTransaction(elem => elem.Create<TAutomationSettings>());
			}
		}
	}
}