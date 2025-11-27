namespace LABS_3_4
{
    public static class TextStatistics
    {
        public static void ShowMetrics(Text text)
        {
            int totalSentences = text.Sentences.Count;
            if (totalSentences == 0) 
                return;

            var allWords = text.Sentences.SelectMany(s => s.GetWords()).ToList();
            int totalWords = allWords.Count;
            int uniqueWords = allWords.Select(w => w.Value.ToLower()).Distinct().Count();

            int totalSyllables = allWords.Sum(w => CountSyllables(w.Value));

            double wordsPerSentence = (double)totalWords / totalSentences;
            double syllablesPerWord = totalWords > 0 ? (double)totalSyllables / totalWords : 0;

            double fleschIndex = 206.835 - (1.015 * wordsPerSentence) - (84.6 * syllablesPerWord);

            Console.WriteLine("\n--- СТАТИСТИКА ТЕКСТА ---");
            Console.WriteLine($"Всего предложений: {totalSentences}");
            Console.WriteLine($"Всего слов:        {totalWords}");
            Console.WriteLine($"Уникальных слов:   {uniqueWords} (Разнообразие: {((double)uniqueWords / totalWords):P1})");
            Console.WriteLine($"Средняя длина предложения: {wordsPerSentence:F1} слов");
            Console.WriteLine($"Индекс читаемости (Flesch): {fleschIndex:F1} (чем выше, тем легче)");

            if (fleschIndex > 90) 
                Console.WriteLine("Вердикт: Очень легкий текст (сказки).");

            else if (fleschIndex > 60) 
                Console.WriteLine("Вердикт: Стандартный текст.");

            else if (fleschIndex > 30) 
                Console.WriteLine("Вердикт: Сложный текст (научная статья).");

            else 
                Console.WriteLine("Вердикт: Очень трудный текст (юридический документ).");
            Console.WriteLine("-------------------------\n");
        }

        private static int CountSyllables(string word)
        {
            string vowels = "aeiouyаеёиоуыэюя";
            word = word.ToLower();
            int count = 0;
            bool lastWasVowel = false;

            foreach (char c in word)
            {
                bool isVowel = vowels.Contains(c);
                if (isVowel && !lastWasVowel)
                    count++;
                lastWasVowel = isVowel;
            }

            return count > 0 ? count : 1;
        }
    }
}