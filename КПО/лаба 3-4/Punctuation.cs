namespace LABS_3_4
{
    public class Punctuation : Token
    {
        public override TokenType Type => TokenType.Punctuation;
        public Punctuation() { }
        public Punctuation(string value) : base(value) { }
    }
}