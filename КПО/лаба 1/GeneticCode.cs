using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace КПО_лаба_1
{
    public class GeneticCode
    {
        public List<GeneticData> data = new List<GeneticData>();
        public void LoadSequences(string filename)
        {
            string[] lines = File.ReadAllLines(filename);
            for (int i = 0; i < lines.Length; i++)
            {
                string[] parts = lines[i].Split('\t'); // белок, организм, аминокислота
                GeneticData entry;
                entry.protein = parts[0]; // белок
                entry.organism = parts[1]; // организм
                entry.amino_acids = Decode(parts[2]); // аминокислота
                data.Add(entry);
            }
        }
        public void ProcessingCommandsWritingResult(string commandFile, string outputFile)
        {
            using (StreamWriter writer = new StreamWriter(outputFile))
            {
                writer.WriteLine("Каракулько Денис");
                writer.WriteLine("Генетический поиск");

                string[] commands = File.ReadAllLines(commandFile);
                for (int i = 0; i < commands.Length; i++)
                {
                    string[] PartsInCommand = commands[i].Split('\t');

                    StringBuilder header = new StringBuilder();
                    header.Append((i + 1).ToString("D3")) // Номер операции
                          .Append('\t').Append(PartsInCommand[0]);

                    if (PartsInCommand.Length > 1)
                    {
                        string param1 = PartsInCommand[0] == "search" ? Decode(PartsInCommand[1]) : PartsInCommand[1];
                        header.Append('\t').Append(param1);
                    }
                    if (PartsInCommand.Length > 2)
                    {
                        header.Append('\t').Append(PartsInCommand[2]);
                    }

                    writer.WriteLine("--------------------------------------------------------------------------");
                    writer.WriteLine(header.ToString());

                    switch (PartsInCommand[0])
                    {
                        case "search":
                            if (PartsInCommand.Length >= 2)
                                Search(PartsInCommand[1], writer);
                            break;
                        case "diff":
                            if (PartsInCommand.Length >= 3)
                                Diff(PartsInCommand[1], PartsInCommand[2], writer);
                            break;
                        case "mode":
                            if (PartsInCommand.Length >= 2)
                                Mode(PartsInCommand[1], writer);
                            break;
                    }
                }
            }
        }
        // Метод поиска аминокислотной последовательности
        public void Search(string str, StreamWriter writer)
        {
            string decompressed = Decode(str);
            bool found = false;

            writer.WriteLine("organism\t\t\tprotein");

            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].amino_acids.Contains(decompressed)) // цепочка содержит искомую последовательность
                {
                    writer.WriteLine(data[i].organism + "\t\t" + data[i].protein);
                    found = true;
                }
            }

            if (!found)
            {
                writer.WriteLine("NOT FOUND");
            }
        }
        // Метод сравнения двух белков
        public void Diff(string prot1, string prot2, StreamWriter writer)
        {
            GeneticData protein1 = new GeneticData();
            GeneticData protein2 = new GeneticData();
            bool found1 = false;
            bool found2 = false;

            for (int i = 0; i < data.Count; i++) // Ищем оба белка по имени
            {
                if (data[i].protein == prot1)
                {
                    protein1 = data[i];
                    found1 = true;
                }
                if (data[i].protein == prot2)
                {
                    protein2 = data[i];
                    found2 = true;
                }
            }

            writer.WriteLine("amino-acids difference:");

            if (!found1 || !found2)
            {
                string missing = "";
                if (!found1) missing += prot1 + " ";
                if (!found2) missing += prot2;
                writer.WriteLine("MISSING:\t" + missing);
                return;
            }

            int diff = 0;
            int len = Math.Min(protein1.amino_acids.Length, protein2.amino_acids.Length);

            for (int i = 0; i < len; i++)
            {
                if (protein1.amino_acids[i] != protein2.amino_acids[i])
                {
                    diff++;
                }
            }
            diff += Math.Abs(protein1.amino_acids.Length - protein2.amino_acids.Length);

            writer.WriteLine(diff.ToString());
        }
        // Метод поиска самой частой аминокислоты
        public void Mode(string proteinName, StreamWriter writer)
        {
            GeneticData p = new GeneticData();
            bool found = false;

            for (int i = 0; i < data.Count; i++) // Ищем белок по имени
            {
                if (data[i].protein == proteinName)
                {
                    p = data[i];
                    found = true;
                    break;
                }
            }

            writer.WriteLine("amino-acid occurs:");

            if (!found)
            {
                writer.WriteLine("MISSING:\t" + proteinName);
                return;
            }

            Dictionary<char, int> frequency = new Dictionary<char, int>();

            for (int i = 0; i < p.amino_acids.Length; i++)
            {
                char c = p.amino_acids[i];
                if (frequency.ContainsKey(c))
                    frequency[c]++;
                else
                    frequency[c] = 1;
            }

            int max_frequency = 0;
            char popular_symbol = ' ';

            List<char> keys = new List<char>(frequency.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                char key = keys[i];
                int value = frequency[key];

                if (value > max_frequency || (value == max_frequency && key < popular_symbol))
                {
                    max_frequency = value;
                    popular_symbol = key;
                }
            }

            writer.WriteLine(popular_symbol + "\t\t" + max_frequency);
        }
        public string Decode(string input_acid)
        {
            StringBuilder full = new StringBuilder();
            for (int i = 0; i < input_acid.Length; i++)
            {
                if (!(char.IsDigit(input_acid[i]) && i + 1 < input_acid.Length))
                {
                    full.Append(input_acid[i]);
                    continue;
                }

                int num = input_acid[i] - '0';
                char letter = input_acid[i + 1];
                full.Append(letter, num);
                i++;
            }
            return full.ToString();
        }
    }
}
