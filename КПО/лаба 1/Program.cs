using System;
using System.IO;
using System.Text;

namespace КПО_лаба_1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("0. X = 0");
            Console.WriteLine("1. X = 1");
            Console.WriteLine("2. X = 2");
            Console.Write("Выберите номер набора данных (sequences.X & commands.X): ");

            string N = Console.ReadLine();
            while (true)
            {
                switch (N)
                {
                    case "0":
                    case "1":
                    case "2":
                        break;
                    default:
                        Console.Write("Неверный ввод! Повторите: ");
                        N = Console.ReadLine();
                        continue;
                }
                break;
            }

            StringBuilder inputSequences = new StringBuilder("sequences.").Append(N).Append(".txt");
            StringBuilder inputCommands = new StringBuilder("commands.").Append(N).Append(".txt");
            StringBuilder outputGenedata = new StringBuilder("genedata.").Append(N).Append(".txt");

            if (!File.Exists(inputSequences.ToString()) || !File.Exists(inputCommands.ToString()))
            {
                Console.WriteLine("Файл не найден!");
                return;
            }

            var proc = new GeneticCode();
            proc.LoadSequences(inputSequences.ToString());
            proc.Processing_Commands_and_Writing_Result(inputCommands.ToString(), outputGenedata.ToString());
            Console.WriteLine("Результаты записаны!");
        }
    }
}
