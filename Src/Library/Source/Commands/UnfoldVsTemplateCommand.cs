using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TemplateWizard;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Automation;
using NuPattern.Library.Automation.Template;
using NuPattern.Library.Design;
using NuPattern.Library.Properties;
using NuPattern.Runtime;
using NuPattern.Runtime.Bindings;
using NuPattern.Runtime.References;
using NuPattern.VisualStudio;
using NuPattern.VisualStudio.Solution;
using NuPattern.VisualStudio.Solution.Templates;
using Dsl = Microsoft.VisualStudio.Modeling.Design;

namespace NuPattern.Library.Commands
{
    /// <summary>
    /// Command that unfolds a VSTemplate.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "VsTemplate")]
    [DisplayNameResource("UnfoldVsTemplateCommand_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_Automation", typeof(Resources))]
    [DescriptionResource("UnfoldVsTemplateCommand_Description", typeof(Resources))]
    [CLSCompliant(false)]
    public partial class UnfoldVsTemplateCommand : FeatureCommand
    {
        private const bool DefaultSyncName = false;
        private const bool DefaultSanitizeName = true;
        private const string DefaultTag = "";
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<UnfoldVsTemplateCommand>();

        /// <summary>
        /// Creates a new instance of the <see cref="UnfoldVsTemplateCommand"/> class.
        /// </summary>
        public UnfoldVsTemplateCommand()
        {
            this.SyncName = DefaultSyncName;
            this.SanitizeName = DefaultSanitizeName;
        }

        /// <summary>
        /// Gets or sets the value of TemplateUri during authoring time.
        /// </summary>
        [Browsable(false)]
        public virtual Uri TemplateAuthoringUri
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value of TemplateUri domain property.
        /// The project or item template to unfold, from the current project.
        /// </summary>
        [Required]
        [DesignOnly(true)]
        [Editor(typeof(VsTemplateUriEditor), typeof(UITypeEditor))]
        [Dsl.DisplayNameResource("NuPattern.Library.Automation.TemplateSettings/TemplateUri.DisplayName", typeof(LibraryDomainModel), "NuPattern.Library.GeneratedCode.DomainModelResx.gen")]
        [Dsl.DescriptionResource("NuPattern.Library.Automation.TemplateSettings/TemplateUri.Description", typeof(LibraryDomainModel), "NuPattern.Library.GeneratedCode.DomainModelResx.gen")]
        public virtual Uri TemplateUri
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the target path.
        /// </summary>
        [Dsl.DisplayNameResource("NuPattern.Library.Automation.TemplateSettings/RawTargetPath.DisplayName", typeof(LibraryDomainModel), "NuPattern.Library.GeneratedCode.DomainModelResx.gen")]
        [Dsl.DescriptionResource("NuPattern.Library.Automation.TemplateSettings/RawTargetPath.Description", typeof(LibraryDomainModel), "NuPattern.Library.GeneratedCode.DomainModelResx.gen")]
        public virtual string TargetPath
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the target.
        /// </summary>
        [DefaultValue("{InstanceName}")]
        [Dsl.DisplayNameResource("NuPattern.Library.Automation.TemplateSettings/RawTargetFileName.DisplayName", typeof(LibraryDomainModel), "NuPattern.Library.GeneratedCode.DomainModelResx.gen")]
        [Dsl.DescriptionResource("NuPattern.Library.Automation.TemplateSettings/RawTargetFileName.Description", typeof(LibraryDomainModel), "NuPattern.Library.GeneratedCode.DomainModelResx.gen")]
        public virtual string TargetFileName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the whether to sync names or not.
        /// </summary>
        [DefaultValue(DefaultSyncName)]
        [DesignOnly(true)]
        [Dsl.DisplayNameResource("NuPattern.Library.Automation.TemplateSettings/SyncName.DisplayName", typeof(LibraryDomainModel), "NuPattern.Library.GeneratedCode.DomainModelResx.gen")]
        [Dsl.DescriptionResource("NuPattern.Library.Automation.TemplateSettings/SyncName.Description", typeof(LibraryDomainModel), "NuPattern.Library.GeneratedCode.DomainModelResx.gen")]
        public virtual bool SyncName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the whether to sanitize names or not.
        /// </summary>
        [DefaultValue(DefaultSanitizeName)]
        [DesignOnly(true)]
        [Dsl.DisplayNameResource("NuPattern.Library.Automation.TemplateSettings/SanitizeName.DisplayName", typeof(LibraryDomainModel), "NuPattern.Library.GeneratedCode.DomainModelResx.gen")]
        [Dsl.DescriptionResource("NuPattern.Library.Automation.TemplateSettings/SanitizeName.Description", typeof(LibraryDomainModel), "NuPattern.Library.GeneratedCode.DomainModelResx.gen")]
        public virtual bool SanitizeName
        {
            get;
            set;
        }

