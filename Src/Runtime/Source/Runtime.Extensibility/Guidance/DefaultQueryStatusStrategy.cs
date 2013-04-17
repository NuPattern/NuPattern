using System.Linq;
using NuPattern.Runtime.Guidance.Workflow;

namespace NuPattern.Runtime.Guidance
{
    /// <summary>
    /// Default querystatus strategy shared by all launchpoints.
    /// </summary>
    internal class DefaultQueryStatusStrategy : IQueryStatusStrategy
    {
        private string launchPointName;

        public DefaultQueryStatusStrategy(string launchPointName)
        {
            this.launchPointName = launchPointName;
        }

        public QueryStatus QueryStatus(IFeatureExtension feature)
        {
            var status = new QueryStatus { Visible = false, Enabled = false };
            if (feature == null)
            {
                return status;
            }


            //
            // First, let's check to see if our feature has a workflow
            //
            if (feature.GuidanceWorkflow != null)
            {
                var associatedActions = feature.GuidanceWorkflow.Successors
                    .Traverse(node => node.Successors)
                    .OfType<IGuidanceAction>()
                    .Where(activity => activity.LaunchPoints.Contains(launchPointName))
                    .ToArray();

                // If there are no associated actions, it's visible + enabled.
                if (!associatedActions.Any())
                {
                    status.Enabled = status.Visible = true;
                }
                else
                {
                    var hasAvailableActions = associatedActions.Any(action => action.State == NodeState.Enabled);
                    status.Visible = status.Enabled = hasAvailableActions;
                }
            }
            else
            {
                //
                // If we don't have a workflow, look at the "active" workflow
                //
                if (feature.FeatureManager != null &&
                    feature.FeatureManager.ActiveFeature != null &&
                    feature.FeatureManager.ActiveFeature.GuidanceWorkflow != null)
                {
                    status.Enabled = status.Visible = true;
                    var associatedActions = feature.FeatureManager.ActiveFeature.GuidanceWorkflow.Successors
                        .Traverse(node => node.Successors)
                        .OfType<IGuidanceAction>()
                        .Where(activity => activity.LaunchPoints.Contains(launchPointName))
                        .ToArray();

                    // If there are no associated actions, it's visible + enabled.
                    if (!associatedActions.Any())
                    {
                        status.Enabled = status.Visible = true;
                    }
                    else
                    {
                        var hasAvailableActions = associatedActions.Any(action => action.State == NodeState.Enabled);
                        status.Visible = status.Enabled = hasAvailableActions;
                    }
                }
            }
            return status;
        }
    }
}