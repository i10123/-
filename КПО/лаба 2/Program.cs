using System;
using System.IO;
using System.Text.RegularExpressions;

namespace КПО_лаба_2
{
    class Program
        {
        static void Main(string[] args)
        {
            int fileCount = 3;

            for (int i = 1; i <= fileCount; i++)
            {
                string inputFile = $"{i}.ChaseData.txt";
                string outputFile = $"{i}.PursuitLog.txt";

                if (!File.Exists(inputFile))
                {
                    Console.WriteLine($"Файл {inputFile} не найден.");
                    continue;
                }
                if (new FileInfo(inputFile).Length == 0)
                {
                    Console.WriteLine($"Файл {inputFile} пустой.");
                    continue;
                }

                using (StreamReader reader = new StreamReader(inputFile))
                using (StreamWriter writer = new StreamWriter(outputFile))
                {
                    string firstLine = reader.ReadLine();
                    int FieldSize = Convert.ToInt32(firstLine);

                    Game game = new Game(FieldSize);

                    writer.WriteLine("Кот и Мышь");
                    writer.WriteLine();
                    writer.WriteLine("Кот   Мышь   Расстояние");
                    writer.WriteLine("-----------------------");

                    bool caught = false;

                    while (!reader.EndOfStream && caught == false)
                    {
                        string line = reader.ReadLine().Trim();

                        if (line == "P")
                            game.Print(writer);
                        else
                        {
                            string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length == 2)
                            {
                                string commandText = parts[0];
                                char command = commandText[0];

                                string moveText = parts[1];
                                bool isNumber = int.TryParse(moveText, out int movement);

                                if (isNumber)
                                {
                                    game.DoingCommand(command, movement);

                                    if (game.CheckCatch())
                                        caught = true;
                                }
                            }

                        }
                    }

                    writer.WriteLine("-------------------");
                    writer.WriteLine();
                    writer.WriteLine();
                    writer.WriteLine("Пройденное расстояние:   Мышь    Кот");
                    writer.WriteLine($"                        {game.mouse.traveledDistance,4}    {game.cat.traveledDistance,3}");
                    writer.WriteLine();

                    if (game.state == GameState.End)
                    {
                        writer.WriteLine("Мышь поймана в клетке: " + game.cat.location);
                    }
                    else
                    {
                        writer.WriteLine("Мышь ускользнула от кота");
                    }
                }

                Console.WriteLine($"Файл {outputFile} записан.");
            }
        }
    }
}
