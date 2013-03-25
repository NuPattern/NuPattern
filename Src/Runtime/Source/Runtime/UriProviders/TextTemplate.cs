using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Modeling.Integration;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using NuPattern.IO;
using NuPattern.Runtime.Properties;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Runtime.UriProviders
{
    /// <summary>
    /// Implementation of <see cref="ITemplate"/> for text templates (T4).
    /// </summary>
    /// <remarks>
    /// Templates can use the variable <c>%TemplatePath%</c> in includes and 
    /// assembly directives, which will be replaced with the directory of the template.
    /// </remarks>
    internal class TextTemplate : ITemplate
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<TextTemplate>();
        private static readonly Regex AssemblyRegex = new Regex(@"(?<=<#@\s?assembly name="".*)%TemplatePath%", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private ITextTemplating templating;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private IModelBus modelBus;
        private string templateFile;
        private string templateContent;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextTemplate"/> class.
        /// </summary>
        public TextTemplate(ITextTemplating templating, IModelBus modelBus, string templateFile)
        {
            Guard.NotNull(() => templating, templating);
            Guard.NotNull(() => modelBus, modelBus);
            Guard.NotNullOrEmpty(() => templateFile, templateFile);

            if (!File.Exists(templateFile))
            {
                throw new FileNotFoundException(Resources.TextTemplate_FileNotFound, templateFile);
            }

            this.templating = templating;
            this.modelBus = modelBus;
            this.templateFile = templateFile;
            this.templateContent = ReplaceTemplatePathVariable(File.ReadAllText(templateFile));
        }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        public dynamic Parameters
        {
            get { return null; }
        }

        /// <summary>
        /// Transforms the template and adds the content with the given name to the specified parent.
        /// </summary>
        public IItemContainer Unfold(string name, IItemContainer parent)
        {
            Guard.NotNull(() => parent, parent);
            Guard.NotNullOrEmpty(() => name, name);

            //var sessionHost = this.templating as ITextTemplatingSessionHost;

            using (tracer.StartActivity(Resources.TextTemplate_TraceStartUnfold, this.templateFile, parent.GetLogicalPath()))
            {
                this.templating.BeginErrorSession();
                try
                {
                    // TODO: add support for parameters.
                    //foreach (var parameter in Parameters)
                    //{
                    //    sessionHost.Session[parameter.Name] = parameter.GetTypedValue();
                    //}
                    var callback = new TemplateCallback(tracer, templateFile);
                    var content = this.templating.ProcessTemplate(templateFile, templateContent, callback);
                    var targetName = name;
                    if (!Path.HasExtension(targetName))
                    {
                        targetName = targetName + callback.FileExtension;
                    }

                    // BUGFIX: FBRT does not checkout the file, if SCC.
                    var targetItem = parent.Find<IItem>(targetName).FirstOrDefault();
                    if (targetItem != null)
                    {
                        targetItem.Checkout();
                    }

                    using (new TempFileCleaner())
                    {
                        // HACK: \o/ feature runtime VsFileContentTemplate creates a temp file and 
                        // doesn't delete it, so we go FSW to detect it.
                        return parent.AddContent(content, targetName, true, false, callback.OutputEncoding);
                    }
                }
                finally
                {
                    this.templating.EndErrorSession();
                }
            }
        }

        private string ReplaceTemplatePathVariable(string content)
        {
            var path = Path.GetDirectoryName(templateFile);
            content = AssemblyRegex.Replace(content, value => path);

            return content;
        }

        private class TemplateCallback : ITextTemplatingCallback
        {
            private ITraceSource tracer;
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
            private string templateFile;

            public TemplateCallback(ITraceSource tracer, string templateFile)
            {
                this.tracer = tracer;
                this.templateFile = templateFile;
                this.FileExtension = string.Empty;
            }

            public string FileExtension { get; private set; }
            public Encoding OutputEncoding { get; private set; }

            public void ErrorCallback(bool warning, string message, int line, int column)
            {
                // TODO: see if it's useful to also add line/colum, or if the VS 
                // template service is already doing this for us.
                if (warning)
                {
                    this.tracer.TraceWarning(Resources.TextTemplate_WarningCallback, message, line, column);
                }
                else
                {
                    this.tracer.TraceError(Resources.TextTemplate_ErrorCallback, message, line, column);
                }
            }

            public void SetFileExtension(string extension)
            {
                this.FileExtension = extension;
            }

            public void SetOutputEncoding(Encoding encoding, bool fromOutputDirective)
            {
                this.OutputEncoding = encoding;
            }
        }
    }
}