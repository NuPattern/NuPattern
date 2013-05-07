using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Modeling.ExtensionEnablement;
using NuPattern.Diagnostics;
using NuPattern.Presentation;
using NuPattern.Reflection;
using NuPattern.Runtime.Automation;
using NuPattern.Runtime.Properties;
using NuPattern.VisualStudio;

namespace NuPattern.Runtime.UI.ViewModels
{
    /// <summary>
    /// The view model for an automation menu.
    /// </summary>
    internal class AutomationMenuOptionViewModel : MenuOptionViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutomationMenuOptionViewModel"/> class.
        /// </summary>
        /// <param name="automation">The menu execution behavior.</param>
        public AutomationMenuOptionViewModel(IAutomationExtension automation)
            : base(GetCaption(automation))
        {
            this.Command = new AutomationCommand(this, automation);
            this.ImagePath = ((IAutomationMenuCommand)automation).IconPath;
            this.IconType = string.IsNullOrEmpty(this.ImagePath) ? IconType.None : IconType.Image;
            this.SortOrder = ((IAutomationMenuCommand)automation).SortOrder;
        }

        internal void ReEvaluateCommand()
        {
            ((AutomationCommand)this.Command).OnCanExecuteChanged();
        }

        private static string GetCaption(IAutomationExtension automation)
        {
            Guard.NotNull(() => automation, automation);

            var menu = automation as IMenuCommand;
            return automation != null ? menu.Text : null;
        }

        private class NullQueryStatus : ICommandStatus
        {
            public void QueryStatus(IMenuCommand menu)
            {
            }
        }

        private class AutomationCommand : System.Windows.Input.ICommand
        {
            private static readonly Dictionary<string, Action<MenuOptionViewModel, IMenuCommand>> propertyMappings =
                new Dictionary<string, Action<MenuOptionViewModel, IMenuCommand>>
                {
                    { Reflector<IMenuCommand>.GetProperty(x => x.Visible).Name, (vm, m) => vm.IsVisible = m.Visible },
                    { Reflector<IMenuCommand>.GetProperty(x => x.Enabled).Name, (vm, m) => vm.IsEnabled = m.Enabled }
                };

            private static ITracer tracer = Tracer.Get<AutomationCommand>();

            private MenuOptionViewModel parent;
            private IAutomationExtension automation;
            private ICommandStatus status;
            private IMenuCommand menu;

            public AutomationCommand(MenuOptionViewModel parent, IAutomationExtension automation)
            {
                this.parent = parent;
                this.automation = automation;
                this.menu = (IMenuCommand)automation;
                this.status = automation as ICommandStatus ?? new NullQueryStatus();

                parent.IsVisible = this.menu.Visible;
                parent.IsEnabled = this.menu.Enabled;

                var propertyChanged = automation as INotifyPropertyChanged;

                if (propertyChanged != null)
                {
                    propertyChanged.PropertyChanged += this.OnMenuPropertyChanged;
                }
            }

            public event EventHandler CanExecuteChanged;

            [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
            public bool CanExecute(object parameter)
            {
                var propertyChanged = this.automation as INotifyPropertyChanged;

                try
                {
                    // Prevent re-entrancy on query status
                    if (propertyChanged != null)
                        propertyChanged.PropertyChanged -= this.OnMenuPropertyChanged;

                    this.status.QueryStatus(this.menu);
                    parent.IsEnabled = this.menu.Enabled;
                    parent.IsVisible = this.menu.Visible;
                }
                catch (Exception e)
                {
                    tracer.Error(e, Resources.AutomationCommand_QueryStatusFailed);
                    return false;
                }
                finally
                {
                    // Enable status monitoring again.
                    if (propertyChanged != null)
                        propertyChanged.PropertyChanged += this.OnMenuPropertyChanged;
                }

                return this.menu.Enabled;
            }

            public void Execute(object parameter)
            {
                tracer.ShieldUI(
                    () =>
                    {
                        using (new MouseCursor(System.Windows.Input.Cursors.Wait))
                        {
                            this.automation.Execute();
                        }
                    },
                    Resources.AutomationCommand_CommandExecuteFailed,
                    this.automation.Name);
            }

            internal void OnCanExecuteChanged()
            {
                if (this.CanExecuteChanged != null)
                {
                    this.CanExecuteChanged(this, EventArgs.Empty);
                }
            }

            private void OnMenuPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                Action<MenuOptionViewModel, IMenuCommand> propertyName;
                if (propertyMappings.TryGetValue(e.PropertyName, out propertyName))
                {
                    propertyName(parent, this.menu);
                    this.OnCanExecuteChanged();
                }
            }
        }
    }
}