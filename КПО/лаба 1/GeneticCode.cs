using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
        private List<GeneticData> data = new List<GeneticData>();
        private int operationCount = 1;
        private string author;

        public GeneticCode(string authorName)
        {
            author = authorName;
        }

        public void LoadSequences(string filename)
        {
            foreach (var line in File.ReadAllLines(filename))
            {
                var parts = line.Split('\t');
                if (parts.Length == 3)
                {
                    GeneticData entry;
                    entry.protein = parts[0];
                    entry.organism = parts[1];
                    entry.amino_acids = Decode(parts[2]);
                    data.Add(entry);
                }
            }
        }

        public void ProcessCommands(string commandFile, string outputFile)
        {
            using (var writer = new StreamWriter(outputFile))
            {
                writer.WriteLine(author);
                writer.WriteLine("Генетический поиск");

                foreach (var line in File.ReadAllLines(commandFile))
                {
                    string[] parts = line.Split('\t');
                    string opNum = operationCount.ToString("D3");
                    string header = $"{opNum}\t{parts[0]}";

                    if (parts[0] == "search" && parts.Length >= 2)
                        header += $"\t{parts[1]}";
                    else if (parts[0] == "diff" && parts.Length >= 3)
                        header += $"\t{parts[1]}\t{parts[2]}";
                    else if (parts[0] == "mode" && parts.Length >= 2)
                        header += $"\t{parts[1]}";

                    writer.WriteLine(new string('-', 74));
                    writer.WriteLine(header);

                    switch (parts[0])
                    {
                        case "search":
                            if (parts.Length >= 2) Search(parts[1], writer);
                            break;
                        case "diff":
                            if (parts.Length >= 3) Diff(parts[1], parts[2], writer);
                            break;
                        case "mode":
                            if (parts.Length >= 2) Mode(parts[1], writer);
                            break;
                    }

                    operationCount++;
                }
            }
        }

        private void Search(string query, StreamWriter writer)
        {
            string decoded = Decode(query);
            bool found = false;

            writer.WriteLine("organism\t\t\tprotein");

            foreach (var item in data)
            {
                if (item.amino_acids.Contains(decoded))
                {
                    writer.WriteLine($"{item.organism,-24}\t{item.protein}");
                    found = true;
                }
            }

            if (!found)
                writer.WriteLine("NOT FOUND");
        }

        private void Diff(string prot1, string prot2, StreamWriter writer)
        {
            var p1 = data.Find(p => p.protein == prot1);
            var p2 = data.Find(p => p.protein == prot2);

            writer.WriteLine("amino-acids difference:");

            if (string.IsNullOrEmpty(p1.protein) || string.IsNullOrEmpty(p2.protein))
            {
                List<string> missing = new List<string>();
                if (string.IsNullOrEmpty(p1.protein)) missing.Add(prot1);
                if (string.IsNullOrEmpty(p2.protein)) missing.Add(prot2);
                writer.WriteLine("MISSING:\t" + string.Join(", ", missing));
                return;
            }

            int diff = 0;
            int len = Math.Min(p1.amino_acids.Length, p2.amino_acids.Length);
            for (int i = 0; i < len; i++)
                if (p1.amino_acids[i] != p2.amino_acids[i]) diff++;

            diff += Math.Abs(p1.amino_acids.Length - p2.amino_acids.Length);
            writer.WriteLine(diff.ToString());
        }

        private void Mode(string proteinName, StreamWriter writer)
        {
            var p = data.Find(d => d.protein == proteinName);
            writer.WriteLine("amino-acid occurs:");

            if (string.IsNullOrEmpty(p.protein))
            {
                writer.WriteLine("MISSING:\t" + proteinName);
                return;
            }

            Dictionary<char, int> freq = new Dictionary<char, int>();
            foreach (char c in p.amino_acids)
            {
                if (!freq.ContainsKey(c)) freq[c] = 0;
                freq[c]++;
            }

            int max = 0;
            char most = ' ';
            foreach (var kv in freq)
            {
                if (kv.Value > max || (kv.Value == max && kv.Key < most))
                {
                    max = kv.Value;
                    most = kv.Key;
                }
            }

            writer.WriteLine($"{most}\t\t{max}");
        }

        public static string Decode(string input)
        {
            StringBuilder decoded = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsDigit(input[i]) && i + 1 < input.Length)
                {
                    int count = input[i] - '0';
                    char ch = input[i + 1];
                    decoded.Append(new string(ch, count));
                    i++;
                }
                else
                {
                    decoded.Append(input[i]);
                }
            }
            return decoded.ToString();
        }

        public static string Encode(string input)
        {
            StringBuilder encoded = new StringBuilder();
            int count = 1;

            for (int i = 1; i < input.Length; i++)
            {
                if (input[i] == input[i - 1] && count < 9)
                    count++;
                else
                {
                    encoded.Append(count > 2 ? count.ToString() + input[i - 1] : new string(input[i - 1], count));
                    count = 1;
                }
            }

            char lastChar = input[input.Length - 1];
            encoded.Append(count > 2 ? count.ToString() + lastChar : new string(lastChar, count));
            return encoded.ToString();
        }
    }
}
