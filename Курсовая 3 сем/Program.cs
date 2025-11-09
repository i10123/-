using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

// Используем алгоритм основанный на спецификации ISO/IEC 18004
// Для гарантированной читаемости QR кодов

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("=== QR Code Generator V1 Level Q ===\n");
        Console.Write("Введите текст (макс 16 символов): ");
        string text = Console.ReadLine();

        if (string.IsNullOrEmpty(text))
        {
            Console.WriteLine("Текст не может быть пустым!");
            return;
        }

        if (text.Length > 16)
        {
            Console.WriteLine($"Слишком длинный текст! Максимум 16 символов, введено: {text.Length}");
            return;
        }

        try
        {
            var generator = new QRCodeGenerator();
            bool[,] matrix = generator.CreateQRCode(text);

            string filename = $"qrcode_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            SaveQRCode(matrix, filename, 10, 4);

            Console.WriteLine($"\n✓ QR-код успешно создан: {filename}");
            Console.WriteLine($"  Размер матрицы: {matrix.GetLength(0)}x{matrix.GetLength(1)}");
            Console.WriteLine($"  Текст: \"{text}\"");
            Console.WriteLine($"  Версия: 1, Уровень: Q");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Ошибка: {ex.Message}");
        }

        Console.WriteLine("\nНажмите Enter для выхода...");
        Console.ReadLine();
    }

    static void SaveQRCode(bool[,] matrix, string filename, int moduleSize, int quietZone)
    {
        int size = matrix.GetLength(0);
        int imageSize = (size + quietZone * 2) * moduleSize;

        using (Bitmap bitmap = new Bitmap(imageSize, imageSize))
        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            graphics.Clear(Color.White);

            using (SolidBrush brush = new SolidBrush(Color.Black))
            {
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        if (matrix[y, x])
                        {
                            int pixelX = (x + quietZone) * moduleSize;
                            int pixelY = (y + quietZone) * moduleSize;
                            graphics.FillRectangle(brush, pixelX, pixelY, moduleSize, moduleSize);
                        }
                    }
                }
            }

            bitmap.Save(filename, ImageFormat.Png);
        }
    }
}

class QRCodeGenerator
{
    private const int SIZE = 21; // Version 1
    private static readonly int[] GALOIS_EXP = new int[256];
    private static readonly int[] GALOIS_LOG = new int[256];

    static QRCodeGenerator()
    {
        // Инициализация таблиц Галуа GF(256) для Reed-Solomon
        int x = 1;
        for (int i = 0; i < 255; i++)
        {
            GALOIS_EXP[i] = x;
            GALOIS_LOG[x] = i;
            x <<= 1;
            if (x >= 256)
                x ^= 0x11D; // Примитивный полином
        }
        GALOIS_EXP[255] = GALOIS_EXP[0];
    }

    public bool[,] CreateQRCode(string text)
    {
        // Шаг 1: Кодирование данных
        byte[] dataCodewords = EncodeData(text);

        // Шаг 2: Генерация кодов коррекции ошибок
        byte[] errorCodewords = GenerateErrorCorrectionCodewords(dataCodewords, 13);

        // Шаг 3: Создание матрицы и добавление паттернов
        bool[,] matrix = new bool[SIZE, SIZE];
        AddFunctionPatterns(matrix);

        // Шаг 4: Размещение данных
        byte[] allCodewords = CombineCodewords(dataCodewords, errorCodewords);
        PlaceCodewords(matrix, allCodewords);

        // Шаг 5: Применение маски и добавление format info
        int bestMask = FindBestMask(matrix);
        ApplyMask(matrix, bestMask);
        AddFormatInformation(matrix, 1, bestMask); // EC Level Q = 1

        return matrix;
    }

    private byte[] EncodeData(string text)
    {
        // Byte mode encoding
        byte[] textBytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(text);

        // Создаем битовый поток
        BitStream bits = new BitStream();

        // Mode indicator: 0100 (Byte mode)
        bits.AppendBits(4, 4);

        // Character count indicator: 8 bits для Version 1
        bits.AppendBits(textBytes.Length, 8);

        // Данные
        foreach (byte b in textBytes)
        {
            bits.AppendBits(b, 8);
        }

        // Terminator (до 4 нулевых бит)
        int terminatorLength = Math.Min(4, 104 - bits.Length);
        bits.AppendBits(0, terminatorLength);

        // Выравнивание до байта
        while (bits.Length % 8 != 0)
        {
            bits.AppendBit(false);
        }

        // Pad bytes: 11101100 и 00010001
        byte[] padBytes = { 0xEC, 0x11 };
        int padIndex = 0;
        while (bits.Length < 104)
        {
            bits.AppendBits(padBytes[padIndex % 2], 8);
            padIndex++;
        }

        return bits.ToByteArray();
    }

