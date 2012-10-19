using System;
using System.Drawing.Design;
using System.Windows;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Patterning.Extensibility.Properties;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace Microsoft.VisualStudio.Patterning.Extensibility
{
    class ImageUriEditor : UITypeEditor
    {
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

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            Guard.NotNull(() => context, context);
            Guard.NotNull(() => provider, provider);
            Guard.NotNull(() => value, value);

            var solution = provider.GetService<ISolution>();
            var uriService = provider.GetService<IFxrUriReferenceService>();

            var iconUri = value as string;

            var componentModel = provider.GetService<SComponentModel, IComponentModel>();
            var picker = componentModel.GetService<Func<ISolutionPicker>>()();
            picker.Owner = currentWindow;
            picker.Title = Resources.ImageUriEditor_PickerTitle;
            picker.RootItem = solution;
            picker.EmptyItemsMessage = Resources.ImageUriEditor_NoImageMessage;

            picker.Filter.Kind = ItemKind.Solution | ItemKind.SolutionFolder | ItemKind.Project | ItemKind.Folder | ItemKind.Item;
            picker.Filter.IncludeFileExtensions = ImageExtension;

            if (picker.ShowDialog().GetValueOrDefault())
            {
                var item = picker.SelectedItem.As<IItem>();

                ConfigureSolutionItem(item);

                iconUri = uriService.CreateUri<ResourcePack>(new ResourcePack(item), "pack").AbsoluteUri;
            }

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
    }
}
