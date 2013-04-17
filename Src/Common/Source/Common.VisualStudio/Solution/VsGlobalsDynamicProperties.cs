using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;

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
                    "Failed to set data on Globals bag. Key={0}, Value={1}, Error={2}",
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
