using System;

namespace Microsoft.VisualStudio.Patterning.Extensibility
{
	/// <summary>
	/// Specifies that the annotated property should use the given property descriptor to 
	/// wrap the default one.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class PropertyDescriptorAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyDescriptorAttribute"/> class.
		/// </summary>
		/// <param name="descriptorType">Type of the descriptor.</param>
		public PropertyDescriptorAttribute(Type descriptorType)
		{
			this.DescriptorType = descriptorType;
		}

		/// <summary>
		/// Type for the Descriptor
		/// </summary>
		public Type DescriptorType { get; private set; }
	}
}
