namespace Doctrina.Tests
{
    /// <summary>
    /// Total reward for being in a state S at timestep t (how 'good' is it to be in state S). 
    /// The reward for being in the the current state S only acquired while transitioning to St+1
    /// </summary>
    public struct Reward
    {
        public bool Equals(double other) => Value.Equals(other);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Return && Equals((Return)obj);
        }

        public override int GetHashCode() => Value.GetHashCode();

        public double Value { get; }

        public Reward(double reward) => Value = reward;

        public static implicit operator Reward(double value) => new Reward(value);

        public static implicit operator Reward(Return value) => new Reward(value);

        public static implicit operator double(Reward @return) => @return.Value;

        public static bool operator ==(Reward reward, Reward other) =>
            reward.Equals(other.Value);

        public static bool operator !=(Reward reward, Reward other) => !reward
            .Equals(other.Value);
    }
}