
namespace Microsoft.VisualStudio.Patterning.Extensibility.Serialization
{
    /// <summary>
    /// A Json Converter
    /// </summary>
    public abstract class JsonConverter
    {
        private object innerConverter;

        /// <summary>
        /// Creates a new instance of the <see cref="JsonConverter"/> class.
        /// </summary>
        protected JsonConverter(object innerConverter)
        {
            this.innerConverter = innerConverter;
        }

        /// <summary>
        /// Provides access to the encapsulated converter.
        /// </summary>
        internal object InnerConverter
        {
            get
            {
                return this.innerConverter;
            }
        }
    }
}
