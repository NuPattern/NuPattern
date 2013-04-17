using System;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using NuPattern.Runtime;
using NuPattern.VisualStudio;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Settings validation helper class
    /// </summary>
    internal class SettingsValidationHelper
    {
        internal static bool TryToFixId<TSettings>(Store store, IPatternElementSchema elementSchema, Guid elementId, Action<Guid> idFixer) where TSettings : IAutomationSettings
        {
            bool isIdFixed = false;

            var patternModel = GetRootElement(store);

            var manager = store.GetService<IPatternManager>();

            var toolkit = manager.InstalledToolkits.FirstOrDefault(i => i.Id.Equals(patternModel.BaseId));

            if (toolkit != null)
            {
                var baseSettings = toolkit.Schema.FindAll<TSettings>().FirstOrDefault(set => set.Id.Equals(elementId));

                if (baseSettings != null)
                {
                    var settings = elementSchema.GetAutomationSettings<TSettings>().FirstOrDefault(s => s.Name.Equals(baseSettings.Name));

                    if (settings != null)
                    {
                        idFixer(settings.Id);
                        isIdFixed = true;
                    }
                }
            }

            return isIdFixed;
        }

        private static IPatternModelSchema GetRootElement(Store store)
        {
            return store.DefaultPartition.ElementDirectory.AllElements.OfType<IPatternModelSchema>().SingleOrDefault();
        }
    }
}
