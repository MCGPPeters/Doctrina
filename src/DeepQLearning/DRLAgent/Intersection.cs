using System;

namespace DeepQLearning.DRLAgent
{
    [Serializable]
    public struct Intersection
    {
        private bool Equals(Intersection other) => Ua.Equals(other.Ua) && Ub.Equals(other.Ub) && Up.Equals(other.Up) &&
                                                  ObjectType == other.ObjectType && Intersect == other.Intersect;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Intersection && Equals((Intersection) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Ua.GetHashCode();
                hashCode = (hashCode * 397) ^ Ub.GetHashCode();
                hashCode = (hashCode * 397) ^ Up.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) ObjectType;
                hashCode = (hashCode * 397) ^ Intersect.GetHashCode();
                return hashCode;
            }
        }

        public double Ua { get; set; }
        public double Ub { private get; set; }
        public Vector Up { get; set; }
        public ObjectType ObjectType { get; set; }
        public bool Intersect { get; set; }


        public static bool operator ==(Intersection left, Intersection right) => left.Equals(right);

        public static bool operator !=(Intersection left, Intersection right) => !(left == right);
    }
}