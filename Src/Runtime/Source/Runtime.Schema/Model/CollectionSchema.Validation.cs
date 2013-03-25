using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Customizations for the CollectionSchema class.
    /// </summary>
    [ValidationState(ValidationState.Enabled)]
    partial class CollectionSchema
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<CollectionSchema>();
    }
}