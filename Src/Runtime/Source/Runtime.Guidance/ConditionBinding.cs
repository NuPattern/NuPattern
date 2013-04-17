namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    internal class ConditionBinding : Binding<ICondition>
    {
        public ConditionBinding(IFeatureCompositionService featureComposition, string componentTypeId, params PropertyBinding[] propertyBindings)
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