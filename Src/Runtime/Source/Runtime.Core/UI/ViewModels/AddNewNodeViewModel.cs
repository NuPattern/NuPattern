using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using NuPattern.Presentation;
using NuPattern.Runtime.Properties;
using NuPattern.VisualStudio.Shell;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Runtime.UI.ViewModels
{
    /// <summary>
    /// Provides a view model to create a new node.
    /// </summary>
    internal class AddNewNodeViewModel : ValidationViewModel
    {
        private string instanceName;
        private IEnumerable<IProductElement> sibilings;
        private IUserMessageService userMessageService;
        private IPatternElementInfo info;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddNewNodeViewModel"/> class.
        /// </summary>
        /// <param name="siblings">The sibilings.</param>
        /// <param name="info">The info.</param>
        /// <param name="userMessageService">The user message service.</param>
        public AddNewNodeViewModel(
            IEnumerable<IProductElement> siblings,
            IPatternElementInfo info,
            IUserMessageService userMessageService)
        {
            Guard.NotNull(() => siblings, siblings);
            Guard.NotNull(() => info, info);
            Guard.NotNull(() => userMessageService, userMessageService);

            this.info = info;
            this.sibilings = siblings;
            this.instanceName = siblings.GetNewUniqueName(info.DisplayName);
            this.userMessageService = userMessageService;
            this.InitializeCommands();
        }

        /// <summary>
        /// Gets the accept command.
        /// </summary>
        public System.Windows.Input.ICommand AcceptCommand { get; private set; }

        /// <summary>
        /// Gets or sets the name of the instance.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "AddNewNodeViewModel_ProductNameRequired")]
        [RegularExpression(DataFormats.Runtime.InstanceNameExpression, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "AddNewProductViewModel_ProductNameInvalid")]
        public string InstanceName
        {
            get
            {
                return this.instanceName;
            }

            set
            {
                if (this.instanceName != value)
                {
                    this.instanceName = value;
                    this.OnPropertyChanged(() => this.InstanceName);
                }
            }
        }

        /// <summary>
        /// Gets or sets the title of the dialog
        /// </summary>
        public string DialogTitle
        {
            get
            {
                if (this.info != null)
                {
                    return string.Format(CultureInfo.CurrentUICulture,
                        Properties.Resources.AddNewNodeView_TitleFormat, info.DisplayName);
                }
                else
                {
                    return Properties.Resources.AddNewNodeView_DefaultTitle;
                }
            }
        }

        private void CloseDialog(IDialogWindow dialog)
        {
            if (!this.sibilings.Any(product => product.InstanceName.Equals(this.instanceName, StringComparison.OrdinalIgnoreCase)))
            {
                dialog.DialogResult = true;
                dialog.Close();
            }
            else
            {
                this.userMessageService.ShowError(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.AddNewProductViewModel_ProductNameDuplicated,
                    this.instanceName));
            }
        }

        private void InitializeCommands()
        {
            this.AcceptCommand = new RelayCommand<IDialogWindow>(dialog => this.CloseDialog(dialog), dialog => this.IsValid);
        }
    }
}