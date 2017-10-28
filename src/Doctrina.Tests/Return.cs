using System.Collections.Generic;

namespace Doctrina.Tests
{
    /// <summary>
    /// Total discounted reward from a timestamp T on onwards
    /// </summary>
    public struct Return
    {
        public bool Equals(double other) => Value.Equals(other);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Return && Equals((Return)obj);
        }

        public override int GetHashCode() => Value.GetHashCode();

        public double Value { get; }

        public Return(double @return) => Value = @return;

        public static implicit operator Return(double value) => new Return(value);

        public static implicit operator double(Return @return) => @return.Value;

        public static implicit operator Return(Reward value) => new Return(value);

        public static bool operator ==(Return @return, Return other) =>
            @return.Equals(other.Value);

        public static bool operator !=(Return @return, Return other) => !@return
            .Equals(other.Value);
    }
}