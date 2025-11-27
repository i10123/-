using System.Text;
using System.Xml.Serialization;

namespace LABS_3_4
{
    public class Sentence
    {
        [XmlElement("word", typeof(Word))]
        [XmlElement("punctuation", typeof(Punctuation))]
        public List<Token> Tokens { get; set; } = [];

        public void AddToken(Token token) => Tokens.Add(token);

        // Позволяет возвращать список слов, не указывая конкретный тип коллекции (List, Array и т.д.)
        public IEnumerable<Word> GetWords() => Tokens.Where(t => t.Type == TokenType.Word).Cast<Word>();

        public int WordCount() => GetWords().Count();

        public int Length => Tokens.Sum(token => token.Value?.Length ?? 0);

        public bool IsQuestion() => Tokens.Any(token => token.Value == "?");

        public override string ToString()
        {
            var str_builder = new StringBuilder();

            for (int i = 0; i < Tokens.Count; i++)
            {
                var token = Tokens[i];
                string value = token.Value ?? "";

                bool isDash = value == "-" || value == "—";
                bool isColon = value == ":";

                if (token is Punctuation)
                {
                    if (isDash && i > 0)
                        str_builder.Append(' ');

                    str_builder.Append(value);

                    if (isDash && i < Tokens.Count - 1 && Tokens[i + 1] is Word)
                        str_builder.Append(' ');

                    if (isColon)
                        str_builder.Append(' ');
                }
                else
                {
                    if (i > 0 && Tokens[i - 1] is not Punctuation)
                        str_builder.Append(' ');

                    str_builder.Append(value);
                }
            }

            return str_builder.ToString();
        }
    }
}