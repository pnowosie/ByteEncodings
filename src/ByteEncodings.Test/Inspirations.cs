namespace ByteEncodings.Test
{
    using System;
    using Xunit;
    using System.Linq;

    using static System.Linq.Enumerable;

    /// <summary>
    /// Please review following tests to find more about library's edge cases and possible usages & inspirations.
    /// </summary>
    public class Inspirations
    {

        /// <summary>
        /// Let's start with practical use case. Compression of guids to take less space
        /// </summary>
        [Fact]
        public void Compressing_identifiers()
        {
            var id = Guid.Parse("9bd9c926-a2b6-4451-912e-d3b5ba690000");

            // spell it over the phone
            var humanEncoder = ByteEncoding.Base32;
            Assert.Equal("GJST5N2WCNUIUI2FTONV3UB", humanEncoder.GetString(id));
            Assert.Equal(id, humanEncoder.GetGuid("GJST5N2WCNUIUI2FTONV3UB"));

            // use in url
            var urlSafeEncoder = ByteEncoding.Base64Safe;
            Assert.Equal("mkc2baroRRUkuMdt6mG", urlSafeEncoder.GetString(id));
            Assert.Equal(id, urlSafeEncoder.GetGuid("mkc2baroRRUkuMdt6mG"));

            // ogly full-ascii option - but not so much shorter
            var oglyEncoder = ByteEncoding.Ascii;
            Assert.Equal(@"I\)a]d&x/U{lpi^^m", oglyEncoder.GetString(id));
            Assert.Equal(id, oglyEncoder.GetGuid(@"I\)a]d&x/U{lpi^^m"));

            // for better compression your guids needs to be special ;)
            Assert.Equal("", humanEncoder.GetString(Guid.Empty));
        }

        /// <summary>
        /// Have you ever thought how integer is represented on disk. In this case most significant bit
        /// (sign bit) is last, so numbers which have '1' on the right side are negative.
        /// </summary>
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

        /// <summary>
        /// Let's play with simple math and convert number between bases. I'd like to have mathematical 
        /// representation with most significant digit first and a base in '()', e.g.
        /// Number ten in decimal represenation will be printed: 10(10)
        /// Number two (dec) in base 2 looks very similar:       10(2)
        /// By the way we use alphabet '0..9a..z' so max base is 36
        /// </summary>
        public struct NumberInBase
        {
            private readonly string _number;
            private readonly int _radix;

            public NumberInBase(string number, int radix)
            {
                _number = number;
                _radix = radix;
            }

            public override string ToString()
            {
                return $"{_number}({_radix})";
            }

            public NumberInBase ConvertToBase(int newRadix)
            {
                var currentBaseEncoder = new Alphabet(
                        KnownAlphabets.DigitsAndLetters,
                        _radix);

                var newBaseEncoder = new Alphabet(
                        KnownAlphabets.DigitsAndLetters,
                        newRadix);

                // ByteEncoding expects least significant digit first, we need to reverse our number for computation
                var oldnum = string.Concat(_number.Reverse());
                var oldBytes = currentBaseEncoder.GetBytes(oldnum);

                var newnum = newBaseEncoder.GetString(oldBytes);

                return new NumberInBase(
                    String.Concat(newnum.Reverse()),    // don't forget to reverse back ;)
                    newRadix);
            }
        }

        // New shiny convertion structure is ready. Time to see it in action!
        [Fact]
        public void Number_representations()
        {
            Assert.Equal("10(10)", new NumberInBase("10", 10).ToString());
            Assert.Equal("10(2)", new NumberInBase("2", 10).ConvertToBase(2).ToString());
        }

        // That was borring, try someting more exciting, you can check the results here: https://www.tools4noobs.com/online_tools/base_convert/
        [Fact]
        public void Do_you_know_what_my_phone_number_in_base_X_is()
        {
            var phoneNo = new NumberInBase("420987654321", 10);

            Assert.Equal("110001000000100110100011101000010110001(2)", $"{phoneNo.ConvertToBase(2)}");
            Assert.Equal("42262316006403(7)", $"{phoneNo.ConvertToBase( 7)}");
            Assert.Equal("6970bab6929(12)",   $"{phoneNo.ConvertToBase(12)}");
            Assert.Equal("395g30ag3g(17)",    $"{phoneNo.ConvertToBase(17)}");
            Assert.Equal("oblnn4g7(29)",      $"{phoneNo.ConvertToBase(29)}");
            Assert.Equal("j7ehpaal(30)",      $"{phoneNo.ConvertToBase(30)}");
            Assert.Equal("9sw6v223(33)",      $"{phoneNo.ConvertToBase(33)}");
            Assert.Equal("5ded6h0x(36)",      $"{phoneNo.ConvertToBase(36)}");
        }


        /// <summary>
        /// No hacker can guess this password (if you can get true random bytes)
        /// </summary>
        [Fact]
        public void Super_strong_password_generation()
        {
            // I'd go for System.Security.Cryptography.RandomNumberGenerator.GetBytes()
            var randomBytes = new byte[] { 0xde, 0xad, 0xbe, 0xef, 0xca, 0xfe, 0xba, 0xbe, 0xde, 0xfe, 0xc8 };

            // password chars will be take from these 73 chars
            var baseAlphabet = Alphabet.Base73SafeAlphabet;

            // let's uniformly shuffle the alphabet
            var secretAlphabet = new ShuffledAlphabet(baseAlphabet);
            var password = secretAlphabet.GetString(randomBytes);

            var knownValue = Alphabet.Base73SafeAlphabet.GetString(randomBytes);
            Assert.Equal("Q2wDyszc6Ip.O?1", knownValue);

            // so for test purposes let's map password to our known value
            var map = Range(0, 73).ToDictionary(
                k => secretAlphabet.Digits[k],
                v => baseAlphabet.Digits[v]);

            var mappedPwd = string.Concat(password.Select(c => map[c]));

            // Are you still with me here?
            Assert.Equal(knownValue, mappedPwd);
        }

        [Fact]
        public void We_can_make_Ceasar_cipher_absurdally_inefficient()
        {
            // Caesar cipher: ABCD -> XYZA
            var plaintext = "THEQUICKBROWNFOXJUMPSOVERTHELAZYDOG";
            var expected = "QEBNRFZHYOLTKCLUGRJMPLSBOQEBIXWVALD";

            var plainAlphabet = new Alphabet("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            var cipherAlphabet = new Alphabet("XYZABCDEFGHIJKLMNOPQRSTUVW");

            var bytes = plainAlphabet.GetBytes(plaintext);
            var ciphertext = cipherAlphabet.GetString(bytes);
            Assert.Equal(expected, ciphertext);
            // we could check that we can decode back to plaintext, but it's obvious now - isn't it? 
        }
    }
}