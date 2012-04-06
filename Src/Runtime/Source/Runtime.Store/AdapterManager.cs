using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Integration;
using Microsoft.VisualStudio.Modeling.Integration.Shell;
using Microsoft.VisualStudio.TextTemplating.Modeling;
using System.ComponentModel.Composition;
using System;

namespace Microsoft.VisualStudio.Patterning.Runtime.Store
{
	/// <summary>
	/// Adapter manager for the runtime state.
	/// </summary>
	[Export(typeof(ModelBusAdapterManager))]
	[ExportMetadata(CompositionAttributes.AdapterIdKey, Adapter.AdapterId)]
	[HostSpecific(VsTextTemplatingModelingAdapterManager.HostName)]
	[HostSpecific(VsModelingAdapterManager.HostName)]
	[CLSCompliant(false)]
	public partial class AdapterManager : VsTextTemplatingModelingAdapterManager
	{
		/// <summary>
		/// Creates an instance of the <see cref="Adapter"/> class.
		/// </summary>
		protected override ModelingAdapter CreateModelingAdapterInstance(ModelBusReference reference, ModelElement rootModelElement)
		{
			return new Adapter(reference, this, rootModelElement);
		}

		/// <summary>
		/// Returns the <see cref="Adapter.AdapterId"/> constant.
		/// </summary>
		public override IEnumerable<string> GetSupportedLogicalAdapterIds()
		{
			yield return Adapter.AdapterId;
		}

		/// <summary>
		/// Whether the adapter manager can create a reference with the given information.
		/// </summary>
		public override bool CanCreateReference(params object[] modelLocatorInfo)
		{
			// Only interested in project items or Path
			EnvDTE.ProjectItem item = GetProjectItem(modelLocatorInfo);

			return (item != null);
		}

		/// <summary>
		/// Attempts to create and return a model bus reference from the supplied data.
		/// </summary>
		public override ModelBusReference CreateReference(params object[] modelLocatorInfo)
		{
			EnvDTE.ProjectItem item = GetProjectItem(modelLocatorInfo);

			if (item == null)
			{
				return null;
			}

			ModelingAdapterReference mar = new ModelingAdapterReference(null, null, item.get_FileNames(1));
			ModelBusReference mbr = new ModelBusReference(
				this.ModelBus, Adapter.AdapterId,
				Path.GetFileNameWithoutExtension(item.Name),
				mar);

			return mbr;
		}

		private EnvDTE.ProjectItem GetProjectItem(params object[] modelLocatorInfo)
		{
			if (modelLocatorInfo == null || modelLocatorInfo.Length == 0)
			{
				return null;
			}

			// Only interested in project items
			foreach (object item in modelLocatorInfo)
			{
				// Simple case where the argument is directly a ProjectItem
				EnvDTE.ProjectItem projectItem = item as EnvDTE.ProjectItem;

				// Case where this is a pathname (whether in the solution of not)
				if (projectItem == null && item is string)
				{
					EnvDTE.DTE dte = null;
					if (this.ModelBus != null)
					{
						dte = this.ModelBus.GetService(typeof(DTE)) as DTE;
					}

					if (dte == null)
					{
						dte = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(DTE)) as DTE;
					}

					if (dte == null)
					{
						return null;
					}

					projectItem = dte.Solution.FindProjectItem((string)item);
					if (projectItem == null)
					{
						//Item is not in the solution. Add existing item to this current project.
						projectItem = dte.ItemOperations.AddExistingItem((string)item);
					}
				}

				// We only support project items concening models of the right file extension
				if (projectItem != null && !string.IsNullOrEmpty(projectItem.Name))
				{
					global::System.IO.FileInfo fi = new global::System.IO.FileInfo(projectItem.Name);
					if (string.CompareOrdinal(this.FileExtension, fi.Extension) == 0)
					{
						return projectItem;
					}
				}
			}

			return null;
		}
	}
}
