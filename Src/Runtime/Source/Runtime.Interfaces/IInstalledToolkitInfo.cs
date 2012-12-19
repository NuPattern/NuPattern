using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace NuPattern.Runtime
{
	/// <summary>
	/// Represents an installed extension that provides a toolkit.
	/// </summary>
	[CLSCompliant(false)]
	public interface IInstalledToolkitInfo : IToolkitInfo
	{
		/// <summary>
		/// Gets the installed extension information.
		/// </summary>
		IInstalledExtension Extension
		{
			get;
		}

		/// <summary>
		/// Gets the icon of the pattern.
		/// </summary>
		string PatternIconPath
		{
			get;
		}

		/// <summary>
		/// Gets the icon of the toolkit.
		/// </summary>
		string ToolkitIconPath
		{
			get;
		}

		/// <summary>
		/// Gets the schema information.
		/// </summary>
		IPatternModelInfo Schema
		{
			get;
		}

	    /// <summary>
	    /// Gets the VS Templates deployed by the toolkit.
	    /// </summary>
	    IEnumerable<IVsTemplate> Templates
	    {
	        get;
	    }
	}
}