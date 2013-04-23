using NuPattern.Diagnostics;
using NuPattern.Runtime.Properties;

namespace NuPattern.Runtime.Bindings
{
    /// <summary>
    /// A binding for a <see cref="IValueProvider"/>.
    /// </summary>
    public class ValueProviderPropertyBinding : PropertyBinding
    {
        private readonly ITraceSource tracer;

        /// <summary>
        /// Creates a new instance of the <see cref="ValueProviderPropertyBinding"/> class.
        /// </summary>
        public ValueProviderPropertyBinding(string propertyName, Binding<IValueProvider> binding)
            : base(propertyName)
        {
            this.Binding = binding;
            this.tracer = Tracer.GetSourceFor(this.GetType());
        }

        /// <summary>
        /// Gets the binding.
        /// </summary>
        public Binding<IValueProvider> Binding { get; private set; }

        /// <summary>
        /// Sets the value of the binding.
        /// </summary>
        /// <param name="target"></param>
        public override void SetValue(object target)
        {
            if (this.Binding.Evaluate())
            {
                var valueProvider = this.Binding.Value;
                this.SetValue(target, valueProvider.Evaluate());
            }
            else
            {
                tracer.TraceError(Resources.ValueProviderPropertyBinding_TraceFailedEvaluation,
                                  target, this.PropertyName, ObjectDumper.ToString(this.Binding.EvaluationResults, 5));
            }
        }
    }
}