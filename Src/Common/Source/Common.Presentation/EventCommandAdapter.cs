using System.Windows;
using System.Windows.Input;

namespace NuPattern.Common.Presentation
{
	/// <summary>
	/// Defines an adapter to make a bridge between WPF events and WPF command.
	/// </summary>
	internal class EventCommandAdapter
	{
		private readonly RoutedEvent routedEvent;
		private DependencyProperty commandProperty;
		private DependencyProperty parameterProperty;

		/// <summary>
		/// Initializes a new instance of the <see cref="EventCommandAdapter"/> class.
		/// </summary>
		/// <param name="routedEvent">The routed event.</param>
		/// <param name="parameterProperty">The parameter property.</param>
		internal EventCommandAdapter(RoutedEvent routedEvent, DependencyProperty parameterProperty)
		{
			this.routedEvent = routedEvent;
			this.parameterProperty = parameterProperty;
		}

		/// <summary>
		/// Called when the attached property representing the <see cref="ICommand"/> changed.
		/// </summary>
		/// <param name="d">The control containing the command.</param>
		/// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		internal void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (this.commandProperty == null)
			{
				this.commandProperty = e.Property;
			}

			var element = d as UIElement;
			if (element != null)
			{
				if (e.OldValue != null)
				{
					element.RemoveHandler(this.routedEvent, (RoutedEventHandler)this.OnEventRaised);
				}

				if (e.NewValue != null)
				{
					element.AddHandler(this.routedEvent, (RoutedEventHandler)this.OnEventRaised);
				}
			}
		}

		private void OnEventRaised(object sender, RoutedEventArgs e)
		{
            var dep = e.Source as DependencyObject;
            if (dep != null)
            {
                var command = dep.GetValue(this.commandProperty) as ICommand;
                if (command != null)
                {
                    command.Execute(dep.GetValue(this.parameterProperty));
                }
            }
		}
	}
}