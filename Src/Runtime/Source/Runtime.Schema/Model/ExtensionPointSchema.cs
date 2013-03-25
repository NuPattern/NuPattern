using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NuPattern.Extensibility.Bindings;
using NuPattern.Runtime.Bindings;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Customizations to the <see cref="ExtensionPointSchema"/> class.
    /// </summary>
    [TypeDescriptionProvider(typeof(ExtensionPointSchemaTypeDescriptionProvider))]
    partial class ExtensionPointSchema
    {
        private ConditionBindingSettings[] conditionSettings;

        /// <summary>
        /// Gets the condition settings.
        /// </summary>
        public IEnumerable<IBindingSettings> ConditionSettings
        {
            get { return this.conditionSettings ?? (this.conditionSettings = this.GetConditionSettings()); }
        }

        /// <summary>
        /// Gets the autocreate of the contained element.
        /// </summary>
        public bool AutoCreate
        {
            get
            {
                return this.GetProjectedLinkProperty(l => l.AutoCreate);
            }
        }

        /// <summary>
        /// Gets the cardinality of the contained element.
        /// </summary>
        public Runtime.Cardinality Cardinality
        {
            get
            {
                return this.GetProjectedLinkProperty(l => l.Cardinality);
            }
        }

        /// <summary>
        /// Gets the allow add new of the contained element.
        /// </summary>
        public bool AllowAddNew
        {
            get
            {
                return this.GetProjectedLinkProperty(l => l.AllowAddNew);
            }
        }

        /// <summary>
        /// Gets the ordering of the instances of the contained element.
        /// </summary>
        public Int32 OrderGroup
        {
            get
            {
                return this.GetProjectedLinkProperty(l => l.OrderGroup);
            }
        }

        /// <summary>
        /// Gets the comparer for custom ordering.
        /// </summary>
        public string OrderGroupComparerTypeName
        {
            get
            {
                return this.GetProjectedLinkProperty(l => l.OrderGroupComparerTypeName);
            }
        }

        /// <summary>
        /// Gets the pattern line.
        /// </summary>
        public IPatternModelSchema PatternSchemaModel
        {
            get { return this.Store.GetRootElement(); }
        }

        /// <summary>
        /// Returns the value of the RequiredExtensionPointId property.
        /// </summary>
        private string GetRequiredExtensionPointIdValue()
        {
            if (string.IsNullOrEmpty(this.RepresentedExtensionPointId))
            {
                if (this.IsInheritedFromBase)
                {
                    if (this.Properties.All(p => p.IsInheritedFromBase))
                    {
                        return this.GetFullyQualifiedSchemaPath(toolkitId: this.PatternSchemaModel.BaseId);
                    }
                }

                return this.GetFullyQualifiedSchemaPath();
            }
            else
            {
                return this.RepresentedExtensionPointId;
            }
        }

        /// <summary>
        /// Gets the fully toolkit qualified path of the extension point.
        /// </summary>
        /// <remarks>
        /// Format: ToolkitGUID.SchemaPath
        /// </remarks>
        /// <param name="toolkitId">The toolkit id.</param>
        private string GetFullyQualifiedSchemaPath(string toolkitId = "")
        {
            var rootElement = this.Store.GetRootElement();

            if (rootElement != null)
            {
                var patternSchema = rootElement.Pattern;
                var path = this.SchemaPath;

                if (patternSchema != null && !string.IsNullOrEmpty(path))
                {
                    if (!string.IsNullOrEmpty(toolkitId))
                    {
                        return string.Concat(toolkitId, SchemaPathDelimiter, this.SchemaPath);
                    }
                    else
                    {
                        return string.Concat(this.PatternSchemaModel.Pattern.ExtensionId, SchemaPathDelimiter, this.SchemaPath);
                    }
                }
            }

            return string.Empty;
        }

        private ConditionBindingSettings[] GetConditionSettings()
        {
            if (string.IsNullOrEmpty(this.Conditions))
            {
                return new ConditionBindingSettings[0];
            }

            return BindingSerializer.Deserialize<ConditionBindingSettings[]>(this.Conditions);
        }
    }
}
