namespace ByteEncodings.Test
{
    using System;
    using Xunit;
    using System.Collections.Generic;
    using System.Linq;

    public class BytesToDigitsEncodingTests
    {
        private static readonly IBaseConverter baseConverter = new BaseConverter();
        private static readonly Func<IEnumerable<byte>, int, IEnumerable<int>> ToBaseN = baseConverter.ToBaseN; 

        public readonly int[] Radixes = { 2, 3, 7, 10, 16, 32, 64, 128, 256 };

        [Fact]
        public void Zero_encoded_in_any_base_returns_empty_digits()
        {
            var zeros = new byte[3];

            foreach (var radix in Radixes)
            {
                Assert.Empty(ToBaseN(zeros, radix));
            }
        }

        [Fact]
        public void One_encoded_in_any_base_returns_colection_with_first_digit()
        {
            var one = new byte[] {0x01};

            foreach (var radix in Radixes)
            {
                var digits = ToBaseN(one, radix).ToArray();
                Assert.Single(digits);
                Assert.Equal(1, digits[0]);
            }
        }

        [Fact]
        public void Value_lesser_than_radix_returns_single_digit()
        {
            foreach (var radix in Radixes)
            {
                var digits = ToBaseN(new[]{ (byte)(radix - 1) }, radix).ToArray();
                Assert.Single(digits);
                Assert.Equal(radix-1, digits[0]);
            }
        }

        [Fact]
        public void Number_in_its_base_gives_two_first_digits()
        {
            var expected = new[] {0, 1};
            foreach (var radix in Radixes.Where(r => r <= byte.MaxValue))
            {
                Assert.Equal(expected, ToBaseN(new[]{ (byte)radix }, radix));
            }
        }

        [Theory]
        [InlineData(1, "1")]
        [InlineData(2, "01")]
        [InlineData(3, "11")]
        [InlineData(5, "101")]
        [InlineData(16, "00001")]
        [InlineData(53, "101011")]
        public void Binary_encoding_tests(byte number, string expected)
        {
            var binary = ToBaseN(new[] {number}, 2).ToArray();
            var digitsSet = new HashSet<int> { 0, 1 };
            Assert.Subset(digitsSet, new HashSet<int>(binary));
            Assert.Equal(expected, string.Join("", binary));
        }

        [Fact]
        public void LittleEndian_is_used_regarding_byte_order()
        {
            byte[] smalInt = { 1, 0, 0, 0 }, bigInt = { 0, 0, 0, 1 };
            var shorter = Alphabet.Base10Alphabet.GetString(smalInt);
            var longer = Alphabet.Base10Alphabet.GetString(bigInt);
            Assert.True(shorter.Length < longer.Length);
        }

        [Fact]
        public void Throws_if_null_argument_passed()
        {
            Assert.Throws<ArgumentNullException>(() => ToBaseN(null, 2));
        }

        [Fact]
        public void Throws_if_radix_lesser_than_2()
        {
            Assert.Equal(
                string.Concat("Base has to be at least 2", Environment.NewLine, "Parameter name: radix"),
                Assert.Throws<ArgumentOutOfRangeException>(() => ToBaseN(new byte[0], 1)).Message);
        }
    }
}
