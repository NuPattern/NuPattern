using NuPattern.Runtime.Bindings;
using NuPattern.Runtime.Composition;

namespace NuPattern.Runtime.Guidance
{
    internal class ConditionBinding : Binding<ICondition>
    {
        public ConditionBinding(INuPatternCompositionService featureComposition, string componentTypeId, params PropertyBinding[] propertyBindings)
            : base(featureComposition, componentTypeId, propertyBindings)
        {
        }

        public override bool Evaluate()
        {
            if (base.Evaluate())
            {
                this.HasErrors = !this.Value.Evaluate();
            }

            return !this.HasErrors;
        }
    }
}