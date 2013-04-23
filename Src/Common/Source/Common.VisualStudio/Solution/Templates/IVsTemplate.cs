using System;
using System.Collections.Generic;

namespace NuPattern.VisualStudio.Solution.Templates
{
    /// <summary>
    /// A visual studio template
    /// </summary>
    public interface IVsTemplate
    {
        /// <summary>
        /// Full path to the file this template was read from.
        /// </summary>
        string PhysicalPath { get; }

        /// <summary>
        /// The file name of the template, without its full path. 
        /// In a zip template, this name may be different from 
        /// the full path file, as it represents the .vstemplate 
        /// inside the zip.
        /// </summary>
        string TemplateFileName { get; }

        /// <summary>
        /// Gets the type of the template
        /// </summary>
        VsTemplateType Type { get; }

        /// <summary>
        /// Gets the fully qualified type string of the template.
        /// </summary>
        string TypeString { get; }

        /// <summary>
        /// Whether the template was read from a zip file or a 
        /// regular .vstemplate file.
        /// </summary>
        bool IsZip { get; }

        /// <summary>
        /// Gets the  version of the template
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// Gets the content of the template
        /// </summary>
        IVsTemplateContent TemplateContent { get; }

        /// <summary>
        /// Gets the template data
        /// </summary>
        IVsTemplateData TemplateData { get; }

        /// <summary>
        /// Gets the wizard data 
        /// </summary>
        IEnumerable<IVsTemplateWizardData> WizardData { get; }

        /// <summary>
        /// Gets the wizard extensions
        /// </summary>
        IEnumerable<IVsTemplateWizardExtension> WizardExtension { get; }
    }
}
