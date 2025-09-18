using System;
using System.IO;

namespace КПО_лаба_1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string avtor = "Karakulko Denis";

            Console.WriteLine("Выберите номер набора данных:");
            Console.WriteLine("0. sequences.0.txt / commands.0.txt");
            Console.WriteLine("1. sequences.1.txt / commands.1.txt");
            Console.WriteLine("2. sequences.2.txt / commands.2.txt");
            Console.Write("Введите номер (0–2): ");

            string N = Console.ReadLine();
            while (N != "0" && N != "1" && N != "2")
            {
                Console.Write("Неверный ввод! Введите 0, 1 или 2: ");
                N = Console.ReadLine();
            }

            string inputSequences = $"sequences.{N}.txt";
            string inputCommands = $"commands.{N}.txt";
            string outputGenedata = $"genedata.{N}.txt";

            // наличие входных файлов
            if (!File.Exists(inputSequences)){
                Console.WriteLine($"Файл {inputSequences} не найден. Операция отменена.");
                return;
            }

            if (!File.Exists(inputCommands)){
                Console.WriteLine($"Файл {inputCommands} не найден. Операция отменена.");
                return;
            }

             var processor = new GeneticCode(avtor);
             processor.LoadSequences(inputSequences);
             processor.ProcessCommands(inputCommands, outputGenedata);

             Console.WriteLine($"Результаты записаны в {outputGenedata}");
        }
    }
}
