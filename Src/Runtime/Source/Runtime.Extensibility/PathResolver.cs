using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
        private static readonly ITracer tracer = Tracer.Get(typeof(PathResolver));
        private const string SafeDirSeparator = "``";
        private const string NavigateUpCharacters = @"..";
        private const string NavigateCurrentCharacters = @".";
        private const int SafeDirCount = 8;
        private const string PathElemsCharsRegExpression = @"[\w\d\-]{1,}";
        private const string PathDirsCharsRegExpression = @"[ \w\d\.\,\{\}\(\)\-]{1,}";
        private const string PathTagCharsRegExpression = @"[ \w\d\;\,\-]{1,}";
        private const string PathTagRegExpression = @"\" + ResolveArtifactLinkCharacter + @"\[(?<tag>" + PathTagCharsRegExpression + @")\]";

        /// <summary>
        /// The regular expression to verify all paths
        /// </summary>
        /// <remarks>
        /// This guy is a monster!
        /// The use of group names is to help the reader understand it. These are not yet used in the code to parse it.
        /// Basic Rules: 
        ///  Broken in the [model path] and the [physical path], separated by the '~' character.
        /// If no '~' then it is treated as a physical path. All slashes can be either '\' or '/'
        /// In the model part, we are expecting: '.\' or '..\' or the name of a ancestor/relative element plus a slash, this is recursive until we reach the '~' or end of path
        /// Names for ancestors/relative is basic: any word, digit or dash. No spaces or wierd chars. Basically CSharp identifier chars
        /// In the physical part, we are expecting a '~'. It may come with a tag in square brackets. Only simple tag values and some simple delimitiers are allowed here
        /// After the '~' whether there is one or not, these paths can start with slashes or not, they can be '.\' or '..\' or a foldername, this is recursive until we reach the end of the path.
        /// Names for folders are pretty normal file characters.
        /// </remarks>
        public const string PathRegExpression = @"^(?<model>(?<navs>(\.[\/\\]{1})|(\.\.[\/\\]{1})|(?<elems>" 
            + PathElemsCharsRegExpression + @"[\/\\]{1})){0,}){1}" +
            @"(?<physical>((?<sep>[\" + ResolveArtifactLinkCharacter + @"]{0,1})(?<tag>(\[" 
            + PathTagCharsRegExpression + @"\])){0,1}(?<path>(?<topdir>([\/\\]{1})|([\.]{1,2}[\/\\])){0,1}(?<dirs>" 
            + PathDirsCharsRegExpression + @"[\/\\]{0,1}){0,})){1}){1}$";

        /// <summary>
        /// Character for delimiting artifact link tags
        /// </summary>
        public const char ReferenceTagDelimiter = ',';

        /// <summary>
        /// Character that resolves an artifact link.
        /// </summary>
        public const string ResolveArtifactLinkCharacter = @"~";

        /// <summary>
        /// Character that represents a wildcard.
        /// </summary>
        public const string WildcardCharacter = @"*";

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
        public bool TryResolve(Func<IItemContainer, bool> solutionItemFilter = null)
        {
            if (solutionItemFilter == null)
                solutionItemFilter = item => true;
            try
            {
                Resolve(solutionItemFilter);
                return true;
            }
            catch (Exception ex)
            {
                tracer.Error(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Resolves the current element paths and filename.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Features.Diagnostics.ITracer.Verbose(System.String)")]
        public void Resolve(Func<IItemContainer, bool> solutionItemFilter = null)
        {
            tracer.Verbose(
                Resources.PathResolver_TraceResolveInitial, Path ?? string.Empty, FileName ?? string.Empty);

            if (!string.IsNullOrEmpty(this.Path) 
                && !Regex.IsMatch(this.Path, PathRegExpression, RegexOptions.IgnoreCase))
            {
                throw new InvalidOperationException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.PathResolver_ErrorPathInvalidFormat,
                    this.Path));
            }

            if (solutionItemFilter == null)
                solutionItemFilter = item => true;

            // Navigate the path and resolve the link (if any)
            ResolveArtifactLinkInPath(solutionItemFilter);

            if (!string.IsNullOrEmpty(this.Path))
            {
                tracer.Verbose(
                    Resources.PathResolver_TraceResolveApplyingExpression, this.Path, ((IProductElement)this.Context).GetSafeInstanceName());

                var evaluatedPath = ExpressionEvaluator.Evaluate(this.Context, this.Path);

                if (string.IsNullOrEmpty(evaluatedPath))
                {
                    tracer.Verbose(
                        Resources.PathResolver_TraceResolveFailedExpressionEvaluation);

                    throw new InvalidOperationException(string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.PathResolver_PathEvaluatedEmpty,
                        this.Path));
                }

                evaluatedPath = PathResolver.Normalize(evaluatedPath);
                tracer.Verbose(
                    Resources.PathResolver_TraceResolveEvaluatedPath, evaluatedPath);

                // Ensure item name does not contain any invalid file chars
                if (!DataFormats.IsValidSolutionPathName(evaluatedPath))
                {
                    evaluatedPath = DataFormats.MakeValidSolutionPathName(evaluatedPath);
                }

                tracer.Verbose(Resources.PathResolver_TraceEvaluatedPath, this.Path, evaluatedPath);

                evaluatedPath = evaluatedPath.Trim('\\');

                this.Path = evaluatedPath;
            }

            tracer.Verbose(
                Resources.PathResolver_TraceResolvePath, this.Path ?? string.Empty);

            if (!string.IsNullOrEmpty(this.FileName))
            {
                tracer.Verbose(
                    Resources.PathResolver_TraceResolveApplyingExpression, this.FileName, ((IProductElement)this.Context).GetSafeInstanceName());

                var evaluatedName = ExpressionEvaluator.Evaluate(this.Context, this.FileName);

                if (string.IsNullOrEmpty(evaluatedName))
                {
                    tracer.Verbose(
                        Resources.PathResolver_TraceResolveFailedExpressionEvaluation);

                    throw new InvalidOperationException(string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.PathResolver_ErrorFileNameEvaluatedEmpty,
                        this.FileName));
                }

                tracer.Verbose(
                    Resources.PathResolver_TraceResolveEvaluatedFileName2, evaluatedName);

                // Ensure item name does not contain any invalid file chars
                if (!DataFormats.IsValidSolutionItemName(evaluatedName))
                {
                    evaluatedName = DataFormats.MakeValidSolutionItemName(evaluatedName);
                }

                tracer.Info(
                    Resources.PathResolver_TraceEvaluatedFileName, this.FileName, evaluatedName);

                this.FileName = evaluatedName;
            }

            tracer.Verbose(
                Resources.PathResolver_TraceResolveResolvedFileName, this.FileName ?? string.Empty);
        }

        /// <summary>
        /// Normalizes the specified path by removing extra ".." relative path moves.
        /// </summary>
        internal static string Normalize(string path)
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

            var safeRoot = @"C:\" + string.Join(
                System.IO.Path.DirectorySeparatorChar.ToString(),
                Enumerable.Range(0, SafeDirCount).Select(i => SafeDirSeparator)) +
                System.IO.Path.DirectorySeparatorChar;

            // Wildcards are not valid path characters for the calculation.
            var safePath = path.Replace(WildcardCharacter, @"~~");
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
                var newSafeRoot = @"C:\" + string.Join(
                    System.IO.Path.DirectorySeparatorChar.ToString(),
                    Enumerable.Range(0, remainingSeparators).Select(i => SafeDirSeparator)) +
                    System.IO.Path.DirectorySeparatorChar;

                // and remove it.
                fullPath = fullPath.Replace(newSafeRoot, string.Empty);

                // Now we need to restore the "..\\" from the path that took us out of 
                // our root. 
                var relativePathToRestore = string.Join(
                        System.IO.Path.DirectorySeparatorChar.ToString(),
                        Enumerable.Range(0, missingSeparators).Select(i => NavigateUpCharacters));

                fullPath = System.IO.Path.Combine(relativePathToRestore, fullPath);
            }
            else
            {
                fullPath = fullPath.Replace(safeRoot, string.Empty);
            }

            // Restore the wildcards.
            fullPath = fullPath.Replace(@"~~", WildcardCharacter);

            if (fullPath.StartsWith(System.IO.Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                fullPath = fullPath.Substring(1);

            return fullPath;
        }

        private void ResolveArtifactLinkInPath(Func<IItemContainer, bool> solutionItemFilter)
        {
            var productElement = this.Context as IProductElement;

            if (productElement == null)
                return;

            tracer.Verbose(
                Resources.PathResolver_TraceResolveLinkInPathResolvingElement, productElement.GetSafeInstanceName());

            var pathToResolve = this.Path;

            if (string.IsNullOrEmpty(pathToResolve))
            {
                tracer.Verbose(
                    Resources.PathResolver_TraceResolveLinkInPathDefaulting, ResolveArtifactLinkCharacter);
                pathToResolve = ResolveArtifactLinkCharacter;
            }

            // Navigate up hierarchy for first ancestor with artifact link
            if (pathToResolve.StartsWith(ResolveArtifactLinkCharacter, StringComparison.Ordinal))
            {
                var ancestorWithArtifactLink = productElement.Traverse(
                    x => x.GetParent(),
                    x => x.TryGetReference(ReferenceKindConstants.SolutionItem) != null);

                ThrowIfNoAncestorWithLink(productElement, ancestorWithArtifactLink, this.Path);

                tracer.Verbose(
                    Resources.PathResolver_TraceResolveLinkInPathUsingElementReference, ancestorWithArtifactLink.GetSafeInstanceName());

                this.Path = ResolveRelativeTargetPath(ancestorWithArtifactLink, pathToResolve, solutionItemFilter);
            }
                // Otherwise, lets see if we have to navigate the model at all
            else if (pathToResolve.Contains(ResolveArtifactLinkCharacter)) 
            {
                // Navigate to location in specified path
                var elementToResolve = LocateHierarchyContext(productElement, pathToResolve);

                // Ensure we have a link at the element at this point in the path
                ThrowIfNoContext(elementToResolve, this.Path);
                ThrowIfNoLinkOnElement(elementToResolve, this.Path);

                tracer.Verbose(
                    Resources.PathResolver_TraceResolveLinkInPathUsingElementReference, elementToResolve.GetSafeInstanceName());

                // Resolve the link
                this.Path = ResolveRelativeTargetPath(elementToResolve, pathToResolve, solutionItemFilter);
            }
        }

        private static IProductElement LocateHierarchyContext(IProductElement context, string path)
        {
            var currentContext = context;

            // Ensure we have a navigation expression up or down the hierarchy
            if (path.Contains(ResolveArtifactLinkCharacter))
            {
                var parts = path.Split(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
                foreach (var part in parts)
                {
                    // Check if we reached the resolve character
                    if (part.StartsWith(PathResolver.ResolveArtifactLinkCharacter))
                    {
                        break;
                    }

                    // Check for a navigate up the hierarchy to a parent element
                    if (part.Equals(NavigateUpCharacters, StringComparison.Ordinal))
                    {
                        currentContext = currentContext.GetParent();

                        tracer.Verbose(
                            Resources.PathResolver_TraceLocateParentResolvingElement, part, currentContext.GetSafeInstanceName());

                        if (currentContext == null)
                        {
                            break;
                        }

                        continue;
                    }

                    // Check for a navigate to current element
                    if (part.Equals(NavigateCurrentCharacters, StringComparison.Ordinal))
                    {
                        // Ignore this part
                        continue;
                    }

                    // Check for a navigate down hierarchy to a child element
                    var childElement = GetChildElement(currentContext, part);
                    if (childElement != null)
                    {
                        currentContext = childElement;

                        tracer.Verbose(
                            Resources.PathResolver_TraceLocateParentResolvingElement, part, currentContext.GetSafeInstanceName());

                        continue;
                    }

                    break;
                }
            }

            return currentContext;
        }

        private static IProductElement GetChildElement(IProductElement context, string name)
        {
            // locate a child instance that is a collection or element, and has a Any-To-One cardinality
            var element = context.GetChildren()
                .FirstOrDefault(c => c.DefinitionName.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (element != null)
            {
                var abstractElement = element as IAbstractElement;
                if (abstractElement != null && abstractElement.Info != null)
                {
                    if (abstractElement.Info.Cardinality.IsAnyToOne())
                    {
                        return element;
                    }
                }
            }

            return null;
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

        private string ResolveRelativeTargetPath(IProductElement context, string path, Func<IItemContainer, bool> solutionItemFilter)
        {
            // resolve the artifact link on current element
            var targetItem = ResolveReference(context, solutionItemFilter, path);
            ThrowIfInvalidArtifactLink(context, targetItem);

            var oldPath = path;
            var result = path;
            
            // Remove the left side of the expression (model path + ~ + [tag]) leaving just the physical path
            if (path.Contains(ResolveArtifactLinkCharacter))
            {
                // remove everything up to ~, and any tag expression
                var indexOfResolveChar = result.LastIndexOf(ResolveArtifactLinkCharacter,
                    System.StringComparison.OrdinalIgnoreCase);
                var indexOfStartPhysicalPath =
                    result.IndexOfAny(
                        new[] {System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar},
                        indexOfResolveChar);
                result = result.Substring(((indexOfStartPhysicalPath > indexOfResolveChar) ? indexOfStartPhysicalPath : indexOfResolveChar) + 1);
            }

            result = System.IO.Path.Combine(targetItem.GetLogicalPath(), result);

            tracer.Verbose(Resources.PathResolver_TraceResolvedArtifactLink, oldPath, result);

            tracer.Verbose(
                Resources.PathResolver_TraceLocateParentUsingRelativePath, result);

            return result;
        }

        private IItemContainer ResolveReference(IProductElement context, Func<IItemContainer, bool> solutionItemFilter, string path)
        {
            // create a filter function the matches any links that include the tag (if any)
            var tagFilter = new Func<IReference, bool>(x => true);
            var tag = GetArtifactReferenceTag(path);
            if (!string.IsNullOrEmpty(tag))
            {
                tagFilter = r => r.ContainsTag(tag);
            }

            // filter by tag, and then by solution item constraints
            var references = SolutionArtifactLinkReference.GetResolvedReferences(context, this.UriService, tagFilter)
                .Where(solutionItemFilter);
            if (references.Count() > 1)
            {
                tracer.Warn(
                    Resources.PathResolver_TraceResolvedMultipleReferences, context.GetSafeInstanceName());
            }

            return references
                .FirstOrDefault();
        }

        private static string GetArtifactReferenceTag(string path)
        {
            if (path.Contains(ResolveArtifactLinkCharacter))
            {
                var match = Regex.Match(path, PathTagRegExpression, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    return match.Groups["tag"].Value;
                }
            }

            return null;
        }

        private static void ThrowIfInvalidArtifactLink(IProductElement elementWithArtifactLink, IItemContainer targetItem)
        {
            if (targetItem == null)
            {
                tracer.Verbose(
                    Resources.PathResolver_TraceInvalidArtifactLink, elementWithArtifactLink.GetSafeInstanceName());

                throw new InvalidOperationException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.PathResolver_ErrorInvalidArtifactLink,
                    elementWithArtifactLink.TryGetReference(ReferenceKindConstants.SolutionItem),
                    elementWithArtifactLink.GetSafeInstanceName()));
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Features.Diagnostics.ITracer.Verbose(System.String)")]
        private static void ThrowIfNoLinkOnElement(IProductElement context, string path)
        {
            if (string.IsNullOrEmpty(context.TryGetReference(ReferenceKindConstants.SolutionItem)))
            {
                tracer.Verbose(
                    Resources.PathResolver_TraceInvalidParentArtifactLink);

                throw new InvalidOperationException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.PathResolver_ErrorNoArtifactLinkOnParent,
                    path,
                    context.GetSafeInstanceName()));
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Features.Diagnostics.ITracer.Verbose(System.String)")]
        private static void ThrowIfNoContext(IProductElement context, string path)
        {
            if (context == null)
            {
                tracer.Verbose(
                    Resources.PathResolver_TraceNoParent);

                throw new InvalidOperationException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.PathResolver_ErrorNoParentFound,
                    path));
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Features.Diagnostics.ITracer.Verbose(System.String)")]
        private static void ThrowIfNoAncestorWithLink(IProductElement context, IProductElement ancestor, string path)
        {
            if (ancestor == null)
            {
                tracer.Verbose(
                    Resources.PathResolver_TraceNoAncestor);

                throw new InvalidOperationException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.PathResolver_ErrorNoAncestorWithArtifactLink,
                    context.GetSafeInstanceName(),
                    path));
            }
        }
    }
}
