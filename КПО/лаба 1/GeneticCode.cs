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
    public class GeneticCode{
        public List<GeneticData> data = new List<GeneticData>(); // Список всех белков из файла
        public int operationCount = 1;
        public void LoadSequences(string filename){
            string[] lines = File.ReadAllLines(filename); // Читаем все строки из файла
            for (int i = 0; i < lines.Length; i++){
                string[] parts = lines[i].Split('\t'); // Разделяем строку: [белок, организм, аминокислоты]
                GeneticData entry; // Создаём переменную для хранения одной записи
                entry.protein = parts[0]; // белок
                entry.organism = parts[1]; // организм
                entry.amino_acids = Decompress(parts[2]); // Раскодировка аминокислоты
                data.Add(entry); // Добавляем запись в список
            }
        }

        // Метод для обработки команд и записи результата
        public void Processing_Commands_and_Writing_Result(string commandFile, string outputFile){
            using (StreamWriter writer = new StreamWriter(outputFile)){ // Открываем файл для записи
                writer.WriteLine("Каракулько Денис"); 
                writer.WriteLine("Генетический поиск");

                string[] commands = File.ReadAllLines(commandFile); // Читаем все команды
                for (int i = 0; i < commands.Length; i++){
                    string[] parts = commands[i].Split('\t');
                    string operationNum = operationCount.ToString("D3");
                    string header = operationNum + "\t" + parts[0]; // Начало заголовка: номер и тип операции
                    if (parts.Length > 1) 
                        header += "\t" + parts[1]; // Добавляем параметр 1
                    if (parts.Length > 2) 
                        header += "\t" + parts[2]; // Добавляем параметр 2, если есть

                    writer.WriteLine("--------------------------------------------------------------------------"); // Разделитель
                    writer.WriteLine(header);

                    if (parts[0] == "search" && parts.Length >= 2)
                    {
                        Search(parts[1], writer); // Поиск последовательности
                    }
                    else if (parts[0] == "diff" && parts.Length >= 3)
                    {
                        Diff(parts[1], parts[2], writer); // Сравнение двух белков
                    }
                    else if (parts[0] == "mode" && parts.Length >= 2)
                    {
                        Mode(parts[1], writer); // Частотный анализ аминокислот
                    }

                    operationCount++; // Увеличиваем номер операции
                }
            }
        }

        // Метод поиска аминокислотной последовательности
        public void Search(string str, StreamWriter writer)
        {
            string decompressed = Decompress(str); // Раскодируем строку, если она сжата
            bool found = false; // Флаг — нашли ли совпадение

            writer.WriteLine("organism\t\t\tprotein"); // Заголовок таблицы

            for (int i = 0; i < data.Count; i++) // Проходим по всем белкам
            {
                if (data[i].amino_acids.Contains(decompressed)) // Если цепочка содержит искомую последовательность
                {
                    writer.WriteLine(data[i].organism + "\t\t\t" + data[i].protein); // Пишем организм и белок
                    found = true; // Отмечаем, что нашли
                }
            }

            if (!found) // Если ничего не нашли
            {
                writer.WriteLine("NOT FOUND"); // Пишем сообщение
            }
        }

        // Метод сравнения двух белков
        public void Diff(string prot1, string prot2, StreamWriter writer)
        {
            GeneticData p1 = new GeneticData(); // Первый белок
            GeneticData p2 = new GeneticData(); // Второй белок
            bool found1 = false; // Найден ли первый
            bool found2 = false; // Найден ли второй

            for (int i = 0; i < data.Count; i++) // Ищем оба белка по имени
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

            writer.WriteLine("amino-acids difference:"); // Заголовок

            if (!found1 || !found2) // Если хотя бы один не найден
            {
                string missing = "";
                if (!found1) missing += prot1 + " "; // Добавляем имя отсутствующего
                if (!found2) missing += prot2;
                writer.WriteLine("MISSING:\t" + missing); // Пишем сообщение
                return; // Выходим из метода
            }

            int diff = 0; // Счётчик различий
            int len = Math.Min(p1.amino_acids.Length, p2.amino_acids.Length); // Берём минимальную длину

            for (int i = 0; i < len; i++) // Сравниваем символы по одному
            {
                if (p1.amino_acids[i] != p2.amino_acids[i])
                {
                    diff++; // Если разные — увеличиваем счётчик
                }
            }

            diff += Math.Abs(p1.amino_acids.Length - p2.amino_acids.Length); // Добавляем разницу в длине
            writer.WriteLine(diff.ToString()); // Пишем результат
        }

        // Метод поиска самой частой аминокислоты
        public void Mode(string proteinName, StreamWriter writer)
        {
            GeneticData p = new GeneticData(); // Переменная для найденного белка
            bool found = false; // Флаг — найден ли белок

            for (int i = 0; i < data.Count; i++) // Ищем белок по имени
            {
                if (data[i].protein == proteinName)
                {
                    p = data[i];
                    found = true;
                }
            }

            writer.WriteLine("amino-acid occurs:"); // Заголовок

            if (!found) // Если белок не найден
            {
                writer.WriteLine("MISSING:\t" + proteinName); // Пишем сообщение
                return;
            }

            int[] freq = new int[256]; // Массив частот по ASCII (всего 256 символов)

            for (int i = 0; i < p.amino_acids.Length; i++) // Проходим по цепочке
            {
                char c = p.amino_acids[i]; // Берём символ
                freq[c]++; // Увеличиваем его счётчик
            }

            int max = 0; // Максимальная частота
            char most = ' '; // Самый частый символ

            for (int i = 0; i < freq.Length; i++) // Проходим по всем возможным символам
            {
                if (freq[i] > max || (freq[i] == max && i < most)) // Если частота больше или равна, но символ раньше по алфавиту
                {
                    max = freq[i];
                    most = (char)i;
                }
            }

            writer.WriteLine(most + "\t\t" + max); // Пишем результат
        }

        public string Decompress(string input_acid){
            string full_amino_acid = "";
            for (int i = 0; i < input_acid.Length; i++){
                if (char.IsDigit(input_acid[i]) && i + 1 < input_acid.Length){
                    int num = input_acid[i] - '0'; // преобразование ('3' -> 3)
                    char letter = input_acid[i + 1]; // следующий символ — аминокислота
                    for (int j = 0; j < num; j++){ // записываем символ num раз
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
