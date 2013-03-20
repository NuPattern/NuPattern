using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Windows;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Extensibility.Properties;
using NuPattern.Runtime;

namespace NuPattern.Extensibility
{
	class ImageUriEditor : UITypeEditor
	{
		private static readonly ITraceSource tracer = Tracer.GetSourceFor<ImageUriEditor>();
		private const string ImageExtension = ".ico;*.png;*.jpg";
		private Window currentWindow;

		public ImageUriEditor()
		{
			this.currentWindow = Application.Current.MainWindow;
		}

		public ImageUriEditor(Window currentWindow)
		{
			this.currentWindow = currentWindow;
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			Guard.NotNull(() => context, context);
			Guard.NotNull(() => provider, provider);
			Guard.NotNull(() => value, value);

			var uriService = provider.GetService<IFxrUriReferenceService>();

			var iconUri = value as string;

			tracer.ShieldUI(() =>
			{
				var componentModel = provider.GetService<SComponentModel, IComponentModel>();
				var picker = componentModel.GetService<Func<ISolutionPicker>>()();
				picker.Owner = currentWindow;
				picker.RootItem = provider.GetService<ISolution>();
				picker.Title = Resources.ImageUriEditor_PickerTitle;
				picker.EmptyItemsMessage = string.Format(CultureInfo.CurrentCulture, Resources.ImageUriEditor_EmptyItemsMessage, ImageExtension);
				picker.Filter.Kind = ItemKind.Solution | ItemKind.SolutionFolder | ItemKind.Project | ItemKind.Folder | ItemKind.Item;
				picker.Filter.MatchFileExtensions = ImageExtension;
				SetSelectedItem(context, picker, uriService, value);
				if (picker.ShowDialog())
				{
					var item = picker.SelectedItem.As<IItem>();

					ConfigureSolutionItem(item);

					iconUri = uriService.CreateUri<ResourcePack>(new ResourcePack(item), "pack").AbsoluteUri;
				}
			}, Resources.ImageUriEditor_FailedToEdit);

			return iconUri;
		}

		private static void ConfigureSolutionItem(IItem item)
		{
			if (item != null)
			{
				var customTool = item.Data.CustomTool;
				item.Data.ItemType = "Resource";
				item.Data.CustomTool = customTool;
			}
		}

		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}

		private static void SetSelectedItem(ITypeDescriptorContext context, ISolutionPicker picker, IFxrUriReferenceService uriService, object value)
		{
			var uri = value as string;
			if (!string.IsNullOrEmpty(uri))
			{
				var item = uriService.TryResolveUri<ResourcePack>(new Uri(uri));
				if (item != null)
				{
					if (item.Type == ResourcePackType.ProjectItem)
					{
						picker.SelectedItem = item.GetItem();
					}
				}
			}
		}
	}
}
