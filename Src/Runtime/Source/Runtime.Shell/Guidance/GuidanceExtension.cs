
namespace NuPattern.Runtime.Shell.Guidance
{
    /// <summary>
    /// Defines the guidance extension containing the guidance workflow.
    /// </summary>
    partial class GuidanceExtension
    {
        /// <summary>
        /// Gets whether the guidance extension is persisted in solution.
        /// </summary>
        public override bool PersistInstanceInSolution
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets whether the state of the guidance extension is persisted in solution.
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