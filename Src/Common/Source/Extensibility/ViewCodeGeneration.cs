using System;
using NuPattern.Runtime;

namespace NuPattern.Extensibility
{
    /// <summary>
    /// Defines utility methods for use in templates that generate code from a view schema.
    /// </summary>
    [CLSCompliant(false)]
    public class ViewCodeGeneration<TInfo, TRuntime> : CodeGeneration
        where TInfo : IViewInfo
        where TRuntime : IInstanceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewCodeGeneration{TInfo, TRuntime}"/> class.
        /// </summary>
        /// <param name="view">The view.</param>
        public ViewCodeGeneration(TInfo view)
        {
            Guard.NotNull(() => view, view);

            this.BuildTypeMap(view);
        }

        /// <summary>
        /// Builds a map of all the full type names that are used by the given 
        /// schema view and the final type name to use in templates.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "view")]
        private void BuildTypeMap(TInfo view)
        {
            this.AddUsedTypes(typeof(TInfo));
            this.AddUsedTypes(typeof(TRuntime));
            // Force ComponentModel and Drawing.Design to be there always.
            //base.TypeNameMap[typeof(ArrayConverter).FullName] = typeof(ArrayConverter).FullName;
            //base.TypeNameMap[typeof(UITypeEditor).FullName] = typeof(UITypeEditor).FullName;
        }
    }
}
