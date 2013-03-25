using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Integration;
using Microsoft.VisualStudio.Modeling.Integration.Shell;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// PatternModel bus adapter for the runtime state.
    /// </summary>
    internal class Adapter : StandardModelingAdapter, IModelingAdapterWithStore, IModelingAdapterWithRootedModel
    {
        /// <summary>
        /// Identifier for the runtime state adapter.
        /// </summary>
        public const string AdapterId = "NuPattern.Runtime.Schema";

        /// <summary>
        /// Initializes a new instance of the <see cref="Adapter"/> class.
        /// </summary>
        public Adapter(ModelBusReference reference, VsModelingAdapterManager adapterManager, ModelElement rootModelElement)
            : base(reference, adapterManager, rootModelElement)
        {
        }

        /// <summary>
        /// The display name of the adapter, which equals the model file sans extension.
        /// </summary>
        public override string DisplayName
        {
            get { return Path.GetFileNameWithoutExtension(this.DocumentHandler.ModelFile); }
        }

        /// <summary>
        /// Gets the element references that satisfy the given element type.
        /// </summary>
        protected override IEnumerable<ModelBusReference> GetElementReferences(Type elementType)
        {
            if (elementType == null)
            {
                throw new ArgumentNullException("elementType");
            }

            if (ModelRoot != null)
            {
                return this.ModelRoot.Store.ElementDirectory
                    .FindElements(this.ModelRoot.Store.DomainDataDirectory.GetDomainClass(elementType), true)
                    .Select(mel => this.GetElementReference(mel));
            }

            return Enumerable.Empty<ModelBusReference>();
        }

        /// <summary>
        /// Throws <see cref="NotSupportedException"/> as the runtime state does not have views.
        /// </summary>
        public override ModelBusView GetView(ModelBusReference reference)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Accessor for the state
        /// </summary>
        public Microsoft.VisualStudio.Modeling.Store Store
        {
            get { return this.AdapterStore; }
        }

        /// <summary>
        /// Accessor for the single root model element
        /// </summary>
        public ModelElement ModelRoot
        {
            get { return AdapterModelRoot; }
        }
    }
}