using System.Drawing;
using System.IO;

namespace NuPattern.Drawing
{
    /// <summary>
    /// Helpers for the <see cref="Bitmap"/> class.
    /// </summary>
    public static class BitmapHelper
    {
        /// <summary>
        /// Loads a bitmap from file.
        /// </summary>
        public static Bitmap Load(string path, string filename)
        {
            try
            {
                if (!string.IsNullOrEmpty(filename))
                {
                    var fullPath = Path.Combine(path, filename);

                    if (File.Exists(fullPath))
                    {
                        return new Bitmap(Bitmap.FromFile(fullPath));
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Converts a <see cref="Bitmap"/> to a <see cref="Icon"/>
        /// </summary>
        public static Icon ToIcon(this Bitmap bitmap)
        {
            try
            {
                if (bitmap != null)
                {
                    return Icon.FromHandle(bitmap.GetHicon());
                }
            }
            catch
            {
                return null;
            }

            return null;
        }
    }
}
