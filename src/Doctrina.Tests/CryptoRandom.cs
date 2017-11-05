﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace Doctrina.Tests
{
    /// <summary>
    ///     A random number generator based on the RNGCryptoServiceProvider.
    ///     Adapted from the "Tales from the CryptoRandom" article in MSDN Magazine (September 2007)
    ///     but with explicit guarantee to be thread safe. Note that this implementation also includes
    ///     an optional (enabled by default) random buffer which provides a significant speed boost as
    ///     it greatly reduces the amount of calls into unmanaged land.
    /// </summary>
    public class CryptoRandom : Random
    {
        private byte[] _buffer;

        private int _bufferPosition;
        private readonly RNGCryptoServiceProvider _rng = new RNGCryptoServiceProvider();

        /// <summary>
        ///     Initializes a new instance of the <see cref="CryptoRandom" /> class with.
        ///     Using this overload will enable the random buffer pool.
        /// </summary>
        public CryptoRandom() : this(true)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CryptoRandom" /> class.
        ///     This method will disregard whatever value is passed as seed and it's only implemented
        ///     in order to be fully backwards compatible with <see cref="System.Random" />.
        ///     Using this overload will enable the random buffer pool.
        /// </summary>
        /// <param name="ignoredSeed">The ignored seed.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "ignoredSeed",
            Justification = "Cannot remove this parameter as we implement the full API of System.Random")]
        public CryptoRandom(int ignoredSeed) : this(true)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CryptoRandom" /> class with
        ///     optional random buffer.
        /// </summary>
        /// <param name="enableRandomPool">set to <c>true</c> to enable the random pool buffer for increased performance.</param>
        public CryptoRandom(bool enableRandomPool) => IsRandomPoolEnabled = enableRandomPool;

        /// <summary>
        ///     Gets a value indicating whether this instance has random pool enabled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has random pool enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsRandomPoolEnabled { get; }

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
        ///     Returns a nonnegative random number.
        /// </summary>
        /// <returns>
        ///     A 32-bit signed integer greater than or equal to zero and less than <see cref="F:System.Int32.MaxValue" />.
        /// </returns>
        public override int Next() => (int) GetRandomUInt32() & 0x7FFFFFFF;

        /// <summary>
        ///     Returns a nonnegative random number less than the specified maximum.
        /// </summary>
        /// <param name="maxValue">
        ///     The exclusive upper bound of the random number to be generated. <paramref name="maxValue" />
        ///     must be greater than or equal to zero.
        /// </param>
        /// <returns>
        ///     A 32-bit signed integer greater than or equal to zero, and less than <paramref name="maxValue" />; that is, the
        ///     range of return values ordinarily includes zero but not <paramref name="maxValue" />. However, if
        ///     <paramref name="maxValue" /> equals zero, <paramref name="maxValue" /> is returned.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="maxValue" /> is less than zero.
        /// </exception>
        public override int Next(int maxValue)
        {
            if (maxValue < 0)
                throw new ArgumentOutOfRangeException("maxValue");

            return Next(0, maxValue);
        }

        /// <summary>
        ///     Returns a random number within a specified range.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">
        ///     The exclusive upper bound of the random number returned. <paramref name="maxValue" /> must be
        ///     greater than or equal to <paramref name="minValue" />.
        /// </param>
        /// <returns>
        ///     A 32-bit signed integer greater than or equal to <paramref name="minValue" /> and less than
        ///     <paramref name="maxValue" />; that is, the range of return values includes <paramref name="minValue" /> but not
        ///     <paramref name="maxValue" />. If <paramref name="minValue" /> equals <paramref name="maxValue" />,
        ///     <paramref name="minValue" /> is returned.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="minValue" /> is greater than <paramref name="maxValue" />.
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
                var rand = GetRandomUInt32();

                var max = 1 + (long) uint.MaxValue;
                var remainder = max % diff;

                if (rand < max - remainder)
                    return (int) (minValue + rand % diff);
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     Returns a random number between 0.0 and 1.0.
        /// </summary>
        /// <returns>
        ///     A double-precision floating point number greater than or equal to 0.0, and less than 1.0.
        /// </returns>
        public override double NextDouble() => GetRandomUInt32() / (1.0 + uint.MaxValue);

        /// <inheritdoc />
        /// <summary>
        ///     Fills the elements of a specified array of bytes with random numbers.
        /// </summary>
        /// <param name="buffer">An array of bytes to contain random numbers.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="buffer" /> is null.
        /// </exception>
        public override void NextBytes(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            lock (this)
            {
                if (IsRandomPoolEnabled && _buffer == null)
                    InitBuffer();

                // Can we fit the requested number of bytes in the buffer?
                if (IsRandomPoolEnabled && _buffer.Length <= buffer.Length)
                {
                    var count = buffer.Length;

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
        ///     Gets one random unsigned 32bit integer in a thread safe manner.
        /// </summary>
        private uint GetRandomUInt32()
        {
            lock (this)
            {
                EnsureRandomBuffer(4);

                var rand = BitConverter.ToUInt32(_buffer, _bufferPosition);

                _bufferPosition += 4;

                return rand;
            }
        }

        /// <summary>
        ///     Ensures that we have enough bytes in the random buffer.
        /// </summary>
        /// <param name="requiredBytes">The number of required bytes.</param>
        private void EnsureRandomBuffer(int requiredBytes)
        {
            if (_buffer == null)
                InitBuffer();

            if (requiredBytes > _buffer.Length)
                throw new ArgumentOutOfRangeException("requiredBytes", "cannot be greater than random buffer");

            if (_buffer.Length - _bufferPosition < requiredBytes)
                InitBuffer();
        }
    }
}