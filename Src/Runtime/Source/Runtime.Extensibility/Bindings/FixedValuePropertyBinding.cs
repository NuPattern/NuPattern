namespace NuPattern.Runtime.Bindings
{
    internal class FixedValuePropertyBinding : PropertyBinding
    {
        public FixedValuePropertyBinding(string propertyName, string value)
            : base(propertyName)
        {
            this.Value = value;
        }

        public string Value { get; private set; }

        public override void SetValue(object target)
        {
            base.SetValue(target, this.Value);
        }
    }
}