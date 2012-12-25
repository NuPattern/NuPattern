using System;
using System.ComponentModel;

namespace NuPattern.Runtime
{
	/// <summary>
	/// Default base implementation of an <see cref="IAutomationExtension"/> with settings.
	/// </summary>
	/// <typeparam name="TSettings">The type of the settings.</typeparam>
	public abstract class AutomationExtension<TSettings> : IAutomationExtension<TSettings>, ISupportInitialize, IDisposable
		where TSettings : IAutomationSettings
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AutomationExtension{TSettings}"/> class.
		/// </summary>
		protected AutomationExtension(IProductElement owner, TSettings settings)
		{
			Guard.NotNull(() => owner, owner);
			Guard.NotNull(() => settings, settings);
			Guard.NotNull(() => settings.Owner, settings.Owner);

			this.Owner = owner;
			this.Settings = settings;

			this.Name = settings.Name;
		}

		/// <summary>
		/// Gets the name of the automation extension.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Gets the owner of the automation extension.
		/// </summary>
		public IProductElement Owner { get; private set; }

		/// <summary>
		/// Gets the settings for this automation extension.
		/// </summary>
		public TSettings Settings { get; private set; }

		/// <summary>
		/// Executes the automation behavior.
		/// </summary>
		public abstract void Execute();

		/// <summary>
		/// Executes the automation behavior on a pre-build context
		/// </summary>
		public abstract void Execute(IDynamicBindingContext context);

		/// <summary>
		/// Signals the object that initialization is starting, before all imports are satisfied.
		/// </summary>
		public virtual void BeginInit()
		{
		}

		/// <summary>
		/// Signals the object that initialization is complete, after all imports are satisfied.
		/// </summary>
		public virtual void EndInit()
		{
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing">Specifies <see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
		}
	}
}
