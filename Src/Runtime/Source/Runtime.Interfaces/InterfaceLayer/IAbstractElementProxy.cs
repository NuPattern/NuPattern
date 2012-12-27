using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;

namespace NuPattern.Runtime
{
	/// <summary>
	/// Proxy interface for abstract elements.
	/// </summary>
	/// <typeparam name="TInterface">The type of the strong-typed interface for the abstract element.</typeparam>
	[CLSCompliant(false)]
	public interface IAbstractElementProxy<TInterface> : IContainerProxy<TInterface>, IPropertyProxy<TInterface>, IFluentInterface
	{
	}
}
