using System.Xml.Serialization;

namespace LABS_3_4
{
    [XmlRoot("text")]
    public class Text
    {
        [XmlElement("sentence")]
        public List<Sentence> Sentences { get; set; } = [];

        public void AddSentence(Sentence sentence) => Sentences.Add(sentence);

        public IEnumerable<Sentence> SortByWordCount() =>
            Sentences.OrderBy(sentence => sentence.WordCount());

        public IEnumerable<Sentence> SortByLength() =>
            Sentences.OrderBy(sentence => sentence.Length);

        public IEnumerable<string> FindWordsInQuestionsByLength(int length)
        {
            return Sentences
                .Where(sentence => sentence.IsQuestion())
                .SelectMany(sentence => sentence.GetWords())
                .Where(word => word.Length == length)
                .Select(word => word.Value.ToLower())
                .Distinct();
        }

        public void DeleteWords(int length)
        {
            for (int i = 0; i < Sentences.Count; i++)
            {
                var sentence = Sentences[i];
                var filteredTokens = new List<Token>();

                for (int j = 0; j < sentence.Tokens.Count; j++)
                {
                    var token = sentence.Tokens[j];

                    if (!(token is Word word && word.Length == length && word.CheckForSoglasny()))
                        filteredTokens.Add(token);
                }

                sentence.Tokens = filteredTokens;
            }
        }

        public void ReplaceWordsInSentence(int sentenceIndex, int wordLength, string replacement)
        {
            if (sentenceIndex < 0 || sentenceIndex >= Sentences.Count)
            {
                Console.WriteLine("Предложение не найдено");
                return;
            }

            var sentence = Sentences[sentenceIndex];
            for (int i = 0; i < sentence.Tokens.Count; i++)
            {
                if (sentence.Tokens[i] is Word word && word.Length == wordLength)
                    sentence.Tokens[i] = new Word(replacement);
            }
        }

        public void RemoveStopWords(HashSet<string> stopWords)
        {
            if (stopWords == null || stopWords.Count == 0)
            {
                Console.WriteLine("Стоп-слова не заданы");
                return;
            }

            for (int i = 0; i < Sentences.Count; i++)
            {
                var sentence = Sentences[i];
                var filteredTokens = new List<Token>();

                for (int j = 0; j < sentence.Tokens.Count; j++)
                {
                    var token = sentence.Tokens[j];

                    if (!(token is Word word && stopWords.Contains(word.Value.ToLower().Trim())))
                        filteredTokens.Add(token);
                }

                sentence.Tokens = filteredTokens;
            }
        }

        public void SaveToXmlFile(string filePath)
        {
            using var writer = new StreamWriter(filePath);
            new XmlSerializer(typeof(Text)).Serialize(writer, this);
        }

        public List<string> FindWordsByRegex(string pattern)
        {
            var regex = new System.Text.RegularExpressions.Regex(pattern);
            return Sentences
                .SelectMany(s => s.GetWords())
                .Select(w => w.Value)
                .Where(v => regex.IsMatch(v))
                .Distinct()
                .ToList();
        }

        public Dictionary<string, (int count, SortedSet<int> lines)> BuildRepetition()
        {
            var repetition = new Dictionary<string, (int count, SortedSet<int> lines)>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < Sentences.Count; i++)
            {
                var sentence = Sentences[i];
                int lineNumber = i + 1;
                var words = sentence.GetWords().ToList();

                for (int j = 0; j < words.Count; j++)
                {
                    var word = words[j];
                    string lower = (word.Value ?? "").ToLower();

                    if (!repetition.ContainsKey(lower))
                        repetition[lower] = (0, new SortedSet<int>());

                    var entry = repetition[lower];
                    entry.count++;
                    entry.lines.Add(lineNumber);
                    repetition[lower] = entry;
                }
            }
            return repetition;
        }
        
        public override string ToString() => 
            string.Join(" ", Sentences.Select(sentence => sentence.ToString()));
    }
}