namespace LABA3_4
{
    using System.Collections.Generic;
    using System.Linq;

    public class Sentence
    {
        public List<IToken> Tokens { get; } = new();

        public void AddToken(IToken token) => Tokens.Add(token);

        public int WordCount => Tokens.OfType<Word>().Count();

        public int LengthWithoutSpaces => Tokens.OfType<Word>().Sum(w => w.Value.Length) +
                                          Tokens.OfType<Punctuation>().Sum(p => p.Value.Length);

        public bool IsQuestion => Tokens.LastOrDefault()?.Value == "?";

        public override string ToString() => string.Join(" ", Tokens.Select(t => t.Value));
    }
}
