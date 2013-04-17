using System;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Utilities
{
    internal class NullServiceProvider : IServiceProvider
    {
        public static IServiceProvider Instance { get; private set; }

        static NullServiceProvider()
        {
            Instance = new NullServiceProvider();
        }

        private NullServiceProvider()
        {
        }

        public object GetService(Type serviceType)
        {
            return null;
        }
    }
}
