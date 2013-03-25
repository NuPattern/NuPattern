using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NuPattern.Presentation
{
    /// <summary>
    /// Defines common behaviors for WPF controls.
    /// </summary>
    public static class ControlBehavior
    {
        /// <summary>
        /// Identifies the <c>DoubleClickCommandParameter</c> property.
        /// </summary>
        public static readonly DependencyProperty DoubleClickCommandParameterProperty = DependencyProperty.RegisterAttached(
                "DoubleClickCommandParameter",
                typeof(object),
                typeof(ControlBehavior));

        /// <summary>
        /// Identifies the <c>DoubleClickCommand</c> property.
        /// </summary>
        public static readonly DependencyProperty DoubleClickCommandProperty = DependencyProperty.RegisterAttached(
                "DoubleClickCommand",
                typeof(ICommand),
                typeof(ControlBehavior),
                new FrameworkPropertyMetadata(new EventCommandAdapter(
                    Control.MouseDoubleClickEvent,
                    DoubleClickCommandParameterProperty).OnPropertyChanged));

        /// <summary>
        /// Gets the <see cref="ICommand"/> defined for the double click event.
        /// </summary>
        /// <param name="element">The <see cref="Control"/> element.</param>
        /// <returns>The <see cref="ICommand"/> to execute after the user press double click.</returns>
        [AttachedPropertyBrowsableForType(typeof(Control))]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Attached Property")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Attached Property applicable to GridViewColumn")]
        public static ICommand GetDoubleClickCommand(Control element)
        {
            return (ICommand)element.GetValue(DoubleClickCommandProperty);
        }

        /// <summary>
        /// Set the <see cref="ICommand"/> defined for the double click event.
        /// </summary>
        /// <param name="element">The <see cref="Control"/> element.</param>
        /// <param name="value">The <see cref="ICommand"/> to execute after the user press double click.</param>
        [AttachedPropertyBrowsableForType(typeof(Control))]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Attached Property")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Attached Property applicable to GridViewColumn")]
        public static void SetDoubleClickCommand(Control element, ICommand value)
        {
            element.SetValue(DoubleClickCommandProperty, value);
        }

        /// <summary>
        /// Gets the value to send as parameter of the <see cref="ICommand"/> when a double click event happen.
        /// </summary>
        /// <param name="element">The <see cref="Control"/> element.</param>
        /// <returns>The value to send as parameter of the <see cref="ICommand"/>.</returns>
        [AttachedPropertyBrowsableForType(typeof(Control))]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Attached Property")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Attached Property applicable to GridViewColumn")]
        public static object GetDoubleClickCommandParameter(Control element)
        {
            return element.GetValue(DoubleClickCommandParameterProperty);
        }

        /// <summary>
        /// Sets the value to send as parameter of the <see cref="ICommand"/> when a double click event happen.
        /// </summary>
        /// <param name="element">The <see cref="Control"/> element.</param>
        /// <param name="value">The value to send as parameter of the <see cref="ICommand"/>.</param>
        [AttachedPropertyBrowsableForType(typeof(Control))]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Attached Property")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Attached Property applicable to GridViewColumn")]
        public static void SetDoubleClickCommandParameter(Control element, ICommand value)
        {
            element.SetValue(DoubleClickCommandParameterProperty, value);
        }
    }
}