using System.Collections.Generic;
using System.IO;

public static class StopWords
{
    public static HashSet<string> Load(string path)
    {
        return File.Exists(path)
            ? new HashSet<string>(File.ReadAllLines(path))
            : new HashSet<string>();
    }

    public static void RemoveStopWords(Text text, HashSet<string> stopWords)
    {
        foreach (var sentence in text.Sentences)
        {
            sentence.Tokens.RemoveAll(t =>
                t is Word w && stopWords.Contains(w.Value.ToLower()));
        }
    }
}