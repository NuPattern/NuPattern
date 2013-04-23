using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using NuPattern.Diagnostics;
using NuPattern.Drawing;
using NuPattern.Runtime.Composition;
using NuPattern.Runtime.Guidance;
using NuPattern.Runtime.Properties;
using NuPattern.VisualStudio.Extensions;

namespace NuPattern.Runtime.Shell
{
    /// <summary>
    /// This adapter is needed to isolate the <see cref="IGuidanceManager"/> implementation 
    /// from the details of how to get a new instance of a registered guidance extension from 
    /// the MEF container. Guidance extensions have a creation policy of non-shared, meaning 
    /// every time you get a new export of the <see cref="IGuidanceExtension"/> 
    /// you get a new instance created. However, you need to get the export 
    /// from the container in order to get the new instance. Instead of 
    /// passing the container itself around, we pass a func that calls back 
    /// into the container, so the only component aware of the container 
    /// is this adapter.
    /// </summary>
    internal class GuidanceRegistrationAdapter
    {
        static readonly ITraceSource tracer = Tracer.GetSourceFor<GuidanceRegistrationAdapter>();
        const string ManifestFilename = "extension.vsixmanifest";

        [Import(typeof(IExtensionManager))]
        public IExtensionManager ExtensionManager { get; set; }

        /// <summary>
        /// Exposes the registrations that can be used to instantiate guidance extension.
        /// Consumed by the <see cref="IGuidanceManager"/>.
        /// </summary>
        [Export]
        public IEnumerable<IGuidanceExtensionRegistration> RegisteredGuidanceExtensions
        {
            get
            {
                if (ExportedGuidanceExtensions == null)
                {
                    yield break;
                }
                else
                {
                    if (this.ExtensionManager == null)
                    {
                        tracer.TraceError(Resources.GuidanceRegistrationAdapter_TraceExtensionManagerNotAvailable);
                        yield break;
                    }

                    foreach (var export in this.ExportedGuidanceExtensions)
                    {
                        var registration = new GuidanceRegistration();

                        // We need to create the guidance extension to determine where the VSXI manifest is located.
                        // We should find a way to locate the manifest without creating an instance of the guidance extension.
                        // Note: guidance manager, and therefore this adapter, would live in the global VS container, 
                        // not our custom containers.
                        registration.ExtensionId = export.Metadata.ExtensionId;
                        var guidanceExtension = CreateExtension(registration);
                        var extensionManifestFilename = Path.Combine(Path.GetDirectoryName(guidanceExtension.GetType().Assembly.Location), ManifestFilename);

                        if (File.Exists(extensionManifestFilename))
                        {
                            var extension = this.ExtensionManager.CreateExtension(extensionManifestFilename);

                            // The guidance extension id metadata value is used both as part 
                            // of the export definition as well as the assembly-level 
                            // attribute to group components from multiple projects 
                            // into a single feature.
                            // We need the ID to be part of the metadata as it's 
                            // the only way we have of relocating the export 
                            // for a given guidance extension Id (unless we find a way 
                            // to query directly the catalog for the export definition 
                            // but then we'd need to expose either an aggregate catalog 
                            // or something.

                            // Verify that both IDs match and issue a warning if they don't.
                            if (export.Metadata.ExtensionId != extension.Header.Identifier)
                            {
                                tracer.TraceWarning(Resources.GuidanceRegistrationAdapter_TraceSkipIdMismatch,
                                    export.Metadata.ExtensionId, extension.Header.Identifier);
                                continue;
                            }

                            registration.ExtensionId = extension.Header.Identifier;
                            registration.DefaultName = export.Metadata.DefaultName;

                            var installedExtension = this.ExtensionManager.GetInstalledExtension(extension.Header.Identifier);

                            registration.ExtensionManifest = extension;
                            registration.InstalledExtension = installedExtension;
                            registration.InstallPath = installedExtension.InstallPath;
                            registration.PreviewImage = BitmapHelper.Load(registration.InstallPath, extension.Header.PreviewImage);
                            registration.Icon = BitmapHelper.Load(registration.InstallPath, extension.Header.Icon).ToIcon();
                        }
                        else
                        {
                            tracer.TraceWarning(Resources.GuidanceRegistrationAdapter_TraceMissingManifest,
                                extensionManifestFilename, export.Metadata.ExtensionId);
                            continue;
                        }

                        yield return registration;
                    }
                }
            }
        }

        /// <summary>
        /// Exposes the factory of guidance extension
        /// </summary>
        /// <param name="registration"></param>
        /// <returns></returns>
        [Export(typeof(Func<IGuidanceExtensionRegistration, IGuidanceExtension>))]
        public IGuidanceExtension CreateExtension(IGuidanceExtensionRegistration registration)
        {
            // We re-retrieve the export rather than caching the one 
            // we got from ExportedExtensions as we need to 
            // lazily re-evaluate the export as it's a non-shared 
            // component that needs to be re-created every time.
            // If we cache the export, we're basically caching the 
            // lazy instance for *one* particular instantiation. 				
            return NuPatternGlobalContainer.Instance
                .GetExports<IGuidanceExtension, IGuidanceExtensionMetadata>()
                .First(e => e.Metadata.ExtensionId == registration.ExtensionId)
                .Value;
        }

        /// <summary>
        /// Imports the exported guidance extension that are installed. Used to build 
        /// the <see cref="RegisteredGuidanceExtensions"/>.
        /// </summary>
        /// <remarks>
        /// Imported guidance extension must have a <see cref="CreationPolicy.NonShared"/> 
        /// policy applied, otherwise, they are ignored.
        /// </remarks>
        [ImportMany(RequiredCreationPolicy = CreationPolicy.NonShared)]
        public IEnumerable<Lazy<IGuidanceExtension, IGuidanceExtensionMetadata>> ExportedGuidanceExtensions { get; set; }
    }
}