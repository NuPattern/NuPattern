using System;
using System.Runtime.InteropServices;


namespace NuPattern.VisualStudio.Solution
{
    /// <summary>
    /// Provides access to templates registered in an environment.
    /// </summary>
    [Guid("C2D64735-E15D-42B7-A6E5-6EBC6A632277")]
    public interface ITemplateService
    {
        /// <summary>
        /// Tries to find a registered Visual Studio template.
        /// </summary>
        /// <param name="nameOrId">Name or identifier of the registered template, such as 
        /// an item template ZIP file.</param>
        /// <param name="category">Category of the template, such as "CSharp" or "Modeling".</param>
        /// <returns>An instance of a registered template, or <see langword="null"/> 
        /// if none could be found.</returns>
        ITemplate Find(string nameOrId, string category);

        /// <summary>
        /// Tries to find a registered template for the given uri.
        /// </summary>
        /// <returns>An instance of a registered template, or <see langword="null"/> 
        /// if none could be found.</returns>
        /// <remarks>
        /// Leverages the <see cref="IUriReferenceService"/> to resolve the 
        /// <paramref name="uri"/> to a template.
        /// </remarks>
        ITemplate Find(Uri uri);
    }
}