using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Modeling.ExtensionEnablement;
using Microsoft.VisualStudio.Modeling.Shell.ExtensionEnablement;
using NuPattern.Runtime.Authoring;

namespace NuPattern.Runtime.Schema.Commands
{
    /// <summary>
    /// Double-derived class to allow easier code customization.
    /// </summary>
    internal abstract partial class AuthoringCommandExtensionRegistrarBase : CommandExtensionRegistrar
    {
        private const string MetadataFilterProperty = "MetadataFilter";
        private readonly Guid commandSetGuid;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthoringCommandExtensionRegistrarBase"/> class.
        /// </summary>
        protected AuthoringCommandExtensionRegistrarBase()
        {
            this.commandSetGuid = new Guid(Constants.PatternModelCommandSetId);
        }

        /// <summary>
        /// Provides the start Id of the dynamic Command Group in which the command extension will be placed.
        /// This value is used if a CommandExtension does not provide the MenuPlaceholderId value through MEF metadata.
        /// </summary>
        protected override int CommandExtensionDefaultStartId
        {
            get { return 0x4000; }
        }

        /// <summary>
        /// Provide the CommandSet GUID where commands will be placed.
        /// </summary>
        protected override Guid CommandSetGuid
        {
            get { return this.commandSetGuid; }
        }

        /// <summary>
        /// String-based Metadata Key that determines if a particular Exported Type can be imported or not.
        /// The default CanImport implementation filters imports based on this metadata key.
        /// The default value of this property is null indicating that no filter will be applied.
        /// </summary>
        protected override string MetadataFilter
        {
            get { return ExtensibilityConstants.MetadataFilter; }
        }

        /// <summary>
        /// Allow registrars that match the metadatafilter with a key in their metadata to be imported.
        /// </summary>
        /// <param name="lazyImport">The command extensions.</param>
        /// <returns>If it can import.</returns>
        protected override bool CanImport(Lazy<ICommandExtension, IDictionary<string, object>> lazyImport)
        {
            Guard.NotNull(() => lazyImport, lazyImport);

            if (!string.IsNullOrEmpty(this.MetadataFilter))
            {
                return lazyImport.Metadata.ContainsKey(MetadataFilterProperty) &&
                    lazyImport.Metadata[MetadataFilterProperty].Equals(this.MetadataFilter);
            }

            return true;
        }
    }
}