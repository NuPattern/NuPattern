using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Library.Automation;
using NuPattern.Library.Commands;
using NuPattern.Library.Properties;
using NuPattern.Reflection;
using NuPattern.Runtime;
using NuPattern.Runtime.Bindings.Design;
using NuPattern.VisualStudio;
using NuPattern.VisualStudio.Extensions;
using NuPattern.VisualStudio.Shell;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Library.TypeEditors
{
    /// <summary>
    /// An editor that allows selection of a T4 template inside a VSIX project, 
    /// that will be converted to a uri.
    /// </summary>
    internal class TextTemplateUriEditor : UITypeEditor
    {
        private const string FileExtension = ".tt;.t4";
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
                var uriService = componentModel.GetService<IFxrUriReferenceService>();

                var picker = componentModel.GetService<Func<ISolutionPicker>>()();
                picker.Owner = provider.GetService<SVsUIShell, IVsUIShell>().GetMainWindow();
                picker.RootItem = provider.GetService<ISolution>();
                picker.Title = Resources.TextTemplateUriEditor_Title;
                picker.EmptyItemsMessage = string.Format(CultureInfo.CurrentCulture, Resources.TextTemplateUriEditor_EmptyItemsMessage, FileExtension);
                picker.Filter.Kind = ItemKind.Solution | ItemKind.SolutionFolder | ItemKind.Project | ItemKind.Folder | ItemKind.Item;
                picker.Filter.MatchFileExtensions = FileExtension;
                SetSelectedItem(context, picker, uriService, GetAuthoringTemplateProperty(context));
                if (picker.ShowDialog())
                {
                    templateUri = BuildUri(picker.SelectedItem);

                    // Set item properties automatically.
                    var item = (IItem)picker.SelectedItem;
                    item.Data.ItemType = BuildAction.Content.ToString();
                    item.Data.IncludeInVSIX = (!IsIncludeInVSIXAs(item)).ToString().ToLowerInvariant();

                    SetAuthoringUri(context, uriService.CreateUri(picker.SelectedItem));
                }
            }, Resources.TextTemplateUriEditor_FailedToEdit);

            return templateUri;
        }

        /// <summary>
        /// Builds the URI for the given template based on the containing project VSIX manifest identifier.
        /// </summary>
        internal static Uri BuildUri(IItemContainer selectedItem)
        {
            var manifest = selectedItem.GetToolkitManifest();
            var owningProject = selectedItem.Traverse(x => x.Parent, item => item.Kind == ItemKind.Project);

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

            var path = GetLogicalPath(selectedItem).Replace(owningProject.GetLogicalPath(), string.Empty);

            // Use alternative name if IncludeInVSIXAs defined
            var templateItem = (IItem)selectedItem;
            if (IsIncludeInVSIXAs(templateItem))
            {
                path = Path.Combine(Path.GetDirectoryName(path), templateItem.Data.IncludeInVSIXAs);
            }

            return new Uri(new Uri(TextTemplateUri.UriHostPrefix), new Uri(vsixId + path, UriKind.Relative));
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

        /// <summary>
        /// Gets the editor style used by the <see cref="M:System.Drawing.Design.UITypeEditor.EditValue(System.IServiceProvider,System.Object)"/> method.
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        private static void SetAuthoringUri(ITypeDescriptorContext context, Uri uri)
        {
            var prop = GetAuthoringTemplateProperty(context);
            if (prop != null)
            {
                prop.SetValue(uri.AbsoluteUri);
            }
        }

        private static DesignProperty GetAuthoringTemplateProperty(ITypeDescriptorContext context)
        {
            var commandSettings = context.Instance as ICommandSettings;
            if (commandSettings != null)
            {
                // Make up the design property descriptor to access the authoring value.
                var descriptor = new DesignPropertyDescriptor(
                    Reflector<GenerateModelingCodeCommand>.GetPropertyName(c => c.TemplateAuthoringUri),
                    typeof(string),
                    typeof(CommandSettings),
                    new Attribute[0]);

                return (DesignProperty)descriptor.GetValue(commandSettings);
            }

            return null;
        }

        private static bool IsIncludeInVSIXAs(IItem selectedItem)
        {
            return (selectedItem != null
                && (!String.IsNullOrEmpty(selectedItem.Data.IncludeInVSIXAs)));
        }

        private static void SetSelectedItem(ITypeDescriptorContext context, ISolutionPicker picker, IFxrUriReferenceService uriService, DesignProperty prop)
        {
            if (prop != null)
            {
                var value = prop.GetValue().ToString();
                if (!String.IsNullOrEmpty(value))
                {
                    var item = uriService.TryResolveUri<IItemContainer>(new Uri(value));
                    if (item != null)
                    {
                        picker.SelectedItem = item;
                    }
                }
            }
        }
    }
}
