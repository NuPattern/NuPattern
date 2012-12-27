using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Design;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using NuPattern.Runtime.Schema.Properties;

namespace NuPattern.Runtime.Schema
{
	/// <summary>
	/// PatternSchema type descriptor provider. 
	/// </summary>
	public class PatternSchemaTypeDescriptorProvider : ElementTypeDescriptionProvider
	{
		/// <summary>
		/// Overridables for the derived class to provide a custom type descriptor.
		/// </summary>
		/// <param name="parent">Parent custom type descriptor.</param>
		/// <param name="element">Element to be described.</param>
		protected override ElementTypeDescriptor CreateTypeDescriptor(ICustomTypeDescriptor parent, ModelElement element)
		{
			return new PatternSchemaTypeDescriptor(parent, element);
		}

		/// <summary>
		/// PatternSchema type descriptor.
		/// </summary>
		internal class PatternSchemaTypeDescriptor : PatternElementSchemaTypeDescriptor
		{
			private IEnumerable<IExtensionPointSchema> extensionPoints;
			private IUserMessageService messageService;

			/// <summary>
			/// Initializes a new instance of the <see cref="PatternSchemaTypeDescriptor"/> class.
			/// </summary>
			/// <param name="parent">The parent.</param>
			/// <param name="modelElement">The model element.</param>
			public PatternSchemaTypeDescriptor(ICustomTypeDescriptor parent, ModelElement modelElement)
				: base(parent, modelElement)
			{
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="PatternSchemaTypeDescriptor"/> class.
			/// </summary>
			/// <param name="modelElement">The model element.</param>
			public PatternSchemaTypeDescriptor(ModelElement modelElement)
				: base(modelElement)
			{
			}

			internal IEnumerable<IExtensionPointSchema> ExtensionPoints
			{
				get
				{
					if (extensionPoints == null)
					{
						var patternManager = this.ModelElement.Store.GetService<IPatternManager>();

						if (patternManager != null)
						{
							this.extensionPoints =
								patternManager.InstalledToolkits.SelectMany(f => f.Schema.FindAll<IExtensionPointSchema>());
						}
						else
						{
							this.extensionPoints = Enumerable.Empty<IExtensionPointSchema>();
						}
					}

					return extensionPoints;
				}
			}

			internal IUserMessageService MessageService
			{
				get
				{
					if (messageService == null)
					{
						var componentModel = this.ModelElement.Store.GetService<SComponentModel, IComponentModel>();

						if (componentModel != null)
						{
							this.messageService = componentModel.GetService<IUserMessageService>();
						}
					}

					return messageService;
				}
			}

			/// <summary>
			/// Returns the properties for this instance of a component using the attribute array as a filter.
			/// </summary>
			/// <param name="attributes">An array of type Attribute that is used as a filter.</param>
			/// <returns>
			/// An array of type Attribute that represents the properties for this component instance that match the given set of attributes.
			/// </returns>
			public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
			{
				var properties = base.GetProperties(attributes);
				var element = (PatternSchema)this.ModelElement;

				properties.Add(
					new ProvidedExtensionPointsPropertyDescriptor(
						element,
						this.ExtensionPoints,
						this.MessageService,
						"ImplementedExtensionPointsRaw",
						new List<IExtensionPointSchema>(),
						new CategoryAttribute(Resources.ExtensibilityCategory),
						new DisplayNameAttribute(Resources.ProvidedExtensionPointsDisplayName),
						new DescriptionAttribute(Resources.ProvidedExtensionPointsDescription)));

				return properties;
			}
		}
	}
}