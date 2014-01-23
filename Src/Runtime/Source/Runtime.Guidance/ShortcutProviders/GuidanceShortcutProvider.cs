using System.ComponentModel.Composition;
using NuPattern.Runtime.Guidance.Properties;
using NuPattern.Runtime.Shortcuts;
using NuPattern.VisualStudio;

namespace NuPattern.Runtime.Guidance.ShortcutProviders
{
    /// <summary>
    /// Provides shortcuts for guidance
    /// </summary>
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IShortcutProvider))]
    internal class GuidanceShortcutProvider : IShortcutProvider<GuidanceShortcut>
    {
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
            return new GuidanceShortcut(shortcut);
        }

        /// <summary>
        /// Gets the <see cref="IUserMessageService"/>
        /// </summary>
        [Import]
        internal IUserMessageService MessageService { get; set; }

        /// <summary>
        /// Gets the <see cref="IGuidanceProvider"/> 
        /// </summary>
        [Import]
        internal IGuidanceProvider GuidanceProvider { get; set; }

        /// <summary>
        /// Executes the shortcut.
        /// </summary>
        /// <param name="instance">An instance of the pattern shortcut</param>
        public void Execute(GuidanceShortcut instance)
        {
            var instanceName = instance.InstanceName;
            var extensionId = instance.GuidanceExtensionId;
            var forceCreate = instance.AlwaysCreate;

            if (!string.IsNullOrEmpty(extensionId))
            {
                if (!string.IsNullOrEmpty(instanceName))
                {
                    var instanceExists = this.GuidanceProvider.InstanceExists(extensionId, instanceName);
                    var isRegistered = this.GuidanceProvider.IsRegistered(extensionId);

                    if (isRegistered)
                    {
                        if (instanceExists)
                        {
                            if (forceCreate)
                            {
                                instanceName = this.GuidanceProvider.GetUniqueInstanceName(instanceName);
                                this.GuidanceProvider.CreateInstance(extensionId, instanceName);
                            }
                            else
                            {
                                this.GuidanceProvider.ActivateInstance(instanceName);
                            }
                        }
                        else
                        {
                            this.GuidanceProvider.CreateInstance(extensionId, instanceName);
                        }

                    }
                    else
                    {
                        if (instanceExists)
                        {
                            if (forceCreate)
                            {
                                this.MessageService.ShowError(
                                    Resources.GuidanceShortcutProvider_NotRegistered.FormatWith(extensionId));
                            }
                            else
                            {
                                this.GuidanceProvider.ActivateInstance(instanceName);
                            }
                        }
                        else
                        {
                            if (forceCreate)
                            {
                                this.MessageService.ShowError(
                                    Resources.GuidanceShortcutProvider_NotRegistered.FormatWith(extensionId));
                            }
                            else
                            {
                                this.MessageService.ShowError(
                                    Resources.GuidanceShortcutProvider_NotRegisteredInstanceNotExist.FormatWith(
                                        instanceName, extensionId));
                            }
                        }
                    }
                }
                else
                {
                    var defaultInstanceName = this.GuidanceProvider.GetDefaultInstanceName(extensionId);
                    var defaultInstanceExists = this.GuidanceProvider.InstanceExists(extensionId, defaultInstanceName);
                    var isRegistered = this.GuidanceProvider.IsRegistered(extensionId);

                    if (isRegistered)
                    {
                        if (defaultInstanceExists)
                        {
                            if (forceCreate)
                            {
                                instanceName = this.GuidanceProvider.GetUniqueInstanceName(defaultInstanceName);
                                this.GuidanceProvider.CreateInstance(extensionId, instanceName);
                            }
                            else
                            {
                                this.GuidanceProvider.ActivateInstance(defaultInstanceName);
                            }
                        }
                        else
                        {
                            this.GuidanceProvider.CreateInstance(extensionId, defaultInstanceName);
                        }
                    }
                    else
                    {
                        if (defaultInstanceExists)
                        {
                            if (forceCreate)
                            {
                                this.MessageService.ShowError(Resources.GuidanceShortcutProvider_NotRegistered.FormatWith(extensionId));
                            }
                            else
                            {
                                this.GuidanceProvider.ActivateInstance(defaultInstanceName);
                            }
                        }
                        else
                        {
                            if (forceCreate)
                            {
                                this.MessageService.ShowError(Resources.GuidanceShortcutProvider_NotRegistered.FormatWith(extensionId));
                            }
                            else
                            {
                                this.MessageService.ShowError(Resources.GuidanceShortcutProvider_NotRegisteredNoInstance.FormatWith(extensionId));
                            }
                        }
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(instanceName))
                {
                    var instanceExists = this.GuidanceProvider.InstanceExists(extensionId, instanceName);
                    if (instanceExists)
                    {
                        if (forceCreate)
                        {
                            this.MessageService.ShowError(Resources.GuidanceShortcutProvider_RegistrationNotExist);
                        }
                        else
                        {
                            this.GuidanceProvider.ActivateInstance(instanceName);
                        }
                    }
                    else
                    {
                        if (forceCreate)
                        {
                            this.MessageService.ShowError(Resources.GuidanceShortcutProvider_RegistrationNotExist);
                        }
                        else
                        {
                            this.MessageService.ShowError(Resources.GuidanceShortcutProvider_RegistrationNotExistForInstance.FormatWith(instanceName));
                        }
                    }
                }
                else
                {
                    this.MessageService.ShowError(Resources.GuidanceShortcutProvider_InvalidParameters);
                }
            }
        }

        /// <summary>
        /// Creates a new shortcut
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public IShortcut CreateShortcut(GuidanceShortcut instance)
        {
            return instance;
        }
    }
}
