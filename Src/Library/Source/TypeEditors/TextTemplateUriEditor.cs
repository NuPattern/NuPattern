using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Design;
using System.Globalization;
using System.Text;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Extensibility;
using NuPattern.Extensibility.Properties;
using NuPattern.Library.Automation;
using NuPattern.Library.Commands;
using NuPattern.Runtime;

namespace NuPattern.Library.TypeEditors
{
	/// <summary>
	/// An editor that allows selection of a T4 template inside a VSIX project, 
	/// that will be converted to a uri.
	/// </summary>
	[CLSCompliant(false)]
	public class TextTemplateUriEditor : UITypeEditor
	{
		private static readonly ITraceSource tracer = Tracer.GetSourceFor<TextTemplateUriEditor>();

		/// <summary>
		/// Edits the specified object's value using the editor style indicated by the <see cref="M:System.Drawing.Design.UITypeEditor.GetEditStyle"/> method.
		/// </summary>
		[SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Unavoidable")]
		[SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "In the properties window, IncludeInVSIX shows as lowercase.")]
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			Guard.NotNull(() => provider, provider);
			Guard.NotNull(() => value, value);

			var templateUri = (Uri)value;

			tracer.ShieldUI(() =>
			{
				var componentModel = provider.GetService<SComponentModel, IComponentModel>();
				var picker = componentModel.GetService<Func<ISolutionPicker>>()();

				var uriService = componentModel.GetService<IFxrUriReferenceService>();

				// Initialize solution picker
				var solution = provider.GetService<ISolution>();
				picker.Owner = provider.GetService<SVsUIShell, IVsUIShell>().GetMainWindow();
				picker.Title = Resources.TextTemplateUriEditor_Title;
				picker.RootItem = solution;

				picker.Filter.Kind = ItemKind.Solution | ItemKind.SolutionFolder | ItemKind.Project | ItemKind.Folder | ItemKind.Item;
				picker.Filter.IncludeFileExtensions = Resources.TextTemplateUriEditor_FileExtensions;

				if (picker.ShowDialog().GetValueOrDefault())
				{
					templateUri = BuildUri(picker.SelectedItem);

					// Set item properties automatically.
					var item = (IItem)picker.SelectedItem;
					item.Data.ItemType = BuildAction.Content.ToString();
					item.Data.IncludeInVSIX = Boolean.TrueString.ToLowerInvariant();

					SetAuthoringUri(context, uriService.CreateUri(picker.SelectedItem));
				}
			},
			Resources.TextTemplateUriEditor_FailedToEdit);

			return templateUri;
		}

		/// <summary>
		/// Builds the URI for the given template based on the containing project VSIX manifest identifier.
		/// </summary>
		public static Uri BuildUri(IItemContainer templateItem)
		{
			var manifest = templateItem.GetToolkitManifest();
			var owningProject = templateItem.Traverse(x => x.Parent, item => item.Kind == ItemKind.Project);

			tracer.TraceInformation(Properties.Resources.TextTemplateUriEditor_TraceReadingManifest, manifest.GetLogicalPath());

			string vsixId;
			try
			{
				vsixId = Vsix.ReadManifestIdentifier(manifest.PhysicalPath);
			}
			catch (Exception e)
			{
				tracer.TraceError(e,
					String.Format(CultureInfo.CurrentCulture, Properties.Resources.TextTemplateUriEditor_TraceReadingManifestFailed, manifest.GetLogicalPath()));
				throw;
			}

			var path = GetLogicalPath(templateItem).Replace(owningProject.GetLogicalPath(), string.Empty);
			var uri = new Uri(new Uri("t4://extension/"), new Uri(vsixId + path, UriKind.Relative));

			return uri;
		}

		/// <summary>
		/// Gets the path to an item in the solution, ignoring containing files
		/// </summary>
		public static string GetLogicalPath(IItemContainer item)
		{
			Guard.NotNull<IItemContainer>(() => item, item);
			StringBuilder builder = new StringBuilder(0x100);
			builder.Insert(0, item.Name).Insert(0, @"\");

			while (item.Parent != null)
			{
				if (item.Kind != ItemKind.Item)
				{
					builder.Insert(0, item.Name).Insert(0, @"\");
				}
				item = item.Parent;
			}
			return ((builder.Length == 0) ? string.Empty : builder.ToString(1, builder.Length - 1));
		}

		private static void SetAuthoringUri(ITypeDescriptorContext context, Uri uri)
		{
			var commandSettings = context.Instance as ICommandSettings;

			if (commandSettings != null)
			{
				// Make up the design property descriptor to persist the authoring value.
				var descriptor = new DesignPropertyDescriptor(
					Reflector<GenerateModelingCodeCommand>.GetPropertyName(c => c.TemplateAuthoringUri),
					string.Empty,
					string.Empty,
					string.Empty,
					typeof(string),
					typeof(CommandSettings),
					new Attribute[0]);

				var prop = (DesignProperty)descriptor.GetValue(commandSettings);
				prop.Value = uri.AbsoluteUri;
			}
		}

		/// <summary>
		/// Gets the editor style used by the <see cref="M:System.Drawing.Design.UITypeEditor.EditValue(System.IServiceProvider,System.Object)"/> method.
		/// </summary>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}
	}
}
