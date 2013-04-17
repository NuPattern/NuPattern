using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;

namespace NuPattern.VisualStudio.Diagnostics
{
    /// <summary>
    /// A <see cref="TextWriter"/> for the Output Window.
    /// </summary>
    [CLSCompliant(false)]
    public class OutputWindowTextWriter : TextWriter
    {
        IVsOutputWindowPane outputPane;

        /// <summary>
        /// Creates a new instance of the <see cref="OutputWindowTextWriter"/> class.
        /// </summary>
        /// <param name="outputPane"></param>
        public OutputWindowTextWriter(IVsOutputWindowPane outputPane)
        {
            this.outputPane = outputPane;
        }

        /// <summary>
        /// Gets the encoding
        /// </summary>
        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }

        /// <summary>
        /// Writes a value
        /// </summary>
        public override void Write(string value)
        {
            outputPane.OutputStringThreadSafe(value);
        }

        /// <summary>
        /// Writes a new line
        /// </summary>
        public override void WriteLine()
        {
            outputPane.OutputStringThreadSafe(Environment.NewLine);
        }

        /// <summary>
        /// Writes a line value
        /// </summary>
        public override void WriteLine(string value)
        {
            Write(value);
            WriteLine();
        }
    }
}
