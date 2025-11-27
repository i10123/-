using System.Xml.Serialization;

namespace LABS_3_4
{
    public enum TokenType
    {
        Word,
        Punctuation
    }

    public abstract class Token
    {
        [XmlText]
        public string Value { get; set; } = string.Empty;

        public abstract TokenType Type { get; }

        protected Token() { }
        protected Token(string value) => Value = value;

        public override string? ToString() => Value;
    }
}