using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using NuPattern.Presentation;
using NuPattern.Runtime.Guidance.Properties;
using NuPattern.Runtime.Guidance.Workflow;

namespace NuPattern.Runtime.Guidance.UI.ViewModels
{
    internal class GuidanceActionViewModel : NodeViewModel
    {
        private IGuidanceExtension feature;

        internal GuidanceActionViewModel(IGuidanceExtension feature, IGuidanceAction model, ImageSource icon)
            : base(model, icon)
        {
            this.Node.HasStateOverrideChanged += (s, e) => this.OnPropertyChanged(() => this.HasStateOverride);
            this.RequiresUserAcceptance = model.Postconditions.OfType<UserAcceptanceBinding>().Any();
            this.Conditions = GetConditions(model);
            this.Model.StateChanged += (s, e) => this.RefreshDetails();
            this.feature = feature;
        }

        public bool HasStateOverride
        {
            get
            {
                return this.Node.HasStateOverride;
            }
            set
            {
                if (value)
                {
                    this.Node.SetState(NodeState.Completed, true);
                }
                else
                {
                    this.Node.ClearStateOverride();
                }
            }
        }

        public bool IsUserAccepted
        {
            get
            {
                return this.Node.IsUserAccepted;
            }
            set
            {
                if (this.Node.IsUserAccepted != value)
                {
                    //
                    // We don't allow users to "check" Disabled nodes
                    //
                    if (this.Node.State != NodeState.Disabled)
                    {
                        this.Node.IsUserAccepted = value;
                        this.OnPropertyChanged(() => this.IsUserAccepted);

                        //
                        // We do this twice on purpose to facilitate highly dependent
                        // graphs such as those which implement the exclusive decision pattern
                        // and have top level nodes (Decision) with state dependency on bottom
                        // level nodes (Merge)
                        //
                        GuidanceConditionsEvaluator.EvaluateGraph(feature);
                        GuidanceConditionsEvaluator.EvaluateGraph(feature);
                        GuidanceConditionsEvaluator.EvaluateGraph(feature);
                    }
                }
            }
        }

        public new IGuidanceAction Node
        {
            get { return (IGuidanceAction)this.Model; }
        }

        public bool RequiresUserAcceptance { get; private set; }

        public IEnumerable<LabelViewModel> Conditions { get; private set; }

        protected override void RegisterCommands()
        {
            this.RefreshDetailsCommand = new RelayCommand(() => this.RefreshDetails());
        }

        private static IEnumerable<LabelViewModel> GetConditions(IGuidanceAction model)
        {
            yield return new LabelViewModel(Resources.GuidanceActionViewModel_PreConditionsLabel, model.Preconditions);
            yield return new LabelViewModel(Resources.GuidanceActionViewModel_PostConditionsLabel, model.Postconditions);
        }

        private void RefreshDetails()
        {
            this.Conditions = GetConditions(this.Node);
            this.OnPropertyChanged(() => this.Conditions);
        }
    }
}
