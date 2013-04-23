namespace NuPattern.VisualStudio.Solution.Templates
{
    /// <summary>
    /// Template data
    /// </summary>
    public interface IVsTemplateData
    {
        /// <summary>
        /// Gets a value to indicate whether to build on load,
        /// </summary>
        object BuildOnLoad { get; }

        /// <summary>
        /// Gets a value to indicate whether to create in place.
        /// </summary>
        object CreateInPlace { get; }

        /// <summary>
        /// Gets a value to indicate whether to create a new folder for the contents of the template
        /// </summary>
        bool? CreateNewFolder { get; }

        /// <summary>
        /// Gets the default name of the item unfolded by the template
        /// </summary>
        string DefaultName { get; }

        /// <summary>
        /// Gets a description for the unfolded item.
        /// </summary>
        IVsTemplateResourceOrValue Description { get; }

        /// <summary>
        /// Gets a value to indicate whether to enable the location field for the user.
        /// </summary>
        bool? EnableEditOfLocationField { get; }

        /// <summary>
        /// Gets a value to indicate whether to enable the browse button for the user
        /// </summary>
        bool? EnableLocationBrowseButton { get; }

        /// <summary>
        /// Gets a value to indicate whether the template is hidden from use by user.
        /// </summary>
        bool? Hidden { get; }

        /// <summary>
        /// Gets the template icon.
        /// </summary>
        IVsTemplateResourceOrValue Icon { get; }

        /// <summary>
        /// Gets a value to indicate teh visibility of the location field.
        /// </summary>
        VSTemplateTemplateDataLocationField? LocationField { get; }

        /// <summary>
        /// Gets the MRU prefix for the location field.
        /// </summary>
        string LocationFieldMRUPrefix { get; }

        /// <summary>
        /// Gets the name of the unfolded item.
        /// </summary>
        IVsTemplateResourceOrValue Name { get; }

        /// <summary>
        /// Gets the number of parent categories to roll up
        /// </summary>
        string NumberOfParentCategoriesToRollUp { get; }

        /// <summary>
        /// Gets the project sub-type
        /// </summary>
        string ProjectSubType { get; }

        /// <summary>
        /// Gets the project type
        /// </summary>
        string ProjectType { get; }

        /// <summary>
        /// Gets a value to indicate whether to prompt to save the unfolded item
        /// </summary>
        bool? PromptForSaveOnCreation { get; }

        /// <summary>
        /// Gets a value to indicate whether to provide a default name or not.
        /// </summary>
        bool? ProvideDefaultName { get; }

        /// <summary>
        /// Gets the required .NET framework version.
        /// </summary>
        VSTemplateTemplateDataRequiredFrameworkVersion? RequiredFrameworkVersion { get; }

        /// <summary>
        /// Gets a value to indicate whether to show the template by default.
        /// </summary>
        object ShowByDefault { get; }

        /// <summary>
        /// Gets the sort order of the template relative to its peers.
        /// </summary>
        string SortOrder { get; }

        /// <summary>
        /// Gets a value to indicate whether the template supports code separation.
        /// </summary>
        bool? SupportsCodeSeparation { get; }

        /// <summary>
        /// Gets a value to indicate whether the template supports language selection.
        /// </summary>
        bool? SupportsLanguageDropDown { get; }

        /// <summary>
        /// Gets a value to indicate whether the template supports a master page.
        /// </summary>
        bool? SupportsMasterPage { get; }

        /// <summary>
        /// Gets the group id for the template
        /// </summary>
        string TemplateGroupID { get; }

        /// <summary>
        /// Getshte unique identifier of the template.
        /// </summary>
        string TemplateID { get; }

        /// <summary>
        /// Gets the output sub path of where the template will be unfolded.
        /// </summary>
        string OutputSubPath { get; }
    }
}