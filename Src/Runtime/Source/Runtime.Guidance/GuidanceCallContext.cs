using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.Remoting.Messaging;
using NuPattern.Reflection;
using NuPattern.Runtime.Guidance.UI;
using NuPattern.Runtime.Guidance.Workflow;

namespace NuPattern.Runtime.Guidance
{
    /// <summary>
    /// Holds transient state that is part of a guidance extension 
    /// call context (such as during the process of a template 
    /// unfolding operation).
    /// </summary>
    internal class GuidanceCallContext
    {
        internal GuidanceCallContext()
        {
        }

        static GuidanceCallContext()
        {
            Current = new GuidanceCallContext();
        }

        /// <summary>
        /// Gets the current call context.
        /// </summary>
        /// <devdoc>Made setter internal to allow this property to be replaced for tests. Same reason properties are virtual, so it can be mocked.</devdoc>
        public static GuidanceCallContext Current { get; internal set; }

        /// <summary>
        /// Gets or sets the current template context replacements dictionary.
        /// </summary>
        public virtual IDictionary<string, string> TemplateReplacementsDictionary
        {
            get { return LogicalGetData<IDictionary<string, string>>(x => x.TemplateReplacementsDictionary); }
            set { LogicalSetData(x => x.TemplateReplacementsDictionary, value); }
        }

        /// <summary>
        /// Gets or sets the GuidanceBrowser control shown in the Guidance Browser window
        /// </summary>
        public virtual GuidanceBrowser GuidanceBrowserControl
        {
            get { return CallContext.GetData("FeatureBuilderGuidanceBrowser") as GuidanceBrowser; }
            set { CallContext.SetData("FeatureBuilderGuidanceBrowser", value); }
        }

        /// <summary>
        /// Gets or sets the path to the Feature.commands file.
        /// Used in the FeatureBuilder to properly isolate the
        /// model bus references which bind the Feature.commands DSL
        /// document to the GuidanceWorkflow.activitydiagram from
        /// moving the solution within the filesystem.
        /// </summary>
        public virtual string FeatureBuilderDSLPath
        {
            get { return CallContext.GetData("FeatureBuilderDSLPath") as string; }
            set { CallContext.SetData("FeatureBuilderDSLPath", value); }
        }

        /// <summary>
        /// Gets or sets a reference to the node whose conditions are
        /// being evaluated.  In this way a condition, such as PredecessorsComplete,
        /// can know exactly which node is being evaluated as opposed to having to
        /// search for it and potentially being exposed to multiple nodes of the
        /// same name
        /// </summary>
        public virtual INode DefaultConditionTarget
        {
            get { return CallContext.GetData("DefaultConditionTarget") as INode; }
            set { CallContext.SetData("DefaultConditionTarget", value); }
        }

        private T LogicalGetData<T>(Expression<Func<GuidanceCallContext, object>> propertyExpresion)
        {
            return (T)CallContext.LogicalGetData(Reflect<GuidanceCallContext>.GetProperty(propertyExpresion).Name);
        }

        private void LogicalSetData<T>(Expression<Func<GuidanceCallContext, object>> propertyExpresion, T value)
        {
            CallContext.LogicalSetData(Reflect<GuidanceCallContext>.GetProperty(propertyExpresion).Name, value);
        }
    }
}
