using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using NuPattern.Runtime;

namespace NuPattern.Library.Automation
{
    internal class ProductElementDictionaryConverter
    {
        private const string ParentNotation = "Parent";
        private const string ElementNameNotation = "InstanceName";
        private const string DefinitionNameNotation = "DefinitionName";
        private const string PropertyAddressFormat = "{0}.{1}";
        private const string ElementInstanceAddressFormat = "{0}[{1}]";
        private IDictionary<string, string> dictionary;
        private PluralizationService pluralization;

        public static IDictionary<string, string> Convert(IProductElement element)
        {
            var dictionaryConverter = new ProductElementDictionaryConverter(PluralizationService.CreateService(new System.Globalization.CultureInfo("en-US")));
            return dictionaryConverter.ConvertToDictionary(element);
        }

        public ProductElementDictionaryConverter(PluralizationService pluralizationService)
        {
            dictionary = new Dictionary<string, string>();
            this.pluralization = pluralizationService;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Not Applicable")]
        public IDictionary<string, string> ConvertToDictionary(IProductElement element)
        {
            // Add entries for this element
            AddSelfEntries(element, null);

            // Add entries for all descendant elements (down the tree)
            AddDecendantsEntries(element);

            // Add entries for alll ancestor elements (up the tree)
            AddAncestorsEntries(element);

            return this.dictionary;
        }

        private void AddAncestorsEntries(IProductElement element)
        {
            Guard.NotNull(() => element, element);

            var product = element as IProduct;
            if (product != null)
            {
                // no parent elements
                return;
            }
            else
            {
                AddAncestorElementsEntries(element);
            }
        }

        private void AddAncestorElementsEntries(IInstanceBase element, string parentPath = null)
        {
            Guard.NotNull(() => element, element);

            var parentElement = element.Parent;

            string elementPath = ParentNotation;
            if (!string.IsNullOrEmpty(parentPath))
            {
                elementPath = GetPropertyKey(parentPath, elementPath);
            }

            var parentProduct = parentElement as IProduct;
            if (parentProduct != null)
            {
                AddSelfEntries(parentProduct, elementPath);
                return;
            }
            else
            {
                var parentView = parentElement as IView;
                if (parentView != null)
                {
                    AddSelfEntries(parentView, elementPath);
                    AddDecendantElementsEntries(parentView, elementPath, element);
                    AddAncestorElementsEntries(parentView, elementPath);
                }
                else
                {
                    var parentAbstractElement = parentElement as IAbstractElement;
                    if (parentAbstractElement != null)
                    {
                        AddSelfEntries(parentAbstractElement, elementPath);
                        AddDecendantElementsEntries(parentAbstractElement, elementPath, element);
                        AddAncestorElementsEntries(parentAbstractElement, elementPath);
                    }
                    else
                    {
                        throw new NotImplementedException(
                            string.Format(CultureInfo.CurrentCulture, "Element '{0}' does not support ancestors.", element));
                    }
                }
            }
        }

        private void AddDecendantsEntries(IProductElement element)
        {
            Guard.NotNull(() => element, element);

            var product = element as IProduct;
            if (product != null)
            {
                // Skip to views
                foreach (var view in product.Views)
                {
                    AddSelfEntries(view, view.DefinitionName);
                    AddDecendantElementsEntries(view, view.DefinitionName);
                }
            }
            else
            {
                var container = element as IElementContainer;
                if (container != null)
                {
                    AddDecendantElementsEntries(container, null);
                }
                else
                {
                    throw new NotImplementedException(
                        string.Format(CultureInfo.CurrentCulture, "Element '{0}' does not support descendants.", element));
                }
            }
        }

        private void AddDecendantElementsEntries(IElementContainer container, string containerPath = null, IInstanceBase originatingChild = null)
        {
            Guard.NotNull(() => container, container);

            // Maybe one or more instances of different element types (filter the originatingchild instance) to prevent infinite recursion back down to a descendant element.
            foreach (var childElementGroup in container.Elements.Where(e => e != originatingChild).GroupBy(e => e.DefinitionName))
            {
                var groupIndex = 0;

                foreach (var childElement in childElementGroup)
                {
                    string elementPath = GetPluralizedElementPath(childElement, groupIndex);
                    if (!string.IsNullOrEmpty(containerPath))
                    {
                        elementPath = GetPropertyKey(containerPath, elementPath);
                    }

                    AddSelfEntries(childElement, elementPath);
                    AddDecendantElementsEntries(childElement, elementPath);
                    groupIndex++;
                }
            }

            // Maybe one or more instances of different extension types (filter the originatingchild instance) to prevent infinite recursion.
            foreach (var childExtensionGroup in container.Extensions.Where(e => e != originatingChild).GroupBy(e => e.DefinitionName))
            {
                var groupIndex = 0;

                foreach (var childExtension in childExtensionGroup)
                {
                    var extensionPoint = container.Info.ExtensionPoints
                        .FirstOrDefault(ep => childExtension.Info.ProvidedExtensionPoints.FirstOrDefault(x => x.ExtensionPointId == ep.RequiredExtensionPointId) != null);
                    if (extensionPoint != null)
                    {
                        string elementPath = GetPluralizedExtensionPath(extensionPoint, groupIndex);
                        if (!string.IsNullOrEmpty(containerPath))
                        {
                            elementPath = GetPropertyKey(containerPath, elementPath);
                        }

                        AddSelfEntries(childExtension, elementPath);
                        groupIndex++;
                    }
                }
            }
        }

        private void AddSelfEntries(IProductElement element, string elementPath = null)
        {
            AddBasicProperties(element, elementPath);
            AddElementProperties(element, elementPath);
        }

        private void AddSelfEntries(IView view, string viewPath)
        {
            this.dictionary.Add(GetPropertyKey(viewPath, DefinitionNameNotation), view.DefinitionName);
        }

        private void AddBasicProperties(IProductElement element, string elementPath)
        {
            if (element.InstanceName != null)
            {
                this.dictionary.Add(GetPropertyKey(elementPath, ElementNameNotation), element.InstanceName);
            }

            if (element.DefinitionName != null)
            {
                this.dictionary.Add(GetPropertyKey(elementPath, DefinitionNameNotation), element.DefinitionName);
            }
        }

        private void AddElementProperties(IProductElement element, string elementPath)
        {
            if (!element.Properties.Any())
                return;

            var properties = from prop in TypeDescriptor.GetProperties(element).OfType<PropertyDescriptor>()
                             let info = element.Properties.FirstOrDefault(p => p.Info != null && p.Info.Name == prop.Name)
                             where info != null
                             select new
                             {
                                 Name = info.Info.CodeIdentifier,
                                 Descriptor = prop,
                             };

            foreach (var property in properties)
            {
                this.dictionary[GetPropertyKey(elementPath, property.Name)] =
                    property.Descriptor.Converter.ConvertToString(property.Descriptor.GetValue(element));
            }
        }

        private string GetPluralizedElementPath(IAbstractElement element, int index)
        {
            if (element.Info.Cardinality == Cardinality.ZeroToMany ||
                element.Info.Cardinality == Cardinality.OneToMany)
            {
                return string.Format(CultureInfo.CurrentCulture, ElementInstanceAddressFormat,
                    this.pluralization.Pluralize(element.Info.CodeIdentifier), index.ToString(CultureInfo.CurrentCulture));
            }
            else
            {
                return element.Info.CodeIdentifier;
            }
        }

        private string GetPluralizedExtensionPath(IExtensionPointInfo element, int index)
        {
            if (element.Cardinality == Cardinality.ZeroToMany ||
                element.Cardinality == Cardinality.OneToMany)
            {
                return string.Format(CultureInfo.CurrentCulture, ElementInstanceAddressFormat,
                    this.pluralization.Pluralize(element.CodeIdentifier), index.ToString(CultureInfo.CurrentCulture));
            }
            else
            {
                return element.CodeIdentifier;
            }
        }
        private static string GetPropertyKey(string parentName, string propertyKey)
        {
            if (string.IsNullOrEmpty(parentName))
            {
                return propertyKey;
            }

            return string.Format(CultureInfo.InvariantCulture, PropertyAddressFormat, parentName, propertyKey);
        }
    }
}
