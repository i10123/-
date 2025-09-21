using System;
using System.IO;

namespace КПО_лаба_1{
    internal class Program{
        static void Main(string[] args){
            Console.WriteLine("0. X = 0");
            Console.WriteLine("1. X = 1");
            Console.WriteLine("2. X = 2");
            Console.Write("Выберите номер набора данных (sequences.X & commands.X): ");

            string N = Console.ReadLine();
            while (N != "0" && N != "1" && N != "2"){
                Console.Write("Неверный ввод! Повторите: ");
                N = Console.ReadLine();
            }

            string inputSequences = $"sequences.{N}.txt";
            string inputCommands = $"commands.{N}.txt";
            string outputGenedata = $"genedata.{N}.txt";

            if (!File.Exists(inputSequences) || !File.Exists(inputCommands)){
                Console.WriteLine("Файл не найден!");
                return;
            }

            var proc = new GeneticCode();
            proc.LoadSequences(inputSequences);
            proc.Processing_Commands_and_Writing_Result(inputCommands, outputGenedata);
            Console.WriteLine("Результаты записаны!");
        }
    }
}
