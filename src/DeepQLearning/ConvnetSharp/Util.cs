using System;
using System.Collections.Generic;

namespace DeepQLearning.ConvnetSharp
{
    // a window stores _size_ number of values
    // and returns averages. Useful for keeping running
    // track of validation or training accuracy during SGD

    [Serializable]
    public class Util
    {
        // Random number utilities
        private bool _returnV;

        private double _vVal;
        public Random Random { get; } = new Random();

        private double GaussianRandom()
        {
            while (true)
            {
                if (_returnV)
                {
                    _returnV = false;
                    return _vVal;
                }

                var u = 2 * Random.NextDouble() - 1;
                var v = 2 * Random.NextDouble() - 1;
                var r = u * u + v * v;
                const double epsilon = 0.0001;
                if (Math.Abs(r) < epsilon || r > 1) continue;
                var c = Math.Sqrt(-2 * Math.Log(r) / r);
                _vVal = v * c; // cache this
                _returnV = true;
                return u * c;
            }
        }

        public double RandomDouble(double a, double b) => Random.NextDouble() * (b - a) + a;

        public double Randn(double mu, double std) => mu + GaussianRandom() * std;

        // Array utilities
        public static double[] Zeros(int n)
        {
            if (n <= 0)
                return new[] {0.0};
            var arr = new double[n];
            for (var i = 0; i < n; i++) arr[i] = 0;
            return arr;
        }

        public void Assert(bool condition, string message)
        {
            if (!condition)
                throw new Exception(message);
        }

        public override bool Equals(object obj)
        {
            var util = obj as Util;
            return util != null &&
                   EqualityComparer<Random>.Default.Equals(Random, util.Random) &&
                   _returnV == util._returnV &&
                   _vVal == util._vVal;
        }

        public override int GetHashCode()
        {
            var hashCode = 645020419;
            hashCode = hashCode * -1521134295 + EqualityComparer<Random>.Default.GetHashCode(Random);
            hashCode = hashCode * -1521134295 + _returnV.GetHashCode();
            hashCode = hashCode * -1521134295 + _vVal.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Util util1, Util util2) => EqualityComparer<Util>.Default.Equals(util1, util2);
        public static bool operator !=(Util util1, Util util2) => !(util1 == util2);
    }
}