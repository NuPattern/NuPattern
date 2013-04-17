using System;
using System.IO;
using System.Linq;
using System.Text;

namespace NuPattern.VisualStudio.Solution
{
    /// <summary>
    /// Provides usability helpers for <see cref="IItemContainer"/>.
    /// </summary>
    internal static class VsItemContainerExtensions
    {
        /// <summary>
        /// Gets the item that represents the file currently opened in an editor.
        /// </summary>
        /// <param name="solution">The solution to find the active editing document in.</param>
        /// <returns>The item or <see langword="null"/> if no item is currently being edited.</returns>
        public static IItem GetActiveEditorItem(this ISolution solution)
        {
            var vsSolution = solution.As<EnvDTE.Solution>();

            if (vsSolution == null)
                throw new NotSupportedException("Cannot retrieve a VS solution from the given solution interface.");

            var activeDocument = vsSolution.DTE.ActiveDocument;
            if (activeDocument == null)
                return null;

            return solution.Traverse().OfType<IItem>().FirstOrDefault(i => i.PhysicalPath == activeDocument.FullName);
        }

        /// <summary>
        /// Adds an entire directory, recursively, to the given parent container.
        /// </summary>
        /// <param name="parent">The parent container for the directory and its files.</param>
        /// <param name="sourceDirectory">The source directory, which can be anywhere in the 
        /// filesystem, including the same solution structure (even though not added to the project).</param>
        /// <returns>The added folder.</returns>
        public static IFolder AddDirectory(this IItemContainer parent, string sourceDirectory)
        {
            return (IFolder)new VsDirectoryTemplate(sourceDirectory).Unfold(Path.GetFileName(sourceDirectory), parent);
        }

        /// <summary>
        /// Adds an item under the <see cref="IItemContainer"/>.
        /// </summary>
        /// <param name="parent">The parent container to add the file to.</param>
        /// <param name="sourcePath">The full path of the source file to add to the item container.</param>
        /// <param name="name">The optional new name of the file under the parent container.</param>
        /// <param name="overwrite">Indicates whether to overwrite the target file if already exists.</param>
        /// <param name="openFile">Indicates whether to open the file after adding it to the hierarchy.</param>
        /// <returns>The added item.</returns>
        public static IItemContainer Add(
            this IItemContainer parent,
            string sourcePath,
            string name = null,
            bool overwrite = true,
            bool openFile = false)
        {
            return new VsFileTemplate(sourcePath, overwrite, openFile)
                .Unfold(name ?? Path.GetFileName(sourcePath), parent);
        }

        /// <summary>
        /// Adds the given item content as an item under the <see cref="IItemContainer"/>.
        /// </summary>
        /// <param name="parent">The parent container to add the file to.</param>
        /// <param name="content">The content to add to the item container.</param>
        /// <param name="name">The new name of the item under the parent container.</param>
        /// <param name="overwrite">Indicates whether to overwrite the target file if already exists.</param>
        /// <param name="openFile">Indicates whether to open the file after adding it to the hierarchy.</param>
        /// <param name="encoding">Optional encoding, which defaults to UTF8.</param>
        /// <returns>The added item.</returns>
        public static IItem AddContent(
            this IItemContainer parent,
            string content,
            string name,
            bool overwrite = true,
            bool openFile = false,
            Encoding encoding = null)
        {
            Guard.NotNullOrEmpty(() => name, name);

            return (IItem)new VsFileContentTemplate(content, overwrite, openFile, encoding)
                .Unfold(name, parent);
        }

        /// <summary>
        /// Sets the item content to the given string, optionally specifying the encoding.
        /// </summary>
        /// <param name="item">The item to overwrite.</param>
        /// <param name="content">The content to set for the item.</param>
        /// <param name="encoding">Optional encoding, which defaults to UTF8.</param>
        public static void SetContent(this IItem item, string content, Encoding encoding = null)
        {
            Guard.NotNull(() => item, item);
            Guard.NotNull(() => content, content);

            VsHelper.CheckOut(item.PhysicalPath);
            File.WriteAllText(item.PhysicalPath, content, encoding ?? Encoding.UTF8);
        }
    }
}
