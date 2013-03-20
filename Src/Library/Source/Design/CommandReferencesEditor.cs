using System;
using NuPattern.Extensibility;
using NuPattern.Library.Automation;

namespace NuPattern.Library.Design
{
    /// <summary>
    /// Command references type editor
    /// </summary>
    public class CommandReferencesEditor : DesignCollectionEditor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandReferencesEditor"/> class.
        /// </summary>
        /// <param name="type">The type of the collection for this editor to edit.</param>
        public CommandReferencesEditor(Type type)
            : base(type)
        {
        }

        /// <summary>
        /// Creates a new instance of the specified collection item type.
        /// </summary>
        /// <param name="itemType">The type of item to create.</param>
        /// <returns>A new instance of the specified object.</returns>
        protected override object CreateInstance(Type itemType)
        {
            var settings = ((CommandSettings)this.Context.Instance);

            return new CommandReference(settings);
        }
    }
}