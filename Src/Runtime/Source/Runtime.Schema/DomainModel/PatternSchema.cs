using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using NuPattern.Runtime.Schema.Design;
using NuPattern.VisualStudio;

namespace NuPattern.Runtime.Schema
{
    [TypeDescriptionProvider(typeof(PatternSchemaTypeDescriptorProvider))]
    partial class PatternSchema
    {
        private string extensionId;
        private IFxrUriReferenceService uriService;
        private IPatternManager patternManager;

        internal string Identifier { get; set; }

        internal IFxrUriReferenceService UriService
        {
            get
            {
                if (this.uriService == null)
                {
                    var componentModel = this.Store.GetService<SComponentModel, IComponentModel>();

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

        internal IPatternManager PatternManager
        {
            get
            {
                if (this.patternManager == null)
                {
                    this.patternManager = this.Store.GetService<IPatternManager, IPatternManager>();
                }

                return this.patternManager;
            }
        }

        /// <summary>
        /// Gets the autocreate of the contained element.
        /// </summary>
        [Browsable(false)]
        public bool AutoCreate
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the cardinality of the contained element.
        /// </summary>
        [Browsable(false)]
        public Runtime.Cardinality Cardinality
        {
            get
            {
                return Cardinality.ZeroToMany;
            }
        }

        /// <summary>
        /// Gets the allow add new of the contained element.
        /// </summary>
        [Browsable(false)]
        public bool AllowAddNew
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the ordering of the instances of the contained element.
        /// </summary>
        [Browsable(false)]
        public Int32 OrderGroup
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Gets the comparer for custom ordering.
        /// </summary>
        public string OrderGroupComparerTypeName
        {
            get
            {
                return string.Empty;
            }
        }

        private string GetExtensionIdValue()
        {
            if (this.PatternManager == null && this.UriService == null)
            {
                // This can be the case when the GetService call is done from within the T4 Runner appdomain

                return this.extensionId;
            }

            var toolkitInfo = this.PatternManager.InstalledToolkits.FirstOrDefault(f => f.Schema.Pattern != null && f.Schema.Pattern.Id == this.Id);
            if (toolkitInfo != null)
            {
                return toolkitInfo.Id;
            }
            else
            {
                //Design time - Factory not installed
                if (!string.IsNullOrEmpty(this.PatternLink))
                {
                    var product = this.UriService.ResolveUri<IInstanceBase>(new Uri(this.PatternLink)) as IProduct;

                    return product.ToolkitInfo.Identifier;
                }

                return this.extensionId;
            }
        }

        private void SetExtensionIdValue(string value)
        {
            this.extensionId = value;

            if (this.UriService != null && !string.IsNullOrEmpty(this.PatternLink))
            {
                var product = this.UriService.ResolveUri<IInstanceBase>(new Uri(this.PatternLink)) as IProduct;

                if (product != null)
                {
                    product.ToolkitInfo.Identifier = value;
                }
            }
        }
    }
}
