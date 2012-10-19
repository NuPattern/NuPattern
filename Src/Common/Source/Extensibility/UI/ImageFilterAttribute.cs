using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio;

namespace Microsoft.VisualStudio.Patterning.Extensibility
{
	/// <summary>
	/// ImageFilter attribute that is used from <see cref="ImageEditor"/> to filter and configure itself.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class ImageFilterAttribute : Attribute
	{
		private ImageKind kind;
		private IEnumerable<string> extensions;

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageFilterAttribute"/> class.
		/// </summary>
		/// <param name="kind">The image kind.</param>
		public ImageFilterAttribute(ImageKind kind)
		{
			this.kind = kind;
		}

		/// <summary>
		/// Gets the kind.
		/// </summary>
		/// <value>The image kind.</value>
		public ImageKind Kind
		{
			get { return this.kind; }
		}

		/// <summary>
		/// Gets the dialog title.
		/// </summary>
		/// <value>The dialog title.</value>
		public string DialogTitle
		{
			get
			{
				switch (this.kind)
				{
					case ImageKind.Image:
						return Properties.Resources.SelectImage;
					case ImageKind.Bitmap:
						return Properties.Resources.SelectBitmap;
					case ImageKind.Icon:
						return Properties.Resources.SelectIcon;
					default:
						return Properties.Resources.SelectImage;
				}
			}
		}

		/// <summary>
		/// Gets the file filter.
		/// </summary>
		/// <value>The file filter.</value>
		public string FileFilter
		{
			get
			{
				switch (this.kind)
				{
					case ImageKind.Image:
						return Properties.Resources.ImageFilter;
					case ImageKind.Bitmap:
						return Properties.Resources.BitmapFilter;
					case ImageKind.Icon:
						return Properties.Resources.IconFilter;
					default:
						return Properties.Resources.ImageFilter;
				}
			}
		}

		/// <summary>
		/// Gets the extensions.
		/// </summary>
		/// <value>The extensions.</value>
		public IEnumerable<string> Extensions
		{
			get
			{
				if (this.extensions == null)
				{
					var list = new List<string>();
					string[] filterParts = this.FileFilter.Split('|');

					if (filterParts.Length > 0 && filterParts.Length % 2 == 0)
					{
						for (var i = 0; i < filterParts.Length; i += 2)
						{
							foreach (var pattern in filterParts[i + 1].Split(';'))
							{
								if (pattern.IndexOf('.') > 0)
								{
									list.Add(pattern.Substring(pattern.IndexOf('.')));
								}
							}
						}
					}

					this.extensions = list.AsEnumerable();
				}

				return this.extensions;
			}
		}

		/// <summary>
		/// Determines whether the specified file name is supported.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		public bool IsSupported(string fileName)
		{
			try
			{
				if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
				{
					return false;
				}

				var fileExtension = Path.GetExtension(fileName);

				foreach (string extension in this.Extensions)
				{
					if (StringComparer.OrdinalIgnoreCase.Equals(fileExtension, extension))
					{
						return true;
					}
				}

				return false;
			}
			catch (Exception ex)
			{
				if (ErrorHandler.IsCriticalException(ex))
				{
					throw;
				}

				return false;
			}
		}
	}
}