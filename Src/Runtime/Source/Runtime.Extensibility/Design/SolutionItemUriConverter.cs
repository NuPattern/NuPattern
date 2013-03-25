using System;
using System.ComponentModel;
using System.Globalization;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using NuPattern.Runtime.Properties;

namespace NuPattern.Runtime.Design
{
    /// <summary>
    /// Converts an <see cref="string"/> URI to an <see cref="IItemContainer"/> in the solution.
    /// </summary>
    public class SolutionItemUriConverter : TypeConverter
    {
        private IFxrUriReferenceService uriReferenceService;

        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="Type"/> that represents the type you want to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string) || sourceType == typeof(Uri))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">The <see cref="CultureInfo"/> to use as the current culture.</param>
        /// <param name="value">The <see cref="object"/> to convert.</param>
        /// <returns>
        /// An <see cref="object"/> that represents the converted value.
        /// </returns>
        /// <exception cref="NotSupportedException">The conversion cannot be performed. </exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var uriString = value as string;
            if (uriString != null)
            {
                if (uriString.Length > 0)
                {
                    this.EnsureServices(context);
                    return this.uriReferenceService.TryResolveUri<IItemContainer>(new Uri(uriString));
                }

                return null;
            }

            var uri = value as Uri;
            if (uri != null)
            {
                this.EnsureServices(context);
                return this.uriReferenceService.TryResolveUri<IItemContainer>(uri);
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="destinationType">A <see cref="Type"/> that represents the type you want to convert to.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) ||
                destinationType == typeof(Uri) ||
                base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">A <see cref="CultureInfo"/>. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="object"/> to convert.</param>
        /// <param name="destinationType">The <see cref="Type"/> to convert the <paramref name="value"/> parameter to.</param>
        /// <returns>
        /// An <see cref="object"/> that represents the converted value.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="destinationType"/> parameter is null.</exception>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var item = value as IItemContainer;

            if (item != null && destinationType == typeof(string))
            {
                this.EnsureServices(context);
                var uri = this.uriReferenceService.CreateUri<IItemContainer>(item);
                return uri != null ? uri.AbsoluteUri : null;
            }
            else if (item != null && destinationType == typeof(Uri))
            {
                this.EnsureServices(context);
                return this.uriReferenceService.CreateUri<IItemContainer>(item);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        private void EnsureServices(ITypeDescriptorContext context)
        {
            if (this.uriReferenceService == null)
            {
                this.uriReferenceService = context.GetService<IFxrUriReferenceService>();

                if (this.uriReferenceService == null)
                {
                    throw new NotSupportedException(string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.SolutionItemUriConverter_MissingService,
                        typeof(IFxrUriReferenceService)));
                }
            }
        }
    }
}