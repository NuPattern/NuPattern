namespace NuPattern.VisualStudio.Solution.Templates
{
    /// <summary>
    /// A template wizard extension
    /// </summary>
    public interface IVsTemplateWizardExtension
    {
        /// <summary>
        /// Gets the fully qualified name of the wizard extension assembly.
        /// </summary>
        string Assembly { get; }

        /// <summary>
        /// Gets hte fully qualified name of the wizard extension class 
        /// </summary>
        string FullClassName { get; }
    }
}