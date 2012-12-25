
using System;

namespace NuPattern.Extensibility.Serialization
{
    /// <summary>
    /// A custom converter for creating Json objects.
    /// </summary>
    public abstract class CustomCreationConverter<T> : JsonConverter
    {
        /// <summary>
        /// Creates a new instance of the <see cref="CustomCreationConverter{T}"/> class.
        /// </summary>
        protected CustomCreationConverter()
            : base(new CustomCreationConverterInternal<T>())
        {
            ((CustomCreationConverterInternal<T>)this.InnerConverter).CreateCallBack = Create;
        }

        /// <summary>
        /// Creates an object which will then be populated by the serializer.
        /// </summary>
        public abstract T Create(System.Type objectType);
    }

    /// <summary>
    /// A wrapper class for a custom creationc converter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class CustomCreationConverterInternal<T> : Newtonsoft.Json.Converters.CustomCreationConverter<T>
    {
        public Func<Type, T> CreateCallBack;

        /// <summary>
        /// Creates something
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override T Create(Type objectType)
        {
            return CreateCallBack(objectType);
        }
    }
}
