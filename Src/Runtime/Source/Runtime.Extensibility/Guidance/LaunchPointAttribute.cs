using System;
using NuPattern.ComponentModel.Composition;

namespace NuPattern.Runtime.Guidance
{
    /// <summary>
    /// Attribute used to expose launchpoints.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class LaunchPointAttribute : ComponentAttribute
    {
		/// <summary>
		/// Creates a new instance of the <see cref="LaunchPointAttribute"/> class.
		/// </summary>
        public LaunchPointAttribute()
            : base(typeof(ILaunchPoint))
        {
        }
    }
}