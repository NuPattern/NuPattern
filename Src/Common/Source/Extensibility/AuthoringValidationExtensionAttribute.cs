using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Modeling.Validation;

namespace Microsoft.VisualStudio.Patterning.Extensibility
{
	/// <summary>
	/// Authoring Validation Extension MEF attribute.
	/// </summary>
	[MetadataAttribute, AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public sealed class AuthoringValidationExtensionAttribute : ExportAttribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AuthoringValidationExtensionAttribute"/> class.
		/// </summary>
		public AuthoringValidationExtensionAttribute()
			: base(typeof(Action<ValidationContext, object>))
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