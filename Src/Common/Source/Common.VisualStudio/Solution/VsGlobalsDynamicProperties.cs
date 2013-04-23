using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using NuPattern.VisualStudio.Properties;

namespace NuPattern.VisualStudio.Solution
{
    /// <summary>
    /// Provides a dynamic object API on top of a 
    /// DTE Globals object.
    /// </summary>
    internal class VsGlobalsDynamicProperties : DynamicObject
    {
        private static readonly TraceSource tracer = new TraceSource(typeof(VsGlobalsDynamicProperties).FullName);
        private Func<EnvDTE.Globals> globalsGetter;

        public VsGlobalsDynamicProperties(Func<EnvDTE.Globals> globalsGetter)
        {
            this.globalsGetter = globalsGetter;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return ((IEnumerable)globalsGetter().VariableNames).OfType<string>();
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            try
            {
                globalsGetter()[binder.Name] = value;
                globalsGetter().set_VariablePersists(binder.Name, true);
                return true;
            }
            catch (Exception ex)
            {
                tracer.TraceEvent(TraceEventType.Error, 0,
                    Resources.VsGlobalsDynamicProperties_TraceFailedToSetGlobals,
                    binder.Name, value, ex);
                return false;
            }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = globalsGetter().get_VariableExists(binder.Name) ?
                globalsGetter()[binder.Name] : null;

            return true;
        }
    }
}
