using System;
using System.Collections.Generic;

namespace DeepQLearning.ConvnetSharp
{
    [Serializable]
    public class TrainingWindow
    {
        private readonly int _minsize;
        private readonly int _size;
        private readonly List<double> _v;
        private double _sum;

        public TrainingWindow(int size, int minsize)
        {
            _v = new List<double>();
            _size = size <= minsize ? 100 : size;
            _minsize = minsize <= 2 ? 20 : minsize;
            _sum = 0;
        }

        public void Add(double x)
        {
            _v.Add(x);
            _sum += x;
            if (_v.Count <= _size) return;
            var xold = _v[0];
            _v.RemoveAt(0);
            _sum -= xold;
        }

        public double get_average()
        {
            if (_v.Count < _minsize)
                return -1;
            return _sum / _v.Count;
        }
    }
}