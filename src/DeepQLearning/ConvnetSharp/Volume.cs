using System;

namespace DeepQLearning.ConvnetSharp
{
    // Volume is the basic building block of all data in a net.
    // it is essentially just a 3D volume of numbers, with a
    // width (sx), height (sy), and depth (depth).
    // it is used to hold data for all filters, all volumes,
    // all weights, and also stores all gradients w.r.t. 
    // the data. c is optionally a value to initialize the volume
    // with. If c is missing, fills the Vol with random numbers.
    [Serializable]
    public class Volume
    {
        private readonly Util _util = new Util();

        public Volume(int sx, int sy, int depth)
        {
            Init(sx, sy, depth, double.MinValue);
        }

        public Volume(int sx, int sy, int depth, double c)
        {
            Init(sx, sy, depth, c);
        }

        public double[] Dw { get; set; }
        public int Sx { get; private set; }
        public int Sy { get; private set; }
        public int Depth { get; private set; }

        public double[] W { get; set; }

        private void Init(int sx, int sy, int depth, double c)
        {
            Sx = sx;
            Sy = sy;
            Depth = depth;

            var n = sx * sy * depth;
            W = Util.Zeros(n);
            Dw = Util.Zeros(n);

            if (c == double.MinValue)
            {
                // weight normalization is done to equalize the output
                // variance of every neuron, otherwise neurons with a lot
                // of incoming connections have outputs of larger variance
                var scale = Math.Sqrt(1.0 / (sx * sy * depth));
                for (var i = 0; i < n; i++)
                    W[i] = _util.Randn(0.0, scale);
            }
            else
            {
                for (var i = 0; i < n; i++)
                    W[i] = c;
            }
        }

        public void Set(int x, int y, int d, double v)
        {
            var ix = (Sx * y + x) * Depth + d;
            W[ix] = v;
        }

        public double GetGradient(int x, int y, int d)
        {
            var ix = (Sx * y + x) * Depth + d;
            return Dw[ix];
        }

        public Volume Clone()
        {
            var V = new Volume(Sx, Sy, Depth, 0.0);
            var n = W.Length;
            for (var i = 0; i < n; i++) V.W[i] = W[i];
            return V;
        }
    }
}