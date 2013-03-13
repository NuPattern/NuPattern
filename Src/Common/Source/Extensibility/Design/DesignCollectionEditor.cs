using System;

namespace NuPattern.Extensibility
{
    /// <summary>
    /// A cdesign collection editor
    /// </summary>
    public class DesignCollectionEditor : CancelableCollectionEditor
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelableCollectionEditor"/> class.
        /// </summary>
        /// <param name="type">The type of the collection for this editor to edit.</param>
        public DesignCollectionEditor(Type type)
            : base(type)
        {
        }
    }
}
