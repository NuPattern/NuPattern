using System;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NuPattern.IO;
using NuPattern.VisualStudio.Properties;
using NuPattern.VisualStudio.Solution.Hierarchy;

namespace NuPattern.VisualStudio.Solution
{
    /// <summary>
    /// Provides a dynamic object API to store data 
    /// as MSBuild item properties.
    /// </summary>
    [DebuggerDisplay(@"{AllProperties}")]
    internal class VsItemDynamicProperties : DynamicObject
    {
        IHierarchyNode node;
        Lazy<string> debugPropertiesSnapshot;

        public VsItemDynamicProperties(IHierarchyNode item)
        {
            this.node = item;
            this.debugPropertiesSnapshot = new Lazy<string>(() => ThreadHelper.Generic.Invoke<string>(() => GetProperties()));
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return SetValue(binder.Name, value);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = GetValue(binder.Name);
            return true;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if (indexes.Length == 1 && indexes[0] is string)
            {
                result = GetValue((string)indexes[0]);
                return true;
            }

            return base.TryGetIndex(binder, indexes, out result);
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            if (indexes.Length == 1 && indexes[0] is string)
            {
                return SetValue((string)indexes[0], value);
            }

            return base.TrySetIndex(binder, indexes, value);
        }

        public object GetValue(string name)
        {
            string value = null;

            // First try automation properties.
            var item = this.node.ExtObject as ProjectItem;
            if (item != null)
            {
                Property property;
                try
                {
                    property = item.Properties.Item(name);
                }
                catch (ArgumentException)
                {
                    property = null;
                }

                if (property != null)
                {
                    return property.Value;
                }
            }

            // Next go custom MSBuild properties.
            var storage = this.node.GetObject<IVsHierarchy>() as IVsBuildPropertyStorage;
            if (storage != null)
            {
                storage.GetItemAttribute(this.node.ItemId, name, out value);
            }

            // Do value conversion if it's serializable.

            return value;
        }

        public bool SetValue(string name, object value)
        {
            // First try automation properties.
            var item = this.node.ExtObject as ProjectItem;
            if (item != null)
            {
                Property property;
                try
                {
                    property = item.Properties.Item(name);
                }
                catch (ArgumentException)
                {
                    property = null;
                }

                if (property != null)
                {
                    try
                    {
                        property.Value = value;
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }

            // Next go custom MSBuild properties.
            if (value == null)
                throw new NotSupportedException(Resources.VsItemDynamicProperties_ErrorNullItemProperies);

            var storage = this.node.GetObject<IVsHierarchy>() as IVsBuildPropertyStorage;
            if (storage != null)
            {
                return ErrorHandler.Succeeded(
                    storage.SetItemAttribute(this.node.ItemId, name, value.ToString()));
            }

            return false;
        }

        public string AllProperties
        {
            get { return this.debugPropertiesSnapshot.Value; }
        }

        private string GetProperties()
        {
            try
            {
                var properties = new StringBuilder();

                var item = this.node.ExtObject as ProjectItem;
                if (item != null && item.Properties != null)
                {
                    var formattedProperties = item.Properties
                        .Cast<Property>()
                        .Select(prop => string.Format(@"{0}={1}", prop.Name, prop.Value ?? @"null"))
                        .ToArray();
                    properties.Append(string.Join(Environment.NewLine, formattedProperties));
                }

                var projectName = item.ContainingProject.FullName;
                var project = Microsoft.Build.Evaluation.ProjectCollection.GlobalProjectCollection.GetLoadedProjects(projectName).FirstOrDefault();

                if (project != null)
                {
                    properties.Append(String.Join(Environment.NewLine,
                        project.ItemsIgnoringCondition
                            .Where(i => i.UnevaluatedInclude == new RelativePathBuilder(item.ContainingProject.FullName, item.FileNames[1]).Build())
                            .Select(i => new { Item = i, Metadata = i.Metadata })
                            .Select(i =>
                                i.Metadata.Select(md => String.Format(@"{0}={1}", md.Name, md.UnevaluatedValue)))
                    ));
                }

                return properties.ToString();
            }
            catch // NOTE this code is only valid with an attached debugger
            {
                return string.Empty;
            }
        }
    }
}
