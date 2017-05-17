namespace ByteEncodings.Test
{
    using System;
    using Xunit;
    using System.Diagnostics;
    using System.Linq;
    using System.Numerics;
    using System.Reflection;

    public class AlphabetTests
    {
        [Fact]
        public void Throws_if_null_argument_passed_to_ctor()
        {
            Assert.Throws<ArgumentNullException>(() => new Alphabet(null));
        }

        [Fact]
        public void Throws_if_alphabet_too_short_than_base()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Alphabet("01", 3));
        }

        [Fact]
        public void Throws_if_alphabet_too_short()
        {
            Assert.Equal(
                string.Concat("Digits have to be as many as provided radix, at least 2", Environment.NewLine, "Parameter name: digits"),
                Assert.Throws<ArgumentException>(() => new Alphabet("0")).Message);
        }

        [Fact]
        public void Throws_if_alphabet_too_short_wrong_radix()
        {
            Assert.Equal(
                string.Concat("Digits have to be as many as provided radix, at least 2", Environment.NewLine, "Parameter name: digits"),
                Assert.Throws<ArgumentException>(() => new Alphabet("01", 1)).Message);
        }

        [Fact]
        public void Throws_if_alphabet_longer_than_currently_supported()
        {
            Assert.Equal(
                string.Concat("Digits can contain at most 256 characters", Environment.NewLine, "Parameter name: digits"),
                Assert.Throws<ArgumentException>(() => new Alphabet(new string('0', 257))).Message);
        }

        [Fact]
        public void Each_digit_can_occur_once()
        {
            Assert.Equal(
                string.Concat("Each digit can occur only once", Environment.NewLine, "Parameter name: digits"),
                Assert.Throws<ArgumentException>(() => new Alphabet("011", 3)).Message);
        }

        [Fact]
        public void Throws_if_null_argument_passed_to_GetString()
        {
            Assert.Equal(
                string.Concat("Value cannot be null.", Environment.NewLine, "Parameter name: bytes"),
                Assert.Throws<ArgumentNullException>(() => Alphabet.Base2Alphabet.GetString(null)).Message);
        }

        [Fact]
        public void Throws_if_null_argument_passed_to_GetBytes()
        {
            Assert.Equal(
                string.Concat("Value cannot be null.", Environment.NewLine, "Parameter name: encoding"),
                Assert.Throws<ArgumentNullException>(() => Alphabet.Base2Alphabet.GetBytes(null)).Message);
        }

        [Theory]
        [InlineData("Base2Alphabet", "01")]
        [InlineData("Base10Alphabet", "0123456789")]
        [InlineData("Base16Alphabet", "0123456789ABCDEF")]
        [InlineData("Base32Alphabet", "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567")]
        [InlineData("BaseZ32Alphabet", "ybndrfg8ejkmcpqxot1uwisza345h769")]
        [InlineData("Base58Alphabet", "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz")]
        [InlineData("Base62Alphabet", "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz")]
        [InlineData("Base64Alphabet", "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/")]
        [InlineData("Base64SafeAlphabet", "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_")]
        [InlineData("Base73SafeAlphabet", "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-_:+.=^!/*?")]
        [InlineData("Base85Alphabet", "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-_:+.=^!/*?~$(),;@&<>[]")]
        [InlineData("Base95Alphabet", "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-_:+.=^!/*?~$(),;@&<>[]{}%#|`\\ \"'")]
        [InlineData("Base58FlicrAlphabet", "123456789abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ")]
        [InlineData("Base58RippleAlphabet", "rpshnaf39wBUDNEGHJKLM4PQRST7VWXYZ2bcdeCg65jkm8oFqi1tuvAxyz")]
        public void Known_alphabet_properties_test(string alphabetPropertyName, string expectedDigitString)
        {
            var alphabetProperty = typeof(Alphabet).GetProperty(alphabetPropertyName, BindingFlags.Public | BindingFlags.Static);
            var alphabet = alphabetProperty.GetValue(null) as IAlphabet;

            Assert.NotNull(alphabet);
            Assert.Equal(expectedDigitString, alphabet.Digits);
            Assert.Equal(expectedDigitString.Length, alphabet.Radix);
        }
        
        [Fact]
        public void Bitcoin_Digits_Test()
        {
            var bitcoinAlphabet = Alphabet.Base58BitcoinAlphabet;
            Assert.Equal(58, bitcoinAlphabet.Radix);
            Assert.Equal(
                "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz",
                bitcoinAlphabet.Digits);
        }

        [Fact]
        public void Encoding_is_reversable()
        {
            var alphabets = new[]
            {
                Alphabet.Base2Alphabet, Alphabet.Base10Alphabet, Alphabet.Base16Alphabet, Alphabet.Base32Alphabet, 
                Alphabet.BaseZ32Alphabet, Alphabet.Base58Alphabet, Alphabet.Base58BitcoinAlphabet, Alphabet.Base58FlicrAlphabet, 
                Alphabet.Base58RippleAlphabet, Alphabet.Base62Alphabet, Alphabet.Base64Alphabet, Alphabet.Base64SafeAlphabet, 
                Alphabet.Base73SafeAlphabet, Alphabet.Base85Alphabet, Alphabet.Base95Alphabet, 
            };

            const int N = 100, T = 100;
            var r = new Random();
            for (int t = 0; t < T; t++)
            {
                var expected = new byte[N];
                r.NextBytes(expected);
                NonzeroUnsetLeadingBit(expected);

                int alpInd = 0;
                foreach (var alphabet in alphabets)
                {
                    var str = alphabet.GetString(expected);
                    var decoded = alphabet.GetBytes(str).ToArray();

                    Debug.WriteLine($"Iteration: {t:D2}, Alphabet [{alpInd}]");
                    if (expected.SequenceEqual(decoded) == false)
                    {
                        Debug.WriteLine($"Houston we have a problem, decoding alphabet [{alpInd}]");
                        Debug.WriteLine($"Expected [{N}]: [ {string.Join(", ", expected.Take(5))} ... {string.Join(", ", expected.Skip(N-5))}");
                        int D = decoded.Length;
                        Debug.WriteLine($"Decoded  [{D}]: [ {string.Join(", ", decoded.Take(5)) } ... {string.Join(", ", decoded.Skip(D-5))}");
                        Debug.WriteLine("= encoded string =\n"+ str);
                    }

                    Assert.Equal(expected, decoded);
                    ++alpInd;
                }
            }
        }

        [Fact]
        public void Convertion_of_big_number()
        {
            var big = BigInteger.Pow(2, 100);

            var base2 = Alphabet.Base2Alphabet.GetString(big);

            // Note that number is reversed - digits go from least to most significant 
            Assert.All(base2.Take(100), d => Assert.Equal('0', d));
            Assert.Equal('1', base2.Last());
        }

        [Fact]
        public void ShuffledAlphabet_gets_diffent_result_than_alphabet_its_based_on()
        {
            var bytes = BitConverter.GetBytes(0xdeadbeef);

            Assert.NotEqual(
                Alphabet.Base16Alphabet.GetString(bytes),
                new ShuffledAlphabet(Alphabet.Base16Alphabet).GetString(bytes));
        }

        private static void NonzeroUnsetLeadingBit(byte[] expected)
        {
            // Assuming expected array is little-endian number (last byte is most significant)
            // There are 2 cases of expected array which afects the length of decoded array
            //  1. Last byte is 0 => decoded is shorter
            //  2. Last byte has MSB set => decoded is of leading zero byte longer (BigInteger representation)
            // To make this test simple I just avoid these cases
            expected[expected.Length - 1] &= 0x7f;            
            if (expected.Last() == 0)
                expected[expected.Length - 1] = 1;
        }
    }
}