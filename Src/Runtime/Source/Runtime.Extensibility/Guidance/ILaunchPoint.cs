
namespace NuPattern.Runtime.Guidance
{
    /// <summary>
    /// Feature command launch point
    /// </summary>
    public interface ILaunchPoint
    {
        /// <summary>
        /// Whether the launch point can be executed.
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        bool CanExecute(IGuidanceExtension feature);

        /// <summary>
        /// Executes the launch point
        /// </summary>
        /// <param name="feature"></param>
        void Execute(IGuidanceExtension feature);
    }
}
