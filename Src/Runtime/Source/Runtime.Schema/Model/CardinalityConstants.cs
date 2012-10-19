using Microsoft.VisualStudio.Patterning.Runtime;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema
{
    /// <summary>
	/// Provides constants and helpers for managing Cardinality.
    /// </summary>
	internal static class CardinalityConstants
    {
        /// <summary>
		/// Returns the string value of the cardinality value.
        /// </summary>
		public static string ConvertToString(Cardinality value)
        {
            switch (value)
            {
				case Cardinality.OneToOne:
					return CardinalityConstants.Captions.OneToOne;

				case Cardinality.ZeroToOne:
					return CardinalityConstants.Captions.ZeroToOne;

				case Cardinality.OneToMany:
					return CardinalityConstants.Captions.OneToMany;

				case Cardinality.ZeroToMany:
					return CardinalityConstants.Captions.ZeroToMany;
            }

			return CardinalityConstants.Captions.OneToOne;
		}

        /// <summary>
		/// Defines the captions for cardinality.
        /// </summary>
        public static class Captions
        {
            /// <summary>
			/// The text caption for OneToOne cardinality.
            /// </summary>
            public static readonly string OneToOne = "1..1";

			/// <summary>
			/// The text cpation for ZeroToOne cardinality.
			/// </summary>
			public static readonly string ZeroToOne = "0..1";

			/// <summary>
			/// The text cpation for OneToMany cardinality.
			/// </summary>
			public static readonly string OneToMany = "1..*";

            /// <summary>
			/// The text cpation for ZeroToMany cardinality.
            /// </summary>
            public static readonly string ZeroToMany = "0..*";
        }
    }
}