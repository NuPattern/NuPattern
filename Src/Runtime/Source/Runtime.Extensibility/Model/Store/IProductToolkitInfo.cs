using System;
using System.ComponentModel;

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
        [Description("The name of this pattern toolkit.")]
        [DisplayName("Name")]
        [Category("General")]
        String Name { get; set; }

        ///	<summary>
        ///	The original author of this toolkit.
        ///	</summary>
        [Description("The original author of this toolkit.")]
        [DisplayName("Author")]
        [Category("Identification")]
        String Author { get; set; }

        ///	<summary>
        ///	The name of this pattern toolkit.
        ///	</summary>
        [Description("The description of this pattern toolkit.")]
        [DisplayName("Description")]
        [Category("General")]
        String Description { get; set; }

        ///	<summary>
        ///	The current version of this toolkit.
        ///	</summary>
        [Description("The current version of this toolkit.")]
        [DisplayName("Version")]
        [Category("Identification")]
        String Version { get; set; }

        ///	<summary>
        ///	The unique identifier of this toolkit, also used as the VSIX identifier.
        ///	</summary>
        [Description("The unique identifier of this toolkit, also used as the VSIX identifier.")]
        [DisplayName("Identifier")]
        [Category("Identification")]
        String Identifier { get; set; }
    }
}