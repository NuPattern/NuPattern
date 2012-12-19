using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using NuPattern.Extensibility;
using NuPattern.Runtime;

namespace NuPattern.Authoring.Authoring.Assets.Wizards.Pages
{
    /// <summary>
    /// A custom wizard page that edits the properties of the current element.
    /// </summary>
    public partial class ToolkitType : Page
    {
        /// <summary>
        /// Using a DependencyProperty as the backing store for CurrentToolkit.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CurrentToolkitProperty =
            DependencyProperty.Register("CurrentToolkit", typeof(IInstalledToolkitInfo),
            typeof(ToolkitType), new UIPropertyMetadata(new PropertyChangedCallback(OnCurrentToolkitChanged)));

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolkitType"/> class.
        /// </summary>
        public ToolkitType()
        {
            var componentModel = ServiceProvider.GlobalProvider.GetService<SComponentModel, IComponentModel>();
            var export = componentModel.GetService<IEnumerable<IInstalledToolkitInfo>>();
            this.Toolkits = export != null ? export.ToList() : new List<IInstalledToolkitInfo>();

            InitializeComponent();

            this.ExistingRadio.IsEnabled = this.Toolkits.Any();
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
        /// Gets the toolkits installed as VSIX.
        /// </summary>
        /// <value>The toolkits installed as VSIX.</value>
        public IList<IInstalledToolkitInfo> Toolkits { get; private set; }

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
    }
}