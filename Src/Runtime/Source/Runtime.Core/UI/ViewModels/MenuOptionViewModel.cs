using System.Collections.Generic;
using System.Collections.ObjectModel;
using NuPattern.Presentation;
using System;

namespace NuPattern.Runtime.UI.ViewModels
{
    /// <summary>
    /// Defines behavior for dynamic menu options.
    /// </summary>
    [CLSCompliant(false)]
    public class MenuOptionViewModel : ViewModel
    {
        private const long DefaultSortOrder = 100;

        private bool isSelected;
        private bool isEnabled;
        private bool isVisible = true;
        private string imagePath;
        private string inputGestureText;
        private IconType iconType;

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuOptionViewModel"/> class.
        /// </summary>
        /// <param name="caption">The caption.</param>
        public MenuOptionViewModel(string caption)
        {
            Guard.NotNullOrEmpty(() => caption, caption);

            this.Caption = caption;
            this.GroupIndex = 0;
            this.SortOrder = DefaultSortOrder;
            this.MenuOptions = new ObservableCollection<MenuOptionViewModel>();
            this.MenuOptions.CollectionChanged += (s, e) => this.SetEnabledDefault();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuOptionViewModel"/> class.
        /// </summary>
        /// <param name="caption">The caption.</param>
        /// <param name="command">The command.</param>
        public MenuOptionViewModel(string caption, System.Windows.Input.ICommand command)
            : this(caption)
        {
            this.Command = command;
            this.SetEnabledDefault();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuOptionViewModel"/> class.
        /// </summary>
        /// <param name="caption">The caption.</param>
        /// <param name="menuOptions">The menu options.</param>
        public MenuOptionViewModel(string caption, IEnumerable<MenuOptionViewModel> menuOptions)
            : this(caption)
        {
            Guard.NotNull(() => menuOptions, menuOptions);

            this.MenuOptions.AddRange(menuOptions);
            this.SetEnabledDefault();
        }

        /// <summary>
        /// Gets the caption.
        /// </summary>
        public string Caption { get; private set; }

        /// <summary>
        /// Gets the command.
        /// </summary>
        public System.Windows.Input.ICommand Command { get; protected set; }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        public object Model { get; set; }

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        public int GroupIndex { get; set; }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        public long SortOrder { get; set; }

        /// <summary>
        /// Gets or sets the text describing an input gesture that will call the command tied to the specified item.
        /// </summary>
        public string InputGestureText
        {
            get
            {
                return this.inputGestureText;
            }
            set
            {
                if (this.inputGestureText != value)
                {
                    this.inputGestureText = value;
                    this.OnPropertyChanged(() => this.InputGestureText);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }

            set
            {
                if (this.isEnabled != value)
                {
                    this.isEnabled = value;
                    this.OnPropertyChanged(() => this.IsEnabled);
                }
            }
        }

        /// <summary>
        /// Gets or sets the image path.
        /// </summary>
        public string ImagePath
        {
            get
            {
                return this.imagePath;
            }

            set
            {
                if (this.imagePath != value)
                {
                    this.imagePath = value;
                    this.OnPropertyChanged(() => this.ImagePath);
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of the icon.
        /// </summary>
        public IconType IconType
        {
            get
            {
                return this.iconType;
            }

            set
            {
                if (this.iconType != value)
                {
                    this.iconType = value;
                    this.OnPropertyChanged(() => this.IconType);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }

            set
            {
                if (this.isSelected != value)
                {
                    this.isSelected = value;
                    this.OnPropertyChanged(() => this.IsSelected);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is visible.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsVisible
        {
            get
            {
                return this.isVisible;
            }

            set
            {
                if (this.isVisible != value)
                {
                    this.isVisible = value;
                    this.OnPropertyChanged(() => this.IsVisible);
                }
            }
        }

        /// <summary>
        /// Gets the menu options.
        /// </summary>
        /// <value>The menu options.</value>
        public ObservableCollection<MenuOptionViewModel> MenuOptions { get; private set; }

        private void SetEnabledDefault()
        {
            this.IsEnabled = this.Command != null || this.MenuOptions.Count != 0;
        }
    }
}