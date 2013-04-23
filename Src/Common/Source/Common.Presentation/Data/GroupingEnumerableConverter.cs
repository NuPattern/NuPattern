using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using Microsoft.VisualStudio.PlatformUI;
using NuPattern.Presentation.Properties;

namespace NuPattern.Presentation.Data
{
    /// <summary>
    /// Applies grouping to the given <see cref="IEnumerable"/>.
    /// </summary>
    [ValueConversion(typeof(IEnumerable), typeof(ICollectionView))]
    public class GroupingEnumerableConverter : ValueConverter<IEnumerable, ICollectionView>
    {
        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="parameter">The parameter to use as property name in the grouping.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>A <see cref="ICollectionView"/> with grouping applied.</returns>
        protected override ICollectionView Convert(IEnumerable value, object parameter, CultureInfo culture)
        {
            var propertyName = parameter as string;
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new InvalidOperationException(Resources.GroupingEnumerableConverter_InvalidPropertyName);
            }

            var view = CollectionViewSource.GetDefaultView(value);
            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(new SortDescription(propertyName, ListSortDirection.Ascending));
            view.GroupDescriptions.Clear();
            view.GroupDescriptions.Add(new PropertyGroupDescription(propertyName));
            return view;
        }
    }
}