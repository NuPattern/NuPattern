using System;
using System.Collections.Generic;
using Moq;
using NuPattern.Modeling;

namespace NuPattern.Runtime.Schema.UnitTests
{
    internal static class PatternModelSpec
    {
        /// <summary>
        /// Returns a tailored pattern model.
        /// </summary>
        internal static PatternModelSchema TailorPatternModel(PatternModelSchema basePatternModel, Version baseVersion, string toolkitId = "ToolkitId")
        {
            var store = new DslTestStore<PatternModelDomainModel>();

            var serviceProvider = Mock.Get(store.ServiceProvider);
            serviceProvider.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

            PatternModelSchema tailoredPatternModel = null;

            store.TransactionManager.DoWithinTransaction(() =>
            {
                tailoredPatternModel = store.ElementFactory.CreateElement<PatternModelSchema>();
            });

            // Set baseId on the pattern line
            tailoredPatternModel.WithTransaction(pl => pl.BaseId = "Base");

            tailoredPatternModel.ClonerFactory = (b, v, t) =>
                Mock.Of<PatternModelCloner>(
                    c =>
                        c.SourcePatternModel == b &&
                        c.SourceVersion == v &&
                        c.TargetPatternModel == t);

            // Clone the pattern line
            tailoredPatternModel.Tailor(basePatternModel, baseVersion);

            return tailoredPatternModel;
        }
    }
}
