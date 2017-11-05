using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Doctrina.Tests
{
    public class FeatureVector : IEnumerable<Feature>
    {
        private readonly Feature[] _features;

        private FeatureVector(IEnumerable<double> doubles)
        {
            var bias = new[] {new Feature("Bias", new[] {1})};
            var enumerable = doubles.Select((d, i) => new Feature($"x{i}", new[] {d}));
            _features = bias.Concat(enumerable).ToArray();
        }

        /// <summary>Indexer to set items within this collection using array index syntax.</summary>
        /// <param name="slice">The slice.</param>
        /// <returns>The indexed item.</returns>
        public Feature this[IEnumerable<int> slice]
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
        public Feature this[int i]
        {
            get => _features[i];
            set => _features[i] = value;
        }

        /// <summary>Gets the length.</summary>
        /// <value>The length.</value>
        public int Length => _features.Length;

        public IEnumerator<Feature> GetEnumerator()
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
        public static implicit operator FeatureVector(double[] array) => new FeatureVector(array);

        /// <summary>FeatureVector casting operator.</summary>
        /// <param name="array">The array.</param>
        public static implicit operator FeatureVector(int[] array)
        {
            var vector = new double[array.Length];
            for (var i = 0; i < array.Length; i++)
                vector[i] = array[i];

            return new FeatureVector(vector);
        }

        /// <summary>FeatureVector casting operator.</summary>
        /// <param name="array">The array.</param>
        public static implicit operator FeatureVector(float[] array)
        {
            var vector = new double[array.Length];
            for (var i = 0; i < array.Length; i++)
                vector[i] = array[i];

            return new FeatureVector(vector);
        }
    }
}