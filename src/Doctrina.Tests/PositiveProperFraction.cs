using System;
using System.Globalization;

namespace Doctrina.Tests
{
    /// <summary>
    /// A rational number between 0 and 1
    /// </summary>
    public struct PositiveProperFraction
    {
        public bool Equals(double other) => Value.Equals(other);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Return @return && Equals(@return);
        }

        public override int GetHashCode() => Value.GetHashCode();

        public double Value { get; }

        public PositiveProperFraction(double value)
        {
            if(value < 0 || value > 1) throw new ArgumentOutOfRangeException(nameof(value), value, "The propability should be a value between 0 and 1");
            Value = value;
        }

        public static implicit operator PositiveProperFraction(double value) => new PositiveProperFraction(value);

        public static implicit operator double(PositiveProperFraction @return) => @return.Value;

        public static bool operator ==(PositiveProperFraction @return, PositiveProperFraction other) =>
            @return.Equals(other.Value);

        public static bool operator !=(PositiveProperFraction @return, PositiveProperFraction other) => !@return
            .Equals(other.Value);

        public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
    }
}