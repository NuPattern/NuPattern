
namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    /// <summary>
    /// Feature command launch point
    /// </summary>
    internal interface ILaunchPoint
    {
        bool CanExecute(IFeatureExtension feature);
        void Execute(IFeatureExtension feature);
    }
}
