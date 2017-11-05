using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Doctrina.Tests
{
    public class ParameterVector : IEnumerable<Parameter>
    {
        private readonly Parameter[] _weights;

        private ParameterVector(IEnumerable<double> weights)
        {
            _weights = weights.Select(d => new Parameter("Weight", d)).ToArray();
        }

        /// <summary>Indexer to set items within this collection using array index syntax.</summary>
        /// <param name="slice">The slice.</param>
        /// <returns>The indexed item.</returns>
        public Parameter this[IEnumerable<int> slice]
        {
            set
            {
                foreach (var i in slice)
                    this[i] = value;
            }
        }

        /// <summary>
        ///     Indexer to get or set items within this collection using array index syntax.
        /// </summary>
        /// <param name="i">Zero-based index of the entry to access.</param>
        /// <returns>The indexed item.</returns>
        public Parameter this[int i]
        {
            get => _weights[i];
            set => _weights[i] = value;
        }

        /// <summary>Gets the length.</summary>
        /// <value>The length.</value>
        public int Length => _weights.Length;

        public IEnumerator<Parameter> GetEnumerator()
        {
            for (var i = 0; i < Length; i++)
                yield return this[i];
        }

        /// <inheritdoc />
        /// <summary>Gets the enumerator.</summary>
        /// <returns>The enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            for (var i = 0; i < Length; i++)
                yield return this[i];
        }

        /// <summary>FeatureVector casting operator.</summary>
        /// <param name="array">The array.</param>
        public static implicit operator ParameterVector(double[] array) => new ParameterVector(array);

        /// <summary>FeatureVector casting operator.</summary>
        /// <param name="array">The array.</param>
        public static implicit operator ParameterVector(int[] array)
        {
            var vector = new double[array.Length];
            for (var i = 0; i < array.Length; i++)
                vector[i] = array[i];

            return new ParameterVector(vector);
        }

        /// <summary>FeatureVector casting operator.</summary>
        /// <param name="array">The array.</param>
        public static implicit operator ParameterVector(float[] array)
        {
            var vector = new double[array.Length];
            for (var i = 0; i < array.Length; i++)
                vector[i] = array[i];

            return new ParameterVector(vector);
        }
    }
}