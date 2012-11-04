using System;
using Microsoft.VisualStudio.Patterning.Extensibility.Binding;

namespace Microsoft.VisualStudio.Patterning.Library.Commands
{
	/// <summary>
	/// Represents the tag information to associate with an artifact reference.
	/// </summary>
	public class ReferenceTag
	{
		/// <summary>
		/// Attemps to deserialize a <see cref="ReferenceTag"/> from a 
		/// serialized string.
		/// </summary>
		/// <param name="serialized">The serialized reference tag.</param>
		/// <returns>The deserialized instance, or <see langword="null"/> if it can't be deserialized.</returns>
		public static ReferenceTag TryDeserialize(string serialized)
		{
			if (string.IsNullOrEmpty(serialized))
				return null;

			try
			{
				return BindingSerializer.Deserialize<ReferenceTag>(serialized);
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReferenceTag"/> class.
		/// </summary>
		public ReferenceTag()
		{
			this.Tag = string.Empty;
		}

		/// <summary>
		/// Gets or sets an optional tag value for the reference (could be a 
		/// coma-separated list of tags, etc.).
		/// </summary>
		public string Tag { get; set; }

		/// <summary>
		/// Gets or sets the name of the target file.
		/// </summary>
		public string TargetFileName { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether name synchronization should be performed.
		/// </summary>
		public bool SyncNames { get; set; }

		/// <summary>
		/// Gets or sets the id of the originating automation.
		/// </summary>
		public Guid Id { get; set; }
	}
}
