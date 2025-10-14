using LABA3_4;
using System;
using System.IO;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        var parser = new TextParser();
        Text text = null;

        while (true)
        {
            Console.WriteLine("\nМеню:");
            Console.WriteLine("1 - Загрузить русский текст");
            Console.WriteLine("2 - Загрузить английский текст");
            Console.WriteLine("3 - Сортировать предложения по количеству слов");
            Console.WriteLine("4 - Сортировать по длине предложений");
            Console.WriteLine("5 - Найти слова заданной длины в вопросах");
            Console.WriteLine("6 - Удалить слова заданной длины, начинающиеся с согласной");
            Console.WriteLine("7 - Заменить слова в предложении");
            Console.WriteLine("8 - Удалить стоп-слова");
            Console.WriteLine("9 - Статистика по длине слов");
            Console.WriteLine("10 - Экспорт в XML");
            Console.WriteLine("0 - Выход");

            Console.Write("Выбор: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "0": return;
                case "1":
                    text = parser.Parse(File.ReadAllText("text_ru.txt"));
                    Console.WriteLine("Русский текст загружен.");
                    break;
                case "2":
                    text = parser.Parse(File.ReadAllText("text_en.txt"));
                    Console.WriteLine("Английский текст загружен.");
                    break;
                case "3":
                    foreach (var s in text.SortByWordCount())
                        Console.WriteLine(s);
                    break;
                case "4":
                    foreach (var s in text.SortByLength())
                        Console.WriteLine(s);
                    break;
                case "5":
                    Console.Write("Введите длину слова: ");
                    int lenQ = int.Parse(Console.ReadLine());
                    var words = text.FindWordsInQuestions(lenQ);
                    Console.WriteLine(string.Join(", ", words));
                    break;
                case "6":
                    Console.Write("Введите длину слова: ");
                    int lenC = int.Parse(Console.ReadLine());
                    text.RemoveWordsStartingWithConsonant(lenC);
                    Console.WriteLine("Удалено.");
                    break;
                case "7":
                    Console.Write("Номер предложения: ");
                    int index = int.Parse(Console.ReadLine());
                    Console.Write("Длина слова: ");
                    int lenR = int.Parse(Console.ReadLine());
                    Console.Write("Подстрока: ");
                    string sub = Console.ReadLine();
                    text.ReplaceWordsInSentence(index, lenR, sub);
                    Console.WriteLine("Заменено.");
                    break;
                case "8":
                    var stopWords = StopWords.Load("stopwords_ru.txt");
                    StopWords.RemoveStopWords(text, stopWords);
                    Console.WriteLine("Стоп-слова удалены.");
                    break;
                case "9":
                    var stats = text.WordLengthStats();
                    foreach (var kv in stats)
                        Console.WriteLine($"Длина {kv.Key}: {kv.Value} слов");
                    break;
                case "10":
                    XmlExporter.Export(text, "output.xml");
                    Console.WriteLine("Экспорт завершён.");
                    break;
                default:
                    Console.WriteLine("Неверный выбор.");
                    break;
            }
        }
    }
}
