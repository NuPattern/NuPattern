using System;
using System.Collections;
using System.Dynamic;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace NuPattern.VisualStudio.Solution
{
    partial class VsProject
    {
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

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                return TryGetMember(binder.Name, out result);
            }

            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                return TrySetMember(binder.Name, value);
            }

            private bool TryGetMember(string memberName, out object result)
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
                        property = project.Properties.Item(memberName);
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
                            storage.GetPropertyValue(memberName, configuration + BuildConfigurationSeparator + platform.Replace(@" ", string.Empty), (uint)storageType, out value);
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

            private bool TrySetMember(string memberName, object value)
            {
                // First try automation properties.
                var project = this.project.As<Project>();
                if (project != null)
                {
                    Property property;
                    try
                    {
                        property = project.Properties.Item(memberName);
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
                            ErrorHandler.Succeeded(storage.SetPropertyValue(memberName,
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

            public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
            {
                // Only direct properties can be set via indexing.
                if (indexes.Length == 1 && indexes[0] is string)
                {
                    var index = (string)indexes[0];
                    if (!string.IsNullOrEmpty(index))
                    {
                        TrySetMember(index, value);
                    }
                }

                return base.TrySetIndex(binder, indexes, value);
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
                        var index = (string)indexes[0];
                        var configName = ((IEnumerable)project.As<EnvDTE.Project>().ConfigurationManager.ConfigurationRowNames)
                            .OfType<string>().FirstOrDefault(x => x == index);
                        if (configName != null)
                            return new SpecificConfigurationProperties(project, configName, userData);

                        var platformName = ((IEnumerable)project.As<EnvDTE.Project>().ConfigurationManager.PlatformNames)
                            .OfType<string>().FirstOrDefault(x => x == index);
                        if (platformName != null)
                            return new SpecificPlatformProperties(project, platformName, userData);

                        var configPlatforms = from config in ((IEnumerable)project.As<EnvDTE.Project>().ConfigurationManager.ConfigurationRowNames).OfType<string>()
                                              from platform in ((IEnumerable)project.As<EnvDTE.Project>().ConfigurationManager.PlatformNames).OfType<string>()
                                              select config + BuildConfigurationSeparator + platform.Replace(" ", string.Empty);

                        var configPlatform = configPlatforms.FirstOrDefault(x => x == index);
                        if (configPlatform != null)
                            return new SpecificConfigurationAndPlatformProperties(this.project, index, userData);

                        // Finally fall back to treating the value as a property.
                        object result;
                        TryGetMember(index, out result);
                        return result;
                    }
                }

                return null;
            }
        }
    }
}