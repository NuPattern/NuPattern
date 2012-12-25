using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Base class for integration tests that need to deploy content.
/// </summary>
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

	private string TestDirectory
	{
		get { return ((uint)this.context.TestName.GetHashCode()).ToString(); }
	}

	/// <summary>
	/// Retruns a path relative to the test deployment directory.
	/// </summary>
	/// <param name="deploymentRelativePath">A relative path that is resolved from the deployment directory.</param>
	/// <returns>The path relative to the test deployment directory.</returns>
	protected string PathTo(string deploymentRelativePath)
	{
		return Path.Combine(this.DeploymentDirectory, deploymentRelativePath);
	}

	private static void RunXCopy(string sourceDir, string targetDir)
	{
		var startInfo = new ProcessStartInfo(
			"xcopy",
			"\"" + sourceDir + "\" " + "\"" + targetDir + "\" " + " /D /S /I /F /Y");
		startInfo.RedirectStandardError = startInfo.RedirectStandardInput = startInfo.RedirectStandardOutput;
		startInfo.CreateNoWindow = true;
		startInfo.UseShellExecute = false;
		startInfo.WorkingDirectory = sourceDir;

		var process = Process.Start(startInfo);
		process.WaitForExit();
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
                Directory.Delete(this.DeploymentDirectory, true);
            }
            catch (UnauthorizedAccessException)
            {
                // Ignore and continue
            }
		}
	}
}