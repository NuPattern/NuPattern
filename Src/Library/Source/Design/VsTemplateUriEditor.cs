using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Library.Automation;
using NuPattern.Library.Commands;
using NuPattern.Library.Properties;
using NuPattern.Library.TemplateWizards;
using NuPattern.Reflection;
using NuPattern.Runtime;
using NuPattern.Runtime.Bindings.Design;
using NuPattern.Runtime.UI;
using NuPattern.VisualStudio;
using NuPattern.VisualStudio.Shell;
using NuPattern.VisualStudio.Solution.Templates;

namespace NuPattern.Library.Design
{
    /// <summary>
    /// Representes the editor to change a VsTemplate Uri.
    /// </summary>
    /// <devdoc>
    /// Made internal because this is not for public consumption.
    /// </devdoc>
    internal class VsTemplateUriEditor : UITypeEditor
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<VsTemplateUriEditor>();
        private const string FileExtension = ".vstemplate";

        /// <summary>
        /// Edits the specified object's value using the editor style indicated by the <see cref="UITypeEditor.GetEditStyle()"/> method.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
        /// <param name="provider">An <see cref="IServiceProvider"/> that this editor can use to obtain services.</param>
        /// <param name="value">The object to edit.</param>
        /// <returns>
        /// The new value of the object. If the value of the object has not changed, this should return the same object it was passed.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            Guard.NotNull(() => context, context);
            Guard.NotNull(() => provider, provider);
            Guard.NotNull(() => value, value);

            var uriService = provider.GetService<IFxrUriReferenceService>();

            var templateUri = (value is string) ? (string)value : (string)null;

            tracer.ShieldUI(() =>
            {
                var componentModel = provider.GetService<SComponentModel, IComponentModel>();
                var picker = componentModel.GetService<Func<ISolutionPicker>>()();
                picker.Owner = Application.Current.MainWindow;
                picker.RootItem = provider.GetService<ISolution>();
                picker.Title = Properties.Resources.VsTemplateUriEditor_PickerTitle;
                picker.EmptyItemsMessage = string.Format(CultureInfo.CurrentCulture, Resources.VsTemplateUriEditor_EmptyItemsMessage, FileExtension);
                picker.Filter.Kind = ItemKind.Solution | ItemKind.SolutionFolder | ItemKind.Project | ItemKind.Folder | ItemKind.Item;
                picker.Filter.MatchFileExtensions = FileExtension;
                SetSelectedItem(context, picker, uriService, GetAuthoringTemplateProperty(context));
                if (picker.ShowDialog())
                {
                    // Create and return the Uri for the template
                    var owner = GetPropertyOwner(context);
                    var configurator = new VsTemplateConfigurator(provider);

                    IItem selectedItem;

                    if (!ExtensionMatches(picker.SelectedItem) || picker.SelectedItem.Parent.Kind == ItemKind.Item)
                    {
                        selectedItem = GetTemplateItemFromTT(picker.SelectedItem);
                        //message telling the user to do his magic on his own
                        var messageService = provider.GetService<IUserMessageService>();
                        if (messageService != null)
                        {
                            messageService.ShowWarning(Resources.VsTemplateUriEditor_ReferToGuidance);
                        }
                    }
                    else
                    {
                        selectedItem = picker.SelectedItem.As<IItem>();
                    }

                    // Update the vstemplate
                    var template = configurator.Configure(selectedItem, owner.DisplayName, owner.Description, owner.GetSchemaPathValue());

                    template.RemoveWizardExtension(typeof(InstantiationTemplateWizard));
                    template.RemoveWizardExtension(typeof(ElementReplacementsWizard));

                    if (context.Instance is ITemplateSettings)
                    {
                        // The editor is being used from the template launchpoint, so we add 
                        // the template wizard 
                        template.AddWizardExtension(typeof(InstantiationTemplateWizard));
                        template.SetHidden(!((ITemplateSettings)context.Instance).CreateElementOnUnfold);
                    }
                    else
                    {
                        // The editor is being used from a command, don't show it in Add/Remove Programs
                        template.SetHidden(true);
                    }

                    template.AddWizardExtension(typeof(ElementReplacementsWizard));

                    // Set also the authoring URI value.
                    SetAuthoringUri(context, uriService.CreateUri(selectedItem));

                    templateUri = uriService.CreateUri<IVsTemplate>(template).AbsoluteUri;
                }
            }, Resources.VsTemplateUriEditor_FailedToEdit);

            return templateUri;
        }

        private static bool ExtensionMatches(IItemContainer item)
        {
            return string.Equals(Path.GetExtension(item.PhysicalPath), FileExtension);
        }

        private static IItem GetTemplateItemFromTT(IItemContainer item)
        {
            return ExtensionMatches(item) ? item.As<IItem>() : item.Traverse().First(i => ExtensionMatches(i)).As<IItem>();
        }

        private static IPatternElementSchema GetPropertyOwner(ITypeDescriptorContext context)
        {
            var templateSettings = context.Instance as ITemplateSettings;
            var commandSettings = context.Instance as CommandSettings;

            if (templateSettings != null)
                return templateSettings.Owner;
            else if (commandSettings != null)
                return commandSettings.Owner;

            throw new NotSupportedException(context.Instance.GetType().FullName);
        }

        private static void SetAuthoringUri(ITypeDescriptorContext context, Uri uri)
        {
            var templateSettings = context.Instance as ITemplateSettings;
            var commandSettings = context.Instance as ICommandSettings;

            if (templateSettings != null)
            {
                templateSettings.TemplateAuthoringUri = uri.AbsoluteUri;
            }
            else if (commandSettings != null)
            {
                // Make up the design property descriptor to persist the authoring value.
                var descriptor = new DesignPropertyDescriptor(
                    Reflector<UnfoldVsTemplateCommand>.GetPropertyName(c => c.TemplateAuthoringUri),
                    typeof(string),
                    typeof(CommandSettings),
                    new Attribute[0]);

                var prop = (DesignProperty)descriptor.GetValue(commandSettings);
                prop.SetValue(uri.AbsoluteUri);
            }
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

        private static string GetAuthoringTemplateProperty(ITypeDescriptorContext context)
        {
            var templateSettings = context.Instance as ITemplateSettings;
            var commandSettings = context.Instance as ICommandSettings;

            if (templateSettings != null)
            {
                return templateSettings.TemplateAuthoringUri;
            }
            else if (commandSettings != null)
            {
                // Make up the design property descriptor to persist the authoring value.
                var descriptor = new DesignPropertyDescriptor(
                    Reflector<UnfoldVsTemplateCommand>.GetPropertyName(c => c.TemplateAuthoringUri),
                    typeof(string),
                    typeof(CommandSettings),
                    new Attribute[0]);

                var prop = (DesignProperty)descriptor.GetValue(commandSettings);
                return prop.GetValue().ToString();
            }

            return null;
        }

        private static void SetSelectedItem(ITypeDescriptorContext context, ISolutionPicker picker, IFxrUriReferenceService uriService, string uri)
        {
            if (!string.IsNullOrEmpty(uri))
            {
                var item = uriService.TryResolveUri<IItemContainer>(new Uri(uri));
                if (item != null)
                {
                    picker.SelectedItem = item;
                }
            }
        }
    }
}