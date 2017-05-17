namespace ByteEncodings.Test
{
    using System;
    using System.Linq;
    using Xunit;
    using System.Diagnostics;
    using Moq;

    public class ByteEncodingTests
    {
        private const int T = 100;
        private static readonly ByteEncoding[] Encoders = new ByteEncoding[]
        {
            new ByteEncoding(Alphabet.Base2Alphabet),
            ByteEncoding.Dec, 
            ByteEncoding.Hex, 
            ByteEncoding.Base32, 
            new ByteEncoding(Alphabet.BaseZ32Alphabet), 
            ByteEncoding.Base58, 
            new ByteEncoding(Alphabet.Base62Alphabet),
            ByteEncoding.Base64, 
            ByteEncoding.Base64Safe,
            ByteEncoding.AsciiSafe,
            new ByteEncoding(Alphabet.Base85Alphabet),
            ByteEncoding.Ascii,
            new ByteEncoding(Alphabet.Base58BitcoinAlphabet),
            new ByteEncoding(Alphabet.Base58FlicrAlphabet),
            new ByteEncoding(Alphabet.Base58RippleAlphabet),
        };

        [Fact]
        public void Int_convertion_is_reversable()
        {
            var r = new Random();

            for (var i = 0; i < T; i++)
            {
                var expected = r.Next();
                int encPos = 0;
                foreach (var encoder in Encoders)
                {
                    var encoded = encoder.GetString(expected);
                    var decoded = encoder.GetInt(encoded);

                    if (expected != decoded)
                        Debug.WriteLine(
                            $"Encoder[{encPos}] converting value {expected} gives {decoded} (encoded: {encoded})");

                    Assert.Equal(expected, decoded);
                    encPos++;
                }
            }
        }

        [Fact]
        public void Long_convertion_is_reversable()
        {
            var r = new Random();
            for (var i = 0; i < T; i++)
            {
                long expected = (r.Next() << 32) + r.Next();
                int encPos = 0;
                foreach (var encoder in Encoders)
                {
                    var encoded = encoder.GetString(expected);
                    var decoded = encoder.GetLong(encoded);

                    if (expected != decoded)
                        Debug.WriteLine(
                            $"Encoder[{encPos}] converting value {expected} gives {decoded} (encoded: {encoded})");

                    Assert.Equal(expected, decoded);
                    encPos++;
                }
            }
        }

        [Fact]
        public void Guid_convertion_is_reversable()
        {
            for (var i = 0; i < T; i++)
            {
                var expected = Guid.NewGuid();
                int encPos = 0;
                foreach (var encoder in Encoders)
                {
                    var encoded = encoder.GetString(expected);
                    var decoded = encoder.GetGuid(encoded);

                    if (expected != decoded)
                        Debug.WriteLine(
                            $"Encoder[{encPos}] converting value {expected} gives {decoded} (encoded: {encoded})");

                    Assert.Equal(expected, decoded);
                    encPos++;
                }
            }
        }

        [Fact]
        public void This_strange_integer_representation()
        {
            var longOne  = new string('1', 31);
            var longZero = new string('0', 31);

            var binary = new ByteEncoding(Alphabet.Base2Alphabet);

            Assert.Equal(           0, binary.GetInt(longZero + "0"));
            Assert.Equal(int.MaxValue, binary.GetInt(longOne  + "0"));
            Assert.Equal(          -1, binary.GetInt(longOne  + "1"));
            Assert.Equal(int.MinValue, binary.GetInt(longZero + "1"));
            Assert.Equal(           1, binary.GetInt("1" + longZero));
        }

        private readonly byte[] _testBytes = { 0xde, 0xad, 0xbe, 0xef, 0xde, 0xfe, 0xc8 };
        private readonly Mock<IAlphabet> _alphabetMock = new Mock<IAlphabet>();
        private readonly ByteEncoding _encoder;

        public ByteEncodingTests()
        {
            _alphabetMock
                .Setup(a => a.GetBytes(It.IsAny<string>()))
                .Returns(_testBytes);
            _encoder = new ByteEncoding(_alphabetMock.Object);
        }

        [Fact]
        public void ToBytes_equal_size_arrays_are_copies()
        {
            var result = new byte[_testBytes.Length];
            _encoder.ToBytes("any-string", result, true);

            Assert.Equal(_testBytes, result);
        }

        [Fact]
        public void ToBytes_longer_array_is_sufixed_with_zeros()
        {
            var expected = _testBytes.Concat(new byte[] { 0, 0, 0 }).ToArray();

            var result = new byte[expected.Length];
            _encoder.ToBytes("any-string", result, true);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void ToBytesWithErrors_throws_if_shorter_array_provided()
        {
            var result = new byte[_testBytes.Length-1];

            Assert.Equal(
                string.Concat("Provided array is too short to contain result", Environment.NewLine, "Parameter name: bytes"),
                Assert.Throws<ArgumentException>(() => _encoder.ToBytes("any-string", result, true)).Message);
        }

        [Fact]
        public void ToBytesNoErrors_shorter_array_contains_first_bytes_of_result()
        {
            var expected = _testBytes.Take(3).ToArray();

            var result = new byte[expected.Length];
            _encoder.ToBytes("any-string", result, false);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void ToBytes_result_array_is_zeroed_if_empty_decoded_result()
        {
            var alphabetMock = new Mock<IAlphabet>();
            alphabetMock
                .Setup(a => a.GetBytes("this_decodes_to_empty_array"))
                .Returns(new byte[0]);

            var expected = Enumerable.Repeat((byte)0, 5).ToArray();
            var result = new byte[] { 0xde, 0xad, 0xbe, 0xef, 0x0f };

            new ByteEncoding(alphabetMock.Object)
                .ToBytes("this_decodes_to_empty_array", result);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Simple_single_value_test()
        {
            Assert.Equal("1", ByteEncoding.Dec.GetString(1));
            Assert.Equal(1, ByteEncoding.Dec.GetInt("1"));
        }
    }
}