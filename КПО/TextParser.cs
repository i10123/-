namespace LABA3_4
{
    using System.Text.RegularExpressions;

    public class TextParser
    {
        public Text Parse(string input)
        {
            var text = new Text();
            var sentenceStrings = Regex.Split(input, @"(?<=[.!?])\s+");

            foreach (var sentenceStr in sentenceStrings)
            {
                var sentence = new Sentence();
                var tokens = Regex.Matches(sentenceStr, @"\w+|[^\w\s]");

                foreach (Match match in tokens)
                {
                    string token = match.Value;
                    if (Regex.IsMatch(token, @"^\w+$"))
                        sentence.AddToken(new Word(token));
                    else
                        sentence.AddToken(new Punctuation(token));
                }

                text.AddSentence(sentence);
            }

            return text;
        }
    }

}
