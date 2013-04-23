using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using NuPattern.ComponentModel.Design;
using NuPattern.Reflection;
using NuPattern.Runtime.Properties;

namespace NuPattern.Runtime.Guidance.Workflow
{
    /// <summary>
    /// Defines the base class for all guidance nodes.
    /// </summary>
    public abstract class Node : INode
    {
        private List<Node> predecessors = new List<Node>();
        private List<Node> successors = new List<Node>();
        private Cardinality fanIn;
        private Cardinality fanOut;

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        /// <param name="stateComposer">The strategy to compose the state of this node from the predecessors state.</param>
        /// <param name="fanIn">The input cardinality of this predecessor.</param>
        /// <param name="fanOut">The output cardinality of this predecessor.</param>
        protected Node(IStateComposer stateComposer, Cardinality fanIn, Cardinality fanOut)
        {
            Guard.NotNull(() => stateComposer, stateComposer);

            this.StateComposer = stateComposer;
            this.fanIn = fanIn;
            this.fanOut = fanOut;
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };

        /// <summary>
        /// Occurs when state property changes.
        /// </summary>
        public event EventHandler StateChanged = (s, e) => { };

        /// <summary>
        /// Gets or sets the node name.
        /// </summary>
        /// <value>The process element name.</value>
        [DisplayNameResource(@"GuidanceNode_Name_DisplayName", typeof(Resources))]
        [DescriptionResource(@"GuidanceNode_Name_Description", typeof(Resources))]
        [ReadOnly(true)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the node parent object.  Used only to build the outline from the graph
        /// </summary>
        /// <value>The process parent object.</value>
        [Browsable(false)]
        public object ParentObject { get; set; }

        /// <summary>
        /// Gets or sets the node description.
        /// </summary>
        /// <value>The guidance predecessor description.</value>
        [DisplayNameResource(@"GuidanceNode_Description_DisplayName", typeof(Resources))]
        [DescriptionResource(@"GuidanceNode_Description_Description", typeof(Resources))]
        [ReadOnly(true)]
        public string Description { get; set; }

        /// <summary>
        /// Gets the predecessors of this node.
        /// </summary>
        [Browsable(false)]
        public IEnumerable<INode> Predecessors
        {
            get { return this.predecessors; }
        }

        /// <summary>
        /// Gets the node state.
        /// </summary>
        [DisplayNameResource(@"GuidanceNode_State_DisplayName", typeof(Resources))]
        [DescriptionResource(@"GuidanceNode_State_Description", typeof(Resources))]
        [ReadOnly(true)]
        public virtual NodeState State { get; protected set; }

        /// <summary>
        /// Gets the state composer which calculates the state of this node in the graph.
        /// </summary>
        [Browsable(false)]
        protected IStateComposer StateComposer { get; private set; }

        /// <summary>
        /// Gets the successors of this node.
        /// </summary>
        [Browsable(false)]
        public IEnumerable<INode> Successors
        {
            get { return this.successors; }
        }

        /// <summary>
        /// Optional guidance identifier associated with this node.
        /// </summary>
        [Browsable(false)]
        public string GuidanceId { get; set; }

        /// <summary>
        /// Optional link to documentation about this node.
        /// </summary>
        [DisplayNameResource(@"GuidanceNode_Link_DisplayName", typeof(Resources))]
        [DescriptionResource(@"GuidanceNode_Link_Description", typeof(Resources))]
        [ReadOnly(true)]
        public string Link { get; set; }

        /// <summary>
        /// Gets the type of this Node.
        /// </summary>
        [Browsable(false)]
        internal abstract NodeType Type { get; }

        /// <summary>
        /// Insert a workflow along side this node in a workflow
        /// (same successors and predecessors) and optionally delete this node
        /// </summary>
        /// <param name="w">Workflow to insert</param>
        /// <param name="replaceMode">True if this node is to be deleted after insert</param>
        public void InsertWorkflow(GuidanceWorkflow w, bool replaceMode)
        {
            Node startOfWorkflow;
            Node endOfWorkflow;

            if (this.predecessors.Count != 1 ||
                this.predecessors.Count != 1)
            {
                throw new Exception(Resources.Node_ErrorOnlySinglePredecessor);
            }


            if (w != null)
            {
                startOfWorkflow = w.InitialNode as Node;
                startOfWorkflow = startOfWorkflow.successors[0];

                endOfWorkflow = w.Successors.Traverse<INode>(s => s.Successors).OfType<IFinal>().FirstOrDefault() as Node;
                endOfWorkflow = endOfWorkflow.predecessors[0];

                startOfWorkflow.predecessors.Clear();
                endOfWorkflow.successors.Clear();
            }
            else
            {
                //
                // If the incoming "workflow" is null, then we simply want to delete this node.
                //
                // One of the assumptions made for "include://" nodes is that they have one and only one
                // successor and predecessor
                //
                startOfWorkflow = this.successors.ToArray()[0];
                endOfWorkflow = this.predecessors.ToArray()[0];
            }

            //
            // Make all of my predecessors point at the new start
            //
            foreach (Node pred in predecessors)
            {
                foreach (Node predRef in pred.successors.ToArray())
                {
                    if (predRef == this)
                    {
                        if (replaceMode)
                            pred.successors.Remove(this);
                        break;
                    }
                }
                pred.successors.Add(startOfWorkflow);
                startOfWorkflow.predecessors.Add(pred);
            }

            //
            // Make all of my successors point at my new end
            //
            foreach (Node succ in successors)
            {
                foreach (Node succRef in succ.predecessors.ToArray())
                {
                    if (succRef == this)
                    {
                        if (replaceMode)
                            succ.predecessors.Remove(this);
                        break;
                    }
                }
                succ.predecessors.Add(endOfWorkflow);
                endOfWorkflow.successors.Add(succ);
            }

        }

        /// <summary>
        /// Connects the <paramref name="nodes"/> as successors of this <see cref="Node"/>.
        /// </summary>
        /// <param name="nodes">The nodes to add as successors.</param>
        protected virtual void ConnectTo(params Node[] nodes)
        {
            if (nodes.Length == 0)
            {
                throw new ArgumentException(Resources.GuidanceNode_ParameterNotEmpty, "nodes");
            }

            foreach (var successor in nodes)
            {
                if (this.fanOut.To < this.successors.Count + 1)
                {
                    throw new InvalidOperationException(string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.GuidanceNode_OutputCardinalityExceed,
                        this.GetType().Name));
                }

                if (successor.fanIn.To < successor.predecessors.Count + 1)
                {
                    throw new InvalidOperationException(string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.GuidanceNode_InputCardinalityExceed,
                        successor.GetType().Name));
                }

                this.successors.Add(successor);
                successor.predecessors.Add(this);

                SubscribeStateChanged(successor);
            }
        }