        /// <summary>
        /// An optional value to atg the generated reference for the generated file.
        /// </summary>
        [Dsl.DisplayNameResource("NuPattern.Library.Automation.TemplateSettings/Tag.DisplayName", typeof(LibraryDomainModel), "NuPattern.Library.GeneratedCode.DomainModelResx.gen")]
        [Dsl.DescriptionResource("NuPattern.Library.Automation.TemplateSettings/Tag.Description", typeof(LibraryDomainModel), "NuPattern.Library.GeneratedCode.DomainModelResx.gen")]
        [DefaultValue(DefaultTag)]
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets the service that resolves templates.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public virtual IUriReferenceService UriService
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the current solution.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public virtual ISolution Solution
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the current element.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IProductElement CurrentElement
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the generated item in the solution
        /// </summary>
        protected virtual IItemContainer GeneratedItem
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the VS service proider
        /// </summary>
        [Import(typeof(SVsServiceProvider))]
        public IServiceProvider ServiceProvider
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the command automation that instantiated this command.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IAutomationExtension<ICommandSettings> CommandAutomation
        {
            get;
            set;
        }

        /// <summary>
        /// Unfolds the configured template for the element.
        /// </summary>
        public override void Execute()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.UnfoldVsTemplateCommand_TraceInitial,
                this.CurrentElement.InstanceName, this.TemplateUri, this.TargetPath, this.TargetFileName, this.SanitizeName, this.SyncName);

            tracer.TraceData(TraceEventType.Verbose, Resources.UnfoldVsTemplateCommand_StartedEvent, this.CurrentElement);

            var tag = new ReferenceTag
            {
                SyncNames = this.SyncName,
                TargetFileName = this.TargetFileName
            };