    private byte[] GenerateErrorCorrectionCodewords(byte[] data, int ecCount)
    {
        // Генерация генераторного полинома
        int[] generator = new int[ecCount + 1];
        generator[0] = 1;

        for (int i = 0; i < ecCount; i++)
        {
            for (int j = i + 1; j > 0; j--)
            {
                generator[j] = generator[j - 1];
                if (generator[j] != 0)
                {
                    generator[j] = GALOIS_EXP[(GALOIS_LOG[generator[j]] + i) % 255];
                }
            }
            generator[0] = GALOIS_EXP[i];

            for (int j = 1; j <= i + 1; j++)
            {
                generator[j] ^= generator[j - 1];
            }
        }

        // Полиномиальное деление
        int[] remainder = new int[ecCount];
        for (int i = 0; i < data.Length; i++)
        {
            int coef = data[i] ^ remainder[0];

            Array.Copy(remainder, 1, remainder, 0, ecCount - 1);
            remainder[ecCount - 1] = 0;

            if (coef != 0)
            {
                for (int j = 0; j < ecCount; j++)
                {
                    if (generator[j] != 0)
                    {
                        remainder[j] ^= GALOIS_EXP[(GALOIS_LOG[generator[j]] + GALOIS_LOG[coef]) % 255];
                    }
                }
            }
        }

        byte[] result = new byte[ecCount];
        for (int i = 0; i < ecCount; i++)
        {
            result[i] = (byte)remainder[i];
        }

        return result;
    }

    private byte[] CombineCodewords(byte[] data, byte[] ec)
    {
        byte[] result = new byte[data.Length + ec.Length];
        Array.Copy(data, 0, result, 0, data.Length);
        Array.Copy(ec, 0, result, data.Length, ec.Length);
        return result;
    }

    private void AddFunctionPatterns(bool[,] matrix)
    {
        // Добавление Finder Patterns
        AddFinderPattern(matrix, 0, 0);
        AddFinderPattern(matrix, 0, SIZE - 7);
        AddFinderPattern(matrix, SIZE - 7, 0);

        // Добавление Separators
        AddSeparators(matrix);

        // Добавление Timing Patterns
        for (int i = 8; i < SIZE - 8; i++)
        {
            matrix[6, i] = (i % 2 == 0);
            matrix[i, 6] = (i % 2 == 0);
        }

        // Dark module
        matrix[4 * 1 + 9, 8] = true;
    }

    private void AddFinderPattern(bool[,] matrix, int row, int col)
    {
        // 7x7 внешний черный квадрат
        for (int r = 0; r < 7; r++)
        {
            for (int c = 0; c < 7; c++)
            {
                bool isEdge = (r == 0 || r == 6 || c == 0 || c == 6);
                bool isCenter = (r >= 2 && r <= 4 && c >= 2 && c <= 4);
                matrix[row + r, col + c] = isEdge || isCenter;
            }
        }
    }

    private void AddSeparators(bool[,] matrix)
    {
        // Белые сепараторы вокруг finder patterns
        for (int i = 0; i < 8; i++)
        {
            matrix[7, i] = false;
            matrix[i, 7] = false;
            matrix[7, SIZE - 8 + i] = false;
            matrix[i, SIZE - 8] = false;
            matrix[SIZE - 8, i] = false;
            matrix[SIZE - 8 + i, 7] = false;
        }
    }

    private void PlaceCodewords(bool[,] matrix, byte[] codewords)
    {
        BitStream bits = new BitStream();
        foreach (byte b in codewords)
        {
            bits.AppendBits(b, 8);
        }

        int bitIndex = 0;
        bool upward = true;

        for (int col = SIZE - 1; col > 0; col -= 2)
        {
            if (col == 6) col--; // Пропуск timing column

            for (int row = 0; row < SIZE; row++)
            {
                int r = upward ? SIZE - 1 - row : row;

                for (int c = 0; c < 2; c++)
                {
                    int currentCol = col - c;

                    if (!IsFunctionModule(r, currentCol))
                    {
                        if (bitIndex < bits.Length)
                        {
                            matrix[r, currentCol] = bits.GetBit(bitIndex);
                            bitIndex++;
                        }
                    }
                }
            }

            upward = !upward;
        }
    }

