using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NuPattern.ComponentModel.Design;

namespace NuPattern.Reflection
{
    /// <summary>
    /// Miscelaneous extensions to facilitate reflection.
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Retrieves the given property from the expression.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "target", Justification = "Target used to aid intellisense only.")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Intended API design.")]
        public static PropertyInfo GetPropertyInfo<T, TResult>(this T target, Expression<Func<T, TResult>> property)
        {
            return Reflector<T>.GetProperty(property);
        }

        /// <summary>
        /// Retrieves the given property name from the expression.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "target", Justification = "NotApplicable")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "NotApplicable")]
        public static string GetPropertyName<T, TResult>(this T target, Expression<Func<T, TResult>> property)
        {
            return Reflector<T>.GetProperty(property).Name;
        }

        /// <summary>
        /// Provides direct access to the value of the <see cref="CategoryAttribute"/> if present.
        /// </summary>
        /// <param name="provider">Any type implementing the interface, which can be an assembly, type, 
        /// property, method, etc.</param>
        /// <returns>The attribute value if defined; <see langword="null"/> otherwise.</returns>
        public static string Category(this ICustomAttributeProvider provider)
        {
            var attribute = GetCustomAttribute<CategoryAttribute>(provider, true);
            if (attribute != null)
            {
                return attribute.Category;
            }

            attribute = GetCustomAttribute<CategoryResourceAttribute>(provider, true);
            if (attribute != null)
            {
                return attribute.Category;
            }

            return null;
        }

        /// <summary>
        /// Provides direct access to the value of the <see cref="DisplayNameAttribute"/> if present.
        /// </summary>
        /// <param name="provider">Any type implementing the interface, which can be an assembly, type, 
        /// property, method, etc.</param>
        /// <returns>The attribute value if defined; <see langword="null"/> otherwise.</returns>
        public static string DisplayName(this ICustomAttributeProvider provider)
        {
            var attribute = GetCustomAttribute<DisplayNameAttribute>(provider, true);
            if (attribute != null)
            {
                return attribute.DisplayName;
            }

            attribute = GetCustomAttribute<DisplayNameResourceAttribute>(provider, true);
            if (attribute != null)
            {
                return attribute.DisplayName;
            }

            return null;
        }

        /// <summary>
        /// Provides direct access to the value of the <see cref="DescriptionAttribute"/> if present.
        /// </summary>
        /// <param name="provider">Any type implementing the interface, which can be an assembly, type, 
        /// property, method, etc.</param>
        /// <returns>The attribute value if defined; <see langword="null"/> otherwise.</returns>
        public static string Description(this ICustomAttributeProvider provider)
        {
            var attribute = GetCustomAttribute<DescriptionAttribute>(provider, true);
            if (attribute != null)
            {
                return attribute.Description;
            }

            attribute = GetCustomAttribute<DescriptionResourceAttribute>(provider, true);
            if (attribute != null)
            {
                return attribute.Description;
            }

            return null;
        }

        /// <summary>
        /// Retrieves the first defined attribute of the given type from the provider if any.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute, which must inherit from <see cref="Attribute"/>.</typeparam>
        /// <param name="provider">Any type implementing the interface, which can be an assembly, type, 
        /// property, method, etc.</param>
        /// <param name="inherit">Optionally, whether the attribute will be looked in base classes.</param>
        /// <returns>The attribute instance if defined; <see langword="null"/> otherwise.</returns>
        public static TAttribute GetCustomAttribute<TAttribute>(this ICustomAttributeProvider provider, bool inherit /* = true */)
            where TAttribute : Attribute
        {
            return GetCustomAttributes<TAttribute>(provider, inherit).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the first defined attribute of the given type from the provider if any.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute, which must inherit from <see cref="Attribute"/>.</typeparam>
        /// <param name="provider">Any type implementing the interface, which can be an assembly, type, 
        /// property, method, etc.</param>
        /// <param name="inherit">Optionally, whether the attribute will be looked in base classes.</param>
        /// <returns>The attribute instance if defined; <see langword="null"/> otherwise.</returns>
        public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this ICustomAttributeProvider provider, bool inherit /* = true */)
            where TAttribute : Attribute
        {
            Guard.NotNull(() => provider, provider);

            return provider
                .GetCustomAttributes(typeof(TAttribute), inherit)
                .Cast<TAttribute>();
        }

        /// <summary>
        /// Tries to find the declaring method from an interface implemented by the type declaring the <paramref name="method"/>. 
        /// Otherwise, returns the <paramref name="method"/> itself.
        /// </summary>
        public static MethodInfo FindDeclaringMethod(this MethodInfo method)
        {
            Guard.NotNull(() => method, method);

            var declaringType = method.DeclaringType;
            var mappings = declaringType.GetInterfaces().Select(x => declaringType.GetInterfaceMap(x));

            foreach (var map in mappings)
            {
                for (int i = 0; i < map.TargetMethods.Length; i++)
                {
                    if (map.TargetMethods[i] == method)
                    {
                        return map.InterfaceMethods[i];
                    }
                }
            }

            return method;
        }

        /// <summary>
        /// Returns the public key token as a string
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetPublicKeyTokenString(this AssemblyName name)
        {
            Guard.NotNull(() => name, name);

            return name.GetPublicKeyToken().Select(x => x.ToString("x2")).Aggregate((x, y) => x + y);
        }
    }
}