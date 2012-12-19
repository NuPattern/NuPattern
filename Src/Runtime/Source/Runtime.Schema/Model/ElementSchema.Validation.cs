using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Customizations for the ElementSchema class.
    /// </summary>
    [ValidationState(ValidationState.Enabled)]
    public partial class ElementSchema
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<ElementSchema>();
    }
}