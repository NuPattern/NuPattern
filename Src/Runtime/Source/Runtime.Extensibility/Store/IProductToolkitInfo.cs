using System;
using NuPattern.ComponentModel.Design;
using NuPattern.Runtime.Properties;

namespace NuPattern.Runtime
{
    /// <summary>
    /// General toolkit information for the pattern
    /// </summary>
    public interface IProductToolkitInfo
    {
        ///	<summary>
        ///	The name of this pattern toolkit.
        ///	</summary>
        [DisplayNameResource(@"IProductToolkitInfo_Name_DisplayName", typeof(Resources))]
        [DescriptionResource(@"IProductToolkitInfo_Name_Description", typeof(Resources))]
        [CategoryResource(@"DefaultCategory", typeof(Resources))]
        String Name { get; set; }

        ///	<summary>
        ///	The original author of this toolkit.
        ///	</summary>
        [DisplayNameResource(@"IProductToolkitInfo_Author_DisplayName", typeof(Resources))]
        [DescriptionResource(@"IProductToolkitInfo_Author_Description", typeof(Resources))]
        [CategoryResource(@"Category_Identification", typeof(Resources))]
        String Author { get; set; }

        ///	<summary>
        ///	The name of this pattern toolkit.
        ///	</summary>
        [DisplayNameResource(@"IProductToolkitInfo_Description_DisplayName", typeof(Resources))]
        [DescriptionResource(@"IProductToolkitInfo_Description_Description", typeof(Resources))]
        [CategoryResource(@"DefaultCategory", typeof(Resources))]
        String Description { get; set; }

        ///	<summary>
        ///	The current version of this toolkit.
        ///	</summary>
        [DisplayNameResource(@"IProductToolkitInfo_Version_DisplayName", typeof(Resources))]
        [DescriptionResource(@"IProductToolkitInfo_Version_Description", typeof(Resources))]
        [CategoryResource(@"Category_Identification", typeof(Resources))]
        String Version { get; set; }

        ///	<summary>
        ///	The unique identifier of this toolkit, also used as the VSIX identifier.
        ///	</summary>
        [DisplayNameResource(@"IProductToolkitInfo_Identifier_DisplayName", typeof(Resources))]
        [DescriptionResource(@"IProductToolkitInfo_Identifier_Description", typeof(Resources))]
        [CategoryResource(@"Category_Identification", typeof(Resources))]
        String Identifier { get; set; }
    }
}