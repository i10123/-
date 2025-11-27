namespace LABS_3_4
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("Выберите текст для обработки:\n" +
                    "1 — Русский текст (RU_text.txt)\n" +
                    "2 — Английский текст (ENG_text.txt)\n" +
                    "0 — Выход\n" +
                    "Ваш выбор: ");
                string? choice = Console.ReadLine();

                string? inputFile;
                string? languageCode;

                switch (choice)
                {
                    case "0":
                        return;
                    case "1":
                        inputFile = "RU_text.txt";
                        languageCode = "ru";
                        break;
                    case "2":
                        inputFile = "ENG_text.txt";
                        languageCode = "en";
                        break;
                    default:
                        Console.WriteLine("\nНеверный выбор текста!\n");
                        continue;
                }

                if (!File.Exists(inputFile))
                {
                    Console.WriteLine($"Файл {inputFile} не найден!");
                    continue;
                }

                string originalText = File.ReadAllText(inputFile);
                if (string.IsNullOrWhiteSpace(originalText))
                {
                    Console.WriteLine("Файл пуст!");
                    continue;
                }

                var text = Parser.Parse(originalText);
                TextProcessor.Menu(text, originalText, languageCode);
            }
        }
    }
}