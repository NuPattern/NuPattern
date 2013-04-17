using System.IO;
using System.Text;

namespace NuPattern.VisualStudio.Solution
{
    internal class VsFileContentTemplate : VsFileTemplate
    {
        public VsFileContentTemplate(string content, bool overwrite = true, bool openFile = false, Encoding encoding = null)
            : base(GetSourceFilePath(content, encoding ?? Encoding.UTF8), overwrite, openFile)
        {
        }

        private static string GetSourceFilePath(string content, Encoding encoding)
        {
            Guard.NotNull(() => content, content);

            var sourcePath = Path.GetTempFileName();
            File.WriteAllText(sourcePath, content, encoding);
            return sourcePath;
        }

        public override IItemContainer Unfold(string name, IItemContainer parent)
        {
            var container = base.Unfold(name, parent);
            File.Delete(this.SourcePath);
            return container;
        }

    }
}