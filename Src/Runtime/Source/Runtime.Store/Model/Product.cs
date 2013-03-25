using System;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Runtime.Extensibility;
using NuPattern.Runtime.Store.Properties;
using NuPattern.Runtime.ToolkitInterface;

namespace NuPattern.Runtime.Store
{
    partial class Product
    {
        private static string ToolkitInfoElementId = "c00ddfa6-b949-4c7c-b8b9-f46b8e3ccf1e";
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<Product>();
        private IView currentView;

        /// <summary>
        /// Provides registration information for the pattern
        /// </summary>
        public IProductToolkitInfo ToolkitInfo
        {
            get
            {
                var element = this.GetChildren().OfType<IAbstractElement>()
                    .FirstOrDefault(e => e.DefinitionId == new Guid(ToolkitInfoElementId));

                if (element != null)
                {
                    var proxy = element.ProxyFor<IProductToolkitInfo>();

                    return new ProductToolkitInfo(proxy);
                }

                tracer.TraceWarning(Resources.Product_ToolkitInfoIsNull);
                return null;
            }
        }

        /// <summary>
        /// Gets or sets the current view.
        /// </summary>
        public IView CurrentView
        {
            get
            {
                if (this.currentView == null)
                {
                    this.currentView = this.Views.FirstOrDefault(v => v.Info.IsDefault);
                }

                return this.currentView;
            }
            set
            {
                if (value == null)
                {
                    this.currentView = this.Views.FirstOrDefault(v => v.Info.IsDefault);
                }
                else
                {
                    if (!this.Views.Contains(value))
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                            Resources.Product_ErrorUnknownView, value.Info.Name));
                    }
                    else
                    {
                        this.currentView = value;
                    }
                }
            }
        }
    }
}