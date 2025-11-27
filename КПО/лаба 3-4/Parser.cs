using System.Text;
using System.Text.RegularExpressions;

namespace LABS_3_4
{
    public class Parser
    {
        // разбивает после [.], [!], [?], но не после [...] и внутри кавычек
        private static readonly Regex SentenceRegex = new(@"(?<=[\.!\?])(?<!\.\.\.)(?=\s+(?![^\«»]*»))", RegexOptions.Compiled);
        // слова с дефисами и апострофами — единый токен; знаки препинания и многоточие — отдельный токен
        private static readonly Regex TokenRegex = new(@"\.{3}|[\p{L}]+(?:['’-][\p{L}]+)*|[^\p{L}\s'’-]", RegexOptions.Compiled);
        // начинается с букв или цифр; допускаются дефисы, апострофы, тире внутри слова
        private static readonly Regex WordRegex = new(@"^[\p{L}\p{Nd}]+(?:['’-][\p{L}\p{Nd}]+)*$", RegexOptions.Compiled);

        public static Text Parse(string inputText)
        {
            Text parsedText = new();
            bool vnutiKavichek = false;

            try
            {
                string[] sentences = SentenceRegex.Split(inputText);
                List<string> Sentences_s_kovichkami = [];

                StringBuilder current = new();
                for (int sentenceIndex = 0; sentenceIndex < sentences.Length; sentenceIndex++)
                {
                    string fragment = sentences[sentenceIndex];
                    if (string.IsNullOrWhiteSpace(fragment))
                        continue;

                    current.Append(fragment.Trim());

                    if (fragment.Contains('«'))
                        vnutiKavichek = true;

                    if (fragment.Contains('»'))
                        vnutiKavichek = false;

                    if (vnutiKavichek)
                        current.Append(' ');
                    else
                    {
                        Sentences_s_kovichkami.Add(current.ToString());
                        current.Clear();
                    }
                }

                for (int index = 0; index < Sentences_s_kovichkami.Count; index++)
                {
                    string sentenceFragment = Sentences_s_kovichkami[index];
                    Sentence sentenceObject = new();
                    MatchCollection tokens = TokenRegex.Matches(sentenceFragment);

                    for (int j = 0; j < tokens.Count; j++)
                    {
                        string tokenValue = tokens[j].Value;
                        if (WordRegex.IsMatch(tokenValue))
                            sentenceObject.AddToken(new Word(tokenValue));
                        else
                            sentenceObject.AddToken(new Punctuation(tokenValue));
                    }

                    parsedText.AddSentence(sentenceObject);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при парсинге текста: {ex.Message}");
            }

            return parsedText;
        }
    }
}