using System;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell.Interop;

namespace NuPattern.VisualStudio.Solution.Hierarchy.Design
{
    /// <summary>
    /// Converter for <see cref="HierarchyNode"/> objects
    /// </summary>
    public class HierarchyNodeConverter : TypeConverter
    {
        /// <summary>
        /// Specifies conversion from <see cref="string"/> type
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            Guard.NotNull(() => context, context);
            if (sourceType == typeof(string) || sourceType == typeof(HierarchyNode))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Specifies conversion to <see cref="string"/> type
        /// </summary>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            Guard.NotNull(() => context, context);
            if (destinationType == typeof(string) || destinationType == typeof(HierarchyNode))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Converts from string objects
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            Guard.NotNull(() => context, context);
            if (value is string)
            {
                IVsSolution vsSolution = (IVsSolution)context.GetService(typeof(IVsSolution));
                return new HierarchyNode(vsSolution, value.ToString());
            }
            else if (value is HierarchyNode)
            {
                return ((HierarchyNode)value).UniqueName;
            }
            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Converts to string objects
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            Guard.NotNull(() => context, context);
            if (destinationType == typeof(string))
            {
                if (value is HierarchyNode)
                {
                    return ((HierarchyNode)value).UniqueName;
                }
            }
            else if (destinationType == typeof(HierarchyNode))
            {
                if (value is string)
                {
                    IVsSolution vsSolution = (IVsSolution)context.GetService(typeof(IVsSolution));
                    return new HierarchyNode(vsSolution, value.ToString());
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
