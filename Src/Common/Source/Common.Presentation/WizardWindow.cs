using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace NuPattern.Presentation
{
    /// <summary>
    /// Represents a window that supports pages to compose a wizard.
    /// </summary>
    [TemplatePart(Name = @"PART_NavWinCP", Type = typeof(ContentPresenter))]
    public abstract class WizardWindow : NavigationWindow, IServiceProvider
    {
        private static readonly DependencyPropertyKey PagesPropertyKey = DependencyProperty.RegisterReadOnly(
            "Pages",
            typeof(Collection<FrameworkElement>),
            typeof(WizardWindow),
            new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Pages"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PagesProperty = PagesPropertyKey.DependencyProperty;

        private int currentIndex;
        private IServiceProvider serviceProvider;

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static WizardWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WizardWindow), new FrameworkPropertyMetadata(typeof(WizardWindow)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WizardWindow"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        protected WizardWindow(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.SetValue(PagesPropertyKey, new Collection<FrameworkElement>());

            this.CommandBindings.Add(new CommandBinding(WizardCommands.NextPage, this.OnNextExecuted, this.OnNextCanExecute));
            this.CommandBindings.Add(new CommandBinding(WizardCommands.FinishWizard, this.OnFinishedExecuted, this.OnFinishedCanExecute));

            this.Loaded += this.OnWindowLoaded;
            this.LoadCompleted += this.OnPageLoadCompleted;
        }

        /// <summary>
        /// Gets the pages in the wizard.
        /// </summary>
        /// <value>The pages in the wizard.</value>
        public Collection<FrameworkElement> Pages
        {
            get { return (Collection<FrameworkElement>)this.GetValue(PagesProperty); }
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.-or- null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        public virtual object GetService(Type serviceType)
        {
            if (this.serviceProvider != null)
            {
                return this.serviceProvider.GetService(serviceType);
            }

            return null;
        }

        /// <summary>
        /// Called when the wizard is finished.
        /// </summary>
        protected virtual void OnFinishedExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        /// <summary>
        /// Determines whether the Finish button can be executed or not.
        /// </summary>
        protected virtual void OnFinishedCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute =
                // Either we don't have any pages
                this.Pages.Count == 0 ||
                // Or we're the last one.
                this.currentIndex == this.Pages.Count - 1;
        }

        /// <summary>
        /// Called when the Next button is executed.
        /// </summary>
        protected virtual void OnNextExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.CanGoForward)
            {
                this.GoForward();
            }
            else
            {
                this.Navigate(this.Pages[this.currentIndex + 1]);
            }
        }

        /// <summary>
        /// Determines whether the Finish button can be executed or not.
        /// </summary>
        protected virtual void OnNextCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            // As long as we're in a page prior to the last one. 
            // For non-pages wizards, this will always return false therefore.
            e.CanExecute = this.currentIndex < (this.Pages.Count - 1);
        }

        /// <summary>
        /// Called when the window is loaded.
        /// </summary>
        protected virtual void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (this.Pages.Count > 0)
            {
                this.Navigate(this.Pages[this.currentIndex]);
            }
        }

        /// <summary>
        /// Called when the page is loaded.
        /// </summary>
        protected virtual void OnPageLoadCompleted(object sender, NavigationEventArgs e)
        {
            this.currentIndex = this.Pages.IndexOf((FrameworkElement)e.Content);
        }
    }
}