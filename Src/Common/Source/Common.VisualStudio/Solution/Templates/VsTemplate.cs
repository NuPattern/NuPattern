#pragma warning disable 1591
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace NuPattern.VisualStudio.Solution.Templates
{
    partial class VSTemplate : NuPattern.VisualStudio.Solution.Templates.IVsTemplate
    {
        private const string ArchiveFileExtension = @".zip";

        [XmlIgnore]
        public string PhysicalPath { get; set; }

        [XmlIgnore]
        public bool IsZip { get { return PhysicalPath.EndsWith(ArchiveFileExtension, StringComparison.InvariantCultureIgnoreCase); } }

        [XmlIgnore]
        public string TemplateFileName { get; set; }

        NuPattern.VisualStudio.Solution.Templates.VsTemplateType NuPattern.VisualStudio.Solution.Templates.IVsTemplate.Type
        {
            get
            {
                NuPattern.VisualStudio.Solution.Templates.VsTemplateType type;
                if (Enum.TryParse<NuPattern.VisualStudio.Solution.Templates.VsTemplateType>(this.Type, out type))
                    return type;

                return NuPattern.VisualStudio.Solution.Templates.VsTemplateType.Custom;
            }
        }

        string NuPattern.VisualStudio.Solution.Templates.IVsTemplate.TypeString
        {
            get { return this.Type; }
        }

        NuPattern.VisualStudio.Solution.Templates.IVsTemplateContent NuPattern.VisualStudio.Solution.Templates.IVsTemplate.TemplateContent
        {
            get { return this.TemplateContent; }
        }

        NuPattern.VisualStudio.Solution.Templates.IVsTemplateData NuPattern.VisualStudio.Solution.Templates.IVsTemplate.TemplateData
        {
            get { return this.TemplateData; }
        }

        IEnumerable<NuPattern.VisualStudio.Solution.Templates.IVsTemplateWizardData> NuPattern.VisualStudio.Solution.Templates.IVsTemplate.WizardData
        {
            get { return this.WizardData ?? Enumerable.Empty<NuPattern.VisualStudio.Solution.Templates.IVsTemplateWizardData>(); }
        }

        IEnumerable<NuPattern.VisualStudio.Solution.Templates.IVsTemplateWizardExtension> NuPattern.VisualStudio.Solution.Templates.IVsTemplate.WizardExtension
        {
            get { return this.WizardExtension ?? Enumerable.Empty<NuPattern.VisualStudio.Solution.Templates.IVsTemplateWizardExtension>(); }
        }

        Version NuPattern.VisualStudio.Solution.Templates.IVsTemplate.Version
        {
            get { return System.Version.Parse(this.Version); }
        }
    }

    partial class VSTemplateTemplateContent : NuPattern.VisualStudio.Solution.Templates.IVsTemplateContent
    {
        IEnumerable<NuPattern.VisualStudio.Solution.Templates.IVsTemplateCustomParameter> NuPattern.VisualStudio.Solution.Templates.IVsTemplateContent.CustomParameters
        {
            get { return this.CustomParameters ?? Enumerable.Empty<NuPattern.VisualStudio.Solution.Templates.IVsTemplateCustomParameter>(); }
        }

        IEnumerable<object> NuPattern.VisualStudio.Solution.Templates.IVsTemplateContent.Items
        {
            get { return this.Items ?? Enumerable.Empty<object>(); }
        }
    }

    partial class VSTemplateTemplateContentCustomParameter : NuPattern.VisualStudio.Solution.Templates.IVsTemplateCustomParameter
    {
    }

    partial class NameDescriptionIcon : NuPattern.VisualStudio.Solution.Templates.IVsTemplateResourceOrValue
    {

        public bool HasValue
        {
            get { return !String.IsNullOrEmpty(this.Value); }
        }
    }

    partial class VSTemplateTemplateData : NuPattern.VisualStudio.Solution.Templates.IVsTemplateData
    {
        NuPattern.VisualStudio.Solution.Templates.IVsTemplateResourceOrValue NuPattern.VisualStudio.Solution.Templates.IVsTemplateData.Description
        {
            get { return this.Description; }
        }

        NuPattern.VisualStudio.Solution.Templates.IVsTemplateResourceOrValue NuPattern.VisualStudio.Solution.Templates.IVsTemplateData.Icon
        {
            get { return this.Icon; }
        }

        NuPattern.VisualStudio.Solution.Templates.IVsTemplateResourceOrValue NuPattern.VisualStudio.Solution.Templates.IVsTemplateData.Name
        {
            get { return this.Name; }
        }

        bool? NuPattern.VisualStudio.Solution.Templates.IVsTemplateData.CreateNewFolder
        {
            get { return this.CreateNewFolderSpecified ? (bool?)this.CreateNewFolder : null; }
        }

        bool? NuPattern.VisualStudio.Solution.Templates.IVsTemplateData.EnableEditOfLocationField
        {
            get { return this.EnableEditOfLocationFieldSpecified ? (bool?)this.EnableEditOfLocationField : null; }
        }

        bool? NuPattern.VisualStudio.Solution.Templates.IVsTemplateData.EnableLocationBrowseButton
        {
            get { return this.EnableLocationBrowseButtonSpecified ? (bool?)this.EnableLocationBrowseButton : null; }
        }

        bool? NuPattern.VisualStudio.Solution.Templates.IVsTemplateData.Hidden
        {
            get { return this.HiddenSpecified ? (bool?)this.Hidden : null; }
        }

        VSTemplateTemplateDataLocationField? NuPattern.VisualStudio.Solution.Templates.IVsTemplateData.LocationField
        {
            get { return this.LocationFieldSpecified ? (VSTemplateTemplateDataLocationField?)this.LocationField : null; }
        }

        bool? NuPattern.VisualStudio.Solution.Templates.IVsTemplateData.PromptForSaveOnCreation
        {
            get { return this.PromptForSaveOnCreationSpecified ? (bool?)this.PromptForSaveOnCreation : null; }
        }

        bool? NuPattern.VisualStudio.Solution.Templates.IVsTemplateData.ProvideDefaultName
        {
            get { return this.ProvideDefaultNameSpecified ? (bool?)this.ProvideDefaultName : null; }
        }

        VSTemplateTemplateDataRequiredFrameworkVersion? NuPattern.VisualStudio.Solution.Templates.IVsTemplateData.RequiredFrameworkVersion
        {
            get { return this.RequiredFrameworkVersionSpecified ? (VSTemplateTemplateDataRequiredFrameworkVersion?)this.RequiredFrameworkVersion : null; }
        }

        bool? NuPattern.VisualStudio.Solution.Templates.IVsTemplateData.SupportsCodeSeparation
        {
            get { return this.SupportsCodeSeparationSpecified ? (bool?)this.SupportsCodeSeparation : null; }
        }

        bool? NuPattern.VisualStudio.Solution.Templates.IVsTemplateData.SupportsLanguageDropDown
        {
            get { return this.SupportsLanguageDropDownSpecified ? (bool?)this.SupportsLanguageDropDown : null; }
        }

        bool? NuPattern.VisualStudio.Solution.Templates.IVsTemplateData.SupportsMasterPage
        {
            get { return this.SupportsMasterPageSpecified ? (bool?)this.SupportsMasterPage : null; }
        }

        [XmlElement(Namespace = "http://schemas.microsoft.com/developer/vssdktemplate/2007")]
        public string OutputSubPath { get; set; }
    }

    partial class VSTemplateWizardData : NuPattern.VisualStudio.Solution.Templates.IVsTemplateWizardData
    {
        IEnumerable<XmlElement> NuPattern.VisualStudio.Solution.Templates.IVsTemplateWizardData.Elements
        {
            get { return this.Any ?? Enumerable.Empty<XmlElement>(); }
        }
    }

    partial class VSTemplateWizardExtension : NuPattern.VisualStudio.Solution.Templates.IVsTemplateWizardExtension
    {
        string NuPattern.VisualStudio.Solution.Templates.IVsTemplateWizardExtension.Assembly
        {
            get { return this.Assembly.OfType<XmlNode[]>().SelectMany(nodes => nodes).Select(node => node.Value).FirstOrDefault(); }
        }

        string NuPattern.VisualStudio.Solution.Templates.IVsTemplateWizardExtension.FullClassName
        {
            get { return this.FullClassName.OfType<XmlNode[]>().SelectMany(nodes => nodes).Select(node => node.Value).FirstOrDefault(); }
        }
    }
}

#pragma warning restore 1591