using System;
using System.Globalization;
using System.Linq;
using NuPattern.Runtime.Properties;

namespace NuPattern.Runtime.Extensibility
{
    /// <summary>
    /// Provides helper methods for for automation element containers.
    /// </summary>
    public static class ElementContainerExtensions
    {
        /// <summary>
        /// Returns the instance name for the element container.
        /// </summary>
        /// <returns></returns>
        public static string GetInstanceName(this IElementContainer container)
        {
            var view = container as IView;
            if (view != null)
            {
                // If only single view, then no view distinction required.
                if (view.Product.Info.Views.Count() == 1)
                {
                    return view.Product.InstanceName;
                }
                else
                {
                    // Distinguish the view.
                    return string.Format(CultureInfo.CurrentCulture, Resources.ElementContainerExtensions_ViewInstanceNameFormat,
                        view.Product.InstanceName, view.Info.DisplayName);
                }
            }

            var abstractElement = container as IAbstractElement;
            if (abstractElement != null)
            {
                return abstractElement.InstanceName;
            }

            throw new InvalidOperationException(Resources.ElementContainerExtensions_UnknownElementContainer);
        }
    }
}
