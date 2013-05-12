using System;
using NuPattern.Presentation;
using NuPattern.Runtime.Bindings;
using NuPattern.VisualStudio;

namespace NuPattern.Runtime.UI.ViewModels
{
    /// <summary>
    /// Defines the context for Solution Builder view
    /// </summary>
    public interface ISolutionBuilderContext
    {
        /// <summary>
        /// Gets the binding factory.
        /// </summary>
        IBindingFactory BindingFactory { get; }

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        ISolutionBuilderViewModel ViewModel { get; set; }

        /// <summary>
        /// Gets the pattern manager.
        /// </summary>
        IPatternManager PatternManager { get; }

        /// <summary>
        /// Gets the set selected action.
        /// </summary>
        Action<IProductElementViewModel> SetSelected { get; set; }

        /// <summary>
        /// Gets the show properties action.
        /// </summary>
        Action ShowProperties { get; }

        /// <summary>
        /// Gets the new pattern dialog factory.
        /// </summary>
        Func<object, IDialogWindow> NewProductDialogFactory { get; }

        /// <summary>
        /// Gets the new node dialog factory.
        /// </summary>
        Func<object, IDialogWindow> NewNodeDialogFactory { get; }

        /// <summary>
        /// Gets the user message service.
        /// </summary>
        IUserMessageService UserMessageService { get; }
    }
}
