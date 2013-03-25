using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using NuPattern.Extensibility;
using NuPattern.Presentation;
using NuPattern.Reflection;
using NuPattern.Runtime.Properties;
using NuPattern.VisualStudio.Extensions;
using NuPattern.VisualStudio.Shell;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Runtime.UI
{
    /// <summary>
    /// Provides a view model to create a new pattern.
    /// </summary>
    internal partial class AddNewProductViewModel : ValidationViewModel
    {
        private string productName;
        private IInstalledToolkitInfo currentToolkit;
        private IPatternManager patternManager;
        private IUserMessageService userMessageService;
        private bool wasUserAssigned;
        private ObservableCollection<IInstalledToolkitInfo> allToolkits;
        private CollectionView toolkitsView;
        private bool showAllPatterns = false;
        private SortItem sortingItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddNewProductViewModel"/> class.
        /// </summary>
        /// <param name="patternManager">The pattern manager.</param>
        /// <param name="userMessageService">The user message service.</param>
        public AddNewProductViewModel(IPatternManager patternManager, IUserMessageService userMessageService)
        {
            Guard.NotNull(() => patternManager, patternManager);
            Guard.NotNull(() => userMessageService, userMessageService);

            this.allToolkits = new ObservableCollection<IInstalledToolkitInfo>(patternManager.InstalledToolkits);
            this.toolkitsView = new ListCollectionView(this.allToolkits);
            this.patternManager = patternManager;
            this.userMessageService = userMessageService;
            this.CurrentToolkit = this.allToolkits.FirstOrDefault();
            this.SortingItems = GetSortingItems();
            this.SortingItem = this.SortingItems.FirstOrDefault();
            this.InitializeCommands();
            this.ArrangeToolkits();
        }

        /// <summary>
        /// Gets or sets the current toolkit.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "AddNewProductViewModel_ProductTypeRequired")]
        public IInstalledToolkitInfo CurrentToolkit
        {
            get
            {
                return this.currentToolkit;
            }

            set
            {
                if (this.currentToolkit != value)
                {
                    this.currentToolkit = value;
                    this.OnPropertyChanged(() => this.CurrentToolkit);
                    var generatedName = this.currentToolkit != null ?
                        this.patternManager.Products.GetNewUniqueName(this.currentToolkit.Schema.Pattern.DisplayName) :
                        string.Empty;
                    this.SetProductName(generatedName, false);
                }
            }
        }

        /// <summary>
        /// Gets all the installed toolkits
        /// </summary>
        internal IEnumerable<IInstalledToolkitInfo> AllToolkits
        {
            get
            {
                return this.allToolkits;
            }
        }

        /// <summary>
        /// Gets the list of displayed toolkits
        /// </summary>
        public CollectionView Toolkits
        {
            get
            {
                return this.toolkitsView;
            }
        }

        /// <summary>
        /// Gets or sets the name of the pattern.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "AddNewProductViewModel_ProductNameRequired")]
        [RegularExpression(DataFormats.Runtime.InstanceNameExpression, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "AddNewProductViewModel_ProductNameInvalid")]
        public string ProductName
        {
            get
            {
                return this.productName;
            }
            set
            {
                this.SetProductName(value, true);
            }
        }

        /// <summary>
        /// The item that sorts the displayed toolkits.
        /// </summary>
        public SortItem SortingItem
        {
            get
            {
                if (this.sortingItem == null)
                {
                    // Set the first one
                    this.sortingItem = this.SortingItems.FirstOrDefault();
                }

                return this.sortingItem;
            }
            set
            {
                if (this.sortingItem == value)
                    return;

                this.sortingItem = value;

                ArrangeToolkits();
            }
        }

        /// <summary>
        /// The items displayed for sorting toolkits.
        /// </summary>
        public IEnumerable<SortItem> SortingItems { get; private set; }

        /// <summary>
        /// Gets the select toolkit command.
        /// </summary>
        public System.Windows.Input.ICommand SelectToolkitCommand { get; private set; }

        /// <summary>
        /// Gets the homepage command.
        /// </summary>
        public System.Windows.Input.ICommand HomePageCommand { get; private set; }

        /// <summary>
        /// Gets the showallpatterns command.
        /// </summary>
        public System.Windows.Input.ICommand ToggleAllPatternsCommand { get; private set; }

        private void CloseDialog(IDialogWindow dialog)
        {
            if (!this.patternManager.Products.Any(product => product.InstanceName.Equals(this.productName, StringComparison.OrdinalIgnoreCase)))
            {
                dialog.DialogResult = true;
                dialog.Close();
            }
            else
            {
                this.userMessageService.ShowError(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.AddNewProductViewModel_ProductNameDuplicated,
                    this.productName));
            }
        }

        private void InitializeCommands()
        {
            this.SelectToolkitCommand = new RelayCommand<IDialogWindow>(dialog => this.CloseDialog(dialog), dialog => this.IsValid);
            this.HomePageCommand = new RelayCommand(this.NavigateToHomePage, () => true);
            this.ToggleAllPatternsCommand = new RelayCommand<bool>(x => this.ToggleAllPatterns(x), x => true);
        }

        private void SetProductName(string value, bool isUserAssigned)
        {
            if (this.productName != value && (isUserAssigned || !this.wasUserAssigned))
            {
                this.productName = value;
                this.OnPropertyChanged(() => this.ProductName);
                this.wasUserAssigned = !string.IsNullOrEmpty(value) && isUserAssigned;
            }
        }

        private void NavigateToHomePage()
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(HomePageUrl));
        }

        private void ToggleAllPatterns(bool isChecked)
        {
            this.showAllPatterns = isChecked;
            ArrangeToolkits();
        }

        private IEnumerable<SortItem> GetSortingItems()
        {
            var sortingItems = new List<SortItem>();
            sortingItems.Add(new SortItem(
                    Resources.AddNewProductViewModel_SortFormatNameAscending,
                    String.Format(CultureInfo.InvariantCulture, "{{{0}.{1}.{2}}}",
                        Reflector<IInstalledToolkitInfo>.GetPropertyName(x => x.Schema),
                        Reflector<IPatternModelInfo>.GetPropertyName(x => x.Pattern),
                        Reflector<IPatternInfo>.GetPropertyName(x => x.DisplayName)),
                    ListSortDirection.Ascending));
            sortingItems.Add(new SortItem(
                    Resources.AddNewProductViewModel_SortFormatNameDescending,
                    String.Format(CultureInfo.InvariantCulture, "{{{0}.{1}.{2}}}",
                        Reflector<IInstalledToolkitInfo>.GetPropertyName(x => x.Schema),
                        Reflector<IPatternModelInfo>.GetPropertyName(x => x.Pattern),
                        Reflector<IPatternInfo>.GetPropertyName(x => x.DisplayName)),
                    ListSortDirection.Descending));

            return sortingItems;
        }

        private void ArrangeToolkits()
        {
            FilterToolkits();
            SortToolkits();

            // Notify filter changed
            OnPropertyChanged(Reflector<AddNewProductViewModel>.GetPropertyName(x => x.Toolkits));
        }

        private void SortToolkits()
        {
            // Resort the toolkits
            // Note: Regular SortDescription are not powerful enough to reflect over nested properties, must use a custom comparer
            var defaultView = this.toolkitsView as ListCollectionView;
            defaultView.CustomSort = new InstalledToolkitInfoComparer(this.sortingItem);
        }

        private void FilterToolkits()
        {
            // Filter the toolkits
            if (this.showAllPatterns)
            {
                // All toolkits, except 'hidden' visibility
                this.toolkitsView.Filter += new Predicate<object>(delegate(object x)
                {
                    var toolkitInfo = (IInstalledToolkitInfo)x;
                    return toolkitInfo.Classification.CreateVisibility != ExtensionVisibility.Hidden;
                });
            }
            else
            {
                // Only toolkits with 'expanded' visibility 
                this.toolkitsView.Filter += new Predicate<object>(delegate(object x)
                {
                    var toolkitInfo = (IInstalledToolkitInfo)x;
                    return toolkitInfo.Classification.CreateVisibility == ExtensionVisibility.Expanded;
                });
            }
        }

        private class InstalledToolkitInfoComparer : IComparer
        {
            private SortItem sortItem;

            /// <summary>
            /// Creates a new instance of the <see cref="InstalledToolkitInfoComparer"/> class.
            /// </summary>
            /// <param name="sortItem"></param>
            public InstalledToolkitInfoComparer(SortItem sortItem)
            {
                Guard.NotNull(() => sortItem, sortItem);

                this.sortItem = sortItem;
            }

            public int Compare(object x, object y)
            {
                var toolkitInfo1 = x as IInstalledToolkitInfo;
                var toolkitInfo2 = y as IInstalledToolkitInfo;
                if (toolkitInfo1 == null || toolkitInfo2 == null)
                {
                    return -1;
                }

                // Evaluate the sort expression
                var value1 = ExpressionEvaluator.Evaluate(toolkitInfo1, this.sortItem.SortPropertyExpression);
                var value2 = ExpressionEvaluator.Evaluate(toolkitInfo2, this.sortItem.SortPropertyExpression);

                // Sort in right direction
                if (this.sortItem.Direction == ListSortDirection.Ascending)
                {
                    return value1.CompareTo(value2);
                }
                else
                {
                    return value2.CompareTo(value1);

                }
            }
        }
    }
}