        /// <summary>
        /// Called when a property changes.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Called when the the state property changes.
        /// </summary>
        protected void OnStateChanged()
        {
            this.OnPropertyChanged(Reflect<Node>.GetProperty(g => g.State).Name);
            this.StateChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Refreshes the state depending on the state of the predecessors and 
        /// successors, using the associated <see cref="StateComposer"/>.
        /// </summary>
        /// <devdoc>
        /// We save this calculation of the node state in the current graph location 
        /// (WRT predecessors) as an optimization for the PropertyChanged event. 
        /// We want to prevent a case where an already blocked node (i.e. a join) 
        /// raises the changed event when one of its predecessors changes to Blocked. 
        /// In this case, the predecessor would have changed its state, but the 
        /// join itself would not, as it was already blocked. This optimization 
        /// would benefit the entire tree spanning from that join, potentially 
        /// reducing quite importantly the refresh rates of the graph UI. 
        /// This is important for all kinds of nodes, and is virtual because 
        /// different ones have different calculation of their state WRT 
        /// to the predecessors.
        /// </devdoc>
        protected virtual void ComposeState(StateChangeSource changeSource)
        {
            var calculatedState = this.StateComposer.ComposeState(this, changeSource);
            if (this.State != calculatedState)
            {
                this.State = calculatedState;
                this.OnStateChanged();
            }
        }

        /// <summary>
        /// Subscribe to the state change of the node.
        /// </summary>
        /// <param name="successor">The node to subscribe the state changes.</param>
        protected virtual void SubscribeStateChanged(Node successor)
        {
            this.StateChanged += (s, e) => successor.ComposeState(StateChangeSource.Predecessor);
            successor.ComposeState(StateChangeSource.Predecessor);

            successor.StateChanged += (s, e) => this.ComposeState(StateChangeSource.Successor);
            this.ComposeState(StateChangeSource.Successor);
        }
    }
}