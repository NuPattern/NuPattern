using NuPattern.ComponentModel.Composition;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Specifies that the component is a provided <see cref="ICommand"/>.
    /// </summary>
    public class CommandAttribute : ComponentAttribute
    {
        /// <summary>
        /// Creates a new instance of the <see cref="CommandAttribute"/> class.
        /// </summary>
        public CommandAttribute()
            : base(typeof(ICommand))
        {
        }
    }
}