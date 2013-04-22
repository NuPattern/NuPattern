namespace NuPattern.Runtime.Bindings
{
	/// <summary>
	/// A fixed value property binding.
	/// </summary>
    public class FixedValuePropertyBinding : PropertyBinding
    {
		/// <summary>
		/// Creates a new instance of the <see cref="FixedValuePropertyBinding"/> class.
		/// </summary>
        public FixedValuePropertyBinding(string propertyName, string value)
            : base(propertyName)
        {
            this.Value = value;
        }

		/// <summary>
		/// Gets the value of the binding.
		/// </summary>
        public string Value { get; private set; }

		/// <summary>
		/// Sets the value of the binding.
		/// </summary>
		/// <param name="target"></param>
        public override void SetValue(object target)
        {
            base.SetValue(target, this.Value);
        }
    }
}