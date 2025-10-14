namespace LABA3_4
{
    public class Word : IToken
    {
        public string Value { get; }
        public bool IsStopWord { get; set; }

        public Word(string value)
        {
            Value = value;
        }

        public override string ToString() => Value;
    }
}
