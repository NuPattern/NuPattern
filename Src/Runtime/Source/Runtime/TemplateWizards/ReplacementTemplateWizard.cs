using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Runtime.Properties;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using Microsoft.VisualStudio.TemplateWizard;

namespace Microsoft.VisualStudio.Patterning.Runtime
{
	/// <summary>
	/// Custom template wizard extension that parses custom wizard data 
	/// looking for additional replacement entries to use during template unfolding.
	/// </summary>
	/// <example>
	/// The following example adds a custom replacement entry with a 
	/// version number:
	/// <code>
	/// 	&lt;WizardData&gt;
	/// 		&lt;Replacement Key="$dslVersion$"  Value="1.1.0.0" /&gt;
	/// 	&lt;/WizardData&gt;
	/// </code>
	/// This additional key can be used in any content unfolded by the template
	/// as follows:
	/// <code>
	/// // This class was generated from version $dslVersion$.
	/// </code>
	/// </example>
	[CLSCompliant(false)]
	public class ReplacementTemplateWizard : Extensibility.TemplateWizard
	{
		private static readonly ITraceSource tracer = Tracer.GetSourceFor<ReplacementTemplateWizard>();

		private const string ReplacementElementName = "Replacement";
		private const string ReplacementKeyName = "Key";
		private const string ReplacementValueName = "Value";

		/// <summary>
		/// Runs custom wizard logic at the beginning of a template wizard run.
		/// </summary>
		/// <param name="automationObject">The automation object being used by the template wizard.</param>
		/// <param name="replacementsDictionary">The list of standard parameters to be replaced.</param>
		/// <param name="runKind">A <see cref="T:Microsoft.VisualStudio.TemplateWizard.WizardRunKind"/> indicating the type of wizard run.</param>
		/// <param name="customParams">The custom parameters with which to perform parameter replacement in the project.</param>
		public override void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
		{
			base.RunStarted(automationObject, replacementsDictionary, runKind, customParams);

			Guard.NotNull(() => replacementsDictionary, replacementsDictionary);
			Guard.NotNull(() => customParams, customParams);

			var wizardData = this.TemplateSchema.WizardData.FirstOrDefault();
			if (wizardData != null)
			{
				var additionalReplacements = from element in wizardData.Elements
											 where element.Name.Equals(ReplacementElementName, StringComparison.OrdinalIgnoreCase) &&
												 !string.IsNullOrEmpty(element.GetAttribute(ReplacementKeyName))
											 select new
											 {
												 Key = element.GetAttribute(ReplacementKeyName),
												 Value = element.GetAttribute(ReplacementValueName)
											 };

				foreach (var replacement in additionalReplacements)
				{
					tracer.TraceVerbose(Resources.ReplacementTemplateWizard_Replacing, replacement.Key, replacement.Value);
					replacementsDictionary[replacement.Key] = replacement.Value;
				}
			}
		}
	}
}