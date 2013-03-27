using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Runtime.Properties;
using NuPattern.Runtime.UI;
using NuPattern.VisualStudio;
using NuPattern.VisualStudio.Shell;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Runtime.Design
{
    /// <summary>
    /// Representes the editor to change icon paths.
    /// </summary>
    public class IconEditor : UITypeEditor
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<IconEditor>();
        private const string ImageExtension = ".gif;.jpg;.jpeg;.bmp;.png;.ico";

        /// <summary>
        /// Edits the specified object's value using the editor style indicated by the <see cref="UITypeEditor.GetEditStyle()"/> method.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
        /// <param name="provider">An <see cref="IServiceProvider"/> that this editor can use to obtain services.</param>
        /// <param name="value">The object to edit.</param>
        /// <returns>
        /// The new value of the object. If the value of the object has not changed, this should return the same object it was passed.
        /// </returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            Guard.NotNull(() => context, context);
            Guard.NotNull(() => provider, provider);

            tracer.ShieldUI(() =>
            {
                var componentModel = provider.GetService<SComponentModel, IComponentModel>();
                var picker = componentModel.GetService<Func<ISolutionPicker>>()();
                picker.Owner = provider.GetService<SVsUIShell, IVsUIShell>().GetMainWindow();
                picker.RootItem = provider.GetService<ISolution>();
                picker.Title = Resources.ImageUriEditor_PickerTitle;
                picker.EmptyItemsMessage = string.Format(CultureInfo.CurrentCulture, Resources.ImageUriEditor_EmptyItemsMessage, ImageExtension);
                picker.Filter.Kind = ItemKind.Solution | ItemKind.SolutionFolder | ItemKind.Project | ItemKind.Folder | ItemKind.Item;
                picker.Filter.MatchFileExtensions = ImageExtension;
                if (picker.ShowDialog())
                {
                    var item = (IItem)picker.SelectedItem;
                    item.Data.ItemType = BuildAction.Content.ToString();
                    item.Data.IncludeInVSIX = Boolean.TrueString.ToLower(CultureInfo.CurrentCulture);
                    value = item;
                }
            }, Resources.ImageUriEditor_FailedToEdit);

            var converter = context.PropertyDescriptor.Converter;

            if (converter != null && converter.CanConvertTo(context, context.PropertyDescriptor.PropertyType))
            {
                return converter.ConvertTo(context, CultureInfo.CurrentCulture, value, context.PropertyDescriptor.PropertyType);
            }

            return value;
        }

        /// <summary>
        /// Gets the editor style used by the <see cref="UITypeEditor.EditValue(System.IServiceProvider,System.Object)"/> method.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
        /// <returns>
        /// A <see cref="UITypeEditorEditStyle"/> value that indicates the style of editor used by the <see cref="UITypeEditor.EditValue(IServiceProvider,Object)"/> method. If the <see cref="UITypeEditor"/> does not support this method, then <see cref="UITypeEditor.GetEditStyle()"/> will return <see cref="UITypeEditorEditStyle.None"/>.
        /// </returns>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}