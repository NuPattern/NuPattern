using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Modeling.ExtensionEnablement;

namespace NuPattern.Extensibility
{
	/// <summary>
	/// Authoring Command extension.
	/// </summary>
	[MetadataAttribute, AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class AuthoringCommandExtensionAttribute : ExportAttribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AuthoringCommandExtensionAttribute"/> class.
		/// </summary>
		public AuthoringCommandExtensionAttribute()
			: base(typeof(ICommandExtension))
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