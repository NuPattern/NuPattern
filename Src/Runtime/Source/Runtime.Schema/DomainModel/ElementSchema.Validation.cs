using Microsoft.VisualStudio.Modeling.Validation;
using NuPattern.Diagnostics;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Customizations for the ElementSchema class.
    /// </summary>
    [ValidationState(ValidationState.Enabled)]
    partial class ElementSchema
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<ElementSchema>();
    }
}