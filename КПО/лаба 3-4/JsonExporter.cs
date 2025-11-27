using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace LABS_3_4
{
    public static class JsonExporter
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
        };

        public static void Save(Text text, string filePath)
        {
            string jsonString = JsonSerializer.Serialize(text, _options);

            File.WriteAllText(filePath, jsonString);
        }
    }
}