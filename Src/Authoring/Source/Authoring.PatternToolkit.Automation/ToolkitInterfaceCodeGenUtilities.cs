using NuPattern.Runtime;
using NuPattern.Runtime.Schema;

namespace NuPattern.Authoring.PatternToolkit.Automation
{
    /// <summary>
    /// ToolkitInterface generation utilities
    /// </summary>
    public static class ToolkitInterfaceCodeGenUtilities
    {
        /// <summary>
        /// Loads a PatternModel definition into memory from the specified path. 
        /// </summary>
        /// <param name="patternModelFile">Full path to the pattern model</param>
        public static IPatternModelInfo LoadPatternModelFromFile(string patternModelFile)
        {
            Guard.NotNullOrEmpty(() => patternModelFile, patternModelFile);

            return PatternModelSerializationHelper.LoadPatternModelFromFile(patternModelFile);
        }
    }
}
