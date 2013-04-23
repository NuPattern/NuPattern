using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Extensions for authoring shapes.
    /// </summary>
    internal static class ShapeExtensions
    {
        private const int CompositeImageSpacing = 0;

        /// <summary>
        /// Returns the list of elements valid at this time.
        /// </summary>
        public static IList FilterElementsFromCompartment<T, TSortKey>(LinkedElementCollection<T> result, Func<T, bool> includeExpression, Func<T, TSortKey> ordering) where T : NamedElementSchema
        {
            List<T> items = new List<T>(result);

            // Get parent element
            if (items.Count > 0)
            {
                var store = items[0].Store;

                // Filter for only the included files of the expression
                if (includeExpression != null)
                {
                    items = items.Where(includeExpression).ToList<T>();
                }

                // Filter system items (based on diagram state)
                var diagram = ModelStoreExtensions.GetCurrentDiagram(store);
                if (diagram != null)
                {
                    if (diagram.ShowHiddenEntries == false)
                    {
                        items = items
                            .Where(item => item.IsSystem == false)
                            .ToList<T>();
                    }
                }

                // Sort and return
                return items
                    .OrderBy(ordering)
                    .ToList<T>();
            }

            return items;
        }

        /// <summary>
        /// Annotest elements in a compartment with the inherited icon.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Instance created should be disposed by caller.")]
        public static CompartmentMapping[] DecorateInheritedCompartmentNamedElements(this CompartmentShape shape, CompartmentMapping[] mappings)
        {
            Guard.NotNull(() => shape, shape);
            Guard.NotNull(() => mappings, mappings);

            // Iterate through each element in the compartment and update its image.
            foreach (var elementMap in mappings.OfType<ElementListCompartmentMapping>())
            {
                if (elementMap != null)
                {
                    // Set the image
                    elementMap.ImageGetter = mel =>
                    {
                        // Construct a composite image with both Inherited and Customization icons
                        var customizableElement = mel as CustomizableElementSchema;
                        var namedElement = mel as NamedElementSchema;
                        if (namedElement != null)
                        {
                            // Example images for sizing calculation only
                            using (Bitmap inheritedImage = Properties.Resources.Inherited)
                            {
                                using (Bitmap tempCustomizationImage = Properties.Resources.CustomizationInheritedDisabled)
                                {
                                    // Calculate composite image
                                    Bitmap compositeGlyph = new Bitmap((inheritedImage.Width + tempCustomizationImage.Width + CompositeImageSpacing), System.Math.Max(inheritedImage.Height, tempCustomizationImage.Height));
                                    using (Graphics graphics = Graphics.FromImage(compositeGlyph))
                                    {
                                        // Add Inherited icon
                                        if (namedElement.IsInheritedFromBase)
                                        {
                                            // Add customizable icon
                                            if (customizableElement != null)
                                            {
                                                Bitmap customizationImage = GetCustomizationStateImage(customizableElement.IsCustomizationEnabledState);

                                                // Draw combined image
                                                graphics.DrawImage(inheritedImage, 0, 0);
                                                graphics.DrawImage(customizationImage, (inheritedImage.Width + CompositeImageSpacing), 0);
                                            }
                                            else
                                            {
                                                // Just draw inherited image
                                                graphics.DrawImage(inheritedImage, 0, 0);
                                            }
                                        }
                                        else
                                        {
                                            if (customizableElement != null)
                                            {
                                                Bitmap customizationImage = GetCustomizationStateImage(customizableElement.IsCustomizationEnabledState);

                                                // Just draw the customiable icon
                                                graphics.DrawImage(customizationImage, (inheritedImage.Width + CompositeImageSpacing), 0);
                                            }
                                        }

                                        return compositeGlyph;
                                    }
                                }
                            }
                        }

                        return null;
                    };

                    // Set the displayed text
                    elementMap.StringGetter = mel =>
                    {
                        // Custom format for Properties
                        var property = mel as PropertySchema;
                        if (property != null)
                        {
                            return property.GetDisplayText();
                        }

                        // Custom format for AutomationSettings
                        var automation = mel as AutomationSettingsSchema;
                        if (automation != null)
                        {
                            return automation.GetDisplayText();
                        }

                        // Returns default string for other items
                        return SerializationUtilities.GetElementName(mel);
                    };
                }
            }

            return mappings;
        }

        /// <summary>
        /// Returns the image for the corresponding customization state.
        /// </summary>
        private static Bitmap GetCustomizationStateImage(CustomizationEnabledState state)
        {
            switch (state)
            {
                case CustomizationEnabledState.FalseEnabled:
                    return Properties.Resources.CustomizationFalseEnabled;
                case CustomizationEnabledState.InheritedDisabled:
                    return Properties.Resources.CustomizationInheritedDisabled;
                case CustomizationEnabledState.InheritedEnabled:
                    return Properties.Resources.CustomizationInheritedEnabled;
                case CustomizationEnabledState.TrueDisabled:
                    return Properties.Resources.CustomizationTrueDisabled;
                case CustomizationEnabledState.TrueEnabled:
                    return Properties.Resources.CustomizationTrueEnabled;
                default:
                    return Properties.Resources.CustomizationFalseDisabled;
            }
        }
    }
}
