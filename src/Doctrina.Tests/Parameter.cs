using System.Collections.Generic;

namespace Doctrina.Tests
{
    public struct Parameter
    {
        public bool Equals(Feature other) => EqualityComparer<string>.Default.Equals(Descriptor, other.Description) &&
                                             Scalar.Equals(other.Value);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Feature && Equals((Feature) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<string>.Default.GetHashCode(Descriptor) * 397) ^ Scalar.GetHashCode();
            }
        }

        public string Descriptor { get; }

        /// <summary>
        ///     Gets or sets the value. In the domain of neural networks this is often called the weight
        /// </summary>
        /// <value>
        ///     The value.
        /// </value>
        public double Scalar { get; }

        public Parameter(string descriptor, double scalar)
        {
            Descriptor = descriptor;
            Scalar = scalar;
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="double" /> to <see cref="Parameter" />.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator double(Parameter parameter) => parameter.Scalar;

        /// <summary>
        ///     Performs an implicit conversion from <see cref="Parameter" /> to <see cref="double" />.
        /// </summary>
        /// <param name="weight"></param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator Parameter(double weight) => new Parameter("weight", weight);

        public static bool operator ==(Parameter left, Parameter right) => left.Equals(right);

        public static bool operator !=(Parameter left, Parameter right) => !(left == right);
    }
}