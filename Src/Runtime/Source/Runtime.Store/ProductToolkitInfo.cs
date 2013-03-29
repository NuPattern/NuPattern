using System;
using NuPattern.ComponentModel.Design;
using NuPattern.Runtime.Store.Properties;
using NuPattern.Runtime.ToolkitInterface;

namespace NuPattern.Runtime.Store
{
    /// <summary>
    /// General toolkit information for the pattern
    /// </summary>
    internal class ProductToolkitInfo : IProductToolkitInfo
    {
        private IAbstractElementProxy<IProductToolkitInfo> proxy;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductToolkitInfo"/> class.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        public ProductToolkitInfo(IAbstractElementProxy<IProductToolkitInfo> proxy)
        {
            this.proxy = proxy;
        }

        ///	<summary>
        ///	The name of this pattern toolkit.
        ///	</summary>
        [DescriptionResource("ProductToolkitInfo_NameDescription", typeof(Resources))]
        [CategoryResource("Category_General", typeof(Resources))]
        public String Name
        {
            get { return this.proxy.GetValue(() => this.Name); }
            set { this.proxy.SetValue(() => this.Name, value); }
        }

        ///	<summary>
        ///	The description of this pattern toolkit.
        ///	</summary>
        [DescriptionResource("ProductToolkitInfo_DescriptionDescription", typeof(Resources))]
        [CategoryResource("Category_General", typeof(Resources))]
        public String Description
        {
            get { return this.proxy.GetValue(() => this.Description); }
            set { this.proxy.SetValue(() => this.Description, value); }
        }

        ///	<summary>
        ///	The original author of this toolkit.
        ///	</summary>
        [DescriptionResource("ProductToolkitInfo_AuthorDescription", typeof(Resources))]
        [CategoryResource("Category_Identification", typeof(Resources))]
        public String Author
        {
            get { return this.proxy.GetValue(() => this.Author); }
            set { this.proxy.SetValue(() => this.Author, value); }
        }

        ///	<summary>
        ///	The current version of this toolkit.
        ///	</summary>
        [DescriptionResource("ProductToolkitInfo_VersionDescription", typeof(Resources))]
        [CategoryResource("Category_Identification", typeof(Resources))]
        public String Version
        {
            get { return this.proxy.GetValue(() => this.Version); }
            set { this.proxy.SetValue(() => this.Version, value); }
        }

        ///	<summary>
        ///	The unique identifier of this toolkit, also used as the VSIX identifier.
        ///	</summary>
        [DescriptionResource("ProductToolkitInfo_IdentifierDescription", typeof(Resources))]
        [CategoryResource("Category_Identification", typeof(Resources))]
        public String Identifier
        {
            get { return this.proxy.GetValue(() => this.Identifier); }
            set { this.proxy.SetValue(() => this.Identifier, value); }
        }
    }
}