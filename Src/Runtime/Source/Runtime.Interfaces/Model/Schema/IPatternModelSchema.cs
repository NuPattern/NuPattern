using System.Collections.Generic;

namespace Microsoft.VisualStudio.Patterning.Runtime
{
    /// <summary>
    /// Provides a model of pattern schema
    /// </summary>
    public partial interface IPatternModelSchema
    {
        /// <summary>
        /// Gets instances of a given type.
        /// </summary>
        IEnumerable<T> FindAll<T>();

        /// <summary>
        /// Determines whether pattern line is being tailored or authored.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if in tailor mode; otherwise, <c>false</c>.
        /// </returns>
        bool IsInTailorMode();
    }
}