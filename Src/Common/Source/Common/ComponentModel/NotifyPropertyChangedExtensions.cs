using System;
using System.ComponentModel;
using System.Linq.Expressions;
using NuPattern.Reflection;

namespace NuPattern.ComponentModel
{
    /// <summary>
    /// Extensions to the <see cref="INotifyPropertyChanged"/> interface.
    /// </summary>
    public static class NotifyPropertyChangedExtensions
    {
        /// <summary>
        /// Raises the property changed event.
        /// </summary>
        public static void NotifyChanged<T, TProperty>(this T sender, PropertyChangedEventHandler handler, Expression<Func<T, TProperty>> property)
            where T : INotifyPropertyChanged
            where TProperty : class
        {
            if (handler != null)
                handler.Invoke(sender, new PropertyChangedEventArgs(Reflector<T>.GetPropertyName(property)));
        }

        /// <summary>
        /// Subscribes to the nested event handler.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "sender")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "handler")]
        public static void SubscribeNested<T, TProperty>(this T sender, PropertyChangedEventHandler handler,
            PropertyChangedEventHandler nestedHandler, TProperty oldValue, TProperty newValue)
            where T : INotifyPropertyChanged
            where TProperty : class, INotifyPropertyChanged
        {
            if (oldValue != null && oldValue != newValue)
                oldValue.PropertyChanged -= nestedHandler;

            if (newValue != null)
                newValue.PropertyChanged += nestedHandler;
        }
    }
}
