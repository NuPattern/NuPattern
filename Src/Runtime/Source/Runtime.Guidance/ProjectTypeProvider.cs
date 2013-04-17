using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Xml;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Design;
using Microsoft.VisualStudio.Shell.Interop;
using NuPattern;
using NuPattern.Diagnostics;
using NuPattern.Presentation;
using NuPattern.VisualStudio.Solution;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    [Export(typeof(IProjectTypeProvider))]
    internal class ProjectTypeProvider : IProjectTypeProvider
    {
        static readonly ITraceSource tracer = Tracer.GetSourceFor<ProjectTypeProvider>();

        List<Type> availableTypes = new List<Type>();
        bool initialized;
        bool loadingTypes;
        IServiceProvider serviceProvider;
        ISolution solution;
        DynamicTypeService dynamicTypeService;
        EnvDTE.BuildEvents buildEvents;

        [ImportingConstructor]
        public ProjectTypeProvider([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider, ISolution solution)
        {

            this.serviceProvider = serviceProvider;
            this.solution = solution;
        }


        private void Initialize()
        {
            if (!initialized)
            {
                this.dynamicTypeService = serviceProvider.GetService(typeof(DynamicTypeService)) as DynamicTypeService;

                //this.dynamicTypeService.AssemblyRefreshed += new AssemblyRefreshedEventHandler(dynamicTypeService_AssemblyRefreshed);
                //this.dynamicTypeService.AssemblyDeleted += new AssemblyDeletedEventHandler(dynamicTypeService_AssemblyDeleted);

                var vs = this.serviceProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
                this.buildEvents = vs.Events.BuildEvents;
                //this.buildEvents.OnBuildDone += new _dispBuildEvents_OnBuildDoneEventHandler(BuildEvents_OnBuildDone);

                initialized = true;

                //LoadTypes();
            }
        }

        void BuildEvents_OnBuildDone(vsBuildScope Scope, vsBuildAction Action)
        {
            LoadTypes();
        }

        void dynamicTypeService_AssemblyDeleted(object sender, AssemblyDeletedEventArgs e)
        {
            LoadTypes();
        }

        void dynamicTypeService_AssemblyRefreshed(object sender, AssemblyRefreshedEventArgs e)
        {
            LoadTypes();
        }

        private void LoadTypes()
        {
            if (loadingTypes)
                return;

            try
            {
                tracer.TraceVerbose("Reloading types available in the current solution");
                loadingTypes = true;

                this.availableTypes.Clear();

                foreach (var project in solution.Find<IProject>())
                {
                    var hierarchy = project.As<IVsHierarchy>();
                    if (hierarchy != null)
                    {
                        tracer.TraceVerbose("Retrieving types from project {0}", project.Name);
                        var typeDiscoveryService = dynamicTypeService.GetTypeDiscoveryService(hierarchy);
                        string currentProjectAssemblyName;

                        try
                        {
                            currentProjectAssemblyName = project.As<EnvDTE.Project>().Properties.Item("AssemblyName").Value as string;
                        }
                        catch (Exception)
                        {
                            continue;
                        }

                        // We should be able to leverage Progression for this!
                        var types = typeDiscoveryService.GetTypes(typeof(object), true);

                        foreach (Type type in types)
                        {
                            if (string.Equals(type.Assembly.GetName().Name, currentProjectAssemblyName, StringComparison.InvariantCultureIgnoreCase))
                            {
                                availableTypes.Add(type);
                            }
                        }
                    }
                }
            }
            finally
            {
                loadingTypes = false;
            }
        }

        public IEnumerable<Type> GetTypes<T>()
        {
            using (new MouseCursor(Cursors.Wait))
            {
                Initialize();

                // Investigate why the the BuildDone or AssemblyRefreshed events work only randomly.
                LoadTypes();

                foreach (Type type in availableTypes)
                {
                    if (type.Traverse(t => t.BaseType, t => t.AssemblyQualifiedName == typeof(T).AssemblyQualifiedName) != null)
                        yield return type;

                    if (type.GetInterfaces().FirstOrDefault(t => t.AssemblyQualifiedName == typeof(T).AssemblyQualifiedName) != null)
                        yield return type;
                }
            }
        }

        private IVsHierarchy ToHierarchy(EnvDTE.Project project)
        {
            if (project == null) throw new ArgumentNullException("project");

            string projectGuid = null;

            if (File.Exists(project.FileName))
            {
                // DTE does not expose the project GUID that exists at in the msbuild project file.
                // Cannot use MSBuild object model because it uses a static instance of the Engine, 
                // and using the Project will cause it to be unloaded from the engine when the 
                // GC collects the variable that we declare.
                using (XmlReader projectReader = XmlReader.Create(project.FileName))
                {
                    projectReader.MoveToContent();
                    object nodeName = projectReader.NameTable.Add("ProjectGuid");
                    while (projectReader.Read())
                    {
                        if (Object.Equals(projectReader.LocalName, nodeName))
                        {
                            projectGuid = projectReader.ReadElementContentAsString();
                            break;
                        }
                    }
                }

                return VsShellUtilities.GetHierarchy(serviceProvider, new Guid(projectGuid));
            }

            return null;
        }
    }
}
