using System.ComponentModel;

namespace Microsoft.VisualStudio.Patterning.Runtime
{
	/// <summary>
	/// Cardinality of the relationship.
	/// </summary>
	public enum Cardinality
	{
		/// <summary>
		/// OneToOne instance of this item.
		/// </summary>
		[Description("OneToOne instance of this item.")]
		OneToOne = 0,

		/// <summary>
		/// ZeroToOne instance of this item.
		/// </summary>
		[Description("ZeroToOne instance of this item.")]
		ZeroToOne = 1,

		/// <summary>
		/// OneToMany instance of this item.
		/// </summary>
		[Description("OneToMany instance of this item.")]
		OneToMany = 2,

		/// <summary>
		/// ZeroToMany instances of this item.
		/// </summary>
		[Description("ZeroToMany instances of this item.")]
		ZeroToMany = 3,
	}
}