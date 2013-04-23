using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using NuPattern.VisualStudio.Properties;
using NuPattern.VisualStudio.Solution.Hierarchy;

namespace NuPattern.VisualStudio.Solution
{
    internal class VsProject : HierarchyItem, IProject
    {
        private const string BuildConfigurationSeparator = @"|";

        // For testing
        internal VsProject() { }

        public VsProject(IServiceProvider serviceProvider, IHierarchyNode node)
            : base(serviceProvider, node)
        {
            this.Data = new AllConfigurationsProperties(this, userData: false);
            this.UserData = new AllConfigurationsProperties(this, userData: true);
        }

        public dynamic Data { get; private set; }
        public dynamic UserData { get; private set; }

        public override ItemKind Kind
        {
            get { return ItemKind.Project; }
        }

        public override string Id
        {
            get { return this.HierarchyNode.ProjectGuid.ToString(); }
        }

        public override string PhysicalPath
        {
            get
            {
                var dteProject = this.ExtenderObject as EnvDTE.Project;
                return dteProject.FullName;
            }
        }

        public IFolder CreateFolder(string name)
        {
            if (this.Items.Where(i => i.Name == name).FirstOrDefault() != null)
                throw new ArgumentException(String.Format(
                    CultureInfo.CurrentCulture, Resources.VsItem_DuplicateItemName, this.Name, name));

            var project = (Project)base.ExtenderObject;

            project.ProjectItems.AddFolder(name, EnvDTE.Constants.vsProjectItemKindPhysicalFolder);

            return this.Items
                .OfType<IFolder>()
                .Where(folder => folder.Name == name)
                .Single();
        }

        /// <summary>
        /// implements IFolderContainer.Folders, returns the list of all solution folders in this container
        /// </summary>
        public IEnumerable<IFolder> Folders
        {
            get { return this.Items.OfType<IFolder>(); }
        }

        internal class AllConfigurationsProperties : DynamicObject
        {
            private IProject project;
            private _PersistStorageType storageType;
            private bool userData;

            public AllConfigurationsProperties(IProject project, bool userData)
            {
                this.project = project;
                this.userData = userData;
                this.storageType = userData ? _PersistStorageType.PST_USER_FILE : _PersistStorageType.PST_PROJECT_FILE;
            }

            private static object GetPropertyValue(Property prop)
            {
                try
                {
                    return prop.Value;
                }
                catch
                {
                    return string.Empty;
                }
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                string value = null;
                result = null;

                // First try automation properties.
                var project = this.project.As<Project>();
                if (project != null)
                {
                    Property property;
                    try
                    {
                        property = project.Properties.Item(binder.Name);
                    }
                    catch (ArgumentException)
                    {
                        property = null;
                    }

                    if (property != null)
                    {
                        result = property.Value;
                        return true;
                    }
                }

                // Next go custom MSBuild properties.
                var storage = this.project.As<IVsHierarchy>() as IVsBuildPropertyStorage;
                if (storage != null)
                {
                    foreach (string configuration in (IEnumerable)this.project.As<EnvDTE.Project>().ConfigurationManager.ConfigurationRowNames)
                    {
                        foreach (string platform in (IEnumerable)this.project.As<EnvDTE.Project>().ConfigurationManager.PlatformNames)
                        {
                            // Remove the whitespace from platform (Any CPU becomes AnyCPU)
                            storage.GetPropertyValue(binder.Name, configuration + BuildConfigurationSeparator + platform.Replace(@" ", string.Empty), (uint)storageType, out value);
                            if (!string.IsNullOrEmpty(value))
                            {
                                result = value;
                                return true;
                            }
                        }
                    }
                }

                // Do value conversion if it's serializable.
                result = value;
                return true;
            }

            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                // First try automation properties.
                var project = this.project.As<Project>();
                if (project != null)
                {
                    Property property;
                    try
                    {
                        property = project.Properties.Item(binder.Name);
                    }
                    catch (ArgumentException)
                    {
                        property = null;
                    }

                    if (property != null)
                    {
                        property.Value = value;
                        return true;
                    }
                }

                // Next go custom MSBuild properties.
                var storage = this.project.As<IVsHierarchy>() as IVsBuildPropertyStorage;
                if (storage != null)
                {
                    foreach (string configuration in (IEnumerable)this.project.As<EnvDTE.Project>().ConfigurationManager.ConfigurationRowNames)
                    {
                        foreach (string platform in (IEnumerable)this.project.As<EnvDTE.Project>().ConfigurationManager.PlatformNames)
                        {
                            ErrorHandler.Succeeded(storage.SetPropertyValue(binder.Name,
                                // Remove the whitespace from platform (Any CPU becomes AnyCPU)
                                configuration + BuildConfigurationSeparator + platform.Replace(@" ", string.Empty), (uint)storageType, value.ToString()));
                        }
                    }

                    return true;
                }

                return false;
            }

            public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
            {
                var properties = GetIndexedProperties(indexes);
                if (properties != null)
                {
                    result = properties;
                    return true;
                }

                return base.TryGetIndex(binder, indexes, out result);
            }

