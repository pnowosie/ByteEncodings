namespace ByteEncodings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using JetBrains.Annotations;
    using System.Numerics;

    /// <summary>
    /// Allows convertion from bytes to text (and reverse) by mapping digits list given by BaseConverter to chosen alphabet.
    /// </summary>
    public interface IAlphabet
    {
        /// <summary>
        /// Available alphabet - digits in number representation
        /// </summary>
        string Digits { get; }

        /// <summary>
        /// The radix (convertion base)
        /// </summary>
        int Radix { get; }

        /// <summary>
        /// Converts <see cref="number"/> to its representation in base <see cref="Radix"/>
        /// </summary>
        /// <param name="number"></param>
        /// <returns>Representation of a <see cref="number"/> in base <see cref="Radix"/></returns>
        string GetString(BigInteger number);

        /// <summary>
        /// Converts <see cref="bytes"/> expressed as <see cref="BigInteger"/> to its representation in base <see cref="Radix"/>
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns>Representation of a <see cref="BigInteger"/> in base <see cref="Radix"/></returns>
        string GetString([NotNull]IEnumerable<byte> bytes);

        /// <summary>
        /// Converts <see cref="encoding"/> representation in base <see cref="Radix"/> to <see cref="byte"/>s
        /// </summary>
        /// <param name="encoding">Number representation in base <see cref="Radix"/></param>
        /// <returns><see cref="BigInteger"/> value in <see cref="byte"/>s</returns>
        IEnumerable<byte> GetBytes([NotNull]string encoding);
    }

    public partial class Alphabet : IAlphabet
    {
        private readonly IBaseConverter _baseConverter;
        private readonly int[] _digits;

        public Alphabet([NotNull]string digits, [NotNull]IBaseConverter baseConverter)
        {
            if (digits == null)
                throw new ArgumentNullException(nameof(digits));

            _baseConverter = baseConverter ?? throw new ArgumentNullException(nameof(baseConverter));
            var digitBytes = Encoding.ASCII.GetBytes(digits);

            if (digits.Length != digitBytes.Length
             || digitBytes.Length < 2)
                throw new ArgumentException("Digits have to be as many as provided radix, at least 2", nameof(digits));
            if (digitBytes.Length > byte.MaxValue+1)
                throw new ArgumentException("Digits can contain at most 256 characters", nameof(digits));

            // validate every digit occurs once
            _digits = Enumerable.Repeat(-1, byte.MaxValue + 1).ToArray();
            for (var i = 0; i < digitBytes.Length; i++)
            {
                if (_digits[digitBytes[i]] != -1)
                    throw new ArgumentException("Each digit can occur only once", nameof(digits));
                _digits[digitBytes[i]] = i;
            }

            Radix = digitBytes.Length;
            Digits = digits;
        }

        public Alphabet([NotNull]string digits)
            : this(digits, new BaseConverter())
        { }

        public Alphabet([NotNull]string digits, int radix)
            : this(digits.Substring(0, radix))
        { }

        public string Digits { get; }

        public int Radix { get; }

        public string GetString(BigInteger number)
        {
            return JoinDigits(
                _baseConverter.ToBaseN(number, Radix),
                Radix,
                Digits);
        }

        public string GetString(IEnumerable<byte> bytes)
        {
            bytes = bytes?.ToArray() 
                  ?? throw new ArgumentNullException(nameof(bytes));

            return JoinDigits(
                _baseConverter.ToBaseN(bytes, Radix), 
                Radix, 
                Digits);
        }

        public IEnumerable<byte> GetBytes(string encoding)
        {
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            return _baseConverter.FromBaseN(
                Encoding.ASCII.GetBytes(encoding).Select(b => _digits[b]),
                Radix);
        }

        private static string JoinDigits(IEnumerable<int> digits, int radix, string alphabet)
        {
            return string.Join(
                string.Empty,
                digits
                    .Select(d => {
                        if (d >= radix)
                            throw new ArgumentException($"Digit '{d}' too big to represent value in base {radix}.");
                        return alphabet[d];
                    }));
        }
    }
}
