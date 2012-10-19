using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace Microsoft.VisualStudio.Patterning.Extensibility
{
	/// <summary>
	/// Same as IProjectTypeProvider from Feature Builder, but allow as to reexport.
	/// Export does not need additional attributes.
	/// </summary>
	[CLSCompliant(false)]
	public interface IPlatuProjectTypeProvider : IProjectTypeProvider
	{
	}
}
