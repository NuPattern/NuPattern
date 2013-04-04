
namespace NuPattern.Runtime.Shell.Guidance
{
    /// <summary>
    /// Defines the feature extension containing the guidance workflow.
    /// </summary>
    public partial class Feature
    {
        /// <summary>
        /// Gets whether the feature extension is persisted in solution.
        /// </summary>
        public override bool PersistInstanceInSolution
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets whether the state of the feature extension is persisted in solution.
        /// </summary>
        public override bool PersistStateInSolution
        {
            get
            {
                return false;
            }
        }
    }
}