namespace ByteEncodings.Test
{
    using System;
    using Xunit;
    using System.Collections.Generic;
    using System.Linq;

    public class DigitsToBytesDecodingTests
    {
        private static readonly IBaseConverter baseConverter = new BaseConverter();
        private static readonly Func<IEnumerable<int>, int, IEnumerable<byte>> FromBaseN = baseConverter.FromBaseN;

        public readonly int[] Radixes = { 2, 3, 7, 10, 16, 32, 64, 128, 256 };

        [Fact]
        public void Zero_encoded_in_any_base_returns_empty_digits()
        {
            var zeros = new int[3];

            foreach (var radix in Radixes)
            {
                Assert.Equal(0, FromBaseN(zeros, radix).Single());
            }
        }

        [Fact]
        public void One_encoded_in_any_base_returns_colection_with_first_digit()
        {
            var one = new int[] { 1 };

            foreach (var radix in Radixes)
            {
                var digits = FromBaseN(one, radix).ToArray();
                Assert.Single(digits);
                Assert.Equal(1, digits[0]);
            }
        }

        [Fact]
        public void Value_lesser_than_radix_returns_single_digit()
        {
            foreach (var radix in Radixes)
            {
                var bytes = FromBaseN(new[]{ radix - 1 }, radix).ToArray();
                if (HasLeadingBitUnset(radix-1))
                {
                    Assert.Equal(radix - 1, bytes.Single());
                }
                else
                {
                    // Note: BigInteger internals: leading zero prevents treat value as negative
                    Assert.Equal(new byte[] { (byte)(radix - 1), 0 }, bytes);
                }
            }
        }

        [Fact]
        public void Number_in_its_base_gives_two_first_digits()
        {
            foreach (var radix in Radixes.Where(r => r <= byte.MaxValue))
            {
                var bytes = FromBaseN(new[] { 0, 1 }, radix).ToArray();

                if (HasLeadingBitUnset(radix))
                {
                    Assert.Equal(new byte[] { (byte)radix }, bytes);
                }
                else
                {
                    // Note: BigInteger internals: leading zero prevents treat value as negative
                    Assert.Equal(new byte[] { (byte)radix, 0 }, bytes);
                }
            }
        }

        [Theory]
        [InlineData(1, "1")]
        [InlineData(2, "01")]
        [InlineData(3, "11")]
        [InlineData(5, "101")]
        [InlineData(16, "00001")]
        [InlineData(53, "101011")]
        public void Binary_encoding_tests(byte number, string base2str)
        {
            var digits = base2str.Select(i => i == '1' ? 1 : 0).ToArray();
            Assert.Equal(number, FromBaseN(digits, 2).Single());
        }

        [Fact]
        public void Throws_if_null_argument_passed()
        {
            Assert.Throws<ArgumentNullException>(() => FromBaseN(null, 2));
        }

        [Fact]
        public void Throws_if_radix_lesser_than_2()
        {
            Assert.Equal(
                string.Concat("Base has to be at least 2", Environment.NewLine, "Parameter name: radix"),
                Assert.Throws<ArgumentOutOfRangeException>(() => FromBaseN(new int[0], 1)).Message);
        }

        [Fact]
        public void Throws_if_digit_greater_than_radix()
        {
            Assert.Equal(
                string.Concat("Digit cannot be grater than radix", Environment.NewLine, "Parameter name: digits"),
                Assert.Throws<ArgumentOutOfRangeException>(() => FromBaseN(new[] {3}, 2)).Message);
        }

        private static bool HasLeadingBitUnset(int radix) => (radix & 0x80) == 0;
    }
}
