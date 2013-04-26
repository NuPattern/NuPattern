﻿using System.Windows.Controls;
using NuPattern.Diagnostics;

namespace NuPattern.Presentation
{
    /// <summary>
    /// This is a base class for all XAML controls.
    /// </summary>
    /// <remarks>
    /// Since we have a number of shared XAML resources in this assembly, all XAML forms using those shared resources need
    /// to derive from this base class to ensure that this assembly and its shared XAML resources are loaded in the same AppDomain
    /// as the assembly using the shared resources. This is a workaround to fix the intermittent XAML rsource missing exception.
    /// </remarks>
    public class CommonUserControl : UserControl
    {
        private static readonly ITracer tracer = Tracer.Get(typeof(CommonUserControl));

        /// <summary>
        /// Creates a new instance of the <see cref="CommonDialogWindow"/> class.
        /// </summary>
        public CommonUserControl()
            : base()
        {
            tracer.Verbose(Properties.Resources.CommonDialogWindow_TraceResourcesLoaded);
        }
    }
}
