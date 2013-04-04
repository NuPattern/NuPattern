using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Design;
using System.Linq;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
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
        [ImportMany(typeof(ILaunchPoint))]
        private IEnumerable<Lazy<ILaunchPoint, IDictionary<string, object>>> LaunchPoints
        {
            get;
            set;
        }

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

            this.InitializeVsLaunchPoints();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
            }
        }

        private void InitializeVsLaunchPoints()
        {
            var menuCommandService = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (menuCommandService != null)
            {
                var featureLaunchPoints = this.LaunchPoints
                    .Select(lazy => lazy.Value)
                    .OfType<VsLaunchPoint>()
                    .Where(launchPoint => launchPoint.GetType().Assembly == this.GetType().Assembly);

                foreach (var launchPoint in featureLaunchPoints)
                {
                    menuCommandService.AddCommand(launchPoint);
                }
            }
        }
    }
}
