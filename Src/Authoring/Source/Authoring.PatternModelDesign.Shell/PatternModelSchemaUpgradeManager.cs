using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using NuPattern.Diagnostics;
using NuPattern.Runtime.Schema.Properties;
using TraceSourceExtensions = NuPattern.VisualStudio.TraceSourceExtensions;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Schema upgrade manager
    /// </summary>
    [Export(typeof(IPatternModelSchemaUpgradeManager))]
    internal class PatternModelSchemaUpgradeManager : IPatternModelSchemaUpgradeManager
    {
        private static readonly ITracer tracer = Tracer.Get<PatternModelSchemaUpgradeManager>();

        /// <summary>
        /// Executes the upgrade.
        /// </summary>
        /// <param name="context">The context of the upgrade</param>
        public void Execute(ISchemaUpgradeContext context)
        {
            Guard.NotNull(() => context, context);

            tracer.Info(ShellResources.PatternModelSchemaUpgradeManager_TraceExecute, context.SchemaFilePath);

            TraceSourceExtensions.Shield(tracer, () =>
            {
                if (context.UpgradeProcessors != null && context.UpgradeProcessors.Any())
                {
                    // Order processors by declared 'Order' field
                    var upgradeProcessors = context.UpgradeProcessors.ToList()
                                                .OrderBy(proc => proc.Metadata.Order);

                    // Trace all processors found
                    upgradeProcessors
                        .ForEach(proc =>
                            {
                                tracer.Verbose(
                                    ShellResources.PatternModelSchemaUpgradeManager_TraceFoundUpgradeRule,
                                    proc.Value.GetType(), proc.Metadata.TargetVersion, proc.Metadata.Order);
                            });

                    // Determine schema versions
                    var previousVersion = context.SchemaVersion;
                    var currentVersion = context.RuntimeVersion;
                    if (previousVersion < currentVersion)
                    {
                        // Query Processors
                        var processorsToExecute = new List<IPatternModelSchemaUpgradeProcessor>();
                        upgradeProcessors
                            .ForEach(proc =>
                            {
                                // Is this processor required to execute for this version of schema? 
                                Version procVersion;
                                if (Version.TryParse(proc.Metadata.TargetVersion, out procVersion))
                                {
                                    if (procVersion == previousVersion)
                                    {
                                        processorsToExecute.Add(proc.Value);
                                    }
                                }
                            });

                        if (processorsToExecute.Any())
                        {
                            tracer.Verbose(
                                ShellResources.PatternModelSchemaUpgradeManager_TraceExecuteUpgradeProcessors, previousVersion, currentVersion);

                            // Initialize document
                            var document = context.OpenSchema();

                            // Backup original schema
                            context.BackupSchema();

                            // Execute processors
                            processorsToExecute.ForEach(proc =>
                                {
                                    try
                                    {
                                        tracer.Info(ShellResources.PatternModelSchemaUpgradeManager_TraceExecuteUpgradeRule,
                                            proc.GetType(), previousVersion);

                                        proc.ProcessSchema(document);
                                    }
                                    catch (Exception ex)
                                    {
                                        tracer.Error(ShellResources.PatternModelSchemaUpgradeManager_ErrorUpgradeRuleFailed,
                                            proc.GetType(), ex.Message);
                                        throw;
                                    }
                                });
                        }

                        // Update and save
                        if ((previousVersion < currentVersion)
                            || context.UpgradeProcessors.Any(mp => mp.Value.IsModified))
                        {
                            // Upgrade schema version
                            context.SchemaVersion = currentVersion;

                            // Save migrated document
                            context.SaveSchema();
                        }
                    }
                }
            }, ShellResources.PatternModelSchemaUpgradeManager_ErrorUpgradeRulesFailed);
        }
    }
}