    private bool IsFunctionModule(int row, int col)
    {
        // Finder patterns + separators
        if ((row < 9 && col < 9) || (row < 9 && col >= SIZE - 8) || (row >= SIZE - 8 && col < 9))
            return true;

        // Timing patterns
        if (row == 6 || col == 6)
            return true;

        // Format information areas
        if (row == 8 || col == 8)
            return true;

        return false;
    }

    private int FindBestMask(bool[,] matrix)
    {
        int bestMask = 0;
        int lowestPenalty = int.MaxValue;

        for (int mask = 0; mask < 8; mask++)
        {
            bool[,] testMatrix = (bool[,])matrix.Clone();
            ApplyMaskPattern(testMatrix, mask);

            int penalty = CalculatePenaltyScore(testMatrix);

            if (penalty < lowestPenalty)
            {
                lowestPenalty = penalty;
                bestMask = mask;
            }
        }

        return bestMask;
    }

    private void ApplyMask(bool[,] matrix, int maskPattern)
    {
        ApplyMaskPattern(matrix, maskPattern);
    }

    private void ApplyMaskPattern(bool[,] matrix, int pattern)
    {
        for (int row = 0; row < SIZE; row++)
        {
            for (int col = 0; col < SIZE; col++)
            {
                if (!IsFunctionModule(row, col))
                {
                    bool invert = GetMaskCondition(row, col, pattern);
                    if (invert)
                    {
                        matrix[row, col] = !matrix[row, col];
                    }
                }
            }
        }
    }

    private bool GetMaskCondition(int row, int col, int pattern)
    {
        switch (pattern)
        {
            case 0: return (row + col) % 2 == 0;
            case 1: return row % 2 == 0;
            case 2: return col % 3 == 0;
            case 3: return (row + col) % 3 == 0;
            case 4: return (row / 2 + col / 3) % 2 == 0;
            case 5: return (row * col) % 2 + (row * col) % 3 == 0;
            case 6: return ((row * col) % 2 + (row * col) % 3) % 2 == 0;
            case 7: return ((row + col) % 2 + (row * col) % 3) % 2 == 0;
            default: return false;
        }
    }

    private int CalculatePenaltyScore(bool[,] matrix)
    {
        int penalty = 0;

        // Rule 1: Consecutive modules in row/column
        penalty += CalculateRule1Penalty(matrix);

        // Rule 2: Block of modules in same color
        penalty += CalculateRule2Penalty(matrix);

        // Rule 3: Finder-like patterns
        penalty += CalculateRule3Penalty(matrix);

        // Rule 4: Balance of dark/light modules
        penalty += CalculateRule4Penalty(matrix);

        return penalty;
    }

    private int CalculateRule1Penalty(bool[,] matrix)
    {
        int penalty = 0;

        // Horizontal
        for (int row = 0; row < SIZE; row++)
        {
            int count = 1;
            bool last = matrix[row, 0];

            for (int col = 1; col < SIZE; col++)
            {
                if (matrix[row, col] == last)
                {
                    count++;
                }
                else
                {
                    if (count >= 5)
                        penalty += count - 2;
                    count = 1;
                    last = matrix[row, col];
                }
            }
            if (count >= 5)
                penalty += count - 2;
        }

        // Vertical
        for (int col = 0; col < SIZE; col++)
        {
            int count = 1;
            bool last = matrix[0, col];

            for (int row = 1; row < SIZE; row++)
            {
                if (matrix[row, col] == last)
                {
                    count++;
                }
                else
                {
                    if (count >= 5)
                        penalty += count - 2;
                    count = 1;
                    last = matrix[row, col];
                }
            }
            if (count >= 5)
                penalty += count - 2;
        }

        return penalty;
    }

    private int CalculateRule2Penalty(bool[,] matrix)
    {
        int penalty = 0;

        for (int row = 0; row < SIZE - 1; row++)
        {
            for (int col = 0; col < SIZE - 1; col++)
            {
                bool color = matrix[row, col];
                if (matrix[row, col + 1] == color &&
                    matrix[row + 1, col] == color &&
                    matrix[row + 1, col + 1] == color)
                {
                    penalty += 3;
                }
            }
        }

        return penalty;
    }

