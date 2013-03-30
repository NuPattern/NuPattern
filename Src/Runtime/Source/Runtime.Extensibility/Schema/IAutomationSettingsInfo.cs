
namespace NuPattern.Runtime
{
    /// <summary>
    /// Provides schema information about an automation extension setting.
    /// </summary>
    public partial interface IAutomationSettingsInfo
    {
        /// <summary>
        /// Tries to convert this settings element to the given typed automation 
        /// extension setting class.
        /// </summary>
        /// <typeparam name="TSettings">The type of the settings.</typeparam>
        TSettings As<TSettings>() where TSettings : IAutomationSettings;
    }
}
