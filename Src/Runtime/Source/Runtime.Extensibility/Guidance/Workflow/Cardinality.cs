using System;
using System.Globalization;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    /// <summary>
    /// Defines the cardinality of a <see cref="Node"/>.
    /// </summary>
    public struct Cardinality
    {
        /// <summary>
        /// The cardinality is none.
        /// </summary>
        public static Cardinality None = new Cardinality(0, 0);

        /// <summary>
        /// The cardinality is exactly one.
        /// </summary>
        public static Cardinality One = new Cardinality(1, 1);

        /// <summary>
        /// The cardinality is zero or one.
        /// </summary>
        public static Cardinality ZeroOrOne = new Cardinality(0, 1);

        /// <summary>
        /// The cardinality is zero to *.
        /// </summary>
        public static Cardinality Any = new Cardinality(0, int.MaxValue);

        /// <summary>
        /// The cardinality is one to *.
        /// </summary>
        public static Cardinality OneOrMore = new Cardinality(1, int.MaxValue);

        private int from;
        private int to;

        private Cardinality(int from, int to)
        {
            this.from = from;
            this.to = to;
        }

        /// <summary>
        /// Gets the lower limit for the cardinality.
        /// </summary>
        /// <value>The lower limit for the cardinality.</value>
        public int From
        {
            get { return this.from; }
        }

        /// <summary>
        /// Gets the upper limit for the cardinality.
        /// </summary>
        /// <value>The upper limit for the cardinality.</value>
        public int To
        {
            get { return this.to; }
        }

        /// <summary>
        /// Returns a <see cref="String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "# " +
                (this.from == int.MaxValue ? "*" : this.from.ToString(CultureInfo.CurrentCulture)) +
                ".." +
                (this.to == int.MaxValue ? "*" : this.to.ToString(CultureInfo.CurrentCulture));
        }
    }
}