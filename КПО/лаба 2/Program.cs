using System;
using System.IO;

namespace КПО_лаба_2
{
    class Program
    {
        static void Main(string[] args)
        {
            // Process each test case file
            string[] inputFiles = { "1.ChaseData.txt", "2.ChaseData.txt", "3.ChaseData.txt" };

            foreach (string inputFile in inputFiles)
            {
                // Extract number from filename (e.g., "1" from "1.ChaseData.txt")
                string number = inputFile.Split('.')[0];
                string outputFile = number + ".PursuitLog.txt";

                using (StreamReader reader = new StreamReader(inputFile))
                using (StreamWriter writer = new StreamWriter(outputFile))
                {
                    int N = int.Parse(reader.ReadLine().Trim());

                    Game game = new Game(N);

                    writer.WriteLine("Cat and Mouse");
                    writer.WriteLine();
                    writer.WriteLine("Cat   Mouse   Distance");
                    writer.WriteLine("-------------------");

                    bool caught = false;

                    while (!reader.EndOfStream && !caught)
                    {
                        string line = reader.ReadLine().Trim();
                        if (string.IsNullOrEmpty(line)) continue;

                        if (line == "P")
                        {
                            game.DoPrintCommand(writer);
                        }
                        else
                        {
                            string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length == 2)
                            {
                                char command = parts[0][0];
                                int steps;
                                if (int.TryParse(parts[1], out steps))
                                {
                                    game.DoMoveCommand(command, steps);
                                    if (game.CheckCatch())
                                    {
                                        caught = true;
                                    }
                                }
                            }
                        }
                    }

                    writer.WriteLine("-------------------");
                    writer.WriteLine();
                    writer.WriteLine();
                    writer.WriteLine("Distance traveled:   Mouse    Cat");
                    writer.WriteLine("                                  " + game.mouse.distanceTraveled.ToString().PadLeft(5) + "      " + game.cat.distanceTraveled.ToString().PadLeft(5));
                    writer.WriteLine();

                    if (game.state == GameState.End)
                    {
                        writer.WriteLine("Mouse caught at: " + game.cat.location);
                    }
                    else
                    {
                        writer.WriteLine("Mouse evaded Cat");
                    }
                }
            }
        }
    }
}
