using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using NuPattern.Runtime.Shortcuts;

namespace NuPattern.Runtime.Shell.Shortcuts
{
    /// <summary>
    /// A shortcut file handler.
    /// </summary>
    internal class ShortcutFileHandler : IShortcutPersistenceHandler
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ShortcutFileHandler"/> class.
        /// </summary>
        /// <param name="filePath"></param>
        public ShortcutFileHandler(string filePath)
        {
            Guard.NotNullOrEmpty(() => filePath, filePath);

            this.FilePath = filePath;
        }

        /// <summary>
        /// Gets and sets the path to the file.
        /// </summary>
        public string FilePath
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns the shortcut from the specified file.
        /// </summary>
        /// <exception cref="FileNotFoundException">The file not found.</exception>
        /// <exception cref="ShortcutFileAccessException">The format of the shortcut was invalid</exception>
        public IShortcut ReadShortcut()
        {
            if (!File.Exists(this.FilePath))
            {
                throw new FileNotFoundException();
            }

            try
            {
                // Load the file
                var xDoc = XDocument.Load(this.FilePath);

                var serializer = new XmlSerializer(typeof(Shortcut));
                var reader = xDoc.CreateReader();
                if (!serializer.CanDeserialize(reader))
                {
                    throw new ShortcutFileFormatException();
                }

                // Deserialize the shortcut
                var shortcut = (Shortcut)serializer.Deserialize(reader);

                return shortcut;
            }
            catch (Exception)
            {
                throw new ShortcutFileFormatException();
            }
        }

        /// <summary>
        /// Writes the shortcut to the specified file.
        /// </summary>
        /// <param name="shortcut">The shortcut to write to the file</param>
        public void WriteShortcut(IShortcut shortcut)
        {
            Guard.NotNull(() => shortcut, shortcut);

            if (!File.Exists(this.FilePath))
            {
                throw new FileNotFoundException();
            }

            try
            {
                var serializer = new XmlSerializer(typeof(Shortcut));
                using (var stream = new FileStream(this.FilePath, FileMode.OpenOrCreate))
                {
                    using (var writer = new XmlTextWriter(stream, Encoding.Unicode))
                    {
                        // Pretty print
                        writer.Formatting = Formatting.Indented;

                        // Serialize the shortcut
                        serializer.Serialize(writer, shortcut);
                    }
                }
            }
            catch (Exception)
            {
                throw new ShortcutFileAccessException();
            }
        }
    }
}
