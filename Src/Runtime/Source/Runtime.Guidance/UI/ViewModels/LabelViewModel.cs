using System.Collections.Generic;
using System.Linq;
using NuPattern.Runtime.Bindings;

namespace NuPattern.Runtime.Guidance.UI.ViewModels
{
    internal class LabelViewModel : TreeNodeViewModel<string>
    {
        public LabelViewModel(string label, IEnumerable<IBinding<ICondition>> conditions)
            : base(label)
        {
            this.IsExpanded = true;
            this.Conditions = conditions.Select(c => new ConditionViewModel(c));
        }

        public IEnumerable<ConditionViewModel> Conditions { get; private set; }

        public string Label
        {
            get { return this.Model; }
        }
    }
}