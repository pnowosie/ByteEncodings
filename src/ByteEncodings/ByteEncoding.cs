namespace ByteEncodings
{
    using System;
    using System.Linq;
    using JetBrains.Annotations;

    public class ByteEncoding
    {
        private readonly IAlphabet _alphabet;

        public ByteEncoding([NotNull]IAlphabet alphabet)
        {
            _alphabet = alphabet ?? throw new ArgumentNullException(nameof(alphabet));
        }

        public string GetString(int n)
        {
            return _alphabet.GetString(BitConverter.GetBytes(n));
        }

        public string GetString(long n)
        {
            return _alphabet.GetString(BitConverter.GetBytes(n));
        }

        public string GetString(Guid id)
        {
            return _alphabet.GetString(id.ToByteArray());
        }

        public string GetString([NotNull]byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            return _alphabet.GetString(bytes);
        }

        public int GetInt([NotNull]string s)
        {
            var intBytes = new byte[4];
            ToBytes(s, intBytes);
            return BitConverter.ToInt32(intBytes, 0);
        }

        public long GetLong([NotNull]string s)
        {
            var longBytes = new byte[8];
            ToBytes(s, longBytes);
            return BitConverter.ToInt64(longBytes, 0);
        }

        public Guid GetGuid([NotNull]string s)
        {
            var guidBytes = new byte[16];
            ToBytes(s, guidBytes);
            return new Guid(guidBytes);
        }

        public void ToBytes([NotNull]string s, [NotNull]byte[] bytes, bool throwIfArrayTooShort = false)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            var decoded = _alphabet.GetBytes(s).ToArray();
            if (throwIfArrayTooShort && decoded.Length > bytes.Length)
                throw new ArgumentException("Provided array is too short to contain result", nameof(bytes));

            using (var enumerator =  decoded.Concat(
                                            Enumerable.Repeat((byte)0, bytes.Length))
                                        .GetEnumerator())
            {
                for (var i = 0; i < bytes.Length; i++)
                {
                    enumerator.MoveNext();
                    bytes[i] = enumerator.Current;
                }
            }
        }

        // Known alphabet encodings - just a few most usefull
        public static ByteEncoding Dec { get; } = new ByteEncoding(Alphabet.Base10Alphabet);
        public static ByteEncoding Hex { get; } = new ByteEncoding(Alphabet.Base16Alphabet);
        public static ByteEncoding Base32 { get; } = new ByteEncoding(Alphabet.Base32Alphabet);
        public static ByteEncoding Base58 { get; } = new ByteEncoding(Alphabet.Base58Alphabet);
        public static ByteEncoding Base64 { get; } = new ByteEncoding(Alphabet.Base64Alphabet);
        public static ByteEncoding Base64Safe { get; } = new ByteEncoding(Alphabet.Base64SafeAlphabet);
        public static ByteEncoding AsciiSafe { get; } = new ByteEncoding(Alphabet.Base73SafeAlphabet);
        public static ByteEncoding Ascii { get; } = new ByteEncoding(Alphabet.Base95Alphabet);

    }
}
