using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Modeling;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// DomainModel Provider extension MEF registration.
    /// </summary>
    [Export(typeof(DomainModelExtensionProvider))]
    [ProvidesExtensionToDomainModel("15a342fd-f046-4b7a-9dc8-73b0a8eec119")]
    internal class DomainModelProvider : DomainModelExtensionProvider
    {
        /// <summary>
        /// Returns the type of the extension domain model.
        /// </summary>
        public override Type DomainModelType
        {
            get
            {
                return typeof(LibraryDomainModel);
            }
        }
    }
}