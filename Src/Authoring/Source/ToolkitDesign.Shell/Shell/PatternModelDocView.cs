using System;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Modeling.Shell;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema
{
	/// <summary>
	/// Double-derived class to allow easier code customization.
	/// </summary>
	internal partial class PatternModelDocView
	{
		private string physicalView;

		/// <summary>
		/// Initializes a new instance of the <see cref="PatternModelDocView"/> class.
		/// </summary>
		/// <param name="docData">The doc data.</param>
		/// <param name="serviceProvider">The service provider.</param>
		/// <param name="physicalView">The physical view.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "MEF")]
		public PatternModelDocView(ModelingDocData docData, IServiceProvider serviceProvider, string physicalView)
			: base(docData, serviceProvider)
		{
			this.physicalView = physicalView;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "MEF")]
		[Import]
		private ModelingPackage ModelingPackage { get; set; }

		/// <summary>
		/// Overriden to publish context bag.  For editors, general context should be associated with the SEID.
		/// </summary>
		protected override void Initialize()
		{
			base.Initialize();

			ModelingCompositionContainer.CompositionService.SatisfyImportsOnce(this);
		}

		/// <summary>
		/// Called to initialize the view after the corresponding document has been loaded.
		/// </summary>
		/// <returns>If there were errors.</returns>
		protected override bool LoadView()
		{
			this.BaseLoadView();

			if (this.DocData.RootElement == null)
			{
				return false;
			}

			if (string.IsNullOrEmpty(this.physicalView))
			{
				var docData = this.DocData as PatternModelDocDataBase;
				var diagramPartition = docData.GetDiagramPartition();

				if (diagramPartition != null)
				{
					this.SetCurrentDiagram(docData);

					return true;
				}
			}

			return false;
		}

		private void SetCurrentDiagram(PatternModelDocDataBase docData)
		{
			this.Diagram = docData.Store.GetDiagramForDefaultView();
		}
	}
}