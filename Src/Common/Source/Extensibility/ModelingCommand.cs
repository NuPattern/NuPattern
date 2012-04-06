using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Microsoft.VisualStudio.Modeling.ExtensionEnablement;
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.VisualStudio.Patterning.Extensibility
{
	/// <summary>
	/// Extension command class for dsls.
	/// </summary>
	public abstract class ModelingCommand<TTarget> : ICommandExtension where TTarget : class
	{
		private IMonitorSelectionService monitorSelection;

		/// <summary>
		/// Gets the text for the command.
		/// </summary>
		public virtual string Text
		{
			get { return string.Empty; }
		}

		/// <summary>
		/// Gets the monitor selection.
		/// </summary>
		/// <value>The monitor selection.</value>
		[CLSCompliant(false)]
		protected IMonitorSelectionService MonitorSelection
		{
			get
			{
				if (this.monitorSelection == null)
				{
					this.monitorSelection =
						(IMonitorSelectionService)((IServiceProvider)this.ModelingPackage)
							.GetService(typeof(IMonitorSelectionService));
				}

				return this.monitorSelection;
			}
		}

		/// <summary>
		/// Gets the view.
		/// </summary>
		/// <value>The single diagram view.</value>
		[CLSCompliant(false)]
		protected SingleDiagramDocView View
		{
			get
			{
				return this.MonitorSelection.CurrentDocumentView as SingleDiagramDocView;
			}
		}

		/// <summary>
		/// Gets the doc data.
		/// </summary>
		/// <value>The doc data.</value>
		[CLSCompliant(false)]
		protected ModelingDocData DocData
		{
			get
			{
				return this.MonitorSelection.CurrentDocument as ModelingDocData;
			}
		}

		/// <summary>
		/// Gets the current selection.
		/// </summary>
		/// <value>The current selection.</value>
		protected virtual IEnumerable<TTarget> CurrentSelection
		{
			get
			{
				var currentSelectionContainer = this.MonitorSelection.CurrentSelectionContainer as ModelingWindowPane;

				if (currentSelectionContainer != null)
				{
					var selectedShapes = currentSelectionContainer.GetSelectedComponents().OfType<ShapeElement>();

					return selectedShapes.Where(shape => shape.ModelElement is TTarget)
						.Select(shape => shape.ModelElement as TTarget);
				}

				return null;
			}
		}

		/// <summary>
		/// Gets or sets the service provider.
		/// </summary>
		/// <value>The service provider.</value>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "MEF")]
		[Import(typeof(SVsServiceProvider))]
		private IServiceProvider ServiceProvider { get; set; }

		/// <summary>
		/// Gets or sets the component model package.
		/// </summary>
		/// <value>The component model package.</value>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "MEF")]
		[Import(typeof(ModelingPackage))]
		private ModelingPackage ModelingPackage { get; set; }

		/// <summary>
		/// Run the command.
		/// </summary>
		/// <param name="command">The menu command.</param>
		public virtual void Execute(IMenuCommand command)
		{
		}

		/// <summary>
		/// Decide dynamically  whether to show this menu command.
		/// </summary>
		/// <param name="command">The menu command.</param>
		public virtual void QueryStatus(IMenuCommand command)
		{
			Guard.NotNull(() => command, command);

			command.Visible = command.Enabled =
				this.CurrentSelection != null && this.CurrentSelection.Count() > 0;
		}
	}
}