using System;

namespace DeepQLearning.ConvnetSharp
{
    [Serializable]
    public struct Entry
    {
        public int Dim { get; set; }
        public double Val { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is Entry))
            {
                return false;
            }

            var entry = (Entry)obj;
            return Dim == entry.Dim &&
                   Val == entry.Val;
        }

        public override int GetHashCode()
        {
            var hashCode = 830338781;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + Dim.GetHashCode();
            hashCode = hashCode * -1521134295 + Val.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Entry left, Entry right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Entry left, Entry right)
        {
            return !(left == right);
        }
    }
}