using NuPattern.Runtime.Bindings;
using NuPattern.Runtime.Composition;

namespace NuPattern.Runtime.Guidance
{
    /// <summary>
    /// A binding for a command
    /// </summary>
    public class CommandBinding : Binding<ICommand>, ICommandBinding
    {
        /// <summary>
        /// Creates a new instance of the <see cref="CommandBinding"/> class.
        /// </summary>
        public CommandBinding(
            INuPatternCompositionService composition,
            string componentTypeId,
            params PropertyBinding[] propertyBindings)
            : base(composition, componentTypeId, propertyBindings)
        {
        }

		/// <summary>
		/// Gets the name of the binding.
		/// </summary>
        public string Name { get; set; }
    }
}