using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media;
using NuPattern;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    /// <summary>
    /// Transforms the IGuidanceWorkflow instance into an NodeViewModel
    /// </summary>
    internal class DefaultWorkflowViewModelBuilder : IWorkflowViewModelBuilder
    {
        private static readonly Tuple<Type, ImageSource>[] nodeImages =
            new Tuple<Type, ImageSource>[]
            {
                new Tuple<Type, ImageSource>(typeof(IGuidanceAction), ActivityIcons.ActionIcon),
                new Tuple<Type, ImageSource>(typeof(IInitial), ActivityIcons.InitialIcon),
                new Tuple<Type, ImageSource>(typeof(IFinal), ActivityIcons.FinalIcon),
                new Tuple<Type, ImageSource>(typeof(IDecision), ActivityIcons.DecisionIcon),
                new Tuple<Type, ImageSource>(typeof(IMerge), ActivityIcons.MergeIcon),
                new Tuple<Type, ImageSource>(typeof(IFork), ActivityIcons.ForkIcon),
                new Tuple<Type, ImageSource>(typeof(IJoin), ActivityIcons.JoinIcon)
            };
        private Dictionary<INode, NodeViewModel> viewModelMappings;
        private IFeatureExtension feature;

        public DefaultWorkflowViewModelBuilder(IFeatureExtension feature)
        {
            Guard.NotNull(() => feature, feature);

            this.feature = feature;
        }

        public virtual IEnumerable<NodeViewModel> GetNodes()
        {
            var workflow = this.feature.GuidanceWorkflow;
            if (workflow == null || workflow.InitialNode == null)
            {
                yield break;
            }

            this.viewModelMappings = new Dictionary<INode, NodeViewModel>();

            var rootViewModel = this.GetNodeViewModel(workflow.InitialNode);
            this.BuildTreeFromGraph(rootViewModel, workflow.InitialNode.Successors);
            yield return rootViewModel;
        }

        protected virtual void BuildTreeFromGraph(TreeNodeViewModel<INode> parentNode, IEnumerable<INode> nodes)
        {
            foreach (var node in nodes)
            {
                if (node is IFork)
                {
                    IJoin matchingJoin = FindMatchingJoin(node);
                    if (matchingJoin == null)
                        throw new Exception("Could not find matching Join for Fork");
                    matchingJoin.ParentObject = parentNode;
                }

                if (node is IDecision)
                {
                    IMerge matchingMerge = FindMatchingMerge(node);
                    if (matchingMerge == null)
                        throw new Exception("Could not find matching Merge for Decision");
                    matchingMerge.ParentObject = parentNode;
                }


                if (node is IJoin)
                {
                    if (node.ParentObject == null)
                        throw new Exception("Encountered Join with parent not set");
                    this.BuildTreeFromGraph((TreeNodeViewModel<INode>)node.ParentObject, node.Successors);
                }
                else if (node is IMerge)
                {
                    if (node.ParentObject == null)
                        throw new Exception("Encountered Merge with parent not set");
                    this.BuildTreeFromGraph((TreeNodeViewModel<INode>)node.ParentObject, node.Successors);
                }
                else if (!(node is IFinal))
                {
                    var childViewModel = this.GetNodeViewModel(node);
                    childViewModel.ParentNode =
                        parentNode.ParentNode != null && !(parentNode.Model is IFork) && !(parentNode.Model is IDecision) ?
                        parentNode.ParentNode :
                        parentNode;
                    this.BuildTreeFromGraph(childViewModel, node.Successors);
                }
            }
        }

        private IMerge FindMatchingMerge(INode node)
        {
            IMerge result = null;
            int level = 0;
            INode nextNode = node.Successors.FirstOrDefault();

            while (nextNode != null && !(nextNode is IFinal))
            {
                if (nextNode is IDecision)
                    level++;
                else if (nextNode is IMerge)
                {
                    if (level == 0)
                    {
                        result = nextNode as IMerge;
                        break;
                    }
                    else
                        level--;
                }
                nextNode = nextNode.Successors.FirstOrDefault();
            }

            return result;
        }

        private IJoin FindMatchingJoin(INode node)
        {
            IJoin result = null;
            int level = 0;
            INode nextNode = node.Successors.FirstOrDefault();

            while (nextNode != null && !(nextNode is IFinal))
            {
                if (nextNode is IFork)
                    level++;
                else if (nextNode is IJoin)
                {
                    if (level == 0)
                    {
                        result = nextNode as IJoin;
                        break;
                    }
                    else
                        level--;
                }
                nextNode = nextNode.Successors.FirstOrDefault();
            }

            return result;
        }

        protected virtual ImageSource GetImageForNode(INode node)
        {
            if (node != null)
            {
                var nodeType = node.GetType();
                return nodeImages.Where(nodeImage => nodeImage.Item1.IsAssignableFrom(nodeType))
                    .Select(nodeImage => nodeImage.Item2)
                    .FirstOrDefault();
            }

            return null;
        }

        protected virtual string GetNodeDisplayName(INode node)
        {
            if (!Regex.IsMatch(node.Name, @"^[A-Z][a-zA-Z]+\d+$"))
            {
                return node.Name;
            }

            if (node is IFork || node is IJoin || node is IDecision || node is IMerge)
            {
                return string.Empty;
            }

            if (node is IInitial)
            {
                return "Start";
            }

            if (node is IFinal)
            {
                return "End";
            }

            return node.GetType().Name;
        }

        protected NodeViewModel GetNodeViewModel(INode node)
        {
            NodeViewModel viewModel;
            if (!this.viewModelMappings.TryGetValue(node, out viewModel))
            {
                if (node is IGuidanceAction)
                {
                    viewModel = new GuidanceActionViewModel(feature, (IGuidanceAction)node, this.GetImageForNode(node));
                }
                else
                {
                    viewModel = new NodeViewModel(node, this.GetImageForNode(node), this.GetNodeDisplayName(node));
                }

                viewModel.IsExpanded = true;
                this.viewModelMappings.Add(node, viewModel);
            }

            return viewModel;
        }
        /// <summary>
        /// Search the parent node in the view model for a node that presents node pair, eg.: Fork/Join or Desicion/Merge.
        /// </summary>
        protected TreeNodeViewModel<INode> GetParentNodeViewModel<T>(INode node)
        {
            var pairNode = node.Predecessors.Traverse(n => n.Predecessors).FirstOrDefault(n => n is T);
            if (pairNode != null)
            {
                return this.GetNodeViewModel(pairNode).ParentNode;
            }

            return null;
        }
    }
}