using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using Microsoft.VisualStudio.TemplateWizard;
using NuPattern.Extensibility;
using NuPattern.Runtime.Properties;
using Ole = Microsoft.VisualStudio.OLE.Interop;

namespace NuPattern.Runtime
{
	/// <summary>
	/// Custom template wizard extension that adds the root suffix of the 
	/// currently running Visual Studio instance to the parameter replacements 
	/// dictionary with the key <c>$vsrootsuffix$</c>.
	/// </summary>
	[CLSCompliant(false)]
	public class VsRootSuffixTemplateWizard : Extensibility.TemplateWizard
	{
		private static readonly ITraceSource tracer = Tracer.GetSourceFor<VsRootSuffixTemplateWizard>();
#if VSVER10
        private static string VsSettingsRegistryKey = @"Software\Microsoft\VisualStudio\10.0";
#endif
#if VSVER11
        private static string VsSettingsRegistryKey = @"Software\Microsoft\VisualStudio\11.0";
#endif
		/// <summary>
		/// Runs custom wizard logic at the beginning of a template wizard run.
		/// </summary>
		/// <param name="automationObject">The automation object being used by the template wizard.</param>
		/// <param name="replacementsDictionary">The list of standard parameters to be replaced.</param>
		/// <param name="runKind">A <see cref="T:Microsoft.VisualStudio.TemplateWizard.WizardRunKind"/> indicating the type of wizard run.</param>
		/// <param name="customParams">The custom parameters with which to perform parameter replacement in the project.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Never dispose the service provider.")]
		public override void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
		{
			base.RunStarted(automationObject, replacementsDictionary, runKind, customParams);

			var vs = automationObject as EnvDTE.DTE;
			if (vs != null)
			{
				using (var serviceProvider = new ServiceProvider((Ole.IServiceProvider)vs))
				{
					tracer.ShieldUI(() =>
					{
						var shell = serviceProvider.GetService<SVsShell, IVsShell>();

						var registryRoot = VsHelper.GetPropertyOrDefault<string>(shell.GetProperty, (int)__VSSPROPID.VSSPROPID_VirtualRegistryRoot);
						var suffix = registryRoot.Replace(VsSettingsRegistryKey, string.Empty);

						replacementsDictionary.Add("$vsrootsuffix$", suffix);

						tracer.TraceInformation(Resources.VsRootSuffixTemplateWizard_RootSuffixDetermined, suffix);
					},
					Resources.VsRootSuffixTemplateWizard_FailedToRetrieveRegistryRoot);
				}
			}
		}
	}
}