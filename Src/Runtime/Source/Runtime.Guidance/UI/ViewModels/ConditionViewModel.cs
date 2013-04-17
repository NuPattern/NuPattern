using System.Collections.Generic;
using System.Linq;
using NuPattern.Runtime.Bindings;

namespace NuPattern.Runtime.Guidance.UI.ViewModels
{
    internal class ConditionViewModel : TreeNodeViewModel<IBinding<ICondition>>
    {
        public ConditionViewModel(IBinding<ICondition> condition)
            : base(condition)
        {
        }

        public IEnumerable<BindingResultViewModel> EvaluationResults
        {
            get
            {
                this.Model.Evaluate();
                return this.Model.EvaluationResults.Select(r => new BindingResultViewModel(r));
            }
        }

        public bool HasErrors
        {
            get { return this.Model.HasErrors; }
        }

        public string UserMessage
        {
            get { return this.Model.UserMessage; }
        }

        public void Refresh()
        {
            this.OnPropertyChanged(() => this.HasErrors);
        }
    }
}