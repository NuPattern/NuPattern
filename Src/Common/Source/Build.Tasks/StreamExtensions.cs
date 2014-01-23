using System.IO;

namespace NuPattern.Build.Tasks
{
    /// <summary>
    /// Extensions to the <see cref="Stream"/> class.
    /// </summary>
    internal static class StreamExtensions
    {
        private const int MaxStreamSize = 4096;

        /// <summary>
        /// Copies the source stream to the target stream.
        /// </summary>
        public static void CopyStream(this Stream source, Stream target)
        {
            byte[] buffer = new byte[MaxStreamSize];
            int count;
            while ((count = source.Read(buffer, 0, MaxStreamSize)) > 0)
            {
                target.Write(buffer, 0, count);
            }
        }
    }
}
