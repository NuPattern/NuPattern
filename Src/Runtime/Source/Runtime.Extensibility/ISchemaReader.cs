using System;
using System.IO;

namespace NuPattern.Runtime
{
	/// <summary>
	/// Provides persistence behavior for a schema.
	/// </summary>
	[CLSCompliant(false)]
	public interface ISchemaReader
	{
		/// <summary>
		/// Loads the model from the given <paramref name="stream"/>.
		/// </summary>
		IPatternModelInfo Load(Stream stream);
	}
}
