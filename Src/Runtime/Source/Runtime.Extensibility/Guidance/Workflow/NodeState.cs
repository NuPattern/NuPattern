using NuPattern.ComponentModel.Design;
using NuPattern.Runtime.Properties;

namespace NuPattern.Runtime.Guidance.Workflow
{
    /// <summary>
    /// State of an activity in the process guidance workflow.
    /// </summary>
    public enum NodeState
    {
        /// <summary>
        /// The activity state is unknown.
        /// </summary>
        [DescriptionResource(@"NodeState_Unknown", typeof(Resources))]
        Unknown = 0,
        /// <summary>
        /// The activity is enabled.
        /// </summary>
        [DescriptionResource(@"NodeState_Enabled", typeof(Resources))]
        Enabled = 1,
        /// <summary>
        /// The activity is finished.
        /// </summary>
        [DescriptionResource(@"NodeState_Completed", typeof(Resources))]
        Completed = 2,
        /// <summary>
        /// The activity is blocked.
        /// </summary>
        [DescriptionResource(@"NodeState_Blocked", typeof(Resources))]
        Blocked = 3,
        /// <summary>
        /// The activity is disabled.
        /// </summary>
        [DescriptionResource(@"NodeState_Disabled", typeof(Resources))]
        Disabled = 4,
        /// <summary>
        /// Default value, which equals <see cref="Unknown"/>.
        /// </summary>
        Default = Unknown
    }
}