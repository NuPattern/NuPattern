
namespace Microsoft.VisualStudio.Patterning.Runtime
{
    public partial interface IExtensionPointSchema : IContainedElementSchema
    {
        /// <summary>
        /// Gets the pattern model
        /// </summary>
        /// <value>The pattern model.</value>
        IPatternModelSchema PatternSchemaModel { get; }
    }
}