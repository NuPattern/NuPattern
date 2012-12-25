using System;

namespace NuPattern.Extensibility.UI
{
	/// <summary>
	/// Defines a base editor attribute.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes")]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class EditorBaseTypeAttribute : Attribute
	{
		/// <summary>
		/// Gets or sets the base type.
		/// </summary>
		public Type BaseType { get; set; }

		/// <summary>
		/// Creates a new instance of the <see cref="EditorBaseTypeAttribute"/> class.
		/// </summary>
		/// <param name="baseType"></param>
		public EditorBaseTypeAttribute(Type baseType)
		{
			this.BaseType = baseType;
		}
	}
}
