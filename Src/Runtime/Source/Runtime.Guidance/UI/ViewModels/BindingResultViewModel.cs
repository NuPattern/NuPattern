using System.Collections.Generic;
using System.Linq;
using NuPattern.Runtime.Bindings;

namespace NuPattern.Runtime.Guidance.UI.ViewModels
{
    internal class BindingResultViewModel : TreeNodeViewModel<BindingResult>
    {
        public BindingResultViewModel(BindingResult bindingResult)
            : base(bindingResult)
        {
            this.Results = bindingResult.Errors.Cast<object>().Concat(bindingResult.InnerResults);
        }

        public string PropertyName
        {
            get { return this.Model.PropertyName; }
        }

        public IEnumerable<object> Results { get; private set; }

        public object Value
        {
            get { return this.Model.Value; }
        }
    }
}