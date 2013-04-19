using System;
using NuPattern.ComponentModel.Composition;

namespace NuPattern.Runtime.Guidance
{
    /// <summary>
    /// Attribute used to expose launchpoints.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    internal class LaunchPointAttribute : ComponentAttribute
    {
        public LaunchPointAttribute()
            : base(typeof(ILaunchPoint))
        {
        }
    }
}