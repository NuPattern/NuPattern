using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextTemplating.VSHost;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Double-derived class to allow easier code customization.
    /// </summary>
    [ProvideIncludeFolder(".tt", 100, @"Assets\Templates\Text")]
    [ProvideAutoLoad(UIContextGuids.SolutionExists)]
    internal sealed partial class PatternModelPackage
    {
        /// <summary>
        /// Initialization method called by the package base class when this package is loaded.
        /// </summary>
        protected override void Initialize()
        {
            ((CompositionContainer)ModelingCompositionContainer.CompositionService)
                .ComposeExportedValue<ModelingPackage>(this);

            base.Initialize();

            var componentModel = this.GetService(typeof(SComponentModel)) as IComponentModel;
            componentModel.DefaultCompositionService.SatisfyImportsOnce(this);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
            }
        }
    }
}
