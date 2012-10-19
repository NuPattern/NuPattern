
namespace Microsoft.VisualStudio.Patterning.Extensibility
{
    /// <summary>
    /// CopyToOutput enum
    /// </summary>
    public enum CopyToOutput
    {
        /// <summary>
        /// Do not copy
        /// </summary>
        DoNotCopy = VSLangProj80.__COPYTOOUTPUTSTATE.COPYTOOUTPUTSTATE_Never,
        /// <summary>
        /// Copy always
        /// </summary>
        Always = VSLangProj80.__COPYTOOUTPUTSTATE.COPYTOOUTPUTSTATE_Always,
        /// <summary>
        /// Copy only if newer
        /// </summary>
        PreserveNewest = VSLangProj80.__COPYTOOUTPUTSTATE.COPYTOOUTPUTSTATE_PreserveNewestFile
    }
}
