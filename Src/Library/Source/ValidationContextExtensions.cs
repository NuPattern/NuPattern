using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using NuPattern.Runtime;
using NuPattern.Runtime.Extensibility;

namespace NuPattern.Library
{
    /// <summary>
    /// Provides usability extensions for DSL validation.
    /// </summary>
    internal static class ValidationContextExtensions
    {
        /// <summary>
        /// Gets the project-reachable type for the current solution explorer selection, matching the 
        /// given <paramref name="typeId"/> and only if the type found is assignable to 
        /// <typeparamref name="TInterface"/>. Only components that have the 
        /// <see cref="FeatureComponentAttribute"/> are considered, which are the 
        /// only ones that can be located by <paramref name="typeId"/>.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "By Design")]
        public static Type TryGetProjectComponentType<TInterface>(this ValidationContext context, string typeId)
        {
            Guard.NotNull(() => context, context);

            var allTypes = context.GetCache<List<Type>>();
            var assignableTo = typeof(TInterface);

            if (!allTypes.Any() && ServiceProvider.GlobalProvider != null)
            {
                var projectProvider = ServiceProvider.GlobalProvider.GetService<INuPatternProjectTypeProvider>();
                if (projectProvider != null)
                    allTypes.AddRange(projectProvider.GetTypes<object>());
            }

            var typedCache = context.GetCache<Dictionary<string, Type>>(typeof(TInterface).FullName);

            if (!typedCache.Any())
            {
                var compatibleTypes = from type in allTypes
                                      where type.IsAssignableTo(typeof(TInterface))
                                      let component = type.AsProjectFeatureComponent()
                                      where component != null
                                      select new { Id = component.Id ?? type.ToString(), Type = type };

                foreach (var type in compatibleTypes)
                {
                    typedCache[type.Id] = type.Type;
                }
            }

            var cachedType = default(Type);

            if (!typedCache.TryGetValue(typeId, out cachedType))
                return null;

            return cachedType;
        }
    }
}
