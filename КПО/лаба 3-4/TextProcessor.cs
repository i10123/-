namespace LABS_3_4
{
    public static class TextProcessor
    {
        private static HistoryManager _history = new();

        public static void Menu(Text text, string originalText, string languageCode)
        {
            _history = new HistoryManager();

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\n--- ГЛАВНОЕ МЕНЮ ({languageCode.ToUpper()}) ---");
                Console.ResetColor();

                Console.Write(
                    "1  — Сортировка предложений (по кол-ву слов)\n" +
                    "2  — Сортировка предложений (по длине символов)\n" +
                    "3  — Поиск слов в вопросах\n" +
                    "4  — Удаление слов заданной длины (на согласную)\n" +
                    "5  — Замена слов в предложении\n" +
                    "6  — Удаление стоп-слов\n" +
                    "7  — Экспорт (XML и JSON)\n" +
                    "8  — Таблица частоты слов\n" +
                    "9  — Расширенная статистика\n" +
                    "10 — Поиск слов по Regex\n" +
                    "Z  — Отменить последнее действие\n" +
                    "R  — Сбросить всё\n" +
                    "0  — Выход в меню выбора файла\n" +
                    "Ваш выбор: ");

                string? choice = Console.ReadLine()?.Trim().ToUpper();
                Console.Clear();

                if (choice == "0") 
                    break;

                if (choice == "R")
                {
                    text = Parser.Parse(originalText);
                    _history = new HistoryManager(); // Сброс истории
                    Console.WriteLine("Текст полностью восстановлен.");
                    continue;
                }

                if (choice == "Z")
                {
                    if (_history.CanUndo)
                    {
                        var previous = _history.Undo();
                        if (previous != null)
                        {
                            text = previous;
                            Console.WriteLine("Действие отменено!");
                        }
                    }
                    else
                        Console.WriteLine("История пуста, отменять нечего.");
                    continue;
                }

                Run(choice, text, languageCode);
            }
        }

        private static void Run(string? choice, Text text, string languageCode)
        {
            // Для операций, которые меняют текст, сначала сохраняем состояние
            bool isMutating = choice == "4" || choice == "5" || choice == "6";
            if (isMutating)
            {
                _history.SaveState(text);
            }

            switch (choice)
            {
                case "1":
                    var sortedCount = text.SortByWordCount().Select(s => s.ToString());
                    File.WriteAllLines("out_sort_count.txt", sortedCount);
                    Console.WriteLine("Готово. Результат в [out_sort_count.txt]");
                    break;

                case "2":
                    var sortedLen = text.SortByLength().Select(s => s.ToString());
                    File.WriteAllLines("out_sort_length.txt", sortedLen);
                    Console.WriteLine("Готово. Результат в [out_sort_length.txt]");
                    break;

                case "3":
                    Console.Write("Введите длину слов для поиска: ");
                    if (int.TryParse(Console.ReadLine(), out int qLen))
                    {
                        var words = text.FindWordsInQuestionsByLength(qLen);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Найденные слова: " + string.Join(", ", words));
                        Console.ResetColor();
                        File.WriteAllLines("out_questions_search.txt", words);
                    }
                    break;

                case "4":
                    Console.Write("Введите длину слов для удаления: ");
                    if (int.TryParse(Console.ReadLine(), out int dLen))
                    {
                        text.DeleteWords(dLen);
                        Console.WriteLine($"Слова длиной {dLen}, начинающиеся на согласную, удалены.");
                    }
                    break;

                case "5":
                    Console.WriteLine($"Всего предложений: {text.Sentences.Count}");
                    Console.Write("Номер предложения: ");
                    if (int.TryParse(Console.ReadLine(), out int strNum) && strNum > 0 && strNum <= text.Sentences.Count)
                    {
                        Console.Write("Длина слова: ");
                        int.TryParse(Console.ReadLine(), out int wLen);
                        Console.Write("На что меняем (строка): ");
                        string? replacement = Console.ReadLine();

                        if (!string.IsNullOrEmpty(replacement))
                        {
                            text.ReplaceWordsInSentence(strNum - 1, wLen, replacement);
                            Console.WriteLine("Замена выполнена.");
                        }
                    }
                    break;

                case "6":
                    string stopFile = languageCode == "ru" ? "stopwords_ru.txt" : "stopwords_en.txt";
                    var stopWords = LoadStopWords(stopFile);
                    if (stopWords.Count > 0)
                    {
                        text.RemoveStopWords(stopWords);
                        Console.WriteLine($"Удалены стоп-слова ({stopWords.Count} шт. в базе).");
                    }
                    else Console.WriteLine($"Файл {stopFile} не найден или пуст.");
                    break;

                case "7":
                    text.SaveToXmlFile("export_data.xml");
                    JsonExporter.Save(text, "export_data.json");
                    Console.WriteLine("Экспортировано в XML и JSON файлы.");
                    break;

                case "8":
                    var repet = text.BuildRepetition();
                    var lines = repet.OrderBy(e => e.Key).Select(e => $"{e.Key,-20} | {e.Value.count} раз | предложения: {string.Join(", ", e.Value.lines)}");
                    File.WriteAllLines("out_concordance.txt", lines);
                    Console.WriteLine("Таблица построена в [out_concordance.txt]");
                    break;

                case "9":
                    TextStatistics.ShowMetrics(text);
                    break;

                case "10":
                    Console.Write("Введите Regex шаблон (например, ^[A-Z].*e$ для слов с большой буквы, заканч. на e): ");
                    string? pattern = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(pattern))
                    {
                        try
                        {
                            var matches = text.FindWordsByRegex(pattern);
                            Console.WriteLine($"Найдено уникальных совпадений: {matches.Count}");
                            Console.WriteLine(string.Join(", ", matches));
                        }
                        catch (ArgumentException)
                        {
                            Console.WriteLine("Некорректный Regex.");
                        }
                    }
                    break;

                default:
                    Console.WriteLine("Неверный ввод.");
                    break;
            }
        }

        private static HashSet<string> LoadStopWords(string file)
        {
            var res = new HashSet<string>();
            if (File.Exists(file))
            {
                foreach (var line in File.ReadAllLines(file))
                {
                    if (!string.IsNullOrWhiteSpace(line)) 
                        res.Add(line.Trim().ToLower());
                }
            }
            return res;
        }
    }
}