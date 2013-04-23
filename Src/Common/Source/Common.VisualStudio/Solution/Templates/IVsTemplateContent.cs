using System.Collections.Generic;

namespace NuPattern.VisualStudio.Solution.Templates
{
    /// <summary>
    /// The templatte content
    /// </summary>
    public interface IVsTemplateContent
    {
        /// <summary>
        /// Gets the custom parameters
        /// </summary>
        IEnumerable<IVsTemplateCustomParameter> CustomParameters { get; }

        /// <summary>
        /// Gets the content items.
        /// </summary>
        IEnumerable<object> Items { get; }

        //[System.Xml.Serialization.XmlElementAttribute("Project", typeof(VSTemplateTemplateContentProject))]
        //[System.Xml.Serialization.XmlElementAttribute("ProjectCollection", typeof(VSTemplateTemplateContentProjectCollection))]
        //[System.Xml.Serialization.XmlElementAttribute("ProjectItem", typeof(VSTemplateTemplateContentProjectItem))]
        //[System.Xml.Serialization.XmlElementAttribute("References", typeof(VSTemplateTemplateContentReferences))]
    }
}