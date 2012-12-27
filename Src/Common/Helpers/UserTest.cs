using System.Windows.Input;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITesting.WpfControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;

/// <summary>
/// Base class for a User Test
/// </summary>
[CodedUITest]
public abstract class UserTest : IntegrationTest
{
    internal static readonly IAssertion Assert = new Assertion();

    [TestInitialize]
    public virtual void InitializeContext()
    {
        // Slow down the playback in case there are breaks in the UI thread activities.
        Playback.PlaybackSettings.DelayBetweenActions = 1000;
    }

    [TestCleanup]
    public virtual void Cleanup()
    {
        // Close any opened solutions
        VsIdeTestHostContext.Dte.Solution.Close(false);
    }
}