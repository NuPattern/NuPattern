using ExtMan = Microsoft.VisualStudio.ExtensionManager;

namespace NuPattern.VisualStudio.Extensions
{
    /// <summary>
    /// Defines the states that an extension may have.
    /// </summary>
    public enum EnabledState
    {
        /// <summary>
        /// The extension is enabled
        /// </summary>
        Enabled = ExtMan.EnabledState.Enabled,

        /// <summary>
        /// The extension is disabled
        /// </summary>
        Disabled = ExtMan.EnabledState.Disabled,

        /// <summary>
        /// The extension has been marked to be disabled (restart required)
        /// </summary>
        PendingDisable = ExtMan.EnabledState.PendingDisable,

        /// <summary>
        /// The extension has been marked to be enabled (restart required)
        /// </summary>
        PendingEnable = ExtMan.EnabledState.PendingEnable,
    }
}
