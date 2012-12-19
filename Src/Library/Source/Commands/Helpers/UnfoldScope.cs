using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Runtime;

namespace NuPattern.Library.Commands
{
	/// <summary>
	/// Holds state related to the current template execution and unfolding 
	/// context.
	/// </summary>
	[CLSCompliant(false)]
	public class UnfoldScope : MarshalByRefObject, IDisposable
	{
		private static readonly ITraceSource tracer = Tracer.GetSourceFor<UnfoldScope>();
		private static Stack<UnfoldScope> scopes = new Stack<UnfoldScope>(new UnfoldScope[] { null });

		private bool disposed;
		private string templateUri;
		private UnfoldScope parentScope;

		/// <summary>
		/// Initializes a new instance of the <see cref="UnfoldScope"/> class.
		/// </summary>
		public UnfoldScope(IAutomationExtension<IAutomationSettings> originatingAutomation, ReferenceTag artifactReferenceTag, string templateUri)
		{
			Guard.NotNull(() => originatingAutomation, originatingAutomation);
			Guard.NotNull(() => artifactReferenceTag, artifactReferenceTag);
			Guard.NotNullOrEmpty(() => templateUri, templateUri);

			this.parentScope = scopes.Peek();

			scopes.Push(this);
			this.templateUri = templateUri;
			this.Automation = originatingAutomation;
			this.ReferenceTag = artifactReferenceTag;
			tracer.TraceVerbose(Resources.UnfoldScope_Initialized, templateUri);
		}

		/// <summary>
		/// Gets a value indicating whether there is an active scope.
		/// </summary>
		public static bool IsActive
		{
			get { return scopes.Peek() != null; }
		}

		/// <summary>
		/// Gets the current scope.
		/// </summary>
		public static UnfoldScope Current
		{
			get { return scopes.Peek(); }
		}

		/// <summary>
		/// Gets the automation that created the scope.
		/// </summary>
		public IAutomationExtension<IAutomationSettings> Automation { get; private set; }

		/// <summary>
		/// Determines whether the specified template URI has already been unfolded in the current scope.
		/// </summary>
		public bool HasUnfolded(string templateUri)
		{
			return templateUri == this.templateUri ||
				(this.parentScope != null && parentScope.HasUnfolded(templateUri));
		}

		/// <summary>
		/// Gets or sets the unfolded item if any.
		/// </summary>
		public IItemContainer UnfoldedItem { get; set; }

		/// <summary>
		/// Gets the artifact link reference tag information to associated with the <see cref="UnfoldedItem"/>.
		/// </summary>
		public ReferenceTag ReferenceTag { get; private set; }

		/// <summary>
		/// Finalizes an instance of the <see cref="UnfoldScope"/> class.
		/// </summary>
		~UnfoldScope()
		{
			this.Dispose(false);
		}

		/// <summary>
		/// Clears the scope.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				while (Current != this && Current != null)
				{
					// This would automatically pop the nested scopes.
					Current.Dispose();
				}

				// Pop myself now.
				scopes.Pop();

				this.disposed = true;
			}
		}
	}
}