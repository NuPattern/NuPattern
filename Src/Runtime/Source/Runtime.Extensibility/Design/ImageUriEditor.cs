using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Windows;
using Microsoft.VisualStudio.ComponentModelHost;
using NuPattern.Diagnostics;
using NuPattern.Runtime.Properties;
using NuPattern.Runtime.UI;
using NuPattern.VisualStudio;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Runtime.Design
{
    /// <summary>
    /// An editor for images.
    /// </summary>
    public class ImageUriEditor : UITypeEditor
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<ImageUriEditor>();
        private const string ImageExtension = @".ico;*.png;*.jpg";
        private Window currentWindow;

        /// <summary>
        /// Creates a new instance of the <see cref="ImageUriEditor"/> class.
        /// </summary>
        public ImageUriEditor()
        {
            this.currentWindow = Application.Current.MainWindow;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ImageUriEditor"/> class.
        /// </summary>
        public ImageUriEditor(Window currentWindow)
        {
            this.currentWindow = currentWindow;
        }

        /// <summary>
        /// Edits the value of the editor.
        /// </summary>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            Guard.NotNull(() => context, context);
            Guard.NotNull(() => provider, provider);
            Guard.NotNull(() => value, value);

            var uriService = provider.GetService<IUriReferenceService>();

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

                    iconUri = uriService.CreateUri<ResourcePack>(new ResourcePack(item), PackUri.UriScheme).AbsoluteUri;
                }
            }, Resources.ImageUriEditor_FailedToEdit);

            return iconUri;
        }

        private static void ConfigureSolutionItem(IItem item)
        {
            if (item != null)
            {
                var customTool = item.Data.CustomTool;
                item.Data.ItemType = @"Resource";
                item.Data.CustomTool = customTool;
            }
        }

        /// <summary>
        /// Returns the editors style.
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        private static void SetSelectedItem(ITypeDescriptorContext context, ISolutionPicker picker, IUriReferenceService uriService, object value)
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
