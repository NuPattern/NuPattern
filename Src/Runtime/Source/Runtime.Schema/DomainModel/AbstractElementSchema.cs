using System;
using System.Collections.Generic;

namespace NuPattern.Runtime.Schema
{
    partial class AbstractElementSchema
    {
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

        IEnumerable<IAbstractElementInfo> IElementInfoContainer.Elements
        {
            get { return this.Elements; }
        }

        IEnumerable<IAbstractElementSchema> IElementSchemaContainer.Elements
        {
            get { return this.Elements; }
        }

        IEnumerable<IExtensionPointInfo> IElementInfoContainer.ExtensionPoints
        {
            get { return this.ExtensionPoints; }
        }

        IEnumerable<IExtensionPointSchema> IElementSchemaContainer.ExtensionPoints
        {
            get { return this.ExtensionPoints; }
        }
    }
}
