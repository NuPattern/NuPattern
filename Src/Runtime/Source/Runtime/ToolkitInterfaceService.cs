using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Patterning.Runtime
{
	[PartCreationPolicy(CreationPolicy.Shared)]
	[Export(typeof(IToolkitInterfaceService))]
	internal class ToolkitInterfaceService : IToolkitInterfaceService
	{
		[ImportMany]
		public IEnumerable<Lazy<IToolkitInterface, IToolkitInterfaceMetadata>> AllInterfaces { get; set; }
	}
}
