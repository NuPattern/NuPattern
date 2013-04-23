using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Modeling;
using NuPattern.Modeling;
using NuPattern.Runtime.Schema.Design;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Customizations for the NamedElementSchema class.
    /// </summary>
    [TypeDescriptionProvider(typeof(NamedElementTypeDescriptionProvider))]
    partial class NamedElementSchema
    {
        internal const string SchemaPathDelimiter = ".";

        /// <summary>
        /// Returns whether the state containing this instance is currently 
        /// in a transaction or not.
        /// </summary>
        public bool InTransaction
        {
            get { return this.Store.TransactionActive; }
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        public ITransaction BeginTransaction()
        {
            return new ModelingTransaction(this.Store.TransactionManager.BeginTransaction());
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        public ITransaction BeginTransaction(string name)
        {
            return new ModelingTransaction(this.Store.TransactionManager.BeginTransaction(name));
        }

        INamedElementInfo INamedElementInfo.Parent
        {
            get { return FindEmbeddingElement() as INamedElementInfo; }
        }

        INamedElementSchema INamedElementSchema.Parent
        {
            get { return FindEmbeddingElement() as INamedElementSchema; }
        }

        private ModelElement FindEmbeddingElement()
        {
            var mel = this as ModelElement;
            if (mel == null)
            {
                return null;
            }

            return DomainRelationshipInfo.FindEmbeddingElement(mel);
        }

        /// <summary>
        /// Returns a spaced string representation for the given pascal cased string.
        /// </summary>
        internal static string MakePascalIntoWords(string potentialPascal)
        {
            if (String.IsNullOrEmpty(potentialPascal))
            {
                return potentialPascal;
            }

            if (char.IsLower(potentialPascal[0]))
            {
                potentialPascal = char.ToUpper(potentialPascal[0], CultureInfo.CurrentCulture) + potentialPascal.Substring(1);
            }

            return Regex.Replace(potentialPascal, @"(?<low>[^\p{Lu}\p{Lt}\p{Nl}])(?<hi>[\p{Lu}\p{Lt}\p{Nl}])", "${low} ${hi}");
        }

        /// <summary>
        /// Provide default name of element, based upon the DisplayName property.
        /// </summary>
        protected override void MergeConfigure(ElementGroup elementGroup)
        {
            if (DomainClassInfo.HasNameProperty(this))
            {
                DomainClassInfo.SetUniqueName(this, SanitizeName(this.GetDomainClass().DisplayName));
            }

            base.MergeConfigure(elementGroup);
        }

        /// <summary>
        /// Makes a name from the given value.
        /// </summary>
        private static string SanitizeName(string value)
        {
            Guard.NotNull(() => value, value);

            return value.Replace(@" ", string.Empty);
        }

        /// <summary>
        /// Returns the calculated value of the DisplayName property while being tracked.
        /// </summary>
        internal string CalculateDisplayNameTrackingValue()
        {
            if (string.IsNullOrEmpty(this.Name))
            {
                return string.Empty;
            }

            // Separate pascal cased words in the string
            return MakePascalIntoWords(this.Name);
        }

        /// <summary>
        /// Returns the calculated value of the Description property while being tracked.
        /// </summary>
        internal string CalculateDescriptionTrackingValue()
        {
            if (string.IsNullOrEmpty(this.SchemaPath))
            {
                return string.Empty;
            }

            // Separate pascal cased words in the string
            return string.Format(CultureInfo.CurrentCulture, Properties.Resources.NamedElementSchema_DescriptionFormat, this.SchemaPath);
        }

        /// <summary>
        /// Returns the calculated value of the CodeIdentifier property while being tracked.
        /// </summary>
        internal string CalculateCodeIdentifierTrackingValue()
        {
            if (string.IsNullOrEmpty(this.Name))
            {
                return string.Empty;
            }

            return this.Name;
        }

        /// <summary>
        /// Gets the IsInheritedFromBase property value.
        /// </summary>
        private bool GetIsInheritedFromBaseValue()
        {
            return !string.IsNullOrEmpty(this.BaseId);
        }

        /// <summary>
        /// Gets the SchemaPath property value.
        /// </summary>
        /// <remarks>
        /// Returns the path [PatternName].[ViewName].[ElementOrCollectionName].[PropertyName] of the instance of this element in the variability model 
        /// beginning at the pattern.
        /// </remarks>
        public string GetSchemaPathValue()
        {
            if (this is PatternSchema)
            {
                return this.Name;
            }

            var view = this as ViewSchema;
            if (view != null)
            {
                // Ensure the view is attached to model.
                if (view.Pattern != null)
                {
                    return string.Concat(view.Pattern.Name, SchemaPathDelimiter, view.Name);
                }
            }

            var abstractElement = this as AbstractElementSchema;
            if (abstractElement != null)
            {
                string path = string.Empty;
                if (abstractElement.View != null)
                {
                    // Ensure the element is attached to model.
                    if (abstractElement.View.Pattern != null)
                    {
                        path = string.Concat(abstractElement.View.Pattern.Name, SchemaPathDelimiter, abstractElement.View.Name);
                        return string.Concat(path, SchemaPathDelimiter, abstractElement.Name);
                    }
                }
                else
                {
                    return GetAbstractElementSchemaPath(abstractElement);
                }
            }

            var extensionPointElement = this as ExtensionPointSchema;
            if (extensionPointElement != null)
            {
                var path = string.Empty;
                if (extensionPointElement.View != null)
                {
                    // Ensure the extension is attached to model.
                    if (extensionPointElement.View.Pattern != null)
                    {
                        path = string.Concat(extensionPointElement.View.Pattern.Name, SchemaPathDelimiter, extensionPointElement.View.Name);
                        return string.Concat(path, SchemaPathDelimiter, extensionPointElement.Name);
                    }
                }
                else
                {
                    // Ensure the extension is attached to the model
                    if (extensionPointElement.Owner != null)
                    {
                        path = GetAbstractElementSchemaPath(extensionPointElement.Owner);
                        if (!string.IsNullOrEmpty(path))
                        {
                            return string.Concat(path, SchemaPathDelimiter, extensionPointElement.Name);
                        }
                    }
                }
            }

            var property = this as PropertySchema;
            if (property != null)
            {
                if (property.Owner != null)
                {
                    string path = property.Owner.SchemaPath;
                    if (!String.IsNullOrEmpty(path))
                    {
                        return string.Concat(path, SchemaPathDelimiter, property.Name);
                    }
                }
            }

            return string.Empty;
        }


        /// <summary>
        /// Gets the root pattern ancestor for this instance. Note that for a pattern, 
        /// this may be an ancestor pattern if it has been instantiated as an 
        /// extension point.
        /// </summary>
        /// <remarks>The returned value may be null if the element is not rooted in any pattern.</remarks>
        public IPatternSchema Root
        {
            get
            {
                INamedElementSchema current = this;
                while (current != null && !IsRootPattern(current))
                {
                    current = current.Parent;
                }

                return current as IPatternSchema;
            }
        }

        private static bool IsRootPattern(INamedElementSchema current)
        {
            return current is IPatternSchema && current.Parent == null;
        }

        private static string GetAbstractElementSchemaPath(AbstractElementSchema element)
        {
            var path = string.Empty;
            path = path.Insert(0, string.Concat(SchemaPathDelimiter, element.Name));
            while (element != null && element.View == null)
            {
                element = element.Owner;
                if (element != null)
                {
                    path = path.Insert(0, string.Concat(SchemaPathDelimiter, element.Name));
                }
            }

            if (element == null)
            {
                return string.Empty;
            }

            path = path.Insert(0, element.View.Name);
            return string.Concat(element.View.Pattern.Name, SchemaPathDelimiter, path);
        }
    }
}