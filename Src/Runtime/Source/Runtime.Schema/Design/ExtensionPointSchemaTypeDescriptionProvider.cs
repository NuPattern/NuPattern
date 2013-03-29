using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Design;
using NuPattern.ComponentModel;
using NuPattern.Runtime.Bindings;
using NuPattern.Runtime.Design;
using NuPattern.VisualStudio;
using NuPattern.VisualStudio.Shell;

namespace NuPattern.Runtime.Schema.Design
{
    /// <summary>
    /// ExtensionPointSchema type descriptor provider. 
    /// </summary>
    internal class ExtensionPointSchemaTypeDescriptionProvider : ElementTypeDescriptionProvider
    {
        /// <summary>
        /// Overridables for the derived class to provide a custom type descriptor.
        /// </summary>
        /// <param name="parent">Parent custom type descriptor.</param>
        /// <param name="element">Element to be described.</param>
        /// <returns></returns>
        protected override ElementTypeDescriptor CreateTypeDescriptor(ICustomTypeDescriptor parent, ModelElement element)
        {
            return new ExtensionPointSchemaTypeDescriptor(parent, element);
        }
    }

    /// <summary>
    /// ExtensionPointSchema type descriptor.
    /// </summary>
    internal class ExtensionPointSchemaTypeDescriptor : PatternElementSchemaTypeDescriptor
    {
        private IUserMessageService messageService;

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
        /// Initializes a new instance of the <see cref="ExtensionPointSchemaTypeDescriptor"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="modelElement">The model element.</param>
        public ExtensionPointSchemaTypeDescriptor(ICustomTypeDescriptor parent, ModelElement modelElement)
            : base(parent, modelElement)
        {
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <param name="attributes">The attributes.</param>
        /// <returns></returns>
        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            var properties = base.GetProperties(attributes).Cast<PropertyDescriptor>().ToList();

            // Add descriptor for ValidationRules collection
            properties.ReplaceDescriptor<ExtensionPointSchema, string>(
                ep => ep.Conditions,
                x => new StringCollectionPropertyDescriptor<ConditionBindingSettings>(x));

            var element = (ExtensionPointSchema)this.ModelElement;

            // Replace descriptor for the RepresentedExtensionPointId property
            properties.ReplaceDescriptor<ExtensionPointSchema, string>(
                ep => ep.RepresentedExtensionPointId,
                x => new HostedExtensionPointPropertyDescriptor(x, element, this.MessageService));

            //properties.Add(new HostedExtensionPointPropertyDescriptor(
            //        element,
            //        this.MessageService,
            //        "HostedExtensionPointRaw",
            //        string.Empty,
            //        new CategoryAttribute(Resources.ExtensibilityCategory),
            //        new DisplayNameAttribute(Resources.HostedExtensionPointDisplayName),
            //        new DescriptionAttribute(Resources.HostedExtensionPointDescription)));

            return new PropertyDescriptorCollection(properties.ToArray());
        }

        /// <summary>
        /// Returns a collection of custom attributes for the type represented by this type descriptor.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.ComponentModel.AttributeCollection"/> containing the attributes for the type. The default is <see cref="F:System.ComponentModel.AttributeCollection.Empty"/>.
        /// </returns>
        public override AttributeCollection GetAttributes()
        {
            var extensionPoint = ((ExtensionPointSchema)base.ModelElement);
            var atts = base.GetAttributes().Cast<Attribute>();

            // Format the displayed description
            string contractIdText = string.Format(CultureInfo.CurrentCulture,
                    Properties.Resources.ExtensionPointSchemaTypeDescriptionProvider_Description,
                    extensionPoint.RequiredExtensionPointId);
            var description = contractIdText;
            if (!string.IsNullOrEmpty(extensionPoint.Description))
            {
                description = string.Concat(extensionPoint.Description, Environment.NewLine, contractIdText);
            }

            // Format the displayed DisplayName
            var displayName = string.Format(CultureInfo.CurrentCulture,
                Properties.Resources.ExtensionPointSchemaTypeDescriptionProvider_DisplayName,
                extensionPoint.DisplayName, extensionPoint.SchemaPath);

            var filteredAttributes =
                atts.Where(att => !typeof(DescriptionAttribute).IsAssignableFrom(att.GetType()) &&
                    !typeof(DisplayNameAttribute).IsAssignableFrom(att.GetType()));

            var newAttributes = filteredAttributes.Concat(
                new Attribute[] { 
                        new DescriptionAttribute(description), 
                        new DisplayNameAttribute(displayName) });

            return new AttributeCollection(newAttributes.ToArray());
        }
    }
}
