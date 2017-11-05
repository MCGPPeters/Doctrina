using System.Collections.Generic;
using Doctrina.Math.LinearAlgebra;

namespace Doctrina.Tests
{
    /// <summary>
    ///     Represents an individual measurable property or characteristic of a phenomenon being observed
    /// </summary>
    public struct Feature
    {
        public bool Equals(Feature other) => EqualityComparer<string>.Default.Equals(Description, other.Description) &&
                                             Value.Equals(other.Value);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Feature && Equals((Feature) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<string>.Default.GetHashCode(Description) * 397) ^ Value.GetHashCode();
            }
        }

        public string Description { get; }
        public Vector Value { get; }

        public Feature(string description, Vector value)
        {
            Description = description;
            Value = value;
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="Value" /> to <see cref="double" />.
        /// </summary>
        /// <param name="feature">The stream identifier.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator Vector(Feature feature) => feature.Value;

        public static bool operator ==(Feature left, Feature right) => left.Equals(right);

        public static bool operator !=(Feature left, Feature right) => !(left == right);
    }
}