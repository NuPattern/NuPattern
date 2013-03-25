
namespace NuPattern.Runtime.Serialization
{
    /// <summary>
    /// A data and time converter that uses Iso times.
    /// </summary>
    public class IsoDateTimeConverter : JsonConverter
    {
        /// <summary>
        /// Creates a new instance of the <see cref="IsoDateTimeConverter"/> class.
        /// </summary>
        public IsoDateTimeConverter()
            : base(new IsoDateTimeConverterInternal())
        {
        }
    }

    internal class IsoDateTimeConverterInternal : Newtonsoft.Json.Converters.IsoDateTimeConverter
    {

    }
}
