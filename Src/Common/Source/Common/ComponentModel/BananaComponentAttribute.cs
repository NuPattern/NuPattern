using System;
using System.ComponentModel;
using System.ComponentModel.Composition;

namespace NuPattern.ComponentModel
{
    /// <summary>
    /// Base attribute used by component provided by a .
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class)]
    public class BananaComponentAttribute : InheritedExportAttribute, IBananaComponentMetadata
    {
        /// <summary>
        /// Exports the component using the default contract information.
        /// </summary>
        public BananaComponentAttribute()
        {
        }

        /// <summary>
        /// Exports the component with the given contract type.
        /// </summary>
        public BananaComponentAttribute(Type contractType)
            : base(contractType)
        {
        }

        /// <summary>
        /// Exports the component with the given contract name.
        /// </summary>
        public BananaComponentAttribute(string contractName)
            : base(contractName)
        {
        }

        /// <summary>
        /// Exports the component with the given contract name and type.
        /// </summary>
        public BananaComponentAttribute(string contractName, Type contractType)
            : base(contractName, contractType)
        {
        }

        /// <summary>
        /// Optionally sets the  id of the component, which otherwise defaults 
        /// to the component full type name.
        /// </summary>
        public virtual string Id { get; set; }

        /// <summary>
        /// Gets or sets the category of the component. Alternatively, 
        /// consider using <see cref="CategoryAttribute"/> which allows 
        /// localization.
        /// </summary>
        public virtual string Category { get; set; }

        /// <summary>
        /// Gets or sets the display name of the component. Alternatively, 
        /// consider using <see cref="DisplayNameAttribute"/> which allows 
        /// localization.
        /// </summary>
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the description of the component. Alternatively, 
        /// consider using <see cref="DescriptionAttribute"/> which allows 
        /// localization.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Gets or sets the exporting type. For use by the runtime.
        /// </summary>
        internal Type ExportingType { get; set; }

        /// <summary>
        /// Explicit implementation so that users don't see it in intellisense.
        /// </summary>
        Type IBananaComponentMetadata.ExportingType { get { return ExportingType; } }

        /// <summary>
        /// Explicit implementation so that users don't see it in intellisense.
        /// </summary>
        string IBananaComponentMetadata.CatalogName { get { return null; } }

    }

}
