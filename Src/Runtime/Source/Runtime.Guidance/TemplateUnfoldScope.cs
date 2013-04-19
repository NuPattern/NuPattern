using System;
using System.Collections.Generic;
using NuPattern.Diagnostics;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    /// <summary>
    /// Allows the replacements dictionary to be set on the 
    /// <see cref="FeatureCallContext"/> only once for an 
    /// entire process that may unfold multiple templates. 
    /// </summary>
    /// <remarks>
    /// The first scope to be instantiated when the current 
    /// <see cref="FeatureCallContext.TemplateReplacementsDictionary"/> 
    /// is still null, will set it, and reset it to null 
    /// when it's disposed. Other nested scopes are no-op 
    /// if an existing dictionary exists in the current call context.
    /// </remarks>
    internal class TemplateUnfoldScope : IDisposable
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<TemplateUnfoldScope>();
        private bool templateContextSet;
        private IDictionary<string, string> originalDictionary = null;

        public TemplateUnfoldScope(IDictionary<string, string> replacementsDictionary)
        {
            if (!IsActive)
            {
                tracer.TraceVerbose("No existing template scope found. Storing dictionary in current call context.");
                FeatureCallContext.Current.TemplateReplacementsDictionary = replacementsDictionary;
                this.templateContextSet = true;
            }
            else
            {
                originalDictionary = FeatureCallContext.Current.TemplateReplacementsDictionary;

                foreach (string key in originalDictionary.Keys)
                {
                    if (replacementsDictionary.ContainsKey(key))
                        replacementsDictionary.Remove(key);
                    replacementsDictionary.Add(key, originalDictionary[key]);
                }
                FeatureCallContext.Current.TemplateReplacementsDictionary = replacementsDictionary;
                tracer.TraceVerbose("Previous template scope was found. Existing dictionary in current call context will remain.");
            }
        }

        public static bool IsActive
        {
            get { return FeatureCallContext.Current.TemplateReplacementsDictionary != null; }
        }

        /// <summary>
        /// Clears the current replacement dictionary if this scope was 
        /// the one that originally set it.
        /// </summary>
        public void Dispose()
        {
            if (this.templateContextSet)
            {
                FeatureCallContext.Current.TemplateReplacementsDictionary = originalDictionary;
                tracer.TraceVerbose("Current dictionary in call context cleaned.");
            }
        }
    }
}