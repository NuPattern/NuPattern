using System;
using Microsoft.VisualStudio.Patterning.Library.Automation;
using Microsoft.VisualStudio.Patterning.Library.Commands;
using Microsoft.VisualStudio.Patterning.Extensibility;

namespace Microsoft.VisualStudio.Patterning.Library.Design
{
    /// <summary>
    /// Command references type editor
    /// </summary>
    public class CommandReferencesEditor : CancelableCollectionEditor
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
        /// Creates a new form to display and edit the current collection, with the help
        /// panel on the properties grid turned on.
        /// </summary>
        /// <returns></returns>
        protected override CollectionForm CreateCollectionForm()
        {
            var form = base.CreateCollectionForm();

            form.Text = Properties.Resources.CommandReferencesEditor_Caption;

            return form;
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