using System;
using System.Globalization;
using System.IO;
using System.Linq;
using NuPattern.Diagnostics;
using NuPattern.Runtime.Properties;
using NuPattern.Runtime.References;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Helper class to deal with relative paths.
    /// </summary>
    [CLSCompliant(false)]
    public class PathResolver
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor(typeof(PathResolver));
        private const string SafeDirSeparator = "``";
        private const int SafeDirCount = 8;

        /// <summary>
        /// Character that resolves an artifact link.
        /// </summary>
        public const string ResolveArtifactLinkCharacter = "~";

        /// <summary>
        /// Gets or Sets the path to be resolved.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or Sets the filename to be resolved.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the current element.
        /// </summary>
        public object Context { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IUriReferenceService"/>.
        /// </summary>
        public IUriReferenceService UriService { get; private set; }

        /// <summary>
        /// Creates a new instance of the <see cref="PathResolver"/> class.
        /// </summary>
        public PathResolver(object context, IUriReferenceService uriService, string path = null, string fileName = null)
        {
            Guard.NotNull(() => context, context);
            Guard.NotNull(() => uriService, uriService);

            this.Context = context;
            this.UriService = uriService;

            this.Path = path;
            this.FileName = fileName;
        }

        /// <summary>
        /// Tries to resolves the current element paths and filename.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "By Design")]
        public bool TryResolve(Func<IItemContainer, bool> artifactReferenceFilter = null)
        {
            if (artifactReferenceFilter == null)
                artifactReferenceFilter = item => true;
            try
            {
                Resolve(artifactReferenceFilter);
                return true;
            }
            catch (Exception ex)
            {
                tracer.TraceError(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Resolves the current element paths and filename.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Features.Diagnostics.ITraceSource.TraceVerbose(System.String)")]
        public void Resolve(Func<IItemContainer, bool> artifactReferenceFilter = null)
        {
            tracer.TraceVerbose(
                Resources.PathResolver_TraceResolveInitial, Path ?? string.Empty, FileName ?? string.Empty);

            if (artifactReferenceFilter == null)
                artifactReferenceFilter = item => true;

            ResolveArtifactLinkInPath(artifactReferenceFilter);

            if (!string.IsNullOrEmpty(this.Path))
            {
                tracer.TraceVerbose(
                    Resources.PathResolver_TraceResolveApplyingExpression, this.Path, ((IProductElement)this.Context).InstanceName);

                var evaluatedPath = ExpressionEvaluator.Evaluate(this.Context, this.Path);

                if (string.IsNullOrEmpty(evaluatedPath))
                {
                    tracer.TraceVerbose(
                        Resources.PathResolver_TraceResolveFailedExpressionEvaluation);

                    throw new InvalidOperationException(string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.PathResolver_PathEvaluatedEmpty,
                        this.Path));
                }

                evaluatedPath = PathResolver.Normalize(evaluatedPath);
                tracer.TraceVerbose(
                    Resources.PathResolver_TraceResolveEvaluatedPath, evaluatedPath);

                // Ensure item name does not contain any invalid file chars
                if (!DataFormats.IsValidSolutionPathName(evaluatedPath))
                {
                    evaluatedPath = DataFormats.MakeValidSolutionPathName(evaluatedPath);
                }

                tracer.TraceVerbose(Resources.PathResolver_TraceEvaluatedPath, this.Path, evaluatedPath);

                evaluatedPath = evaluatedPath.Trim('\\');

                this.Path = evaluatedPath;
            }

            tracer.TraceVerbose(
                Resources.PathResolver_TraceResolvePath, this.Path ?? string.Empty);

            if (!string.IsNullOrEmpty(this.FileName))
            {
                tracer.TraceVerbose(
                    Resources.PathResolver_TraceResolveApplyingExpression, this.FileName, ((IProductElement)this.Context).InstanceName);

                var evaluatedName = ExpressionEvaluator.Evaluate(this.Context, this.FileName);

                if (string.IsNullOrEmpty(evaluatedName))
                {
                    tracer.TraceVerbose(
                        Resources.PathResolver_TraceResolveFailedExpressionEvaluation);

                    throw new InvalidOperationException(string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.PathResolver_ErrorFileNameEvaluatedEmpty,
                        this.FileName));
                }

                tracer.TraceVerbose(
                    Resources.PathResolver_TraceResolveEvaluatedFileName2, evaluatedName);

                // Ensure item name does not contain any invalid file chars
                if (!DataFormats.IsValidSolutionItemName(evaluatedName))
                {
                    evaluatedName = DataFormats.MakeValidSolutionItemName(evaluatedName);
                }

                tracer.TraceInformation(
                    Resources.PathResolver_TraceEvaluatedFileName, this.FileName, evaluatedName);

                this.FileName = evaluatedName;
            }

            tracer.TraceVerbose(
                Resources.PathResolver_TraceResolveResolvedFileName, this.FileName ?? string.Empty);
        }

        /// <summary>
        /// Normalizes the specified path by removing extra ".." relative path moves.
        /// </summary>
        public static string Normalize(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            if (System.IO.Path.IsPathRooted(path))
            {
                if (System.IO.Path.GetPathRoot(path) == System.IO.Path.DirectorySeparatorChar.ToString())
                    // When the path starts with "\" it is considered to be rooted :(. 
                    // But we know that it's a solution relative path in that case, 
                    // so we just let it go through.
                    return path;
                else
                    // Otherwise, we simplify the path by returning its full name
                    return new DirectoryInfo(path).FullName;
            }

            // DirectoryInfo has the logic to remove relative path movements
            // and get back a normalized path, even if the target path does 
            // not exist, so we just append a safe root path and then remove it.

            var safeRoot = "C:\\" + string.Join(
                System.IO.Path.DirectorySeparatorChar.ToString(),
                Enumerable.Range(0, SafeDirCount).Select(i => SafeDirSeparator)) +
                "\\";

            // Wildcards are not valid path characters for the calculation.
            var safePath = path.Replace("*", "~~");
            var fullPath = new DirectoryInfo(System.IO.Path.Combine(safeRoot, safePath)).FullName;

            if (!fullPath.StartsWith(safeRoot, StringComparison.OrdinalIgnoreCase))
            {
                // A relative path move took the path outside of our "root" zone.
                // We need to re-add "..\" for each missing separator.
                var remainingSeparators = fullPath
                    .Split(new[] { SafeDirSeparator }, StringSplitOptions.None)
                    // we add 1 which because that's the first path after C:\\
                    .Count(s => s == System.IO.Path.DirectorySeparatorChar.ToString()) + 1;

                var missingSeparators = SafeDirCount - remainingSeparators;

                // Build the new safe root path that remains in the path.
                var newSafeRoot = "C:\\" + string.Join(
                    System.IO.Path.DirectorySeparatorChar.ToString(),
                    Enumerable.Range(0, remainingSeparators).Select(i => SafeDirSeparator)) +
                    "\\";

                // and remove it.
                fullPath = fullPath.Replace(newSafeRoot, "");

                // Now we need to restore the "..\\" from the path that took us out of 
                // our root. 
                var relativePathToRestore = string.Join(
                        System.IO.Path.DirectorySeparatorChar.ToString(),
                        Enumerable.Range(0, missingSeparators).Select(i => ".."));

                fullPath = System.IO.Path.Combine(relativePathToRestore, fullPath);
            }
            else
            {
                fullPath = fullPath.Replace(safeRoot, "");
            }

            // Restore the wildcards.
            fullPath = fullPath.Replace("~~", "*");

            if (fullPath.StartsWith(System.IO.Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                fullPath = fullPath.Substring(1);

            return fullPath;
        }

        /// <summary>
        /// Resolves the given path to a given type of solution item in the solution.
        /// </summary>
        public T ResolvePathToSolutionItem<T>(ISolution solution) where T : IItemContainer
        {
            Guard.NotNull(() => solution, solution);

            this.Resolve();
            if (!String.IsNullOrEmpty(this.Path))
            {
                var item = solution.Find<T>(this.Path).FirstOrDefault();
                if (item != null)
                {
                    return item;
                }
            }

            return default(T);
        }

        private void ResolveArtifactLinkInPath(Func<IItemContainer, bool> artifactReferenceFilter)
        {
            var productElement = this.Context as IProductElement;

            if (productElement == null)
                return;

            tracer.TraceVerbose(
                Resources.PathResolver_TraceResolveLinkInPathResolvingElement, productElement.InstanceName);

            var pathToResolve = this.Path;

            if (string.IsNullOrEmpty(pathToResolve))
            {
                tracer.TraceVerbose(
                    Resources.PathResolver_TraceResolveLinkInPathDefaulting, ResolveArtifactLinkCharacter);
                pathToResolve = ResolveArtifactLinkCharacter;
            }

            if (pathToResolve.StartsWith(ResolveArtifactLinkCharacter, StringComparison.Ordinal))
            {
                var ancestorWithArtifactLink = productElement.Traverse(
                    x => x.GetParentAutomation(),
                    x => x.TryGetReference(ReferenceKindConstants.ArtifactLink) != null);

                ThrowIfNoAncestorWithLink(productElement, ancestorWithArtifactLink, this.Path);

                tracer.TraceVerbose(
                    Resources.PathResolver_TraceResolveLinkInPathUsiingElementReference, ancestorWithArtifactLink.InstanceName);

                this.Path = ResolveRelativeTargetPath(ancestorWithArtifactLink, pathToResolve, artifactReferenceFilter);
            }
            else if (pathToResolve.StartsWith("..", StringComparison.Ordinal))
            {
                var parentWithArtifactLink = LocateRelativeParent(productElement, pathToResolve);

                ThrowIfNoRelativeParent(parentWithArtifactLink, this.Path);
                ThrowIfNoLinkOnParent(parentWithArtifactLink, this.Path);

                tracer.TraceVerbose(
                    Resources.PathResolver_TraceResolveLinkInPathUsiingElementReference, parentWithArtifactLink.InstanceName);

                this.Path = ResolveRelativeTargetPath(parentWithArtifactLink, pathToResolve, artifactReferenceFilter);
            }
        }

        internal static IProductElement LocateRelativeParent(IProductElement context, string path)
        {
            var parentWithArtifactLink = context;
            // Make the path relative to the element asset link.
            if (path.StartsWith("..", StringComparison.Ordinal))
            {
                var parts = path.Split(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
                foreach (var part in parts)
                {
                    if (part.Equals("..", StringComparison.Ordinal))
                    {
                        parentWithArtifactLink = parentWithArtifactLink.GetParentAutomation();

                        tracer.TraceVerbose(
                            Resources.PathResolver_TraceLocateParentResolvingElement, part, parentWithArtifactLink.InstanceName);

                        if (parentWithArtifactLink == null)
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return parentWithArtifactLink;
        }

        /// <summary>
        /// Resolves the current element paths and filename, and returns the item if exists.
        /// </summary>
        public static IItemContainer ResolveToSolutionItem<T>(object context, ISolution solution, IUriReferenceService uriService, string path = null, string fileName = null) where T : IItemContainer
        {
            // Resolve SourcePath
            var resolver = new PathResolver(context, uriService, path, fileName);
            if (resolver.TryResolve())
            {
                // Load file from solution
                var sourceFile = resolver.Path;
                return solution.Find<T>(sourceFile).FirstOrDefault();
            }

            return null;
        }

        private string ResolveRelativeTargetPath(IProductElement context, string path, Func<IItemContainer, bool> artifactReferenceFilter)
        {
            // TODO: what if there are many references? Is it safe to just get the first one?
            var targetItem = SolutionArtifactLinkReference.GetResolvedReferences(context, this.UriService).FirstOrDefault(artifactReferenceFilter);
            ThrowIfInvalidArtifactLink(context, targetItem);

            var oldPath = path;

            var result = path
                .Replace("..\\", string.Empty)
                .Replace(".\\", string.Empty)
                .Replace("../", string.Empty)
                .Replace("./", string.Empty)
                .Replace(ResolveArtifactLinkCharacter + "\\", string.Empty)
                .Replace(ResolveArtifactLinkCharacter + "/", string.Empty)
                // This covers the case where we automatically added it t signal a solution path when it was empty.
                .Replace(ResolveArtifactLinkCharacter, string.Empty);

            result = System.IO.Path.Combine(targetItem.GetLogicalPath(), result);

            tracer.TraceVerbose(Resources.PathResolver_TraceResolvedArtifactLink, oldPath, result);

            tracer.TraceVerbose(
                Resources.PathResolver_TraceLocateParentUsingRelativePath, result);

            return result;
        }

        private static void ThrowIfInvalidArtifactLink(IProductElement parentWithArtifactLink, IItemContainer targetItem)
        {
            if (targetItem == null)
            {
                tracer.TraceVerbose(
                    Resources.PathResolver_TraceInvalidArtifactLink, parentWithArtifactLink.InstanceName);

                throw new InvalidOperationException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.PathResolver_ErrorInvalidArtifactLink,
                    parentWithArtifactLink.TryGetReference(ReferenceKindConstants.ArtifactLink),
                    parentWithArtifactLink.InstanceName));
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Features.Diagnostics.ITraceSource.TraceVerbose(System.String)")]
        private static void ThrowIfNoLinkOnParent(IProductElement context, string path)
        {
            if (string.IsNullOrEmpty(context.TryGetReference(ReferenceKindConstants.ArtifactLink)))
            {
                tracer.TraceVerbose(
                    Resources.PathResolver_TraceInvalidParentArtifactLink);

                throw new InvalidOperationException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.PathResolver_ErrorNoArtifactLinkOnParent,
                    path,
                    context.InstanceName));
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Features.Diagnostics.ITraceSource.TraceVerbose(System.String)")]
        private static void ThrowIfNoRelativeParent(IProductElement context, string path)
        {
            if (context == null)
            {
                tracer.TraceVerbose(
                    Resources.PathResolver_TraceNoParent);

                throw new InvalidOperationException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.PathResolver_ErrorNoParentFound,
                    path));
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Features.Diagnostics.ITraceSource.TraceVerbose(System.String)")]
        private static void ThrowIfNoAncestorWithLink(IProductElement context, IProductElement ancestor, string path)
        {
            if (ancestor == null)
            {
                tracer.TraceVerbose(
                    Resources.PathResolver_TraceNoAncestor);

                throw new InvalidOperationException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.PathResolver_ErrorNoAncestorWithArtifactLink,
                    context.InstanceName,
                    path));
            }
        }
    }
}
