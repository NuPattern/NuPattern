using System;
using System.Collections.Generic;
using System.ComponentModel;
using NuPattern.ComponentModel.Design;
using NuPattern.Reflection;
using NuPattern.Runtime.Bindings;
using NuPattern.Runtime.Properties;

namespace NuPattern.Runtime.Guidance.Workflow
{
    /// <summary>
    /// Implements a <see cref="Node"/> with intrinsic state that is determined by
    /// pre and post conditions, and which can optionally be forced (overriden).
    /// </summary>
    public abstract class ConditionalNode : Node, IConditionalNode
    {
        private NodeState? stateOverride;
        private bool isUserAccepted;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionalNode"/> class.
        /// </summary>
        /// <param name="stateComposer">The strategy to compose the state of this node from the predecessors state.</param>
        /// <param name="fanIn">The input cardinality of this predecessor.</param>
        /// <param name="fanOut">The output cardinality of this predecessor.</param>
        protected ConditionalNode(IStateComposer stateComposer, Cardinality fanIn, Cardinality fanOut)
            : base(stateComposer, fanIn, fanOut)
        {
            this.Preconditions = new List<IBinding<ICondition>>();
            this.Postconditions = new List<IBinding<ICondition>>();
        }

        /// <summary>
        /// Handles when the state is overridden
        /// </summary>
        public event EventHandler HasStateOverrideChanged = (s, e) => { };

        /// <summary>
        /// Handles when user accepts
        /// </summary>
        public event EventHandler IsUserAcceptedChanged = (s, e) => { };

        /// <summary>
        /// Gets the pre-conditions evaluated before the activity begins.
        /// </summary>
        /// <value>The pre-conditions evaluated before the activity begins.</value>
        [DisplayNameResource(@"GuidanceNode_PreConditions_DisplayName", typeof(Resources))]
        [DescriptionResource(@"GuidanceNode_PreConditions_Description", typeof(Resources))]
        public IList<IBinding<ICondition>> Preconditions { get; private set; }

        /// <summary>
        /// Gets the pre-conditions evaluated before the activity begins.
        /// </summary>
        /// <value>The pre-conditions evaluated before the activity begins.</value>
        IEnumerable<IBinding<ICondition>> IConditionalNode.Preconditions
        {
            get { return this.Preconditions; }
        }

        /// <summary>
        /// Gets the post-conditions evaluated when the activity is <see cref="NodeState.Enabled"/>.
        /// </summary>
        /// <value>The post-conditions evaluated when the activity is <see cref="NodeState.Enabled"/>.</value>
        [DisplayNameResource(@"GuidanceNode_PostConditions_DisplayName", typeof(Resources))]
        [DescriptionResource(@"GuidanceNode_PostConditions_Description", typeof(Resources))]
        public IList<IBinding<ICondition>> Postconditions { get; private set; }

        /// <summary>
        /// Gets the post-conditions evaluated when the activity is <see cref="NodeState.Enabled"/>.
        /// </summary>
        /// <value>The post-conditions evaluated when the activity is <see cref="NodeState.Enabled"/>.</value>
        IEnumerable<IBinding<ICondition>> IConditionalNode.Postconditions
        {
            get { return this.Postconditions; }
        }

        /// <summary>
        /// Gets a value indicating whether the state is forced.
        /// </summary>
        /// <value>
        /// 	<see langword="true"/> if the state is forced; otherwise, <see langword="false"/>.
        /// </value>
        [DisplayNameResource(@"GuidanceNode_HasStateOverride_DisplayName", typeof(Resources))]
        [DescriptionResource(@"GuidanceNode_HasStateOverride_Description", typeof(Resources))]
        [ReadOnly(true)]
        public bool HasStateOverride
        {
            get { return stateOverride.HasValue; }
        }

        /// <summary>
        /// Gets the state of the activity.
        /// </summary>
        /// <value>One of the <see cref="NodeState"/> members.</value>
        public override NodeState State
        {
            get { return stateOverride ?? base.State; }
        }

        /// <summary>
        /// Gets or sets whether the user has accepted this node.
        /// </summary>
        [DisplayNameResource(@"GuidanceNode_IsUserAccepted_DisplayName", typeof(Resources))]
        [DescriptionResource(@"GuidanceNode_IsUserAccepted_Description", typeof(Resources))]
        [ReadOnly(true)]
        public bool IsUserAccepted
        {
            get
            {
                return this.isUserAccepted;
            }
            set
            {
                if (this.isUserAccepted != value)
                {
                    this.isUserAccepted = value;
                    this.OnIsUserAcceptedChanged();
                }
            }
        }

        /// <summary>
        /// Clears the state if it was forced.
        /// </summary>
        public virtual void ClearStateOverride()
        {
            var previousState = this.State;
            var hadStateOverride = this.HasStateOverride;
            stateOverride = null;

            if (previousState != base.State)
                this.OnStateChanged();

            if (hadStateOverride)
                this.OnHasStateOverrideChanged();
        }

        /// <summary>
        /// Sets the intrinsic state of the node.
        /// </summary>
        /// <param name="state">The new intrisic state  the node.</param>
        /// <param name="isOverride">if set to <see langword="true"/> the state is forced, otherwise the state is determined from an external evaluation.</param>
        public virtual void SetState(NodeState state, bool isOverride)
        {
            var previousState = this.State;
            var hadStateOverride = this.HasStateOverride;

            if (isOverride)
            {
                stateOverride = state;
                if (previousState != state)
                    this.OnStateChanged();

                if (!hadStateOverride)
                    this.OnHasStateOverrideChanged();
            }
            else
            {
                base.State = state;
                if (!hadStateOverride && previousState != state)
                    this.OnStateChanged();
            }
        }

        /// <summary>
        /// Triggered on state override changed
        /// </summary>
        protected void OnHasStateOverrideChanged()
        {
            this.OnPropertyChanged(Reflect<IConditionalNode>.GetProperty(g => g.HasStateOverride).Name);
            this.HasStateOverrideChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Triggered on user accepting changed
        /// </summary>
        private void OnIsUserAcceptedChanged()
        {
            this.OnPropertyChanged(Reflect<IConditionalNode>.GetProperty(g => g.IsUserAccepted).Name);
            this.IsUserAcceptedChanged(this, EventArgs.Empty);
        }
    }
}