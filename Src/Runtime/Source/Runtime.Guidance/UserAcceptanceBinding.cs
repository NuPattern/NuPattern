using System;
using System.Collections.Generic;
using NuPattern.Runtime.Bindings;
using NuPattern.Runtime.Guidance.Properties;
using NuPattern.Runtime.Guidance.Workflow;

namespace NuPattern.Runtime.Guidance
{
    internal class UserAcceptanceBinding : IBinding<ICondition>
    {
        public UserAcceptanceBinding(IGuidanceAction action)
        {
            this.UserMessage = Resources.UserAcceptanceBinding_UserMessage;
            this.Value = new UserAcceptanceCondition(action);
            this.ComponentId = Guid.NewGuid().ToString();
        }

        public string ComponentId { get; private set; }

        public IEnumerable<BindingResult> EvaluationResults
        {
            get { yield break; }
        }

        public bool HasErrors { get; private set; }

        public string UserMessage { get; set; }

        public ICondition Value { get; private set; }

        public bool Evaluate()
        {
            var evaluationResult = this.Value.Evaluate();
            this.HasErrors = !evaluationResult;
            return evaluationResult;
        }

        internal class UserAcceptanceCondition : ICondition
        {
            private IGuidanceAction action;

            public UserAcceptanceCondition(IGuidanceAction action)
            {
                this.action = action;
            }

            public bool Evaluate()
            {
                return this.action.IsUserAccepted;
            }
        }
    }
}