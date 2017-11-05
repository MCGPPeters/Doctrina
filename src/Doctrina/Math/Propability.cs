namespace Doctrina.Math
{
    /// <inheritdoc />
    /// <summary>
    ///     Learning rate
    /// </summary>
    /// <seealso cref="T:Doctrina.Tests.PositiveProperFraction" />
    public sealed class Propability : ProperFraction
    {
        private Propability(double value) : base(value)
        {
        }

        private bool Equals(ProperFraction other) => System.Math.Abs(Value - other.Value) < 0.001;

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Propability && Equals((Propability) obj);
        }

        public override int GetHashCode() => Value.GetHashCode();

        public static implicit operator Propability(double value) => new Propability(value);

        public static implicit operator double(Propability @return) => @return.Value;

        public static bool operator ==(Propability @return, Propability other) =>
            @return.Equals(other.Value);

        public static bool operator !=(Propability @return, Propability other) => @return != null && !@return
                                                                                      .Equals(other.Value);
    }
}