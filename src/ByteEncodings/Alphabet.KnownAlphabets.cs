namespace ByteEncodings
{
    public partial class Alphabet
    {
        /// <summary>
        /// The binary (base 2) alphabet
        /// </summary>
        public static Alphabet Base2Alphabet { get; } = new Alphabet("01");

        /// <summary>
        /// The decimal (base 10) alphabet
        /// </summary>
        public static Alphabet Base10Alphabet { get; } = new Alphabet(KnownAlphabets.FullASCII95, 10);

        /// <summary>
        /// The hexgonal (base 16) alphabet
        /// </summary>
        public static Alphabet Base16Alphabet { get; } = new Alphabet(KnownAlphabets.FullASCII95, 16);

        /// <summary>
        /// The base 32 alphabet as described in <see href="https://tools.ietf.org/html/rfc4648"/>
        /// </summary>
        public static Alphabet Base32Alphabet { get; } = new Alphabet(KnownAlphabets.BASE32_RFC4648);

        /// <summary>
        /// The human-oriented base-32 encoding <see href="http://philzimmermann.com/docs/human-oriented-base-32-encoding.txt"/>
        /// </summary>
        public static Alphabet BaseZ32Alphabet { get; } = new Alphabet(KnownAlphabets.Z_BASE32);

        /// <summary>
        /// The base 58 alphabet as described in <see href="https://en.wikipedia.org/wiki/Base58"/>
        /// </summary>
        public static Alphabet Base58Alphabet { get; } = new Alphabet(KnownAlphabets.BASE58);

        /// <summary>
        /// The full-ascii alphabet shortened to digits & letters - 62 characters
        /// </summary>
        public static Alphabet Base62Alphabet { get; } = new Alphabet(KnownAlphabets.FullASCII95, 62);

        /// <summary>
        /// Base 64 alphabet as described in <see href="https://tools.ietf.org/html/rfc4648"/>
        /// </summary>
        public static Alphabet Base64Alphabet { get; } = new Alphabet(KnownAlphabets.BASE64);

        /// <summary>
        /// Base 64 alphabet as described in <see href="https://tools.ietf.org/html/rfc4648"/> with url not-safe chars replaced
        /// </summary>
        public static Alphabet Base64SafeAlphabet { get; } = new Alphabet(KnownAlphabets.BASE64_SAFE);

        /// <summary>
        /// The full-ascii alphabet shortened to 73 url-safe characters
        /// </summary>
        public static Alphabet Base73SafeAlphabet { get; } = new Alphabet(KnownAlphabets.FullASCII95, 73);

        /// <summary>
        /// The full-ascii alphabet shortened to 85 characters
        /// </summary>
        public static Alphabet Base85Alphabet { get; } = new Alphabet(KnownAlphabets.FullASCII95, 85);

        /// <summary>
        /// The full-ascii base 95 alphabet
        /// </summary>
        public static Alphabet Base95Alphabet { get; } = new Alphabet(KnownAlphabets.FullASCII95);

        /// <summary>
        /// The base 58 standard alphabet used in Bitcoin <see href="https://en.wikipedia.org/wiki/Base58"/>
        /// </summary>
        public static Alphabet Base58BitcoinAlphabet { get; } = Base58Alphabet;

        /// <summary>
        /// The base 58 modified alphabet used in Flicr <see href="https://en.wikipedia.org/wiki/Base58"/>
        /// </summary>
        public static Alphabet Base58FlicrAlphabet { get; } = new Alphabet(KnownAlphabets.BASE58_FLICR);

        /// <summary>
        /// The base 58 modified alphabet used in Ripple <see href="https://en.wikipedia.org/wiki/Base58"/>
        /// </summary>
        public static Alphabet Base58RippleAlphabet { get; } = new Alphabet(KnownAlphabets.BASE58_RIPPLE);
    }
}
