using System;
using NuPattern.ComponentModel.Composition;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    /// <summary>
    /// Attribute used to expose launchpoints.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    internal class LaunchPointAttribute : FeatureComponentAttribute
    {
        public LaunchPointAttribute()
            : base(typeof(ILaunchPoint))
        {
        }
    }
}