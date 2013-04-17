using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TemplateWizard;
using NuPattern.Diagnostics;
using NuPattern.Library.Design;
using NuPattern.Library.Properties;
using NuPattern.VisualStudio.TemplateWizards;

namespace NuPattern.Library.TemplateWizards
{
    /// <summary>
    /// Wizard extension that adds element properties to the replacements dictionary.
    /// </summary>
    [CLSCompliant(false)]
    public class ElementReplacementsTemplateWizard : TemplateWizard
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor(typeof(ElementReplacementsTemplateWizard));

        /// <summary>
        /// Runs custom wizard logic at the beginning of a template wizard run.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        public override void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            base.RunStarted(automationObject, replacementsDictionary, runKind, customParams);

            if (UnfoldScope.IsActive)
            {
                replacementsDictionary.AddRange(
                    ProductElementDictionaryConverter.Convert(UnfoldScope.Current.Automation.Owner)
                    .ToDictionary(e => string.Format(CultureInfo.InvariantCulture, "${0}$", e.Key), e => e.Value));

                tracer.TraceVerbose(Resources.ElementReplacementWizard_TracerTitle);
                foreach (var item in replacementsDictionary)
                {
                    tracer.TraceVerbose("\t{0}:'{1}'", item.Key, item.Value);
                }
            }
        }
    }
}