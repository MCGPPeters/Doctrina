using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Doctrina.Tests
{
    /// <summary>
    /// A simple example for Reinforcement Learning using table lookup Q-learning method.
    /// An agent "o" is on the left of a 1 dimensional world, the treasure is on the rightmost location.
    /// Run this program and to see how the agent will improve its strategy of finding the treasure.
    /// </summary>
    public class TreaseHuntProperties
    {

    }

    /// <summary>
    /// A policy π is a distribution over actions given a <typeparam name="TState">state</typeparam>. I.e.
    /// the propability that the agent will take action A given being in a state S)
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <typeparam name="TReward"></typeparam>
    /// <param name="state"></param>
    /// <returns></returns>
    public delegate Distribution<Action<TState>> Policy<TState>(TState state, Action<TState>[] actionSpace);

    public class Distribution<T> : ReadOnlyDictionary<T, double>
    {
        public Distribution(IDictionary<T, double> dictionary) : base(dictionary)
        {
            if(Math.Abs(dictionary.Values.Sum()) > 0f) throw new ArgumentException("The sum of all probabilities does not add up to 1");
        }
    }

    public delegate (TState nextState, Reward attainedReward) Action<TState>(TState state);

    /// <summary>
    /// Determines the expected return of taking <paramref name="action"></paramref> in <paramref name="state"></paramref> 
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="action"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public delegate Return ActionValue<TState>(Action<TState> action, TState state);

    /// <summary>
    /// Total reward for being in a state S (how 'good' is it to be in state S)
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

        public static implicit operator double(Reward @return) => @return.Value;

        public static bool operator ==(Reward reward, Reward other) =>
            reward.Equals(other.Value);

        public static bool operator !=(Reward reward, Reward other) => !reward
            .Equals(other.Value);
    }

    /// <summary>
    /// Total discounter reward from a timestamp T on onwards
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

        public static bool operator ==(Return @return, Return other) =>
            @return.Equals(other.Value);

        public static bool operator !=(Return @return, Return other) => !@return
            .Equals(other.Value);
    }

    public static class Policies
    {
        /// <summary>
        /// The epsilon-greedy policy either takes a random action with
        //  probability epsilon, or it takes the action for the highest
        //  Q-value (Q-value, or action value, is the expected return (total commulative reward) taking a action A in state S).
        //
        //    If epsilon is 1.0 then the actions are always random.
        //    If epsilon is 0.0 then the actions are always argmax for the Q-values.
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="epsilon"></param>
        /// <param name="actionValue"></param>
        /// <returns></returns>
        public static Policy<TState> EpsilonGreedy<TState>(float epsilon, ActionValue<TState> actionValue) =>
            (state, actionSpace) =>
            {
                var actionValues = actionSpace.ToDictionary(action => action, action => actionValue(action, state));
                var actionDistribution = new Dictionary<Action<TState>, double>();

                var cryptoRandom = new CryptoRandom();
                if (cryptoRandom.NextDouble() < epsilon)
                {
                    var randomIndex = cryptoRandom.Next(0, actionSpace.Length - 1);
                    actionDistribution.Add(actionValues.ElementAt(randomIndex).Key, 1.0d);
                }
                else
                {
                    var expectedMaximumReturn = actionValues.Values.Select(@return => @return.Value).ToArray().ArgMax();
                    var actionWithMaximumExpectedValue =
                        actionValues.Single(pair => pair.Value == expectedMaximumReturn).Key;
                    actionDistribution.Add(actionWithMaximumExpectedValue, 1.0d);
                }
                
                return new Distribution<Action<TState>>(actionDistribution);
            };
    }
    public static class Math
    {
        // Arg Max 
        /// <summary>
        ///   Gets the maximum element in a vector.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int ArgMax<T>(this T[] values)
            where T : IComparable<T>
        {
            var imax = 0;
            var max = values[0];
            for (var i = 1; i < values.Length; i++)
            {
                if (values[i].CompareTo(max) > 0)
                {
                    max = values[i];
                    imax = i;
                }
            }

            return imax;
        }
    }

        /// <summary>
        /// A random number generator based on the RNGCryptoServiceProvider.
        /// Adapted from the "Tales from the CryptoRandom" article in MSDN Magazine (September 2007)
        /// but with explicit guarantee to be thread safe. Note that this implementation also includes
        /// an optional (enabled by default) random buffer which provides a significant speed boost as
        /// it greatly reduces the amount of calls into unmanaged land.
        /// </summary>
        public class CryptoRandom : Random
        {
            private RNGCryptoServiceProvider _rng = new RNGCryptoServiceProvider();

            private byte[] _buffer;

            private int _bufferPosition;

            /// <summary>
            /// Gets a value indicating whether this instance has random pool enabled.
            /// </summary>
            /// <value>
            ///     <c>true</c> if this instance has random pool enabled; otherwise, <c>false</c>.
            /// </value>
            public bool IsRandomPoolEnabled { get; private set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="CryptoRandom"/> class with.
            /// Using this overload will enable the random buffer pool.
            /// </summary>
            public CryptoRandom() : this(true) { }

            /// <summary>
            /// Initializes a new instance of the <see cref="CryptoRandom"/> class.
            /// This method will disregard whatever value is passed as seed and it's only implemented
            /// in order to be fully backwards compatible with <see cref="System.Random"/>.
            /// Using this overload will enable the random buffer pool.
            /// </summary>
            /// <param name="ignoredSeed">The ignored seed.</param>
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "ignoredSeed", Justification = "Cannot remove this parameter as we implement the full API of System.Random")]
            public CryptoRandom(int ignoredSeed) : this(true) { }

            /// <summary>
            /// Initializes a new instance of the <see cref="CryptoRandom"/> class with
            /// optional random buffer.
            /// </summary>
            /// <param name="enableRandomPool">set to <c>true</c> to enable the random pool buffer for increased performance.</param>
            public CryptoRandom(bool enableRandomPool)
            {
                IsRandomPoolEnabled = enableRandomPool;
            }

            private void InitBuffer()
            {
                if (IsRandomPoolEnabled)
                {
                    if (_buffer == null || _buffer.Length != 512)
                        _buffer = new byte[512];
                }
                else
                {
                    if (_buffer == null || _buffer.Length != 4)
                        _buffer = new byte[4];
                }

                _rng.GetBytes(_buffer);
                _bufferPosition = 0;
            }

            /// <summary>
            /// Returns a nonnegative random number.
            /// </summary>
            /// <returns>
            /// A 32-bit signed integer greater than or equal to zero and less than <see cref="F:System.Int32.MaxValue"/>.
            /// </returns>
            public override int Next()
            {
                // Mask away the sign bit so that we always return nonnegative integers
                return (int)GetRandomUInt32() & 0x7FFFFFFF;
            }

            /// <summary>
            /// Returns a nonnegative random number less than the specified maximum.
            /// </summary>
            /// <param name="maxValue">The exclusive upper bound of the random number to be generated. <paramref name="maxValue"/> must be greater than or equal to zero.</param>
            /// <returns>
            /// A 32-bit signed integer greater than or equal to zero, and less than <paramref name="maxValue"/>; that is, the range of return values ordinarily includes zero but not <paramref name="maxValue"/>. However, if <paramref name="maxValue"/> equals zero, <paramref name="maxValue"/> is returned.
            /// </returns>
            /// <exception cref="T:System.ArgumentOutOfRangeException">
            ///     <paramref name="maxValue"/> is less than zero.
            /// </exception>
            public override int Next(int maxValue)
            {
                if (maxValue < 0)
                    throw new ArgumentOutOfRangeException("maxValue");

                return Next(0, maxValue);
            }

            /// <summary>
            /// Returns a random number within a specified range.
            /// </summary>
            /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
            /// <param name="maxValue">The exclusive upper bound of the random number returned. <paramref name="maxValue"/> must be greater than or equal to <paramref name="minValue"/>.</param>
            /// <returns>
            /// A 32-bit signed integer greater than or equal to <paramref name="minValue"/> and less than <paramref name="maxValue"/>; that is, the range of return values includes <paramref name="minValue"/> but not <paramref name="maxValue"/>. If <paramref name="minValue"/> equals <paramref name="maxValue"/>, <paramref name="minValue"/> is returned.
            /// </returns>
            /// <exception cref="T:System.ArgumentOutOfRangeException">
            ///     <paramref name="minValue"/> is greater than <paramref name="maxValue"/>.
            /// </exception>
            public override int Next(int minValue, int maxValue)
            {
                if (minValue > maxValue)
                    throw new ArgumentOutOfRangeException("minValue");

                if (minValue == maxValue)
                    return minValue;

                long diff = maxValue - minValue;

                while (true)
                {
                    uint rand = GetRandomUInt32();

                    long max = 1 + (long)uint.MaxValue;
                    long remainder = max % diff;

                    if (rand < max - remainder)
                        return (int)(minValue + (rand % diff));
                }
            }

            /// <summary>
            /// Returns a random number between 0.0 and 1.0.
            /// </summary>
            /// <returns>
            /// A double-precision floating point number greater than or equal to 0.0, and less than 1.0.
            /// </returns>
            public override double NextDouble()
            {
                return GetRandomUInt32() / (1.0 + uint.MaxValue);
            }

            /// <summary>
            /// Fills the elements of a specified array of bytes with random numbers.
            /// </summary>
            /// <param name="buffer">An array of bytes to contain random numbers.</param>
            /// <exception cref="T:System.ArgumentNullException">
            ///     <paramref name="buffer"/> is null.
            /// </exception>
            public override void NextBytes(byte[] buffer)
            {
                if (buffer == null)
                    throw new ArgumentNullException("buffer");

                lock (this)
                {
                    if (IsRandomPoolEnabled && _buffer == null)
                        InitBuffer();

                    // Can we fit the requested number of bytes in the buffer?
                    if (IsRandomPoolEnabled && _buffer.Length <= buffer.Length)
                    {
                        int count = buffer.Length;

                        EnsureRandomBuffer(count);

                        Buffer.BlockCopy(_buffer, _bufferPosition, buffer, 0, count);

                        _bufferPosition += count;
                    }
                    else
                    {
                        // Draw bytes directly from the RNGCryptoProvider
                        _rng.GetBytes(buffer);
                    }
                }
            }

            /// <summary>
            /// Gets one random unsigned 32bit integer in a thread safe manner.
            /// </summary>
            private uint GetRandomUInt32()
            {
                lock (this)
                {
                    EnsureRandomBuffer(4);

                    uint rand = BitConverter.ToUInt32(_buffer, _bufferPosition);

                    _bufferPosition += 4;

                    return rand;
                }
            }

            /// <summary>
            /// Ensures that we have enough bytes in the random buffer.
            /// </summary>
            /// <param name="requiredBytes">The number of required bytes.</param>
            private void EnsureRandomBuffer(int requiredBytes)
            {
                if (_buffer == null)
                    InitBuffer();

                if (requiredBytes > _buffer.Length)
                    throw new ArgumentOutOfRangeException("requiredBytes", "cannot be greater than random buffer");

                if ((_buffer.Length - _bufferPosition) < requiredBytes)
                    InitBuffer();
            }
        }
    }
