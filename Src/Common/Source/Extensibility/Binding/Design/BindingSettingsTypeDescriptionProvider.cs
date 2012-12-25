using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace NuPattern.Extensibility.Binding
{
	/// <summary>
	/// Custom type description provider that can be associated with a binding settings class 
	/// to provide dynamic properties based on the binding type id.
	/// </summary>
	/// <typeparam name="T">Type of the binding that will be discovered via the standard mechanisms of MEF and project types</typeparam>
	public class BindingSettingsTypeDescriptionProvider<T> : TypeDescriptionProvider where T : class
	{
		private static readonly TypeDescriptionProvider parent = TypeDescriptor.GetProvider(typeof(BindingSettings));

		/// <summary>
		/// Gets a custom type descriptor for the given type and object.
		/// </summary>
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
		{
			return new BindingSettingsTypeDescriptor(parent.GetTypeDescriptor(objectType, instance), instance as BindingSettings);
		}

		private class BindingSettingsTypeDescriptor : CustomTypeDescriptor
		{
			private ICustomTypeDescriptor parent;
			private BindingSettings settings;

			public BindingSettingsTypeDescriptor(ICustomTypeDescriptor parent, BindingSettings settings)
				: base(parent)
			{
				this.parent = parent;
				this.settings = settings;
			}

			[ImportMany]
			private IEnumerable<Lazy<T, IFeatureComponentMetadata>> Components { get; set; }

			[Import]
			private IPlatuProjectTypeProvider ProjectTypeProvider { get; set; }

			public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
			{
				var properties = parent.GetProperties(attributes).Cast<PropertyDescriptor>().ToList();

				if (!string.IsNullOrEmpty(this.settings.TypeId))
				{
					if (this.ProjectTypeProvider == null)
						FeatureCompositionService.Instance.SatisfyImportsOnce(this);

					var extensionType = this.Components.FromFeaturesCatalog()
						.Where(binding => binding.Metadata.Id == this.settings.TypeId)
						.Select(binding => binding.Metadata.ExportingType)
						.FirstOrDefault();

					if (extensionType == null && !string.IsNullOrEmpty(this.settings.TypeId))
					{
						extensionType = (from type in this.ProjectTypeProvider.GetTypes<T>()
										 let meta = type.AsProjectFeatureComponent()
										 where meta != null && meta.Id == settings.TypeId
										 select type)
										.FirstOrDefault();
					}

					if (extensionType != null)
					{
						foreach (var descriptor in TypeDescriptor.GetProperties(extensionType).Cast<PropertyDescriptor>().Where(d => d.IsPlatuBindableProperty()))
						{
							properties.Add(new DesignPropertyDescriptor(
								descriptor.Name,
								descriptor.PropertyType,
								this.settings.GetType(),
								descriptor.Attributes.Cast<Attribute>().ToArray()));
						}
					}
				}

				return new PropertyDescriptorCollection(properties.ToArray());
			}
		}
	}
}