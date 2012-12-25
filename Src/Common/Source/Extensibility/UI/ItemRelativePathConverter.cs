using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using NuPattern.Runtime;

namespace NuPattern.Extensibility
{
	/// <summary>
	/// IItem relative path type converter
	/// </summary>
	public class ItemRelativePathConverter : TypeConverter
	{
		private ISolution solution;

		/// <summary>
		/// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
		/// </summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
		/// <param name="sourceType">A <see cref="T:System.Type"/> that represents the type you want to convert from.</param>
		/// <returns>
		/// true if this converter can perform the conversion; otherwise, false.
		/// </returns>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
			{
				return true;
			}

			return base.CanConvertFrom(context, sourceType);
		}

		/// <summary>
		/// Converts the given object to the type of this converter, using the specified context and culture information.
		/// </summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
		/// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"/> to use as the current culture.</param>
		/// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
		/// <returns>
		/// An <see cref="T:System.Object"/> that represents the converted value.
		/// </returns>
		/// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string valueString = value as string;

			if (valueString != null)
			{
				if (valueString.Length > 0)
				{
					if (this.solution == null)
					{
						SetSolution(context);
					}

					if (this.solution != null)
					{
						return this.solution.Find<IItem>(item => item.GetLogicalPath()
							.EndsWith(valueString, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
					}
				}
			}

			return value.ToString();
		}

		/// <summary>
		/// Returns whether this converter can convert the object to the specified type, using the specified context.
		/// </summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
		/// <param name="destinationType">A <see cref="T:System.Type"/> that represents the type you want to convert to.</param>
		/// <returns>
		/// true if this converter can perform the conversion; otherwise, false.
		/// </returns>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				return true;
			}

			return base.CanConvertTo(context, destinationType);
		}

		/// <summary>
		/// Converts the given value object to the specified type, using the specified context and culture information.
		/// </summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
		/// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"/>. If null is passed, the current culture is assumed.</param>
		/// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
		/// <param name="destinationType">The <see cref="T:System.Type"/> to convert the <paramref name="value"/> parameter to.</param>
		/// <returns>
		/// An <see cref="T:System.Object"/> that represents the converted value.
		/// </returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="destinationType"/> parameter is null. </exception>
		/// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				var item = value as IItem;

				if (item != null)
				{
					var project = item.GetContainingProject();

					if (project != null)
					{
						return new RelativePathBuilder(project.PhysicalPath, item.PhysicalPath).Build();
					}
				}
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}

		private void SetSolution(ITypeDescriptorContext context)
		{
			if (context != null)
			{
				this.solution = context.TryGetService<ISolution>();
			}
			else
			{
				//If context is null try using the GlobalProvider
				this.solution = ServiceProvider.GlobalProvider.TryGetService<ISolution>();
			}
		}
	}
}