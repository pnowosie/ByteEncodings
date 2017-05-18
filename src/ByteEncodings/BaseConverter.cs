namespace ByteEncodings
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Numerics;
    using JetBrains.Annotations;

    /// <summary>
    /// Provides methods to express a number in any integer base
    /// </summary>
    public interface IBaseConverter
    {
        /// <summary>
        /// Converts nonegative big integer <see cref="number"/> to its representation in base <see cref="radix"/>
        /// </summary>
        /// <param name="number"> The nonegative value.</param>
        /// <param name="radix"> The base radix.</param>
        /// <returns>
        /// The collection of digits in base <see cref="radix"/> from least to most significant.
        /// </returns>
        IEnumerable<int> ToBaseN(BigInteger number, int radix);

        /// <summary>
        /// Converts unsigned little-endian number in <see cref="bytes" /> to its representation in base <see cref="radix"/>
        /// </summary>
        /// <param name="bytes"> The little-endian unsigned value.</param>
        /// <param name="radix"> The base radix.</param>
        /// <returns>
        /// The collection of digits in base <see cref="radix"/> from least to most significant.
        /// </returns>
        IEnumerable<int> ToBaseN([NotNull]IEnumerable<byte> bytes, int radix);

        /// <summary>
        /// Converts little-endian digits of base <see cref="radix"/> to little-endian byte array
        /// </summary>
        /// <param name="digits">The little-endian digits collection</param>
        /// <param name="radix">The base in which number is expressed</param>
        /// <returns>Little-endian byte array representation of BigInteger.</returns>
        IEnumerable<byte> FromBaseN([NotNull]IEnumerable<int> digits, int radix);
    }

    /// <summary>
    /// Provides methods to express a number in any integer base
    /// </summary>
    public class BaseConverter : IBaseConverter
    {
        /// <summary>
        /// Converts unsigned little-endian number in <see cref="bytes" /> to its representation in base <see cref="radix"/>
        /// </summary>
        /// <param name="bytes"> The little-endian unsigned value.</param>
        /// <param name="radix"> The base radix.</param>
        /// <returns>
        /// The collection of digits in base <see cref="radix"/> from least to most significant.
        /// </returns>
        public IEnumerable<int> ToBaseN(IEnumerable<byte> bytes, int radix)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));

            return ToBaseN(
                // appending 0-byte to make resulting BigInteger value unsigned
                new BigInteger(
                    bytes
                        .Concat(new[] { (byte)0 })
                        .ToArray()),
                radix);
        }

        /// <summary>
        /// Converts nonegative big integer <see cref="number"/> to its representation in base <see cref="radix"/>
        /// </summary>
        /// <param name="number"> The nonegative value.</param>
        /// <param name="radix"> The base radix.</param>
        /// <returns>
        /// The collection of digits in base <see cref="radix"/> from least to most significant.
        /// </returns>
        public IEnumerable<int> ToBaseN(BigInteger number, int radix)
        {
            return GetDigits(BigInteger.Abs(number), radix)
                .ToArray()
                .AsEnumerable();
        }

        /// <summary>
        /// Converts little-endian digits of base <see cref="radix"/> to little-endian byte array
        /// </summary>
        /// <param name="digits">The little-endian digits collection</param>
        /// <param name="radix">The base in which number is expressed</param>
        /// <returns>Little-endian byte array representation of BigInteger.</returns>
        public IEnumerable<byte> FromBaseN(IEnumerable<int> digits, int radix)
        {
            if (digits == null) throw new ArgumentNullException(nameof(digits));

            if (radix < 2)
                throw new ArgumentOutOfRangeException(nameof(radix), "Base has to be at least 2");

            BigInteger result = BigInteger.Zero;
            foreach (var digit in digits.Reverse())
            {
                if (digit >= radix)
                    throw new ArgumentOutOfRangeException(nameof(digits), "Digit cannot be grater than radix");
                result = result * radix + digit;
            }

            return result.ToByteArray();
        }

        private static IEnumerable<int> GetDigits(BigInteger number, int radix)
        {
            if (radix < 2) throw new ArgumentOutOfRangeException(nameof(radix), "Base has to be at least 2");

            while (number != BigInteger.Zero)
            {
                yield return (int)(number % radix);
                number /= radix;
            }
        }
    }
}