using System;
using System.Globalization;

namespace Doctrina.Math
{
    /// <summary>
    ///     A rational number in the interval (0, 1), e.g. 0 &gt; x &lt; 1
    /// </summary>
    public class ProperFraction
    {
        public ProperFraction(double value)
        {
            if (value <= 0 || value >= 1)
                throw new ArgumentOutOfRangeException(nameof(value), value,
                    "The propability should be a value between 0 and 1");
            Value = value;
        }

        public double Value { get; }
        public bool Equals(double other) => Value.Equals(other);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ProperFraction @return && Equals(@return);
        }

        public override int GetHashCode() => Value.GetHashCode();

        public static implicit operator ProperFraction(double value) => new ProperFraction(value);

        public static implicit operator double(ProperFraction @return) => @return.Value;

        public static bool operator ==(ProperFraction @return, ProperFraction other) =>
            @return.Equals(other.Value);

        public static bool operator !=(ProperFraction @return, ProperFraction other) => !@return
            .Equals(other.Value);

        public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
    }


    public sealed class Gamma : ProperFraction
    {
        public Gamma(double value) : base(value)
        {
        }
    }

    /// <summary>
    ///     Discount factor
    /// </summary>
    /// <seealso cref="ProperFraction" />
    public sealed class Lambda : ProperFraction
    {
        public Lambda(double value) : base(value)
        {
        }
    }

    public sealed class Epsilon : ProperFraction
    {
        public Epsilon(double value) : base(value)
        {
        }
    }
}