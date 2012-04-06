using System;
using System.Linq;

namespace Microsoft.VisualStudio.Patterning.Runtime
{
	/// <summary>
	/// Usability extension methods to deal with automation extensions.
	/// </summary>
	public static class AutomationExtensionExtensions
	{
		/// <summary>
		/// Attemps to resolve the given <paramref name="automationReferenceId"/> and 
		/// <typeparamref name="TAutomation"/> type to a configured automation 
		/// extension on in the same container as the given <paramref name="automationExtension"/>.
		/// </summary>
		/// <typeparam name="TAutomation">The type of the automation to look up.</typeparam>
		/// <param name="automationExtension">The automation extension that references another automation extension by name.</param>
		/// <param name="automationReferenceId">Name of the automation extension.</param>
		public static TAutomation ResolveAutomationReference<TAutomation>(this IAutomationExtension automationExtension, Guid automationReferenceId)
			where TAutomation : IAutomationExtension
		{
			Guard.NotNull(() => automationExtension, automationExtension);
			Guard.NotNull(() => automationExtension.Owner, automationExtension.Owner);
			Guard.NotNull(() => automationExtension.Owner.Info, automationExtension.Owner.Info);

			// We must lookup the name of the extension from the id on its setting, 
			// as the setting name becomes the extension name.
			return
				(from info in automationExtension.Owner.Info.AutomationSettings
				 from extension in automationExtension.Owner.AutomationExtensions
				 where info.Name == extension.Name
				 let setting = info.As<IAutomationSettings>()
				 where setting != null && setting.Id == automationReferenceId
				 select extension)
				.OfType<TAutomation>()
				.FirstOrDefault();
		}
	}
}
