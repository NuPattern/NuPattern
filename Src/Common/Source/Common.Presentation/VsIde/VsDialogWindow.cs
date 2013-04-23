using Microsoft.VisualStudio.PlatformUI;

namespace NuPattern.Presentation.VsIde
{
    /// <summary>
    /// The VS DialogWindow control.
    /// </summary>
    /// <remarks>We override the <see cref="DialogWindow"/> class to avoid 
    /// referencing Microsoft.VisualStudio.Shell.1X.0 in any xaml files that use this class.</remarks>
    public class VsDialogWindow : DialogWindow
    {
    }
}
