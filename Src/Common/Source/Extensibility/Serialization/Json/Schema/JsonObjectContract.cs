using System;
using System.Reflection;

namespace NuPattern.Extensibility.Serialization.Json
{
    /// <summary>
    /// Contract details for a <see cref="Type"/> used by the <see cref="JsonSerializer"/>.
    /// </summary>
    public class JsonObjectContract : JsonContract
    {
        ///// <summary>
        ///// Gets or sets the object member serialization.
        ///// </summary>
        ///// <value>The member object serialization.</value>
        //public MemberSerialization MemberSerialization { get; set; }

        /// <summary>
        /// Gets the object's properties.
        /// </summary>
        /// <value>The object's properties.</value>
        public JsonPropertyCollection Properties { get; private set; }

        /// <summary>
        /// Gets the constructor parameters required for any non-default constructor
        /// </summary>
        public JsonPropertyCollection ConstructorParameters { get; private set; }

        /// <summary>
        /// Gets or sets the override constructor used to create the object.
        /// This is set when a constructor is marked up using the
        /// JsonConstructor attribute.
        /// </summary>
        /// <value>The override constructor.</value>
        public ConstructorInfo OverrideConstructor { get; set; }

        /// <summary>
        /// Gets or sets the parametrized constructor used to create the object.
        /// </summary>
        /// <value>The parametrized constructor.</value>
        public ConstructorInfo ParametrizedConstructor { get; set; }

        ///// <summary>
        ///// Initializes a new instance of the <see cref="JsonObjectContract"/> class.
        ///// </summary>
        ///// <param name="underlyingType">The underlying type for the contract.</param>
        //public JsonObjectContract(Type underlyingType)
        //    : base(underlyingType)
        //{
        //    Properties = new JsonPropertyCollection(UnderlyingType);
        //    ConstructorParameters = new JsonPropertyCollection(UnderlyingType);
        //}
    }
}