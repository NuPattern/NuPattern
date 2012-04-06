using System;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Library.Properties;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using Microsoft.Win32;

namespace Microsoft.VisualStudio.Patterning.Library.ValueProviders
{
    /// <summary>
    /// A <see cref=" ValueProvider"/> that returns the registered user of the current machine.
    /// </summary>
    [DisplayNameResource("RegisteredMachineUserValueProvider_DisplayName", typeof(Resources))]
    [DescriptionResource("RegisteredMachineUserValueProvider_Description", typeof(Resources))]
    [CategoryResource("AutomationCategory_General", typeof(Resources))]
    [CLSCompliant(false)]
    public class RegisteredMachineUserValueProvider : ValueProvider
    {
        private const string RegKey = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";
        private const string RegKeyValue = @"RegisteredOrganization";

        private static readonly ITraceSource tracer = Tracer.GetSourceFor<RegisteredMachineUserValueProvider>();

        /// <summary>
        /// Evaluates this provider.
        /// </summary>
        public override object Evaluate()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.RegisteredMachineUserValueProvider_TraceInitial, RegKey, RegKeyValue);

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(RegKey, false))
            {
                object value = key.GetValue(RegKeyValue);
                if (value == null)
                {
                    tracer.TraceWarning(
                        Resources.RegisteredMachineUserValueProvider_TraceNoValue, RegKey, RegKeyValue);
                    return string.Empty;
                }
                else
                {
                    tracer.TraceInformation(
                        Resources.RegisteredMachineUserValueProvider_TraceEvaluation, RegKey, RegKeyValue, value);
                    return value.ToString();
                }
            }
        }
    }
}
