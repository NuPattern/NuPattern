using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Features = Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace Microsoft.VisualStudio.Patterning.Extensibility.Binding
{
	/// <summary>
	/// Defines a factory to create <see cref="IDynamicBinding{T}"/>.
	/// </summary>
	[PartCreationPolicy(CreationPolicy.Shared)]
	[Export(typeof(IBindingFactory))]
	[CLSCompliant(false)]
	public class BindingFactory : IBindingFactory
	{
		private IFeatureCompositionService compositionService;

		/// <summary>
		/// Initializes a new instance of the <see cref="BindingFactory"/> class.
		/// </summary>
		/// <param name="compositionService">The composition service.</param>
		[ImportingConstructor]
		public BindingFactory(IBindingCompositionService compositionService)
		{
			Guard.NotNull(() => compositionService, compositionService);

			this.compositionService = compositionService;
		}

		/// <summary>
		/// Creates the specified settings.
		/// </summary>
		/// <typeparam name="T">The type of the binding.</typeparam>
		/// <param name="settings">The settings.</param>
		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "MEF")]
		public IDynamicBinding<T> CreateBinding<T>(IBindingSettings settings)
			where T : class
		{
			Guard.NotNull(() => settings, settings);

			var delegatingComposition = new DelegatingCompositionService(this.compositionService);

			return new DynamicBinding<T>(
				delegatingComposition,
				settings.TypeId,
				this.GetPropertyBindings(settings.Properties, delegatingComposition));
		}

		/// <summary>
		/// Creates the binding dynamic context.
		/// </summary>
		public IDynamicBindingContext CreateContext()
		{
			return new CompositionServiceBindingContext(this.compositionService);
		}

		private PropertyBinding[] GetPropertyBindings(IEnumerable<IPropertyBindingSettings> properties, IFeatureCompositionService composition)
		{
			var bindings = new List<PropertyBinding>();

			if (properties == null)
				return bindings.ToArray();

			foreach (var property in properties)
			{
				if (!string.IsNullOrEmpty(property.Value))
				{
					bindings.Add(new FixedValuePropertyBinding(property.Name, property.Value));
				}
				else if (property.ValueProvider != null)
				{
					bindings.Add(
						new ValueProviderPropertyBinding(
							property.Name,
							new DynamicBinding<Features.IValueProvider>(
								composition,
								property.ValueProvider.TypeId,
								this.GetPropertyBindings(property.ValueProvider.Properties, composition))));
				}
			}

			return bindings.ToArray();
		}
	}
}