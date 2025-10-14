namespace LABA3_4
{
    public class Punctuation : IToken
    {
        public string Value { get; }

        public Punctuation(string value)
        {
            Value = value;
        }

        public override string ToString() => Value;
    }
}