            private dynamic GetIndexedProperties(object[] indexes)
            {
                if (indexes.Length == 2)
                {
                    if (indexes[0] is ProjectConfiguration && indexes[1] is ProjectPlatform)
                    {
                        return new SpecificConfigurationAndPlatformProperties(this.project,
                            indexes[0] + BuildConfigurationSeparator + indexes[1], userData);
                    }
                }
                else if (indexes.Length == 1)
                {
                    if (indexes[0] is ProjectConfiguration)
                    {
                        return new SpecificConfigurationProperties(this.project, indexes[0].ToString(), userData);
                    }
                    else if (indexes[0] is ProjectPlatform)
                    {
                        return new SpecificPlatformProperties(this.project, indexes[0].ToString(), userData);
                    }
                    else if (indexes[0] is string)
                    {
                        return new SpecificConfigurationAndPlatformProperties(this.project, (string)indexes[0], userData);
                    }
                }

                return null;
            }
        }

        internal class SpecificConfigurationAndPlatformProperties : DynamicObject
        {
            private IProject project;
            private string configurationAndPlatform;
            private _PersistStorageType storageType;

            public SpecificConfigurationAndPlatformProperties(IProject project, string configurationAndPlatform, bool userData)
            {
                this.project = project;
                this.configurationAndPlatform = configurationAndPlatform;
                this.storageType = userData ? _PersistStorageType.PST_USER_FILE : _PersistStorageType.PST_PROJECT_FILE;
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                string value = null;

                var storage = this.project.As<IVsHierarchy>() as IVsBuildPropertyStorage;
                if (storage != null)
                {
                    storage.GetPropertyValue(binder.Name, configurationAndPlatform, (uint)storageType, out value);
                }

                // Do value conversion if it's serializable.
                result = value;
                return true;
            }

            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                var storage = this.project.As<IVsHierarchy>() as IVsBuildPropertyStorage;
                if (storage != null)
                {
                    return ErrorHandler.Succeeded(
                        storage.SetPropertyValue(binder.Name, configurationAndPlatform, (uint)storageType, value.ToString()));
                }

                return false;
            }
        }

        internal class SpecificPlatformProperties : DynamicObject
        {
            private IProject project;
            private string platform;
            private _PersistStorageType storageType;

            public SpecificPlatformProperties(IProject project, string platform, bool userData)
            {
                this.project = project;
                this.platform = platform;
                this.storageType = userData ? _PersistStorageType.PST_USER_FILE : _PersistStorageType.PST_PROJECT_FILE;
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                string value = null;
                result = null;

                var storage = this.project.As<IVsHierarchy>() as IVsBuildPropertyStorage;
                if (storage != null)
                {
                    foreach (string configuration in (IEnumerable)this.project.As<EnvDTE.Project>().ConfigurationManager.ConfigurationRowNames)
                    {
                        storage.GetPropertyValue(binder.Name, configuration + BuildConfigurationSeparator + platform, (uint)storageType, out value);
                        if (!string.IsNullOrEmpty(value))
                            return true;
                    }
                }

                // Do value conversion if it's serializable.
                result = value;
                return true;
            }

            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                var storage = this.project.As<IVsHierarchy>() as IVsBuildPropertyStorage;
                if (storage != null)
                {
                    foreach (string configuration in (IEnumerable)this.project.As<EnvDTE.Project>().ConfigurationManager.ConfigurationRowNames)
                    {
                        ErrorHandler.Succeeded(storage.SetPropertyValue(binder.Name,
                            configuration + BuildConfigurationSeparator + platform, (uint)storageType, value.ToString()));
                    }

                    return true;
                }

                return false;
            }
        }

        internal class SpecificConfigurationProperties : DynamicObject
        {
            private IProject project;
            private string configuration;
            private _PersistStorageType storageType;

            public SpecificConfigurationProperties(IProject project, string configuration, bool userData)
            {
                this.project = project;
                this.configuration = configuration;
                this.storageType = userData ? _PersistStorageType.PST_USER_FILE : _PersistStorageType.PST_PROJECT_FILE;
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                string value = null;
                result = null;

                var storage = this.project.As<IVsHierarchy>() as IVsBuildPropertyStorage;
                if (storage != null)
                {
                    foreach (string platform in (IEnumerable)this.project.As<EnvDTE.Project>().ConfigurationManager.PlatformNames)
                    {
                        storage.GetPropertyValue(binder.Name, configuration + BuildConfigurationSeparator + platform, (uint)storageType, out value);
                        if (!string.IsNullOrEmpty(value))
                            return true;
                    }
                }

                // Do value conversion if it's serializable.
                result = value;
                return true;
            }

            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                var storage = this.project.As<IVsHierarchy>() as IVsBuildPropertyStorage;
                if (storage != null)
                {
                    foreach (string platform in (IEnumerable)this.project.As<EnvDTE.Project>().ConfigurationManager.PlatformNames)
                    {
                        ErrorHandler.Succeeded(storage.SetPropertyValue(binder.Name,
                            configuration + BuildConfigurationSeparator + platform, (uint)storageType, value.ToString()));
                    }

                    return true;
                }

                return false;
            }
        }
    }
}
