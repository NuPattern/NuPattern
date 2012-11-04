using System.Diagnostics;
using System.IO;
using Microsoft.ComponentModel.Composition.Diagnostics;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using Microsoft.VisualStudio.ComponentModelHost;

namespace Microsoft.VisualStudio.Patterning.Runtime.IntegrationTests
{
	/// <summary>
	/// Use this helper to quickly dump MEF composition information to a file in %Temp%\mef.txt.
	/// </summary>
	internal static class MefLogger
	{
		public static void Log()
		{
			var globalContainer = VsIdeTestHostContext.ServiceProvider.GetService<SComponentModel, IComponentModel>();
			var tempFile = Path.Combine(Path.GetTempPath(), "mef.txt");
			using (var writer = new StreamWriter(tempFile, false))
			{
				CompositionInfoTextFormatter.Write(new CompositionInfo(globalContainer.DefaultCatalog, globalContainer.DefaultExportProvider), writer);
			}

			Process.Start(tempFile);
		}
	}
}