    private int CalculateRule3Penalty(bool[,] matrix)
    {
        int penalty = 0;
        bool[] pattern1 = { true, false, true, true, true, false, true, false, false, false, false };
        bool[] pattern2 = { false, false, false, false, true, false, true, true, true, false, true };

        // Horizontal
        for (int row = 0; row < SIZE; row++)
        {
            for (int col = 0; col <= SIZE - 11; col++)
            {
                bool match1 = true, match2 = true;
                for (int i = 0; i < 11; i++)
                {
                    if (matrix[row, col + i] != pattern1[i]) match1 = false;
                    if (matrix[row, col + i] != pattern2[i]) match2 = false;
                }
                if (match1 || match2)
                    penalty += 40;
            }
        }

        // Vertical
        for (int col = 0; col < SIZE; col++)
        {
            for (int row = 0; row <= SIZE - 11; row++)
            {
                bool match1 = true, match2 = true;
                for (int i = 0; i < 11; i++)
                {
                    if (matrix[row + i, col] != pattern1[i]) match1 = false;
                    if (matrix[row + i, col] != pattern2[i]) match2 = false;
                }
                if (match1 || match2)
                    penalty += 40;
            }
        }

        return penalty;
    }

    private int CalculateRule4Penalty(bool[,] matrix)
    {
        int darkCount = 0;
        for (int row = 0; row < SIZE; row++)
        {
            for (int col = 0; col < SIZE; col++)
            {
                if (matrix[row, col])
                    darkCount++;
            }
        }

        int totalModules = SIZE * SIZE;
        int percentage = (darkCount * 100) / totalModules;
        int deviation = Math.Abs(percentage - 50) / 5;

        return deviation * 10;
    }

    private void AddFormatInformation(bool[,] matrix, int ecLevel, int maskPattern)
    {
        // Format information: EC level (2 bits) + Mask pattern (3 bits)
        int formatBits = (ecLevel << 3) | maskPattern;

        // BCH error correction
        int bch = CalculateBCH(formatBits, 0x537);
        int formatInfo = (formatBits << 10) | bch;

        // XOR with mask pattern
        formatInfo ^= 0x5412;

        // Place format information (copy 1)
        for (int i = 0; i <= 5; i++)
        {
            matrix[8, i] = ((formatInfo >> i) & 1) == 1;
        }
        matrix[8, 7] = ((formatInfo >> 6) & 1) == 1;
        matrix[8, 8] = ((formatInfo >> 7) & 1) == 1;
        matrix[7, 8] = ((formatInfo >> 8) & 1) == 1;

        for (int i = 9; i <= 14; i++)
        {
            matrix[14 - i, 8] = ((formatInfo >> i) & 1) == 1;
        }

        // Place format information (copy 2)
        for (int i = 0; i <= 7; i++)
        {
            matrix[SIZE - 1 - i, 8] = ((formatInfo >> i) & 1) == 1;
        }

        for (int i = 8; i <= 14; i++)
        {
            matrix[8, SIZE - 15 + i] = ((formatInfo >> i) & 1) == 1;
        }
    }

    private int CalculateBCH(int data, int polynomial)
    {
        int shift = GetBitLength(polynomial) - 1;
        data <<= shift;

        while (GetBitLength(data) >= GetBitLength(polynomial))
        {
            data ^= polynomial << (GetBitLength(data) - GetBitLength(polynomial));
        }

        return data;
    }

    private int GetBitLength(int value)
    {
        int length = 0;
        while (value != 0)
        {
            length++;
            value >>= 1;
        }
        return length;
    }
}

class BitStream
{
    private bool[] bits = new bool[256];
    public int Length { get; private set; }

    public void AppendBit(bool bit)
    {
        if (Length >= bits.Length)
        {
            Array.Resize(ref bits, bits.Length * 2);
        }
        bits[Length++] = bit;
    }

    public void AppendBits(int value, int count)
    {
        for (int i = count - 1; i >= 0; i--)
        {
            AppendBit(((value >> i) & 1) == 1);
        }
    }

    public bool GetBit(int index)
    {
        return bits[index];
    }

    public byte[] ToByteArray()
    {
        int byteCount = (Length + 7) / 8;
        byte[] result = new byte[byteCount];

        for (int i = 0; i < Length; i++)
        {
            if (bits[i])
            {
                result[i / 8] |= (byte)(1 << (7 - (i % 8)));
            }
        }

        return result;
    }
}