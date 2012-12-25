using System;
using System.Collections.Generic;
using System.Globalization;
using EnvDTE;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using Microsoft.VisualStudio.TemplateWizard;
using NuPattern.Extensibility.Properties;

namespace NuPattern.Extensibility
{
	/// <summary>
	/// Base template wizard that performs basic tracing.
	/// </summary>
	[CLSCompliant(false)]
	public abstract class TemplateWizard : IWizard, IDisposable
	{
		private ITraceSource tracer;
		private IDisposable activity;

		/// <summary>
		/// Initializes a new instance of the <see cref="TemplateWizard"/> class.
		/// </summary>
		public TemplateWizard()
		{
			this.tracer = Tracer.GetSourceFor<TemplateWizard>();
		}

		/// <summary>
		/// Gets the template file being unfolded.
		/// </summary>
		public string TemplateFile { get; private set; }

		/// <summary>
		/// Gets the schema of the template being unfolded.
		/// </summary>
		public IVsTemplate TemplateSchema { get; private set; }

		/// <summary>
		/// Executes when the wizard ends.
		/// </summary>
		public virtual void RunFinished()
		{
			if (this.activity != null)
				this.activity.Dispose();

			this.activity = null;
		}

		/// <summary>
		/// Executes when the wizard starts.
		/// </summary>
		public virtual void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
		{
			Guard.NotNull(() => customParams, customParams);
			Guard.NotNull(() => replacementsDictionary, replacementsDictionary);

			this.TemplateFile = (string)customParams[0];
			this.TemplateSchema = VsTemplateFile.Read(this.TemplateFile);
			this.activity = tracer.StartActivity(Resources.TemplateWizard_Unfolding, this.GetType().Name, this.TemplateFile);
		}

		/// <summary>
		/// Executed before a file is opened.
		/// </summary>
		public virtual void BeforeOpeningFile(ProjectItem projectItem)
		{
			Guard.NotNull(() => projectItem, projectItem);
		}

		/// <summary>
		/// Runs custom wizard logic when a project has finished generating.
		/// </summary>
		/// <param name="project">The project that finished generating.</param>
		public virtual void ProjectFinishedGenerating(Project project)
		{
            if (this.TemplateSchema.Type == VsTemplateType.Project)
            {
                if (project == null)
                {
                    throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                        Resources.TemplateWizard_ProjectTemplateNoProject, TemplateFile));
                }
            }
		}

		/// <summary>
		/// Runs custom wizard logic when a project item has finished generating.
		/// </summary>
		/// <param name="projectItem">The project item that finished generating.</param>
		public virtual void ProjectItemFinishedGenerating(ProjectItem projectItem)
		{
            if (projectItem == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                    Resources.TemplateWizard_ItemTemplateNoItem, TemplateFile));
            }
		}

		/// <summary>
		/// Indicates whether the specified project item should be added to the project.
		/// </summary>
		/// <param name="filePath">The path to the project item.</param>
		/// <returns>
		/// true if the project item should be added to the project; otherwise, false.
		/// </returns>
		public virtual bool ShouldAddProjectItem(string filePath)
		{
			Guard.NotNull(() => filePath, filePath);

			return true;
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="TemplateWizard"/> is reclaimed by garbage collection.
		/// </summary>
		~TemplateWizard()
		{
			this.Dispose(false);
		}

		/// <summary>
		/// Disposes the wizard and cleans its state.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing">Specify <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (this.activity != null)
				this.activity.Dispose();

			this.tracer.TraceVerbose(Resources.TemplateWizard_Disposed, this.GetType().Name);
		}
	}
}