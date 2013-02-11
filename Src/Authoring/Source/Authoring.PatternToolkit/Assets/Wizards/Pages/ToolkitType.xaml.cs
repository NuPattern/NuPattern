using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using NuPattern.Extensibility;
using NuPattern.Runtime;

namespace NuPattern.Authoring.PatternToolkit.Assets.Wizards.Pages
{
    /// <summary>
    /// A custom wizard page that edits the properties of the current element.
    /// </summary>
    public partial class ToolkitType : Page, INotifyPropertyChanged
    {
        private const string DefaultSortFilter = "Name";
        private ObservableCollection<IInstalledToolkitInfo> allToolkits;
        private CollectionView toolkitsView;

        /// <summary>
        /// Property change event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raised the property change event.
        /// </summary>
        /// <param name="propertyName">Name fo the property that has changed.</param>
        protected void OnNotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CurrentToolkit.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CurrentToolkitProperty =
            DependencyProperty.Register(Reflector<ToolkitType>.GetPropertyName(x => x.CurrentToolkit), typeof(IInstalledToolkitInfo),
            typeof(ToolkitType), new UIPropertyMetadata(new PropertyChangedCallback(OnCurrentToolkitChanged)));

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolkitType"/> class.
        /// </summary>
        public ToolkitType()
        {
            var componentModel = ServiceProvider.GlobalProvider.GetService<SComponentModel, IComponentModel>();
            var export = componentModel.GetService<IEnumerable<IInstalledToolkitInfo>>();
            this.allToolkits = new ObservableCollection<IInstalledToolkitInfo>((export != null) ? export.ToList() : new List<IInstalledToolkitInfo>());
            this.toolkitsView = new ListCollectionView(this.allToolkits);

            InitializeComponent();

            FilterToolkits();
        }

        /// <summary>
        /// Gets or sets the current toolkit.
        /// </summary>
        /// <value>The current toolkit.</value>
        [CLSCompliant(false)]
        public IInstalledToolkitInfo CurrentToolkit
        {
            get { return (IInstalledToolkitInfo)GetValue(CurrentToolkitProperty); }
            set { SetValue(CurrentToolkitProperty, value); }
        }

        /// <summary>
        /// Gets the currently installed toolkits.
        /// </summary>
        [CLSCompliant(false)]
        public CollectionView Toolkits
        {
            get
            {
                return this.toolkitsView;
            }
        }

        private void ChangeExistingToolkitId(IInstalledToolkitInfo existingToolkitInfo)
        {
            var currentElement = this.DataContext as IProductElement;
            if (currentElement != null)
            {
                // Update the element properties
                var toolkit = currentElement.As<IPatternToolkit>();
                var toolkitInfo = toolkit.Development.PatternToolkitInfo.AsElement();

                toolkit.ExistingToolkitId = existingToolkitInfo != null ? existingToolkitInfo.Id.ToString() : string.Empty;

                var property = toolkitInfo.Properties.FirstOrDefault(p => p.DefinitionName == Reflector<IToolkitInfo>.GetPropertyName(x => x.Description));
                if (property != null)
                {
                    property.Value = existingToolkitInfo != null ? existingToolkitInfo.Description : property.Info.DefaultValue.Value;
                }

                property = currentElement.Properties.FirstOrDefault(p => p.DefinitionName == Reflector<IPatternToolkit>.GetPropertyName(x => x.PatternName));
                if (property != null)
                {
                    property.Value = existingToolkitInfo != null ? existingToolkitInfo.Schema.Pattern.Name : property.Info.DefaultValue.Value;
                }

                property = currentElement.Properties.FirstOrDefault(p => p.DefinitionName == Reflector<IPatternToolkit>.GetPropertyName(x => x.PatternDescription));
                if (property != null)
                {
                    property.Value = existingToolkitInfo != null ? existingToolkitInfo.Schema.Pattern.Description : property.Info.DefaultValue.Value;
                }
            }
        }

        private void OnExistingRadioCheckedChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            bool existing = ((ToggleButton)sender).IsChecked ?? false;
            this.ChangeExistingToolkitId(existing ? this.CurrentToolkit : null);
        }

        private static void OnCurrentToolkitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var page = (ToolkitType)d;
            page.ChangeExistingToolkitId(e.NewValue as IInstalledToolkitInfo);
        }

        private void OnShowAllToolkitsChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            FilterToolkits();
        }

        private void FilterToolkits()
        {
            // Filter the results
            var showAllToolkits = (bool)ShowAllToolkits.IsChecked;
            if (showAllToolkits)
            {
                // All toolkits, except 'hidden' visibility
                this.toolkitsView.Filter += new Predicate<object>(delegate(object x)
                    {
                        var toolkitInfo = (IInstalledToolkitInfo)x;
                        return toolkitInfo.Classification.CustomizeVisibility != ToolkitVisibility.Hidden;
                    });
            }
            else
            {
                // Only toolkits with 'expanded' visibility 
                this.toolkitsView.Filter += new Predicate<object>(delegate(object x)
                {
                    var toolkitInfo = (IInstalledToolkitInfo)x;
                    return toolkitInfo.Classification.CustomizeVisibility == ToolkitVisibility.Expanded;
                });
            }

            // Notify filter changed
            OnNotifyPropertyChanged(Reflector<ToolkitType>.GetPropertyName(x => x.Toolkits));
        }
    }
}