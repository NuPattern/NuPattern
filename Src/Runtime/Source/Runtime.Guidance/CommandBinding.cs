using NuPattern.Runtime.Bindings;
using NuPattern.Runtime.Composition;

namespace NuPattern.Runtime.Guidance
{
    /// <summary>
    /// A binding for a command
    /// </summary>
    internal class CommandBinding : Binding<ICommand>, ICommandBinding
    {
        /// <summary>
        /// Creates a new instance of the <see cref="CommandBinding"/> class.
        /// </summary>
        public CommandBinding(
            INuPatternCompositionService featureComposition,
            string componentTypeId,
            params PropertyBinding[] propertyBindings)
            : base(featureComposition, componentTypeId, propertyBindings)
        {
        }

        public string Name { get; set; }
    }
}