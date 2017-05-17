namespace ByteEncodings
{
    public static class KnownAlphabets
    {
        public const string FullASCII95 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-_:+.=^!/*?~$(),;@&<>[]{}%#|`\\ \"'";

        public const string BASE32_RFC4648 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        public const string Z_BASE32 = "ybndrfg8ejkmcpqxot1uwisza345h769";

        public const string BASE58 = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

        public const string BASE58_FLICR = "123456789abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ";

        public const string BASE58_RIPPLE = "rpshnaf39wBUDNEGHJKLM4PQRST7VWXYZ2bcdeCg65jkm8oFqi1tuvAxyz";

        public const string BASE64 = BASE64_RFC4648_62 + "+/";

        public const string BASE64_SAFE = BASE64_RFC4648_62 + "-_";

        public const string DigitsAndLetters = "0123456789abcdefghijklmnopqrstuvwxyz";

        private const string BASE64_RFC4648_62 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    }
}