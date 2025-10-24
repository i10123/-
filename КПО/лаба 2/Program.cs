using LABA2;

while (true)
{
    Console.Write("Введите номер входного файла (1, 2, 3 или 0 - для выхода): ");
    string? input = Console.ReadLine()?.Trim();

    if (input == null || input == "0")
        return;

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

    var game = new Game(size);
    game.Run(File.ReadAllLines(inputFile));

    Console.WriteLine($"Результат записан в {Game.OutFile}");
}