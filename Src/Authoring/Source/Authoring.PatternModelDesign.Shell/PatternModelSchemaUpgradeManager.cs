using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Extensibility;
using NuPattern.Runtime.Schema.Properties;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Schema upgrade manager
    /// </summary>
    [Export(typeof(IPatternModelSchemaUpgradeManager))]
    internal class PatternModelSchemaUpgradeManager : IPatternModelSchemaUpgradeManager
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<PatternModelSchemaUpgradeManager>();

        /// <summary>
        /// Executes the upgrade.
        /// </summary>
        /// <param name="context">The context of the upgrade</param>
        public void Execute(ISchemaUpgradeContext context)
        {
            Guard.NotNull(() => context, context);

            tracer.TraceInformation(ShellResources.PatternModelSchemaUpgradeManager_TraceExecute, context.SchemaFilePath);

            Extensibility.TracingExtensions.Shield(tracer, () =>
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
                                tracer.TraceVerbose(
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
                            tracer.TraceVerbose(
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
                                        tracer.TraceInformation(ShellResources.PatternModelSchemaUpgradeManager_TraceExecuteUpgradeRule,
                                            proc.GetType(), previousVersion);

                                        proc.ProcessSchema(document);
                                    }
                                    catch (Exception ex)
                                    {
                                        tracer.TraceError(ShellResources.PatternModelSchemaUpgradeManager_ErrorUpgradeRuleFailed,
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
