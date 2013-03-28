using Microsoft.VisualStudio.Modeling.Validation;

namespace NuPattern.Runtime.Schema.Shell
{
    internal static class PatternModelDocHelper
    {
        /// <summary>
        /// Runs the valiation on the pattern model file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool ValidateDocument(string fileName)
        {
            var valid = false;
            NuPattern.Modeling.Windows.WindowExtensions.DoActionOnDesigner(fileName, docData =>
                {
                    var document = docData as PatternModelDocData;
                    if (document != null)
                    {
                        // Validate
                        valid = document.ValidationController.Validate(docData.GetAllElementsForValidation(), ValidationCategories.Save);
                        if (!valid)
                        {
                            // Open document for editing if fails validation
                            NuPattern.VisualStudio.Windows.WindowExtensions.ActivateDocument(fileName);
                        }
                    }
                }, true);

            return valid;
        }
    }
}
