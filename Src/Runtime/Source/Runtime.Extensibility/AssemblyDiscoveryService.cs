using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.EnterpriseLibrary.Configuration.Design.Configuration.Design.HostAdapterV5;
using NuPattern.Diagnostics;
using NuPattern.Runtime.Properties;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Runtime
{
    internal class AssemblyDiscoveryService : IAssemblyDiscoveryService
    {
        private ITypeDescriptorContext ownerContext;
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<AssemblyDiscoveryService>();

        public AssemblyDiscoveryService(ITypeDescriptorContext context)
        {
            this.ownerContext = context;
        }

        public IDictionary<string, IEnumerable<Assembly>> GetAvailableAssemblies()
        {
            return GetAssemblies(ownerContext);
        }

        public bool SupportsAssemblyLoading
        {
            get { return false; }
        }

        private static IDictionary<string, IEnumerable<Assembly>> GetAssemblies(ITypeDescriptorContext context)
        {
            tracer.TraceInformation(
                Resources.AssemblyDiscoveryService_GetAssemblies_TraceInitial);

            var solution = context.GetService<ISolution>();
            if (solution == null)
            {
                tracer.TraceInformation(
                    Resources.AssemblyDiscoveryService_GetAssemblies_TraceNoSolution);

                return new Dictionary<string, IEnumerable<Assembly>>();
            }

            var assemblies = solution.GetAllProjects()
                .SelectMany(p => p.GetAvailableTypes(typeof(object)).Select(t => t.Assembly))
                .Distinct();

            tracer.TraceInformation(
                Resources.AssemblyDiscoveryService_GetAssemblies_TraceAssembliesFound, assemblies.Count());

            return new Dictionary<string, IEnumerable<Assembly>> { { "Types", assemblies } };
        }
    }

    internal class AssemblyDiscoveryServiceProvider : IServiceProvider
    {
        private IServiceProvider previous;
        private ITypeDescriptorContext context;

        public AssemblyDiscoveryServiceProvider(IServiceProvider previous, ITypeDescriptorContext context)
        {
            this.context = context;
            this.previous = previous;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IAssemblyDiscoveryService))
            {
                return new AssemblyDiscoveryService(context);
            }

            return previous.GetService(serviceType);
        }
    }
}
