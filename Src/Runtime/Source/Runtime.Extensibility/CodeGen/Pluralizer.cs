using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;

namespace NuPattern.Runtime.CodeGen
{
    /// <summary>
    /// Pluralizes  text identifiers.
    /// </summary>
    public static class Pluralizer
    {
        private const string UsEnglishCulture = @"en-US";
        private static readonly PluralizationService service = PluralizationService.CreateService(CultureInfo.GetCultureInfo(UsEnglishCulture));

        /// <summary>
        /// Pluralizes the given word.
        /// </summary>
        /// <param name="word">The word to pluralize</param>
        /// <returns>The pluralized word</returns>
        public static string Pluralize(string word)
        {
            return service.Pluralize(word);
        }

        /// <summary>
        /// Singularizes the given word.
        /// </summary>
        /// <param name="word">The word to singularize</param>
        /// <returns>The pluralized word</returns>
        public static string Singularize(string word)
        {
            return service.Singularize(word);
        }

        /// <summary>
        /// Determines whether the word is singular.
        /// </summary>
        /// <param name="word">The word to check</param>
        /// <returns></returns>
        public static bool IsSingular(string word)
        {
            return service.IsSingular(word);
        }

        /// <summary>
        /// Determines whether the word is plural.
        /// </summary>
        /// <param name="word">The word to check</param>
        /// <returns></returns>
        public static bool IsPlural(string word)
        {
            return service.IsPlural(word);
        }
    }
}
