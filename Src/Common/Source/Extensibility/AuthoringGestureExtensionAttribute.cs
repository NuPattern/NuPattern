using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Modeling.Diagrams.ExtensionEnablement;

namespace NuPattern.Extensibility
{
	/// <summary>
	/// Authoring Gesture Extension MEF attribute.
	/// </summary>
	[MetadataAttribute, AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class AuthoringGestureExtensionAttribute : ExportAttribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AuthoringGestureExtensionAttribute"/> class.
		/// </summary>
		public AuthoringGestureExtensionAttribute()
			: base(typeof(IGestureExtension))
		{
		}

		/// <summary>
		/// Gets the metadata filter.
		/// </summary>
		/// <value>The metadata filter.</value>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "MEF")]
		public object MetadataFilter
		{
			get { return ExtensibilityConstants.MetadataFilter; }
		}
	}
}