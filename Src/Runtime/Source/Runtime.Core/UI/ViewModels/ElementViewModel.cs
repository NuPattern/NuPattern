using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace NuPattern.Runtime.UI
{
    /// <summary>
    /// Defines a view model for a <see cref="IAbstractElement"/>.
    /// </summary>
    internal class ElementViewModel : ProductElementViewModel
    {
        internal const string IconPathFormat = "../../Resources/" + "Node{0}.png"; // element images stored as resource with prefix

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementViewModel"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public ElementViewModel(IAbstractElement element, SolutionBuilderContext context)
            : base(element, context)
        {
            // Initialize default value for icon path
            this.IconPath = string.Format(CultureInfo.InvariantCulture, IconPathFormat, element.GetType().Name);

            if (element.Info != null)
            {
                this.CreateAddMenuOptions();
            }
        }

        /// <summary>
        /// Gets the underlying model.
        /// </summary>
        public new IAbstractElement Model
        {
            get { return (IAbstractElement)base.Model; }
        }

        /// <summary>
        /// Gets the element container. It is the place where the children are added.
        /// </summary>
        internal override IElementContainer ElementContainer
        {
            get { return this.Model; }
        }

        /// <summary>
        /// Checks if the element instance can be deleted
        /// </summary>
        /// <returns>True when the element can be deleted, false otherwise.</returns>
        /// <remarks>You can delete any element instance as long as you have a way to mnaully recreate it (i.e. AllowAddNew = true).</remarks>
        protected override bool CanDeleteInstance()
        {
            var info = this.Model.Info;

            switch (info.Cardinality)
            {
                case Cardinality.OneToMany:
                    var parent = this.ElementContainer.Parent as IElementContainer;
                    if (parent != null)
                    {
                        if (parent.Elements.Count(e => e.Info == info) > 1)
                        {
                            return true;
                        }
                        else
                        {
                            return info.AllowAddNew;
                        }
                    }
                    else
                    {
                        return info.AllowAddNew;
                    }

                case Cardinality.OneToOne:
                    return info.AllowAddNew;

                default:
                    //case Cardinality.ZeroToMany:
                    //case Cardinality.ZeroToOne:
                    return true;
            }
        }
    }
}