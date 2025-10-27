namespace LABA2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("Введите номер входного файла (1, 2, 3 или 0 - для выхода): ");
                string? input = Console.ReadLine();

                if (input != null)
                {
                    input = input.Trim();
                }

                if (string.IsNullOrEmpty(input) || input == "0")
                {
                    return;
                }

                if (!TryWorkWithFile.GetFileNumber(input, out int fileNumber))
                {
                    Console.WriteLine("Неверный ввод. Введите 0, 1, 2 или 3.");
                    continue;
                }

                string inputFile = $"{fileNumber}.ChaseData.txt";
                string outFile = $"{fileNumber}.PursuitLog.txt";

                if (!TryWorkWithFile.ReadSizeFromFile(inputFile, out int size, out string? error))
                {
                    Console.WriteLine(error);
                    continue;
                }

                Game.InputFile = inputFile;
                Game.OutFile = outFile;

                Game game = new Game(size);
                string[] lines = File.ReadAllLines(inputFile);
                game.Run(lines);

                Console.WriteLine($"Результат записан в {Game.OutFile}");
            }
        }
    }
}