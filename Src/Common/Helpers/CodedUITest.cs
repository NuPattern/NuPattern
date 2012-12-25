using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using NuPattern.Extensibility;

/// <summary>
/// Base class for a CodeUI Test
/// </summary>
public abstract class CodedUITest : IntegrationTest
{
    private TestContext context;
    private ApplicationUnderTest testApp;
    private ISolution solution;
    private ITemplate templateToUnfold;
    private IProject unfoldedProject;

    /// <summary>
    /// Gets or sets the test context.
    /// </summary>
    public TestContext TestContext
    {
        get
        {
            return this.context;
        }

        set
        {
            this.context = value;
        }
    }

    [TestInitialize]
    public virtual void InitializeContext()
    {
        this.solution = (ISolution)VsIdeTestHostContext.ServiceProvider.GetService(typeof(ISolution));
        this.solution.CreateInstance(this.DeploymentDirectory, "EmptySolution");
    }


    [TestCleanup]
    public virtual void Cleanup()
    {
        VsIdeTestHostContext.Dte.Solution.Close(false);
    }

    public virtual void SelectProjectTemplate(string templateName, string category="CSharp")
    {
        // Locate the project template
        var templates = (IFxrTemplateService)VsIdeTestHostContext.ServiceProvider.GetService(typeof(IFxrTemplateService));
        this.templateToUnfold = templates.Find(templateName, category);
    }

    public virtual void CreateNewProject(string projectName)
    {
        //TODO: Delete existing project directory

        this.unfoldedProject = (IProject)this.templateToUnfold.Unfold(projectName, this.solution);
    }
}