            using (var scope = new UnfoldScope(this.CommandAutomation, tag, this.TemplateUri.AbsoluteUri))
            {
                this.GeneratedItem = UnfoldTemplate(this.Solution, this.UriService, this.ServiceProvider, this.CurrentElement,
                    new UnfoldVsTemplateSettings
                    {
                        TemplateUri = this.TemplateUri.AbsoluteUri,
                        TargetFileName = this.TargetFileName,
                        TargetPath = this.TargetPath,
                        SyncName = this.SyncName,
                        SanitizeName = this.SanitizeName,
                        Tag = this.Tag,
                        Id = this.CommandAutomation.Settings.Id,
                    }, false);
            }
        }

        /// <summary>
        /// Unfolds the specified template.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Dispose is avoided when events should not fire at all")]
        internal static IItemContainer UnfoldTemplate(ISolution solution, IUriReferenceService uriService, IServiceProvider serviceProvider, IProductElement owner, UnfoldVsTemplateSettings settings, bool fromWizard)
        {
            var eventScope = new StoreEventBufferingScope();
            try
            {
                Guard.NotNull(() => solution, solution);
                Guard.NotNull(() => owner, owner);
                Guard.NotNull(() => settings, settings);

                var pathHelper = new UnfoldPathHelper(solution);
                var templateUri = new Uri(settings.TemplateUri);

                // Resolve the designtime template
                tracer.TraceVerbose(
                    Resources.UnfoldVsTemplateCommand_TraceResolvingTemplateUri, owner, settings.TemplateUri.ToString());
                var template = uriService.TryResolveUri<ITemplate>(templateUri);
                if (template == null)
                {
                    throw new FileNotFoundException(
                        string.Format(CultureInfo.CurrentCulture,
                        Resources.UnfoldVsTemplateCommand_ErrorTemplateNotFound, templateUri), settings.TemplateUri.ToString());
                }

                // Resolve the vstemplate
                tracer.TraceVerbose(
                    Resources.UnfoldVsTemplateCommand_TraceResolvingVsTemplateUri, owner, templateUri);
                var vsTemplate = uriService.ResolveUri<IVsTemplate>(templateUri);

                // Get the resolved instance name for the unfolded item
                var unfoldResolver = new UnfoldParentResolver(solution, uriService, owner, vsTemplate);
                unfoldResolver.ResolveParent(settings.TargetPath, settings.TargetFileName);
                var instanceName = unfoldResolver.FileName;
                if (settings.SanitizeName)
                {
                    instanceName = DataFormats.MakePreferredSolutionItemName(instanceName);
                }

                // Ensure name is unique (on disk)
                var solutionItemName = pathHelper.GetUniqueName(instanceName, vsTemplate, unfoldResolver.ParentItem);

                //TODO: We need to close the existing solution (if any) if template is a ProjectGroup 
                // if (vsTemplate.Type == VsTemplateType.ProjectGroup).
                // Otherwise this will fail the unfold

                // Unfold the template
                var generatedItem = template.Unfold(unfoldResolver.ResolveExtension(solutionItemName), unfoldResolver.ParentItem);
                eventScope.Dispose();

                // Perhaps the template unfolded multiple items and none was identifed as the primary
                // (such as in a multi-item item template, with no non-fixed named items)
                if (generatedItem != null)
                {
                    // Prompt user to update element instance name (on name collision) if he is synching names, 
                    // it doesn't make sense to correlate them otherwise
                    if (settings.SyncName)
                    {
                        if (!instanceName.Equals(solutionItemName, StringComparison.OrdinalIgnoreCase))
                        {
                            if (serviceProvider != null)
                            {
                                var shellService = serviceProvider.GetService<IVsUIShell>();
                                if (shellService != null)
                                {
                                    var result = shellService.ShowPrompt(
                                        Resources.UnfoldVsTemplateCommand_PromptToSyncNameTitle,
                                        string.Format(CultureInfo.CurrentCulture,
                                            Resources.UnfoldVsTemplateCommand_PromptToSyncName,
                                            instanceName, solutionItemName));
                                    if (result)
                                    {
                                        owner.InstanceName = solutionItemName;
                                    }
                                }
                                else
                                {
                                    owner.InstanceName = solutionItemName;
                                }
                            }
                            else
                            {
                                owner.InstanceName = solutionItemName;
                            }
                        }
                    }

                    if (!fromWizard)
                    {
                        tracer.TraceInformation(
                            Resources.UnfoldVsTemplateCommand_TraceAddReference, owner);

                        SolutionArtifactLinkReference
                            .AddReference(owner, uriService.CreateUri(generatedItem))
                            .Tag = BindingSerializer.Serialize(new ReferenceTag
                        {
                            Tag = settings.Tag ?? string.Empty,
                            SyncNames = settings.SyncName,
                            TargetFileName = settings.TargetFileName
                        });
                    }
                }
                return generatedItem;
            }
            catch (WizardBackoutException) //cancel the unfold if wizard backout
            {
                tracer.TraceInformation(
                    Resources.UnfoldVsTemplateCommand_TraceWizardCancelled);
                owner.Delete();
                eventScope.Dispose();
                return null;
            }
            catch (COMException comEx)
            {
                tracer.TraceError(
                    comEx, Resources.UnfoldVsTemplateCommand_TraceCOMException, owner.InstanceName, settings.TemplateUri);
                owner.Delete();
                eventScope.Dispose();
                throw;
            }
            catch (OperationCanceledException)
            {
                // This exception can be throw explicitly by author code 
                // that wishes to cancel execution, with a friendly user 
                // message, so we can pass this on as-is.
                throw;
            }
            catch (Exception ex) //cancel the unfold if another unexpected exception happened
            {
                tracer.TraceInformation(
                    Resources.UnfoldVsTemplateCommand_TraceUnexpectedException, owner.InstanceName, settings.TemplateUri);
                owner.Delete();
                eventScope.Dispose();

                throw new OperationCanceledException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.UnfoldVsTemplateCommand_UnexpectedException,
                    settings.TemplateUri, owner.InstanceName, ex.Message));
            }
        }

        /// <summary>
        /// Defines the settings that control the settings for unfolding a vstemplate
        /// </summary>
        internal class UnfoldVsTemplateSettings
        {
            internal IPatternElementSchema OwnerElement
            {
                get;
                set;
            }
            internal IAutomationSettingsSchema SettingsElement
            {
                get;
                set;
            }
            internal string TemplateAuthoringUri
            {
                get;
                set;
            }
            internal string TemplateUri
            {
                get;
                set;
            }
            internal string TargetFileName
            {
                get;
                set;
            }
            internal string TargetPath
            {
                get;
                set;
            }
            internal bool SyncName
            {
                get;
                set;
            }
            internal bool SanitizeName
            {
                get;
                set;
            }
            internal string Tag
            {
                get;
                set;
            }
            internal Guid Id
            {
                get;
                set;
            }

            public bool CreateElementOnUnfold
            {
                get;
                set;
            }
        }
    }
}
