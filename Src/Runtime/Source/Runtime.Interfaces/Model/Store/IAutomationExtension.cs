namespace NuPattern.Runtime
{
	/// <summary>
	/// Represents a runtime behavior of an automation extension.
	/// </summary>
	public interface IAutomationExtension
	{
		/// <summary>
		/// Gets the owner of the automation extension.
		/// </summary>
		IProductElement Owner { get; }

		/// <summary>
		/// Gets the name of the automation element.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Executes the automation behavior.
		/// </summary>
		void Execute();

		/// <summary>
		/// Executes the automation behavior.
		/// </summary>
		void Execute(IDynamicBindingContext context);
	}

	/// <summary>
	/// Represents a runtime behavior of an automation extension that can be configured using settings.
	/// </summary>
	public interface IAutomationExtension<out TSettings> : IAutomationExtension
		where TSettings : IAutomationSettings
	{
		/// <summary>
		/// Gets the settings for this automation extension.
		/// </summary>
		TSettings Settings { get; }
	}
}