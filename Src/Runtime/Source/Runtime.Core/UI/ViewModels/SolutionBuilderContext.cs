using System;
using NuPattern.Presentation;
using NuPattern.Runtime.Bindings;
using NuPattern.VisualStudio;

namespace NuPattern.Runtime.UI.ViewModels
{
    /// <summary>
    /// Defines the needed classes for the <see cref="SolutionBuilderViewModel"/>.
    /// </summary>
    internal class SolutionBuilderContext : NuPattern.Runtime.UI.ViewModels.ISolutionBuilderContext
    {
        /// <summary>
        /// Gets the binding factory.
        /// </summary>
        public IBindingFactory BindingFactory { get; internal set; }

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        public SolutionBuilderViewModel ViewModel { get; internal set; }

        /// <summary>
        /// Gets the pattern manager.
        /// </summary>
        public IPatternManager PatternManager { get; internal set; }

        /// <summary>
        /// Gets the set selected action.
        /// </summary>
        public Action<ProductElementViewModel> SetSelected { get; internal set; }

        /// <summary>
        /// Gets the show properties action.
        /// </summary>
        public Action ShowProperties { get; internal set; }

        /// <summary>
        /// Gets the new pattern dialog factory.
        /// </summary>
        public Func<object, IDialogWindow> NewProductDialogFactory { get; internal set; }

        /// <summary>
        /// Gets the new node dialog factory.
        /// </summary>
        public Func<object, IDialogWindow> NewNodeDialogFactory { get; internal set; }

        /// <summary>
        /// Gets the user message service.
        /// </summary>
        public IUserMessageService UserMessageService { get; internal set; }
    }
}