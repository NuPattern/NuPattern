using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Design;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;

namespace $rootnamespace$
{
    /// <summary>
    /// A custom type converter for returning a list of System.String values.
    /// </summary>
    [DisplayName("$safeitemname$ Custom Enumeration Type Converter")]
    [Category("General")]
    [Description("Returns a list of custom enumerations.")]
    [CLSCompliant(false)]
    public class $safeitemname$ : StringConverter
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<$safeitemname$>();

        /// <summary>
        /// Determines whether this converter supports standard values.
        /// </summary>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Determines whether this converter only allows selection of items returned by <see cref="GetStandardValues"/>.
        /// </summary>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Returns the list of string values for the enumeration
        /// </summary>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var items = new List<string>();

            try
            {
                // Make initial trace statement for this converter
                tracer.TraceInformation(
                    "Determining values for this converter");

                // TODO: Implement code to fetch or calculate the returned values.
                items.Add("One");
                items.Add("Two");
                items.Add("Three");

                // TOOD: If you want a dynamic list of values from somewhere your pattern model,
                //	you can cast the 'context' parameter to be the type of the 
                //  owning element of the property where the this converter is configured 
                //  to gain access to the instances of elements your pattern model.
                //  var currentElement = context.Instance as IProductElement;
                //  var variableElement1 = currentElement.As<IVariableElement1>();


                //	TODO: Use tracer.TraceWarning() to note expected and recoverable errors
                //	TODO: Use tracer.TraceVerbose() to note internal execution logic decisions
                //	TODO: Use tracer.TraceInformation() to note key results of execution
                //	TODO: Raise exceptions for all other errors

                return new StandardValuesCollection(items);
            }
            catch (SomeExpectedException ex)
            {
                // TODO: Only catch expected exceptions, and trace them before re-throwing.
                // TODO: Remove this 'catch' if no expections are expected
                tracer.TraceError(
                    ex, "Some error calculating or fetching values");

                throw;
            }
        }
    }
}
