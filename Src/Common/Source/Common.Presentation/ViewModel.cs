using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using NuPattern.Presentation.Properties;

namespace NuPattern.Presentation
{
    /// <summary>
    /// Defines a view model for dialogs
    /// </summary>
    [DebuggerStepThrough]
    public abstract class ViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ViewModel"/> class.
        /// </summary>
        protected ViewModel()
        {
        }

        /// <summary>
        /// Handles the property changed event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when a value of a property changes
        /// </summary>
        protected void OnPropertyChanged<T>(Expression<Func<T>> propertyExpresion)
        {
            var property = propertyExpresion.Body as MemberExpression;
            if (property == null || !(property.Member is PropertyInfo) ||
                !property.Member.DeclaringType.IsAssignableFrom(this.GetType()))
            {
                throw new InvalidOperationException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.ViewModel_InvalidExpressionProperty,
                    propertyExpresion,
                    this.GetType()));
            }

            this.OnPropertyChanged(property.Member.Name);
        }

        /// <summary>
        /// Called when a value of a property changes
        /// </summary>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// Defines a view model for dialogs
    /// </summary>
    [DebuggerStepThrough]
    public abstract class ViewModel<T> : ViewModel where T : class
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ViewModel{T}"/> class.
        /// </summary>
        protected ViewModel(T model)
        {
            Guard.NotNull(() => model, model);

            this.Model = model;
            this.RegisterCommands();
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        public T Model { get; private set; }

        /// <summary>
        /// Registers the commands for the view.
        /// </summary>
        protected virtual void RegisterCommands()
        {
        }
    }
}