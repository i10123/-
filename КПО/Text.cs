namespace LABA3_4
{
    using System.Collections.Generic;
    using System.Linq;

    public class Text
    {
        public List<Sentence> Sentences { get; } = new();

        public void AddSentence(Sentence sentence) => Sentences.Add(sentence);

        public List<Sentence> SortByWordCount() =>
            Sentences.OrderBy(s => s.WordCount).ToList();

        public List<Sentence> SortByLength() =>
            Sentences.OrderBy(s => s.LengthWithoutSpaces).ToList();

        public List<string> FindWordsInQuestions(int length) =>
            Sentences.Where(s => s.IsQuestion)
                     .SelectMany(s => s.Tokens.OfType<Word>())
                     .Where(w => w.Value.Length == length)
                     .Select(w => w.Value.ToLower())
                     .Distinct()
                     .ToList();

        public void RemoveWordsStartingWithConsonant(int length)
        {
            foreach (var sentence in Sentences)
            {
                sentence.Tokens.RemoveAll(t =>
                    t is Word w &&
                    w.Value.Length == length &&
                    IsConsonant(w.Value[0]));
            }
        }

        private bool IsConsonant(char c)
        {
            string consonants = "бвгджзйклмнпрстфхцчшщbcdfghjklmnpqrstvwxyz";
            return consonants.Contains(char.ToLower(c));
        }

        public void ReplaceWordsInSentence(int sentenceIndex, int length, string replacement)
        {
            if (sentenceIndex < 0 || sentenceIndex >= Sentences.Count) return;

            var sentence = Sentences[sentenceIndex];
            for (int i = 0; i < sentence.Tokens.Count; i++)
            {
                if (sentence.Tokens[i] is Word w && w.Value.Length == length)
                    sentence.Tokens[i] = new Word(replacement);
            }
        }

        public Dictionary<int, int> WordLengthStats()
        {
            return Sentences.SelectMany(s => s.Tokens.OfType<Word>())
                            .GroupBy(w => w.Value.Length)
                            .ToDictionary(g => g.Key, g => g.Count());
        }
    }
}
