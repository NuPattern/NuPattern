using System.Collections.Generic;
using System.Xml.Serialization;

namespace NuPattern.Runtime.Shell.Shortcuts
{
    /// <summary>
    /// A generic (all purpose) shortcut
    /// </summary>
    [XmlRoot(RootXmlElementName)]
    public class GenericShortcut : IGenericShortcut
    {
        private const string RootXmlElementName = "shortcut";
        private const string TypeXmlElementName = "type";
        private const string DescriptionXmlElementName = "description";
        private const string ParametersXmlElementName = "parameters";

        /// <summary>
        /// Creates a new instance of the <see cref="GenericShortcut"/> class.
        /// </summary>
        public GenericShortcut()
        {
            this.Parameters = new ShortcutParameters();
        }

        /// <summary>
        /// Executes the shortcut
        /// </summary>
        public void Execute()
        {
            // Delegate to the proper shortcut
        }

        /// <summary>
        /// Gets a value indicating whether the shortcut requires updating.
        /// </summary>
        [XmlIgnore]
        public bool Upated
        {
            get
            {
                // Delegate to the proper shortcut
                return false;
            }
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
        IDictionary<string, string> IGenericShortcut.Parameters
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
