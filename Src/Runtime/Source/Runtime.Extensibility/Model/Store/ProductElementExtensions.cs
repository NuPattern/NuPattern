using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NuPattern.Runtime.Comparers;
using NuPattern.Runtime.Properties;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Provides helper methods for for automation extension containers.
    /// </summary>
    public static class ProductElementExtensions
    {
        /// <summary>
        /// Returns a new unique name, that is unique amoung the given siblings, based upon the given the baseName.
        /// </summary>
        /// <param name="siblings">The elements to compare existing names.</param>
        /// <param name="baseName">The base name.</param>
        /// <param name="startWithNumeral">Whether to initially append a numeral to <paramref name="baseName"/>.</param>
        public static string GetNewUniqueName(this IEnumerable<IProductElement> siblings, string baseName, bool startWithNumeral = true)
        {
            if (!startWithNumeral
                && (!siblings.Any(e => e.InstanceName.Equals(baseName, StringComparison.OrdinalIgnoreCase))))
            {
                return baseName;
            }

            var index = 1;
            string instanceName = string.Empty;
            do
            {
                instanceName = baseName + index++;
            }
            while (siblings.Any(e => e.InstanceName.Equals(instanceName, StringComparison.OrdinalIgnoreCase)));

            return instanceName;
        }

        /// <summary>
        /// Returns a new unique name, that is unique amoung the parents child elements, based upon the given the baseName.
        /// </summary>
        /// <param name="parentElement">The element where the unique name needs to be determined.</param>
        /// <param name="baseName">The base name to use to determine uniqueness.</param>
        /// <param name="startWithNumeral">Whether the unique name should start with a number (i.e. Class1).</param>
        public static string GetNewUniqueName(this IElementContainer parentElement, string baseName, bool startWithNumeral = true)
        {
            Guard.NotNull(() => parentElement, parentElement);

            var siblings = parentElement.Elements.Cast<IProductElement>();
            return siblings.GetNewUniqueName(baseName, startWithNumeral);
        }

        /// <summary>
        /// Gets the parent automation element of the given container.
        /// </summary>
        public static IProductElement GetParentAutomation(this IProductElement container)
        {
            Guard.NotNull(() => container, container);

            var abstractElement = container as IAbstractElement;
            if (abstractElement == null)
            {
                // Walk up products that are extension points.
                var extensionParent = container.Parent as IProductElement;
                var extensionViewParent = container.Parent as IView;
                if (extensionViewParent != null)
                    return extensionViewParent.Product;

                // May be null?
                return extensionParent;
            }

            if (abstractElement.Owner == null && abstractElement.View == null)
            {
                throw new InvalidOperationException(Resources.ProductElementExtensions_DisconnectedObject);
            }


            return abstractElement.Owner ?? (IProductElement)abstractElement.View.Product;
        }

        /// <summary>
        /// Adds a new reference of the specified kind to the collection of references, or updates an already existing reference of the given kind.
        /// </summary>
        public static IReference AddReference(this IProductElement container, string kind, string value, bool singleton = false)
        {
            Guard.NotNull(() => container, container);
            Guard.NotNullOrEmpty(() => kind, kind);

            if (singleton == true)
            {
                var existingRef = container.References.FirstOrDefault(r => r.Kind == kind);
                if (existingRef != null)
                {
                    // Update existing reference value.
                    existingRef.Value = value;

                    return existingRef;
                }
            }

            // Create a new reference
            return container.CreateReference(r =>
            {
                r.Kind = kind;
                r.Value = value;
            });
        }

        /// <summary>
        /// Returns the references of the given kind from the references collection.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="kind"></param>
        /// <returns></returns>
        public static IEnumerable<string> TryGetReferences(this IProductElement container, string kind)
        {
            Guard.NotNull(() => container, container);
            Guard.NotNullOrEmpty(() => kind, kind);

            return container.References.Where(r => r.Kind == kind).Select(r => r.Value);
        }

        /// <summary>
        /// Returns the first reference of the given kind from the references collection.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="kind"></param>
        /// <returns></returns>
        public static string TryGetReference(this IProductElement container, string kind)
        {
            Guard.NotNull(() => container, container);
            Guard.NotNullOrEmpty(() => kind, kind);

            return container.References.Where(r => r.Kind == kind).Select(r => r.Value).FirstOrDefault();
        }

        /// <summary>
        /// Returns all the children instances of the given element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static IEnumerable<IProductElement> GetChildren(this IProductElement element)
        {
            Guard.NotNull(() => element, element);

            if (element is IProduct)
            {
                var product = element as IProduct;
                return product.Views.SelectMany(v => v.Elements);
            }
            else
            {
                var container = element as IElementContainer;
                return container.Elements;
            }
        }

        /// <summary>
        /// Returns the schema information of all children of the given element.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Infos", Justification = "Doesn't look Hungarian at all and it's the right name here.")]
        public static IEnumerable<IPatternElementInfo> FindAllChildren(this IPatternElementInfo info)
        {
            Guard.NotNull(() => info, info);

            if (info is IPatternInfo)
            {
                var productInfo = info as IPatternInfo;
                return productInfo.Views
                    .SelectMany(viewInfo => viewInfo.Elements);
            }
            else
            {
                var containerInfo = info as IElementInfoContainer;
                return containerInfo.Elements;
            }
        }

        /// <summary>
        /// Returns the schema information of all descendant elements of the given element, for the given type.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Infos", Justification = "Doesn't look Hungarian at all and it's the right name here.")]
        public static IEnumerable<TInfo> FindAllDescendants<TInfo>(this IPatternElementInfo info) where TInfo : IPatternElementInfo
        {
            var infos = new List<TInfo>();

            Guard.NotNull(() => info, info);

            if (info is IPatternInfo)
            {
                var productInfo = info as IPatternInfo;
                foreach (var viewInfo in productInfo.Views)
                {
                    infos.AddRange(viewInfo.Elements.OfType<TInfo>());
                    foreach (var abstractElementInfo in viewInfo.Elements)
                    {
                        infos.AddRange(abstractElementInfo.FindAllDescendants<TInfo>());
                    }
                }
            }
            else
            {
                if (info is IElementInfoContainer)
                {
                    var containerInfo = info as IElementInfoContainer;
                    infos.AddRange(containerInfo.Elements.OfType<TInfo>());
                    foreach (var elementInfo in containerInfo.Elements)
                    {
                        infos.AddRange(elementInfo.FindAllDescendants<TInfo>());
                    }
                    infos.AddRange(containerInfo.ExtensionPoints.OfType<TInfo>());
                    foreach (var extensionPointInfo in containerInfo.ExtensionPoints)
                    {
                        infos.AddRange(extensionPointInfo.FindAllDescendants<TInfo>());
                    }
                }
            }

            return infos;
        }

        /// <summary>
        /// Ensures the element is a child container of given named element.
        /// </summary>
        public static IPatternElementInfo EnsureChildContainer(this IProductElement parentElement, string childElementName)
        {
            Guard.NotNull(() => parentElement, parentElement);
            Guard.NotNull(() => childElementName, childElementName);

            //Ensure container element can contain
            if ((parentElement as IElementContainer) == null && (parentElement as IProduct) == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    Resources.ProductElementExtensions_ErrorNotElementContainer, parentElement.InstanceName, childElementName));
            }

            // Ensure container element has the correct child to create
            var childElementInfo = parentElement.Info.FindAllChildren().Where(i => i.Name.Equals(childElementName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (childElementInfo == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    Resources.ProductElementExtensions_ErrorNotChildElement, parentElement.InstanceName, childElementName));
            }

            return childElementInfo;
        }

        /// <summary>
        /// Creates a new instance of the given child element with an optional instance name.
        /// </summary>
        public static IAbstractElement CreateChildElement(this IProductElement parentElement, IPatternElementInfo childElementInfo, string instanceName = "", bool raiseInstantiateEvents = true)
        {
            Guard.NotNull(() => parentElement, parentElement);
            Guard.NotNull(() => childElementInfo, childElementInfo);

            // Ensure correct container
            var productContainer = parentElement as IProduct;
            var containerElement = parentElement as IElementContainer;
            if (containerElement == null)
            {
                if (productContainer != null)
                {
                    // Redirect to the 'current' view
                    containerElement = productContainer.CurrentView;
                }
                else
                {
                    return null;
                }
            }

            if (string.IsNullOrEmpty(instanceName))
            {
                instanceName = childElementInfo.DisplayName;
            }

            var uniqueInstanceName = containerElement.GetNewUniqueName(instanceName, false);

            if (childElementInfo is ICollectionInfo)
            {
                return containerElement.CreateCollection(c =>
                {
                    c.DefinitionId = childElementInfo.Id;
                    c.InstanceName = uniqueInstanceName;
                }, raiseInstantiateEvents);
            }
            else if (childElementInfo is IElementInfo)
            {
                return containerElement.CreateElement(e =>
                {
                    e.DefinitionId = childElementInfo.Id;
                    e.InstanceName = uniqueInstanceName;
                }, raiseInstantiateEvents);
            }
            else
            {
                throw new NotImplementedException(Resources.ProductElementExtensions_CreateChildElement_InvalidType);
            }
        }

        /// <summary>
        /// Determines if the element's pattern is parented.
        /// </summary>
        public static bool IsProductParented(this IProductElement element)
        {
            // Check if this is the pattern
            var product = element as IProduct;
            if (product != null)
            {
                return (product.Parent != null);
            }

            // Traverse up to pattern
            var currentElement = element as IInstanceBase;
            while (currentElement.Parent as IProduct == null)
            {
                currentElement = currentElement.Parent;
            }

            product = currentElement.Parent as IProduct;
            return (product.Parent != null);
        }


        /// <summary>
        /// Orders the elements of the given collection of elements in their configured order.
        /// </summary>
        public static IEnumerable<T> Order<T>(this IEnumerable<T> elements) where T : IProductElement
        {
            var orderedElements = ResetOrder(elements);

            return orderedElements
                .OrderBy(e => e.InstanceOrder);
        }

        /// <summary>
        /// Resets the <see cref="IProductElement.InstanceOrder"/> of the elements in their configured order.
        /// </summary>
        public static IEnumerable<T> ResetOrder<T>(this IEnumerable<T> elements) where T : IProductElement
        {
            // Save elements that can't be ordered
            var nullInfoElements = elements
                .Where(e => e.Info == null)
                .OrderBy(e => e.InstanceName);

            // Group the elements by their configured 'Order', 
            // sort the groups by the 'Order', and then by 'DefinitionName'.
            var elementGroups = from e in elements
                                where e.Info != null
                                group e by ((IContainedElementInfo)e.Info) into infos
                                let order = infos.Key.OrderGroup
                                let name = infos.Key.Name
                                orderby order ascending, name ascending
                                select infos;

            // Group the infos by 'Order'
            var orderGroups = from eg in elementGroups
                              group eg by eg.Key.OrderGroup into orders
                              select orders;

            // Compare each group with the group comparer and set 'InstanceOrder'
            var orderedElements = new List<T>();
            foreach (var orderGroup in orderGroups)
            {
                IComparer<IProductElement> comparer = new ProductElementInstanceNameComparer(); //Default comparer

                //Get group comparer
                var groupComparer = GetGroupComparer(orderGroup);
                if (!String.IsNullOrEmpty(groupComparer))
                {
                    var customComparer = CreateComparer(groupComparer);
                    if (customComparer != null)
                    {
                        comparer = customComparer;
                    }
                }

                var groupElements = orderGroup
                    .SelectMany(e => e);

                // Reset the grouped element's instance order
                var order = orderGroup.Key;
                var index = 0;
                groupElements
                    .OrderBy(e => e, comparer)
                    .ToList()
                    .ForEach(e =>
                    {
                        var newOrder = (((double)order) + (0.1 * ++index));
                        if (e.InstanceOrder != newOrder)
                        {
                            e.InstanceOrder = newOrder;
                        }
                    });

                orderedElements.AddRange(groupElements);
            }

            return orderedElements
                .Concat(nullInfoElements);
        }

        private static string GetGroupComparer<T>(IGrouping<int, IGrouping<IContainedElementInfo, T>> orderGroup)
        {
            // Finds first non default comparer only
            var comparer = orderGroup
                .ToList()
                .FirstOrDefault(og => !og.Key.IsDefaultOrderComparer());
            if (comparer == null)
            {
                return null;
            }
            else
            {
                return comparer.Key.OrderGroupComparerTypeName;
            }
        }

        private static IComparer<IProductElement> CreateComparer(string comparerTypeName)
        {
            try
            {
                var type = Type.GetType(comparerTypeName, false, true);
                if (type == null)
                {
                    return null;
                }

                var ctor = type.GetConstructor(Type.EmptyTypes);
                if (ctor != null)
                {
                    return ctor.Invoke(null) as IComparer<IProductElement>;
                }
            }
            catch (TypeLoadException)
            {
                // Swallow exception
            }

            return null;
        }
    }
}
