namespace LABA2
{
    public static class TryWorkWithFile
    {
        public static bool GetFileNumber(string input, out int number)
        {
            if (!int.TryParse(input, out number))
                return false;

            return number >= 0 && number <= 3;
        }

        public static bool ReadSizeFromFile(string fileName, out int size, out string? error)
        {
            size = 0;
            error = null;

            if (!File.Exists(fileName))
            {
                error = $"Файл {fileName} не найден.";
                return false;
            }

            string[] lines = File.ReadAllLines(fileName);

            if (lines.Length == 0)
            {
                error = $"Файл {fileName} пуст.";
                return false;
            }

            if (!int.TryParse(lines[0].Trim(), out size) || size <= 0)
            {
                error = "Ошибка: первая строка должна содержать натуральный размер поля.";
                return false;
            }

            return true;
        }
    }
}