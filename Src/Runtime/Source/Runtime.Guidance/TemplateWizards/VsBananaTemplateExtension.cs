using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;
using NuPattern.Diagnostics;
using NuPattern.Runtime.Guidance.Properties;
using NuPattern.VisualStudio.Solution;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    /// <summary>
    /// Base template wizard extension that provides additional context 
    /// for template authors, as well as fixing the solution empty 
    /// path issue.
    /// </summary>
    [TemplateExtension]
    internal abstract class VsBananaTemplateExtension : NuPattern.VisualStudio.TemplateWizards.TemplateWizard
    {
        private const string WizardDataStartKey = "<TargetFeatureExtension xmlns=\"\">";
        private const string WizardDataEndKey = "</TargetFeatureExtension>";

        private ITraceSource tracer;
        protected String solutionPath = null;

        [Import]
        public IFeatureManager FeatureManager { get; set; }

        [Import]
        public ISolutionState SolutionState { get; set; }

        [Import]
        public ISolution Solution { get; set; }

        public IFeatureRegistration FeatureRegistration { get; private set; }

        public DTE DTE { get; private set; }

        public VsTemplateParameters TemplateParameters { get; private set; }

        public IDictionary<string, string> ReplacementsDictionary { get; private set; }

        public override void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            base.RunStarted(automationObject, replacementsDictionary, runKind, customParams);

            if (this.FeatureManager == null)
            {
                FeaturesGlobalContainer.Instance.SatisfyImportsOnce(this);
            }

            this.TemplateFile = (string)customParams[0];
            string targetFeature = string.Empty;
            if (replacementsDictionary.ContainsKey("$wizarddata$"))
            {
                targetFeature = replacementsDictionary["$wizarddata$"];
                if (targetFeature.Contains(WizardDataStartKey))
                {
                    int dataStart = targetFeature.IndexOf(WizardDataStartKey) + WizardDataStartKey.Length;
                    int dataEnd = targetFeature.IndexOf(WizardDataEndKey);
                    targetFeature = targetFeature.Substring(dataStart, dataEnd - dataStart);
                }
            }

            if (targetFeature == string.Empty)
            {
                this.FeatureRegistration = this.FeatureManager.FindFeature(this.TemplateFile);
            }
            else
            {
                this.FeatureRegistration = this.FeatureManager.InstalledFeatures.Where(reg => reg.FeatureId == targetFeature).First();
            }
            ThrowIfNoFeatureRegistration();

            this.DTE = (DTE)automationObject;
            this.ReplacementsDictionary = replacementsDictionary;
            this.TemplateParameters = new VsTemplateParameters(replacementsDictionary);
            this.tracer = FeatureTracer.GetSourceFor(this, this.FeatureRegistration.FeatureId);

            this.FixSolutionNotCreated(runKind);

            if (!this.FeatureManager.IsOpened)
            {
                tracer.TraceVerbose("FeatureManager was not open. Opening with current solution state.");
                this.FeatureManager.Open(this.SolutionState);
            }

            tracer.TraceData(TraceEventType.Verbose, 0, new
            {
                TemplateFile = this.TemplateFile,
                Kind = runKind,
                ReplacementDictionary = this.TemplateParameters,
                CustomParams = customParams,
            });
        }

        private void FixSolutionNotCreated(WizardRunKind runKind)
        {
            if (runKind == WizardRunKind.AsNewProject || runKind == WizardRunKind.AsMultiProject)
            {
                var solutionPath = this.Solution.PhysicalPath;
                if (solutionPath == null ||
                    (!File.Exists(solutionPath) && this.Solution.Data.Created == null))
                {
                    // Web projects can pass in an empty directory, if so don't create the dest directory.
                    if ((new Uri(this.TemplateParameters.DestinationDirectory)).IsFile)
                    {
                        Directory.CreateDirectory(this.TemplateParameters.DestinationDirectory);
                    }

                    // Create causes a Close of whatever solution was there already, 
                    // even the "invisible" solution. 
                    // But our SolutionEvents will not fire the SolutionClosed in this 
                    // case
                    if (solutionPath == null && this.TemplateParameters.DestinationDirectory != null)
                    {
                        char[] splitChar = new char[1];
                        splitChar[0] = System.IO.Path.DirectorySeparatorChar;

                        string[] dirParts = this.TemplateParameters.DestinationDirectory.Split(splitChar);

                        //
                        // Make sure the directory path is valid
                        //
                        if (dirParts.Length < 2)
                            throw (new Exception("Bad Destination Directory in FixSolutionNotCreated"));

                        if (dirParts.Length > 2)
                        {
                            if (dirParts[dirParts.Length - 1] == dirParts[dirParts.Length - 2])
                            {
                                solutionPath = this.TemplateParameters.DestinationDirectory.Substring(0, this.TemplateParameters.DestinationDirectory.Length - dirParts[dirParts.Length - 1].Length - 1);
                                solutionPath += System.IO.Path.DirectorySeparatorChar + dirParts[dirParts.Length - 1] + ".sln";
                            }
                            else
                            {
                                solutionPath = this.TemplateParameters.DestinationDirectory;
                                solutionPath += System.IO.Path.DirectorySeparatorChar + dirParts[dirParts.Length - 1] + ".sln";
                            }
                        }

                        this.DTE.Solution.Create(
                            Path.GetDirectoryName(solutionPath),
                            Path.GetFileNameWithoutExtension(solutionPath));
                        this.solutionPath = solutionPath;
                    }
                    else
                    {
                        tracer.TraceInformation("Creating solution from physical path");
                        this.DTE.Solution.Create(
                            Path.GetDirectoryName(solutionPath),
                            Path.GetFileNameWithoutExtension(solutionPath));
                        this.solutionPath = solutionPath;
                    }


                    this.Solution.Data.Created = true;
                }
                else
                    solutionPath = this.Solution.PhysicalPath;
            }
        }

        private void ThrowIfNoFeatureRegistration()
        {
            if (this.FeatureRegistration == null)
            {
                throw new ArgumentException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.VsTemplateExtension_NoOwningFeatureForExtension,
                    this.GetType().FullName));
            }
        }

        public override bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }
    }
}