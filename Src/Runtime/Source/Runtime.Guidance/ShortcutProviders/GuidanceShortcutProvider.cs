using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Shell;
using NuPattern.Runtime.Guidance.Properties;
using NuPattern.Runtime.Shortcuts;
using NuPattern.VisualStudio;

namespace NuPattern.Runtime.Guidance.ShortcutProviders
{
    /// <summary>
    /// Provides shortcuts for guidance
    /// </summary>
    [Export(typeof(IShortcutProvider))]
    internal class GuidanceShortcutProvider : IShortcutProvider<GuidanceShortcut>
    {
        /// <summary>
        /// Gets the <see cref="IServiceProvider"/>
        /// </summary>
        [Import(typeof(SVsServiceProvider))]
        internal IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Gets the <see cref="IGuidanceManager"/>
        /// </summary>
        [Import]
        internal IGuidanceManager GuidanceManager { get; set; }

        /// <summary>
        /// Gets the <see cref="IUserMessageService"/>
        /// </summary>
        [Import]
        internal IUserMessageService MessageService { get; set; }

        /// <summary>
        /// Gets the type of the shortcut.
        /// </summary>
        public string Type
        {
            get { return GuidanceShortcut.ShortcutType; }
        }

        /// <summary>
        /// Resolves the shortcut to an instance of the pattern shortcut.
        /// </summary>
        /// <param name="shortcut">The shortcut to resolve</param>
        /// <returns></returns>
        public GuidanceShortcut ResolveShortcut(IShortcut shortcut)
        {
            Guard.NotNull(() => shortcut, shortcut);

            return new GuidanceShortcut(shortcut);
        }

        /// <summary>
        /// Executes the shortcut.
        /// </summary>
        /// <param name="instance">An instance of the pattern shortcut</param>
        public GuidanceShortcut Execute(GuidanceShortcut instance)
        {
            Guard.NotNull(() => instance, instance);

            // Execute the shortcut
            switch (instance.CommandType)
            {
                case GuidanceShortcutCommandType.Activation:
                    if (!string.IsNullOrEmpty(instance.InstanceName))
                    {
                        // Locate existing workflow
                        var workflow = this.GuidanceManager.InstantiatedGuidanceExtensions.FirstOrDefault(ge => ge.InstanceName.Equals(instance.InstanceName, StringComparison.OrdinalIgnoreCase));
                        if (workflow != null)
                        {
                            this.GuidanceManager.ActivateGuidanceInstance(this.ServiceProvider, workflow);
                        }
                        else
                        {
                            this.MessageService.ShowError(
                                string.Format(CultureInfo.CurrentCulture, Resources.GuidanceShortcutProvider_ActivateInstanceNotFound, instance.InstanceName));
                        }
                    }
                    else if (!string.IsNullOrEmpty(instance.GuidanceExtensionId))
                    {
                        if (this.GuidanceManager.IsInstalled(instance.GuidanceExtensionId))
                        {
                            // Create a (single instance) of shared workflow
                            this.GuidanceManager.ActivateSharedGuidanceWorkflow(this.ServiceProvider, instance.GuidanceExtensionId);
                        }
                        else
                        {
                            this.MessageService.ShowError(
                                string.Format(CultureInfo.CurrentCulture, Resources.GuidanceShortcutProvider_ActivateSharedExtensionNotFound, instance.GuidanceExtensionId));
                        }
                    }
                    else
                    {
                        this.MessageService.ShowError(Resources.GuidanceShortcutProvider_InvalidParameters);
                    }
                    break;

                case GuidanceShortcutCommandType.Instantiation:
                    if (!string.IsNullOrEmpty(instance.GuidanceExtensionId))
                    {
                        var registration = this.GuidanceManager.FindRegistration(instance.GuidanceExtensionId);
                        if (registration != null)
                        {
                            //Ensure unique instance name
                            var instanceName = this.GuidanceManager.GetUniqueInstanceName(registration.DefaultName);
                            if (!string.IsNullOrEmpty(instance.DefaultName))
                            {
                                instanceName = this.GuidanceManager.GetUniqueInstanceName(instance.DefaultName);
                            }

                            // Create a new instance of guidance 
                            this.GuidanceManager.InstantiateGuidanceInstance(this.ServiceProvider, instance.GuidanceExtensionId, instanceName);
                        }
                        else
                        {
                            this.MessageService.ShowError(
                                string.Format(CultureInfo.CurrentCulture, Resources.GuidanceShortcutProvider_InstantiateExtensionNotFound, instance.GuidanceExtensionId));
                        }
                    }
                    else
                    {
                        this.MessageService.ShowError(Resources.GuidanceShortcutProvider_InvalidParameters);
                    }
                    break;

                default:
                    this.MessageService.ShowError(Resources.GuidanceShortcutProvider_InvalidParameters);
                    break;
            }

            return null;
        }
    }
}
