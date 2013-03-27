namespace NuPattern.Runtime
{
	/// <summary>
	/// Represents a property in the model.
	/// </summary>
	public partial interface IProperty
	{
		/// <summary>
		/// Gets or sets the typed property value.
		/// </summary>
		object Value { get; set; }

		/// <summary>
		/// Resets the property value to its initial value. If a default value 
		/// was specified in the schema, it will be used, as well as a 
		/// value provider, if any.
		/// </summary>
		void Reset();
	}
}
