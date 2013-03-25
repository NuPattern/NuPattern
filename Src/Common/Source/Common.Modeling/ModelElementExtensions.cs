using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.Modeling;
using NuPattern.Reflection;
using NuPattern.Modeling.Properties;

namespace NuPattern.Modeling
{
    /// <summary>
    /// Defines extension methods related to DSL <see cref="ModelElement"/>.
    /// </summary>
    public static class ModelElementExtensions
    {
        /// <summary>
        /// Returns the default value of a property of the given element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static TResult GetPropertyDefaultValue<T, TResult>(this ModelElement element, Expression<Func<T, TResult>> property)
        {
            Guard.NotNull(() => element, element);

            var propertyName = Reflector<T>.GetProperty(property).Name;
            var domainProperty = element.GetDomainClass().FindDomainProperty(propertyName, true);
            if (domainProperty == null)
            {
                throw new InvalidOperationException("Property does not exist on element.");
            }

            return (TResult)domainProperty.DefaultValue;
        }

        /// <summary>
        /// Executes a delegate inside a the transaction.
        /// </summary>
        /// <typeparam name="T">The type of the model element.</typeparam>
        /// <param name="modelElement">The model element.</param>
        /// <param name="action">The action.</param>
        /// <returns>The model element after the execution of the transaction.</returns>
        public static T WithTransaction<T>(this T modelElement, Action<T> action) where T : ModelElement
        {
            if (modelElement.Store.TransactionManager.InTransaction)
            {
                action(modelElement);
            }
            else
            {
                modelElement.Store.TransactionManager.DoWithinTransaction(() => action(modelElement));
            }

            return modelElement;
        }

        /// <summary>
        /// Creates a new element. The generic {T} parameter can be the concrete model element type or an interface.
        /// If {T} is an interface the first domain class implementing the interface will be used to create the element.
        /// </summary>
        /// <typeparam name="T">The target model element type.</typeparam>
        /// <returns>A model element of type {T}.</returns>
        public static T Create<T>(this ModelElement parent, bool raiseInstantiateEvents = true) where T : ModelElement
        {
            Guard.NotNull(() => parent, parent);

            if (!parent.Store.TransactionManager.InTransaction)
            {
                using (var tx = parent.Store.TransactionManager.BeginTransaction("Creating: " + typeof(T).Name))
                {
                    var result = CreateChildElement<T>(parent, raiseInstantiateEvents);
                    tx.Commit();

                    return result;
                }
            }

            return CreateChildElement<T>(parent, raiseInstantiateEvents);
        }

        /// <summary>
        /// Gets the name of the unique.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="baseName">Name of the base.</param>
        public static string GetUniqueName(this ModelElement element, string baseName)
        {
            Guard.NotNull(() => element, element);

            Guard.NotNullOrEmpty(() => baseName, baseName);

            var domainClass = element.GetDomainClass();

            var nameDomainProperty = domainClass.NameDomainProperty;

            if (nameDomainProperty == null)
            {
                throw new InvalidOperationException(
                    string.Format(CultureInfo.CurrentCulture, Resources.ModelElementExtensions_ElementNameNotSupported, element.GetDomainClass().Name));
            }

            var allDomainRolesPlayed = domainClass.AllDomainRolesPlayed;
            int count = allDomainRolesPlayed.Count;

            for (int i = 0; i < count; i++)
            {
                var embeddedDomainRole = allDomainRolesPlayed[i];
                var oppositeDomainRole = embeddedDomainRole.OppositeDomainRole;
                if (oppositeDomainRole.IsEmbedding)
                {
                    var elementLinks = embeddedDomainRole.GetElementLinks(element);
                    if ((elementLinks != null) && (elementLinks.Count > 0))
                    {
                        var rolePlayer = oppositeDomainRole.GetRolePlayer(elementLinks[0]);
                        return GetUniqueName(element, rolePlayer, embeddedDomainRole, baseName);
                    }
                }
            }

            return baseName;
        }

        private static T CreateChildElement<T>(ModelElement parent, bool raiseInstantiateEvents) where T : ModelElement
        {
            var childClass = parent.Partition.DomainDataDirectory.DomainClasses
                .FirstOrDefault(dci => typeof(T).IsAssignableFrom(dci.ImplementationClass) && !dci.ImplementationClass.IsAbstract);
            var elementOperations = new ElementOperations(parent.Store, parent.Partition);

            if (elementOperations != null)
            {
                var elementGroupPrototype = new ElementGroupPrototype(parent.Partition, childClass.Id);

                var partition = elementOperations.ChooseMergeTarget(parent, elementGroupPrototype).Partition;
                var element = (T)partition.ElementFactory.CreateElement(childClass);
                var elementGroup = new ElementGroup(partition);
                elementGroup.Add(element);
                elementGroup.MarkAsRoot(element);
                elementOperations.MergeElementGroup(parent, elementGroup);

                // Flag the element in the state so that the ProductStore class sees it and doesn't 
                // raise the instantiation event.
                if (!raiseInstantiateEvents)
                    element.Store.PropertyBag.Add(element, null);

                return element;
            }

            return default(T);
        }

        private static string GetUniqueName(ModelElement element, ModelElement container, DomainRoleInfo embeddedDomainRole, string baseName)
        {
            Guard.NotNull(() => element, element);
            Guard.NotNull(() => container, container);
            Guard.NotNull(() => embeddedDomainRole, embeddedDomainRole);
            Guard.NotNull(() => baseName, baseName);

            var nameDomainProperty = element.GetDomainClass().NameDomainProperty;
            var dictionary = new Dictionary<string, ModelElement>();
            var linkedElements = embeddedDomainRole.OppositeDomainRole.GetLinkedElements(container);
            var implementationClass = nameDomainProperty.DomainClass.ImplementationClass;
            int count = linkedElements.Count;

            for (int i = 0; i < count; i++)
            {
                var linkedElement = linkedElements[i];

                if ((linkedElement != element) && implementationClass.IsInstanceOfType(linkedElement))
                {
                    var key = nameDomainProperty.GetValue(linkedElement) as string;

                    if ((key != null) && !dictionary.ContainsKey(key))
                    {
                        dictionary.Add(key, linkedElement);
                    }
                }
            }

            return GetUniqueName(element, baseName, dictionary);
        }

        private static string GetUniqueName(ModelElement element, string baseName, IDictionary<string, ModelElement> siblingNames)
        {
            Guard.NotNull(() => element, element);
            Guard.NotNull(() => baseName, baseName);

            return UniqueNameGenerator.EnsureUnique(baseName,
                newName => !siblingNames.ContainsKey(newName), true);
        }
    }
}