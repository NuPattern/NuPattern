using System;
using System.Dynamic;
using System.Linq;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace NuPattern.VisualStudio.Solution
{
    partial class VsProject
    {
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
                return TryGetMember(binder.Name, out result);
            }

            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                return TrySetMember(binder.Name, value);
            }

            public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
            {
                if (indexes.Length == 1 && indexes[0] is string)
                {
                    return this.TryGetMember((string)indexes[0], out result);
                }

                return base.TryGetIndex(binder, indexes, out result);
            }

            public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
            {
                if (indexes.Length == 1 && indexes[0] is string)
                {
                    return this.TrySetMember((string)indexes[0], value);
                }

                return base.TrySetIndex(binder, indexes, value);
            }

            private bool TryGetMember(string memberName, out object result)
            {
                string value = null;

                var storage = this.project.As<IVsHierarchy>() as IVsBuildPropertyStorage;
                if (storage != null)
                {
                    storage.GetPropertyValue(memberName, configurationAndPlatform, (uint)storageType, out value);
                }

                // Do value conversion if it's serializable.
                result = value;
                return true;
            }

            private bool TrySetMember(string memberName, object value)
            {
                var storage = this.project.As<IVsHierarchy>() as IVsBuildPropertyStorage;
                if (storage != null)
                {
                    return ErrorHandler.Succeeded(
                        storage.SetPropertyValue(memberName, configurationAndPlatform, (uint)storageType, value.ToString()));
                }

                return false;
            }
        }
    }
}