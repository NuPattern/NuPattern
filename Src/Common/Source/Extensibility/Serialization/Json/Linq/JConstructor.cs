
using System;
namespace Microsoft.VisualStudio.Patterning.Extensibility.Serialization.Json
{
    /// <summary>
    /// Represents a JSON constructor.
    /// </summary>
    public class JConstructor : JContainer
    {
        private string _name;
        //private readonly IList<JToken> _values = new List<JToken>();


        /// <summary>
        /// Initializes a new instance of the <see cref="JConstructor"/> class.
        /// </summary>
        public JConstructor()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JConstructor"/> class from another <see cref="JConstructor"/> object.
        /// </summary>
        /// <param name="other">A <see cref="JConstructor"/> object to copy from.</param>
        public JConstructor(JConstructor other)
            : base(other)
        {
            _name = other.Name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JConstructor"/> class with the specified name and content.
        /// </summary>
        /// <param name="name">The constructor name.</param>
        /// <param name="content">The contents of the constructor.</param>
        public JConstructor(string name, params object[] content)
            : this(name, (object)content)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JConstructor"/> class with the specified name and content.
        /// </summary>
        /// <param name="name">The constructor name.</param>
        /// <param name="content">The contents of the constructor.</param>
        public JConstructor(string name, object content)
            : this(name)
        {
            Add(content);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JConstructor"/> class with the specified name.
        /// </summary>
        /// <param name="name">The constructor name.</param>
        public JConstructor(string name)
        {
            Guard.NotNull(() => name, name);

            _name = name;
        }

        ///// <summary>
        ///// Gets the container's children tokens.
        ///// </summary>
        ///// <value>The container's children tokens.</value>
        //protected override IList<JToken> ChildrenTokens
        //{
        //    get { return _values; }
        //}

        ///// <summary>
        ///// Gets or sets the name of this constructor.
        ///// </summary>
        ///// <value>The constructor name.</value>
        //public string Name
        //{
        //    get { return _name; }
        //    set { _name = value; }
        //}

        ///// <summary>
        ///// Gets the node type for this <see cref="JToken"/>.
        ///// </summary>
        ///// <value>The type.</value>
        //public override JTokenType Type
        //{
        //    get { return JTokenType.Constructor; }
        //}

        //internal override bool DeepEquals(JToken node)
        //{
        //    JConstructor c = node as JConstructor;
        //    return (c != null && _name == c.Name && ContentsEqual(c));
        //}

        //internal override JToken CloneToken()
        //{
        //    return new JConstructor(this);
        //}

        ///// <summary>
        ///// Writes this token to a <see cref="JsonWriter"/>.
        ///// </summary>
        ///// <param name="writer">A <see cref="JsonWriter"/> into which this method will write.</param>
        ///// <param name="converters">A collection of <see cref="JsonConverter"/> which will be used when writing the token.</param>
        //public override void WriteTo(JsonWriter writer, params JsonConverter[] converters)
        //{
        //    writer.WriteStartConstructor(_name);

        //    foreach (JToken token in Children())
        //    {
        //        token.WriteTo(writer, converters);
        //    }

        //    writer.WriteEndConstructor();
        //}

        ///// <summary>
        ///// Gets the <see cref="JToken"/> with the specified key.
        ///// </summary>
        ///// <value>The <see cref="JToken"/> with the specified key.</value>
        //public override JToken this[object key]
        //{
        //    get
        //    {
        //        ValidationUtils.ArgumentNotNull(key, "o");

        //        if (!(key is int))
        //            throw new ArgumentException("Accessed JConstructor values with invalid key value: {0}. Argument position index expected.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.ToString(key)));

        //        return GetItem((int)key);
        //    }
        //    set
        //    {
        //        ValidationUtils.ArgumentNotNull(key, "o");

        //        if (!(key is int))
        //            throw new ArgumentException("Set JConstructor values with invalid key value: {0}. Argument position index expected.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.ToString(key)));

        //        SetItem((int)key, value);
        //    }
        //}

        //internal override int GetDeepHashCode()
        //{
        //    return _name.GetHashCode() ^ ContentsHashCode();
        //}

        /// <summary>
        /// Loads an <see cref="JConstructor"/> from a <see cref="JsonReader"/>. 
        /// </summary>
        /// <param name="reader">A <see cref="JsonReader"/> that will be read for the content of the <see cref="JConstructor"/>.</param>
        /// <returns>A <see cref="JConstructor"/> that contains the JSON that was read from the specified <see cref="JsonReader"/>.</returns>
        public static new JConstructor Load(JsonReader reader)
        {
            if (reader.TokenType == JsonToken.None)
            {
                if (!reader.Read())
                    throw new Exception("Error reading JConstructor from JsonReader.");
            }

            if (reader.TokenType != JsonToken.StartConstructor)
                throw new Exception("Error reading JConstructor from JsonReader. Current JsonReader item is not a constructor: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));

            JConstructor c = new JConstructor((string)reader.Value);
            c.SetLineInfo(reader as IJsonLineInfo);

            c.ReadTokenFrom(reader);

            return c;
        }
    }
}