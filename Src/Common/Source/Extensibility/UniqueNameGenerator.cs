using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Patterning.Extensibility
{
    /// <summary>
    /// A unique name generator for ensuring unique names
    /// </summary>
    public static class UniqueNameGenerator
    {
        /// <summary>
        /// Ensures the given name is unique within the set of given existing names.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "AtOne")]
        public static string EnsureUnique(string name, IEnumerable<string> existingNames, bool alwaysStartAtOne = false)
        {
            return EnsureUnique(name,
                newName => !existingNames.Contains(newName), alwaysStartAtOne);
        }

        /// <summary>
        /// Ensures the given name is unique by evaluating against the given condition.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "AtOne")]
        public static string EnsureUnique(string name, Func<string, bool> stopCondition, bool alwaysStartAtOne = false)
        {
            Guard.NotNull(() => stopCondition, stopCondition);

            var newName = string.Empty;

            if (!alwaysStartAtOne)
            {
                if (stopCondition(name))
                {
                    return name;
                }
            }

            for (ulong counter = 1L; counter < ulong.MaxValue; counter += (ulong)1L)
            {
                newName = string.Format(CultureInfo.InvariantCulture, "{0}{1}", new object[] { name, counter });

                if (stopCondition(newName))
                {
                    break;
                }
            }

            return newName;
        }
    }
}
