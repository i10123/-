using System;
using System.IO;

namespace КПО_лаба_1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string N;

            do
            {
                Console.WriteLine("0. X = 0");
                Console.WriteLine("1. X = 1");
                Console.WriteLine("2. X = 2");
                Console.WriteLine("Для выхода введите 111");
                Console.Write("Ваш выбор: ");

                N = Console.ReadLine();

                if (N == "111")
                    break;

                if (N != "0" && N != "1" && N != "2")
                {
                    Console.WriteLine("Неверный ввод!");
                    continue;
                }

                string inputSequences = $"sequences.{N}.txt";
                string inputCommands = $"commands.{N}.txt";
                string outputGenedata = $"genedata.{N}.txt";

                if (!File.Exists(inputSequences) || !File.Exists(inputCommands))
                {
                    Console.WriteLine("Файл не найден!");
                    continue;
                }

                var proc = new GeneticCode();
                proc.LoadSequences(inputSequences);
                proc.ProcessingCommandsWritingResult(inputCommands, outputGenedata);
                Console.WriteLine("Результаты записаны!\n");
            }
            while (true);

            Console.WriteLine("Программа завершена.");
        }
    }
}
