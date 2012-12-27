using Microsoft.VisualStudio.PlatformUI;

namespace NuPattern.Common.Presentation.Vs
{
    /// <summary>
    /// The VS DialogButton control
    /// </summary>
    /// <remarks>We override the <see cref="DialogButton"/> class to avoid 
    /// referencing Microsoft.VisualStudio.Shell.1X.0 in any xaml files that use this class.</remarks>
    public class VsDialogButton : DialogButton
    {
    }
}
