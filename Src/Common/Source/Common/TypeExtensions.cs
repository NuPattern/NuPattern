using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NuPattern
{
    /// <summary>
    /// Extensions for <see cref="System.Type"/>.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Determines whether this type is CLS compliant.
        /// </summary>
        public static bool IsClsCompliant(this Type type)
        {
            Guard.NotNull(() => type, type);

            var assemblyIsCompliant = false;

            //Check for overriden attribute
            var assemblyAttribute = type.Assembly.GetCustomAttributes(typeof(CLSCompliantAttribute), true)
                .OfType<CLSCompliantAttribute>()
                .FirstOrDefault();
            if (assemblyAttribute != null)
            {
                assemblyIsCompliant = assemblyAttribute.IsCompliant;
            }

            if (assemblyIsCompliant)
            {
                var typeIsCompliant = true;

                // Check for overriden attribute
                var typeAttribute = type.GetCustomAttributes(typeof(CLSCompliantAttribute), true)
                    .OfType<CLSCompliantAttribute>()
                    .FirstOrDefault();
                if (typeAttribute != null)
                {
                    typeIsCompliant = typeAttribute.IsCompliant;
                }

                return typeIsCompliant;
            }

            return false;
        }

        /// <summary>
        /// Determines whether a type can be assigned to another one, using 
        /// a reflection-only compatible way.
        /// </summary>
        /// <devdoc>
        /// Need to do assignable checks different because the types are not 
        /// loaded the same way and therefore the traditional reflection 
        /// IsAssignableFrom will never work.
        /// </devdoc>
        public static bool IsAssignableTo(this Type type, Type assignableTo)
        {
            try
            {
                if (type.FullName == assignableTo.FullName)
                    return true;

                var assignableToTypeName = assignableTo.FullName;

                return GetBaseTypes(type)
                    .Concat(type.GetInterfaces())
                    // We exclude nulls as they can be present in the 
                    // reflection-only load for some reason.
                    .Where(x => x != null)
                    .Any(x => x.FullName == assignableToTypeName);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static IEnumerable<Type> GetBaseTypes(Type type)
        {
            var baseType = type.BaseType;

            while (baseType != null)
            {
                yield return baseType;
                baseType = baseType.BaseType;
            }
        }

        /// <summary>
        /// Gets the default value for the type as a string. Nullable and reference types 
        /// are returned as empty strings. Value types are instantiated and converted 
        /// to string using the type converter associated with the type.
        /// </summary>
        public static string GetDefaultValueString(this Type type)
        {
            Guard.NotNull(() => type, type);

            if (type == typeof(string) || type.IsNullable())
            {
                return string.Empty;
            }
            else if (type.IsNumber())
            {
                return "0";
            }
            else if (!type.IsValueType)
            {
                return string.Empty;
            }
            else
            {
                // Create a default value for it (as a string)
                return TypeDescriptor.GetConverter(type).ConvertToString(Activator.CreateInstance(type));
            }
        }

        /// <summary>
        /// Determines whether the specified type is number.
        /// </summary>
        public static bool IsNumber(this Type type)
        {
            return typeof(int).IsAssignableFrom(type) ||
                            typeof(long).IsAssignableFrom(type) ||
                            typeof(double).IsAssignableFrom(type);
        }

        /// <summary>
        /// Determines whether the specified type is nullable.
        /// </summary>
        public static bool IsNullable(this Type type)
        {
            Guard.NotNull(() => type, type);

            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
    }
}
