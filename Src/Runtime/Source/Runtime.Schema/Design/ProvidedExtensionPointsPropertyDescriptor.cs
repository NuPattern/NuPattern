using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using NuPattern.ComponentModel.Design;
using NuPattern.Modeling;
using NuPattern.VisualStudio;
using NuPattern.VisualStudio.Shell;

namespace NuPattern.Runtime.Schema.Design
{
    /// <summary>
    /// ImplementedExtensionPoints property descriptor
    /// </summary>
    internal class ProvidedExtensionPointsPropertyDescriptor : SimpleNamedPropertyDescriptor
    {
        private PatternSchema patternSchema;

        private IEnumerable<IExtensionPointSchema> extensionPoints;
        private IUserMessageService messageService;

        private IFxrUriReferenceService uriService;

        internal IFxrUriReferenceService UriService
        {
            get
            {
                if (this.uriService == null)
                {
                    var componentModel = this.patternSchema.Store.GetService<SComponentModel, IComponentModel>();

                    if (componentModel != null)
                    {
                        this.uriService = componentModel.GetService<IFxrUriReferenceService>();
                    }
                }

                return this.uriService;
            }
            set
            {
                this.uriService = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProvidedExtensionPointsPropertyDescriptor"/> class.
        /// </summary>
        /// <param name="patternSchema">The pattern.</param>
        /// <param name="allResolvedExtensionPoints">The extension points.</param>
        /// <param name="messageService">The message service.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <param name="attributes">The attributes.</param>
        public ProvidedExtensionPointsPropertyDescriptor(PatternSchema patternSchema, IEnumerable<IExtensionPointSchema> allResolvedExtensionPoints, IUserMessageService messageService, string propertyName, object value, params Attribute[] attributes)
            : base(propertyName, value, attributes)
        {
            this.patternSchema = patternSchema;
            this.extensionPoints = allResolvedExtensionPoints;
            this.messageService = messageService;
        }

        /// <summary>
        /// Gets an editor of the specified type.
        /// </summary>
        /// <param name="editorBaseType">The base type of editor, which is used to differentiate between multiple editors that a property supports.</param>
        /// <returns>
        /// An instance of the requested editor type, or null if an editor cannot be found.
        /// </returns>
        public override object GetEditor(Type editorBaseType)
        {
            return new CollectionDropDownEditor<IExtensionPointSchema>();
        }

        /// <summary>
        /// Gets the type converter for this property.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.ComponentModel.TypeConverter"/> that is used to convert the <see cref="T:System.Type"/> of this property.</returns>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode"/>
        /// </PermissionSet>
        public override TypeConverter Converter
        {
            get
            {
                var product = this.UriService.ResolveUri<IInstanceBase>(new Uri(patternSchema.PatternLink)) as IProduct;

                return new ExtensionPointsConverter(toolkit => !toolkit.Id.Equals(product.ToolkitInfo.Identifier));
            }
        }

        /// <summary>
        /// Determines whether the proeprty can be reset.
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public override bool CanResetValue(object component)
        {
            return true;
        }

        /// <summary>
        /// Resets the value of the property.
        /// </summary>
        /// <param name="component"></param>
        public override void ResetValue(object component)
        {
            this.patternSchema.Store.TransactionManager.DoWithinTransaction(() =>
            {
                this.patternSchema.ClearProvidedExtensionPoints();
            });
        }

        /// <summary>
        /// Determines if the value should be serialized.
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        /// <summary>
        /// When overridden in a derived class, gets the current value of the property on a component.
        /// </summary>
        /// <param name="component">The component with the property for which to retrieve the value.</param>
        /// <returns>
        /// The value of a property for a given component.
        /// </returns>
        public override object GetValue(object component)
        {
            return this.patternSchema.ProvidedExtensionPoints
                .Select(e => this.extensionPoints
                    .FirstOrDefault(ext => ext.RequiredExtensionPointId.Equals(e.ExtensionPointId, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        /// <summary>
        /// When overridden in a derived class, sets the value of the component to a different value.
        /// </summary>
        /// <param name="component">The component with the property value that is to be set.</param>
        /// <param name="value">The new value.</param>
        public override void SetValue(object component, object value)
        {
            IEnumerable<IExtensionPointSchema> validExtensionPoints = null;
            this.patternSchema.Store.TransactionManager.DoWithinTransaction(() =>
            {
                this.patternSchema.ClearProvidedExtensionPoints();

                var extensionPoints = value as List<IExtensionPointSchema>;

                var offendingExtensionPoints = GetOffendingExtensionPoints(extensionPoints);

                if (offendingExtensionPoints.Count() > 0)
                {
                    this.messageService.ShowWarning(
                        string.Format(CultureInfo.InvariantCulture,
                            Properties.Resources.ProvidedExtensionPointsPropertyDescriptor_OffendingExtensionException,
                            string.Join(
                                Environment.NewLine,
                                offendingExtensionPoints.Select(ext => ext.RequiredExtensionPointId))));
                }

                validExtensionPoints = extensionPoints.Except(offendingExtensionPoints);
                foreach (var extensionPoint in validExtensionPoints)
                {
                    this.patternSchema.AddProvidedExtensionPoint(extensionPoint);
                }
            });

            this.patternSchema.Store.TransactionManager.DoWithinTransaction(() =>
            {
                foreach (var extensionPoint in validExtensionPoints)
                {
                    this.patternSchema.CopyProvidedContractProperties(extensionPoint);
                }
            });

            value = validExtensionPoints;
        }

        private static IEnumerable<IExtensionPointSchema> GetOffendingExtensionPoints(IEnumerable<IExtensionPointSchema> extensionPoints)
        {
            var offendingExtensionPoints = new List<IExtensionPointSchema>();
            var properties = new List<IPropertySchema>();

            foreach (var extensionPoint in extensionPoints)
            {
                foreach (var prop in extensionPoint.Properties)
                {
                    if (properties.Any(p => p.Name.Equals(prop.Name, StringComparison.OrdinalIgnoreCase) &&
                        !p.Type.Equals(prop.Type, StringComparison.OrdinalIgnoreCase)))
                    {
                        offendingExtensionPoints.Add(extensionPoint);
                        break;
                    }

                    properties.Add(prop);
                }
            }

            return offendingExtensionPoints;
        }
    }
}