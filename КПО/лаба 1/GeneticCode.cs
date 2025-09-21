using System;
using System.Collections.Generic;
using System.IO;

namespace КПО_лаба_1
{
    public struct GeneticData
    {
        public string protein;
        public string organism;
        public string amino_acids;
    }
    public class GeneticCode
    {
        public List<GeneticData> data = new List<GeneticData>(); // Список всех белков из файла
        public int operationCount = 1;
        public void LoadSequences(string filename)

        {
            string[] lines = File.ReadAllLines(filename);       // Читаем все строки из файла
            for (int i = 0; i < lines.Length; i++)
            {
                string[] parts = lines[i].Split('\t');          // Разделяем строку: [белок, организм, аминокислоты]
                GeneticData entry;                              // Создаём переменную для хранения одной записи
                entry.protein = parts[0];                       // белок
                entry.organism = parts[1];                      // организм
                entry.amino_acids = Decoding(parts[2]);       // Раскодировка аминокислоты
                data.Add(entry);                                // Добавляем запись в список
            }
        }
        public void Processing_Commands_and_Writing_Result(string commandFile, string outputFile) // обработка команд и запись результата
        {
            using (StreamWriter writer = new StreamWriter(outputFile))  // Открываем файл для записи
            { 
                writer.WriteLine("Каракулько Денис");
                writer.WriteLine("Генетический поиск");

                string[] commands = File.ReadAllLines(commandFile);     // Читаем все команды
                for (int i = 0; i < commands.Length; i++)
                {
                    string[] parts = commands[i].Split('\t');
                    string operationNum = operationCount.ToString("D3");
                    string header = operationNum + "\t" + parts[0];     // Начало заголовка: номер и тип операции
                    if (parts.Length > 1)
                        header += "\t" + parts[1];                      // Добавляем параметр 1
                    if (parts.Length > 2)
                        header += "\t" + parts[2];                      // Добавляем параметр 2, если есть

                    writer.WriteLine("--------------------------------------------------------------------------");
                    writer.WriteLine(header);

                    if (parts[0] == "search" && parts.Length >= 2)
                        Search(parts[1], writer);                       // Поиск последовательности
                    else if (parts[0] == "diff" && parts.Length >= 3)
                        Diff(parts[1], parts[2], writer);               // Сравнение двух белков
                    else if (parts[0] == "mode" && parts.Length >= 2)
                        Mode(parts[1], writer);                         // Частотный анализ аминокислот

                    operationCount++;
                }
            }
        }
        public void Search(string str, StreamWriter writer) // поиск аминокислотной последовательности
        {
            string decoded = Decoding(str);
            bool found = false;

            writer.WriteLine("organism\t\tprotein");                  // Заголовок таблицы

            for (int i = 0; i < data.Count; i++)                        // Проходим по всем белкам
            {
                if (data[i].amino_acids.Contains(decoded))              // цепочка содержит искомую последовательность
                {
                    writer.WriteLine(data[i].organism + "\t" + data[i].protein);
                    found = true;
                }
            }
            if (!found)
            {
                writer.WriteLine("NOT FOUND");
            }
        }
        public void Diff(string prot1, string prot2, StreamWriter writer) // сравнение двух белков
        {
            GeneticData p1 = new GeneticData();     // белок 1
            GeneticData p2 = new GeneticData();     // белок 2
            bool found1 = false;
            bool found2 = false;

            for (int i = 0; i < data.Count; i++)    // Ищем оба белка по имени
            {
                if (data[i].protein == prot1)
                {
                    p1 = data[i];
                    found1 = true;
                }
                if (data[i].protein == prot2)
                {
                    p2 = data[i];
                    found2 = true;
                }
            }

            writer.WriteLine("amino-acids difference:");

            if (!found1 || !found2)                 // Если хотя бы один не найден
            {
                string missing = "";
                if (!found1) 
                    missing += prot1;               // Добавляем имя отсутствующего
                if (!found2) 
                    missing += "\t" + prot2;
                writer.WriteLine("MISSING: " + missing);
                return;
            }

            int diff = 0;
            int len = Math.Min(p1.amino_acids.Length, p2.amino_acids.Length); // Берём минимальную длину

            for (int i = 0; i < len; i++) // Сравниваем символы
            {
                if (p1.amino_acids[i] != p2.amino_acids[i])
                {
                    diff++;
                }
            }

            diff += Math.Abs(p1.amino_acids.Length - p2.amino_acids.Length); // + разница в длине
            writer.WriteLine(diff.ToString());
        }
        public void Mode(string proteinName, StreamWriter writer) // поиск самой частой аминокислоты
        {
            GeneticData p = new GeneticData();
            bool found = false;

            for (int i = 0; i < data.Count; i++)        // Ищем белок по имени
            {
                if (data[i].protein == proteinName)
                {
                    p = data[i];
                    found = true;
                }
            }

            writer.WriteLine("amino-acid occurs:");

            if (!found)
            {
                writer.WriteLine("MISSING: " + proteinName);
                return;
            }

            int[] freq = new int[256];                  // Массив частот по ASCII

            for (int i = 0; i < p.amino_acids.Length; i++)
            {
                char c = p.amino_acids[i];
                freq[c]++;
            }

            int max = 0;
            char most = 'Y';

            for (int i = 0; i < freq.Length; i++)
            {
                if (freq[i] > max || (freq[i] == max && i < most))
                {
                    max = freq[i];
                    most = (char)i;
                }
            }

            writer.WriteLine(most + "\t" + max);
        }
        public string Decoding(string input_acid)
        {
            string full_amino_acid = "";
            for (int i = 0; i < input_acid.Length; i++)
            {
                if (char.IsDigit(input_acid[i]) && i + 1 < input_acid.Length)
                {
                    int num = input_acid[i] - '0';      // преобразование ('3' -> 3)
                    char letter = input_acid[i + 1];    // следующий символ — аминокислота
                    for (int j = 0; j < num; j++)       // записываем символ num раз
                    { 
                        full_amino_acid += letter;
                    }
                    i++;
                }
                else
                    full_amino_acid += input_acid[i];
            }
            return full_amino_acid;
        }
    }
}
