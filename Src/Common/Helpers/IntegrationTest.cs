using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Base class for integration tests that need to deploy content.
/// </summary>
/// <remarks>
/// This class works around the issue of resetting the test data per test that VS has.
/// VS currently only copies test data once per test run, and if a test alters that test data then the test is not correctly reset for other tests.
/// This class copies the test data every test, and ensures that the test datat is always reset between tests.
/// </remarks>
public class IntegrationTest
{
    private TestContext context;

    /// <summary>
    /// Gets or sets the location of the per-test location directory.
    /// </summary>
    public string DeploymentDirectory { get; set; }

    /// <summary>
    /// Gets or sets the test context.
    /// </summary>
    public TestContext TestContext
    {
        get
        {
            return this.context;
        }

        set
        {
            this.context = value;
            this.DeploymentDirectory = Path.Combine(Path.GetTempPath(), this.TestDirectory);
            this.CleanDeploymentDirectory();

            // On set, we copy a branch of the deployment items
            var deploymentItems = this.GetType()
                .GetCustomAttributes(typeof(DeploymentItemAttribute), true)
                .OfType<DeploymentItemAttribute>();

            foreach (var deploymentItem in deploymentItems)
            {
                var sourceDir = string.IsNullOrEmpty(deploymentItem.OutputDirectory) ?
                    this.context.DeploymentDirectory :
                    Path.Combine(this.context.DeploymentDirectory, deploymentItem.OutputDirectory);

                // Fix VSTS non-copy-if-folder exists "issue"
                RunXCopy(sourceDir, Path.Combine(this.DeploymentDirectory, deploymentItem.OutputDirectory));
            }
        }
    }

    protected static void DoActionWithWait(int millisecondsToWait, Action action)
    {
        DoActionWithWaitAndRetry(action, millisecondsToWait, 1, () => true);
    }

    protected static void DoActionWithWaitAndRetry(Action action, int millisecondsToWait, int numberOfRetries, Func<bool> retryCondition)
    {
        int retry = 0;

        do
        {
            action();

            if (retryCondition())
            {
                System.Threading.Thread.Sleep(millisecondsToWait);
                Application.DoEvents();
                retry++;
            }
        }
        while (retryCondition() && retry < numberOfRetries);
    }

    private string TestDirectory
    {
        get { return ((uint)this.context.TestName.GetHashCode()).ToString(); }
    }

    /// <summary>
    /// Returns a path relative to the test deployment directory.
    /// </summary>
    /// <param name="deploymentRelativePath">A relative path that is resolved from the deployment directory.</param>
    /// <returns>The path relative to the test deployment directory.</returns>
    protected string PathTo(string deploymentRelativePath)
    {
        return Path.Combine(this.DeploymentDirectory, deploymentRelativePath);
    }

    private static void RunXCopy(string sourceDir, string targetDir)
    {
        try
        {
            var cmdLine = "\"" + sourceDir + "\" " + "\"" + targetDir + "\" " + " /D /S /I /F /Y";
            var startInfo = new ProcessStartInfo("xcopy", cmdLine);
            startInfo.RedirectStandardError = startInfo.RedirectStandardInput = startInfo.RedirectStandardOutput;
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = sourceDir;

            var process = Process.Start(startInfo);
            process.WaitForExit();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                "Failed to xcopy integration test dependencies from '{0}' to '{1}'.", sourceDir, targetDir), ex);
        }
    }

    private void CleanDeploymentDirectory()
    {
        if (Directory.Exists(this.DeploymentDirectory))
        {
            // Clear readonly attributes
            foreach (var file in Directory.EnumerateFiles(this.DeploymentDirectory, "*.*", SearchOption.AllDirectories))
            {
                File.SetAttributes(file, FileAttributes.Normal);
            }

            try
            {
                // Delete the direectory
                Directory.Delete(this.DeploymentDirectory, true);
            }
            catch (UnauthorizedAccessException)
            {
                // Ignore and continue
            }
            catch (IOException)
            {
                // Ignore and continue
            }
        }
    }
}