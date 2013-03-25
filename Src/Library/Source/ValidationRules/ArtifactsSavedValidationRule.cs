using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using NuPattern.ComponentModel.Design;
using NuPattern.Extensibility;
using NuPattern.Extensibility.References;
using NuPattern.Library.Properties;
using NuPattern.Runtime;

namespace NuPattern.Library.ValidationRules
{
    /// <summary>
    /// Validates that any project items linked via artifact links are in a saved state.
    /// </summary>
    [DescriptionResource("ArtifactsSavedValidationRule_Description", typeof(Resources))]
    [DisplayNameResource("ArtifactsSavedValidationRule_DisplayName", typeof(Resources))]
    [Category("General")]
    [CLSCompliant(false)]
    public class ArtifactsSavedValidationRule : ValidationRule
    {
        /// <summary>
        /// Gets or sets the current element.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IProductElement CurrentElement { get; set; }

        /// <summary>
        /// Gets or sets the URI reference service.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IFxrUriReferenceService UriReferenceService { get; set; }

        /// <summary>
        /// Run the defined validation.
        /// </summary>
        /// <returns>
        /// The <see cref="ValidationResult"/> collection with the validation errors. If the validation was
        /// successful returns an empty enumerable or <see langword="null"/>.
        /// </returns>
        public override IEnumerable<ValidationResult> Validate()
        {
            var solutionItems = SolutionArtifactLinkReference.GetResolvedReferences(this.CurrentElement, this.UriReferenceService).ToList();
            foreach (IItemContainer solutionItem in solutionItems)
            {
                // Filter for items only
                var item = solutionItem as IItem;
                if (item != null)
                {
                    var projectItem = item.As<EnvDTE.ProjectItem>();
                    if (projectItem != null)
                    {
                        // Report validation error if project item is not saved.
                        if (!projectItem.Saved)
                        {
                            yield return new ValidationResult(
                                string.Format(Properties.Resources.ArtifactsSavedValidationRule_ErrorMessage, this.CurrentElement.InstanceName, item.Name));
                        }
                    }
                }
            }
        }
    }
}