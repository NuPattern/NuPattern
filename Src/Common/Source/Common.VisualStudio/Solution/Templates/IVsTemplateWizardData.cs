using System.Collections.Generic;
using System.Xml;

namespace NuPattern.VisualStudio.Solution.Templates
{
    /// <summary>
    /// Template wizard data
    /// </summary>
    public interface IVsTemplateWizardData
    {
        /// <summary>
        /// Gets the elemnts.
        /// </summary>
        IEnumerable<XmlElement> Elements { get; }

        /// <summary>
        /// Gets the name
        /// </summary>
        string Name { get; }
    }
}