using System.Collections.Generic;
using System.Xml.Serialization;
using NuPattern.Runtime.Shortcuts;

namespace NuPattern.Runtime.Shell.Shortcuts
{
    /// <summary>
    /// A generic (all purpose) shortcut
    /// </summary>
    [XmlRoot(RootXmlElementName)]
    public class Shortcut : IShortcut
    {
        private const string RootXmlElementName = "shortcut";
        private const string TypeXmlElementName = "type";
        private const string DescriptionXmlElementName = "description";
        private const string ParametersXmlElementName = "parameters";

        /// <summary>
        /// Creates a new instance of the <see cref="Shortcut"/> class.
        /// </summary>
        public Shortcut()
        {
            this.Parameters = new ShortcutParameters();
        }

        /// <summary>
        /// Gets the type of the shortcut
        /// </summary>
        [XmlAttribute(TypeXmlElementName)]
        public string Type { get; set; }

        /// <summary>
        /// Gets an optional decription of the shortcut.
        /// </summary>
        [XmlElement(DescriptionXmlElementName, typeof(string))]
        public string Description { get; set; }

        [XmlIgnore]
        IDictionary<string, string> IShortcut.Parameters
        {
            get
            {
                return this.Parameters;
            }
        }

        /// <summary>
        /// Gets the parameters of the shortcut 
        /// </summary>
        [XmlElement(ParametersXmlElementName)]
        public ShortcutParameters Parameters { get; set; }
    }
}
