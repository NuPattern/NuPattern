using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning
{
	[TestClass]
	[CLSCompliant(false)]
	public abstract class VsHostedSpec
	{
		protected delegate void ThreadInvoker();

		public TestContext TestContext { get; set; }

		protected static EnvDTE.DTE Dte
		{
			get { return VsIdeTestHostContext.Dte; }
		}

		protected static IServiceProvider ServiceProvider
		{
			get { return VsIdeTestHostContext.ServiceProvider; }
		}

		[TestInitialize]
		public virtual void Initialize()
		{
			if (Dte != null)
			{
				Dte.SuppressUI = false;
				Dte.MainWindow.Visible = true;
				Dte.MainWindow.WindowState = EnvDTE.vsWindowState.vsWindowStateMaximize;
			}
		}

		[TestCleanup]
		public virtual void Cleanup()
		{
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "None")]
		protected void OpenSolution(string solutionFile)
		{
			VsHostedSpec.DoActionWithWaitAndRetry(
				() => Dte.Solution.Open(solutionFile),
				2000,
				3,
				() => !Dte.Solution.IsOpen);
		}

		protected string GetFullPath(string relativePath)
		{
			return Path.Combine(TestContext.TestDeploymentDir, relativePath);
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
					Thread.Sleep(millisecondsToWait);
					Application.DoEvents();
					retry++;
				}
			}
			while (retryCondition() && retry < numberOfRetries);
		}
	}
}