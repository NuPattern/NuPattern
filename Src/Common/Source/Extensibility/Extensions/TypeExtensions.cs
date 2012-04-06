using System;
using System.Linq;

namespace Microsoft.VisualStudio.Patterning.Extensibility
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
    }
}
