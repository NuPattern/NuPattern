using System;
using System.Runtime.Remoting.Messaging;
using NuPattern.Library.Automation;
using System.Collections.Generic;
using Microsoft.VisualStudio.Shell;

namespace NuPattern.Library.Commands
{
	/// <summary>
	/// Holds state related to the current template execution and unfolding 
	/// context.
	/// </summary>
	internal class UnfoldContext : MarshalByRefObject
	{
		private static readonly string TlsKey = typeof(UnfoldContext).FullName;

		/// <summary>
		/// Initializes a new instance of the <see cref="UnfoldContext"/> class.
		/// </summary>
		internal UnfoldContext(UnfoldKind kind)
		{
			this.Kind = kind;
		}

		/// <summary>
		/// Gets the kind of the active unfold operation.
		/// </summary>
		public UnfoldKind Kind { get; internal set; }

		/// <summary>
		/// Gets the template automation.
		/// </summary>
		public NuPattern.Library.Automation.TemplateAutomation Automation { get; internal set; }

		/// <summary>
		/// Gets the current unfold kind in effect, or <see cref="UnfoldKind.None"/> 
		/// if no unfold context is active.
		/// </summary>
		public static UnfoldContext Current
		{
			get
			{
				var context = (UnfoldContext)CallContext.LogicalGetData(TlsKey);

				return context ?? new UnfoldContext(UnfoldKind.None);
			}

			internal set
			{
				CallContext.LogicalSetData(TlsKey, value);
			}
		}

		public CommandAutomation CommandAutomation { get; internal set; }
	}
}