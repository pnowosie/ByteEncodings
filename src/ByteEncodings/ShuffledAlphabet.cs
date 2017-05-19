namespace ByteEncodings
{
    using System;
    using System.Text;

    public class ShuffledAlphabet : Alphabet
    {
        public ShuffledAlphabet(string digits)
            : base(Shuffle(digits))
        { }

        public ShuffledAlphabet(IAlphabet alphabet)
            : this(alphabet.Digits)
        { }

        // Fisher–Yates shuffle, more: https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
        private static string Shuffle(string digits)
        {
            var r = new Random();
            var tab = Encoding.ASCII.GetBytes(digits);

            for (int i = tab.Length - 1; i >= 1; i--)
            {
                int j = r.Next(0, i+1);
                var tmp = tab[i]; tab[i] = tab[j]; tab[j] = tmp;
            }

            return Encoding.ASCII.GetString(tab);
        }
    }
}