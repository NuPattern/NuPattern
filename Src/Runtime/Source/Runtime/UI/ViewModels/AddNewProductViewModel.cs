using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Runtime.Properties;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace Microsoft.VisualStudio.Patterning.Runtime.UI
{
    /// <summary>
    /// Provides a view model to create a new pattern.
    /// </summary>
    [CLSCompliant(false)]
    public class AddNewProductViewModel : ValidationViewModel
    {
        private string productName;
        private IInstalledToolkitInfo currentToolkit;
        private IPatternManager patternManager;
        private IUserMessageService userMessageService;
        private bool wasUserAssigned;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddNewProductViewModel"/> class.
        /// </summary>
        /// <param name="patternManager">The pattern manager.</param>
        /// <param name="userMessageService">The user message service.</param>
        public AddNewProductViewModel(IPatternManager patternManager, IUserMessageService userMessageService)
        {
            Guard.NotNull(() => patternManager, patternManager);
            Guard.NotNull(() => userMessageService, userMessageService);

            this.Toolkits = patternManager.InstalledToolkits;
            this.patternManager = patternManager;
            this.userMessageService = userMessageService;
            this.CurrentToolkit = this.Toolkits.FirstOrDefault();
            this.InitializeCommands();
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
        /// Gets the toolkits installed as VSIX.
        /// </summary>
        public IEnumerable<IInstalledToolkitInfo> Toolkits { get; private set; }

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
        /// Gets the select toolkit command.
        /// </summary>
        /// <value>The select toolkit command.</value>
        public System.Windows.Input.ICommand SelectToolkitCommand { get; private set; }

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
    }
}