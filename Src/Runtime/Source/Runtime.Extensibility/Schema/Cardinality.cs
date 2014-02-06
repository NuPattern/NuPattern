using System.ComponentModel;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Cardinality of the relationship.
    /// </summary>
    public enum Cardinality
    {
        /// <summary>
        /// OneToOne instance of this item.
        /// </summary>
        [Description("OneToOne instance of this item.")]
        OneToOne = 0,

        /// <summary>
        /// ZeroToOne instance of this item.
        /// </summary>
        [Description("ZeroToOne instance of this item.")]
        ZeroToOne = 1,

        /// <summary>
        /// OneToMany instance of this item.
        /// </summary>
        [Description("OneToMany instance of this item.")]
        OneToMany = 2,

        /// <summary>
        /// ZeroToMany instances of this item.
        /// </summary>
        [Description("ZeroToMany instances of this item.")]
        ZeroToMany = 3,
    }

    /// <summary>
    /// Extensions to the <see cref="Cardinality"/> enumeration.
    /// </summary>
    public static class CardinalityExtensions
    {
        /// <summary>
        /// Returns whether the cardinality is either <see cref="Cardinality.ZeroToOne"/> or <see cref="Cardinality.OneToOne"/>
        /// </summary>
        public static bool IsAnyToOne(this Cardinality cardinality)
        {
            return cardinality == Cardinality.OneToOne || cardinality == Cardinality.ZeroToOne;
        }

        /// <summary>
        /// Returns whether the cardinality is either <see cref="Cardinality.ZeroToMany"/> or <see cref="Cardinality.OneToMany"/>
        /// </summary>
        public static bool IsAnyToMany(this Cardinality cardinality)
        {
            return cardinality == Cardinality.OneToMany || cardinality == Cardinality.ZeroToMany;
        }
    }
}