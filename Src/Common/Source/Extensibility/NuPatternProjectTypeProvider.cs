using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Design;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using NuPattern.Presentation;
using NuPattern.Runtime;
using NuPattern.VisualStudio.Solution;
using Ole = Microsoft.VisualStudio.OLE.Interop;

namespace NuPattern.Extensibility
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(INuPatternProjectTypeProvider))]
    internal class NuPatternProjectTypeProvider : INuPatternProjectTypeProvider
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ISolution solution;

        [ImportingConstructor]
        public NuPatternProjectTypeProvider([Import(typeof(SVsServiceProvider))]IServiceProvider serviceProvider, [Import(typeof(ISolution))]ISolution solution)
        {
            this.serviceProvider = serviceProvider;
            this.solution = solution;
        }

        public IEnumerable<Type> GetTypes<T>()
        {
            using (new MouseCursor(Cursors.Wait))
            {
                return GetAvailableTypes(typeof(T));
            }
        }

        private IEnumerable<Type> GetAvailableTypes(Type assignableTo)
        {
            var allTypes = new List<Type>();

            // Get nearest project scope
            var project = this.solution.GetCurrentProjectScope();
            var hierarchy = project.As<IVsHierarchy>();
            var vsProject = hierarchy as IVsProject;

            Ole.IServiceProvider olesp;
            // The design-time assembly resolution (DTAR) must be retrieved from a 
            // local service provider for the project. This is the magic goo.
            if (vsProject.GetItemContext(Microsoft.VisualStudio.VSConstants.VSITEMID_ROOT, out olesp) == Microsoft.VisualStudio.VSConstants.S_OK)
            {
                var localservices = new ServiceProvider(olesp);
                var openScope = this.serviceProvider.GetService(typeof(SVsSmartOpenScope)) as IVsSmartOpenScope;
                var dtar = (IVsDesignTimeAssemblyResolution)localservices.GetService(typeof(SVsDesignTimeAssemblyResolution));

                // As suggested by Christy Henriksson, we reuse the type discovery service 
                // but just for the IDesignTimeAssemblyLoader interface. The actual 
                // assembly reading is done by the TFP using metadata only :)
                var dts = (DynamicTypeService)this.serviceProvider.GetService(typeof(DynamicTypeService));
                var ds = dts.GetTypeDiscoveryService(hierarchy);
                var dtal = ds as IDesignTimeAssemblyLoader;

                var provider = new VsTargetFrameworkProvider(dtar, dtal, openScope);

                var references = project.GetAssemblyReferences();
                foreach (var reference in references)
                {
                    try
                    {
                        var name = AssemblyName.GetAssemblyName(reference.Path);

                        allTypes.AddRange(provider
                            .GetReflectionAssembly(name)
                            .GetExportedTypes()
                            .Where(t => t.IsAssignableTo(assignableTo)));
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }

            return allTypes;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public bool SupportsAssemblyLoading
        {
            get { return false; }
        }
    }
}

