using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace QRCodeGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("=== QR Code Generator Version 1, Error Correction Level Q ===\n");
            Console.Write("Введите текст для кодирования (до 16 символов): ");
            string text = Console.ReadLine();

            if (string.IsNullOrEmpty(text))
            {
                Console.WriteLine("Ошибка: Текст не может быть пустым!");
                return;
            }

            if (text.Length > 16)
            {
                Console.WriteLine($"Ошибка: Максимальная длина для версии 1 уровня Q - 16 символов. Вы ввели {text.Length}.");
                return;
            }

            try
            {
                var generator = new QRCodeV1Generator();
                bool[,] qrMatrix = generator.Generate(text);
                
                string filename = $"qrcode_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                SaveQRCodeToPNG(qrMatrix, filename, 10, 4);
                
                Console.WriteLine($"\n✓ QR-код успешно сохранён в файл: {filename}");
                Console.WriteLine($"  Размер: {qrMatrix.GetLength(0)}x{qrMatrix.GetLength(1)} модулей");
                Console.WriteLine($"  Данные: {text}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✗ Ошибка: {ex.Message}");
                Console.WriteLine($"  {ex.StackTrace}");
            }

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        static void SaveQRCodeToPNG(bool[,] matrix, string filename, int pixelSize, int border)
        {
            int size = matrix.GetLength(0);
            int imgSize = (size + border * 2) * pixelSize;

            using (Bitmap bmp = new Bitmap(imgSize, imgSize))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // Высокое качество рендеринга
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                
                // Белый фон
                g.Clear(Color.White);

                // Рисуем модули
                using (SolidBrush blackBrush = new SolidBrush(Color.Black))
                {
                    for (int y = 0; y < size; y++)
                    {
                        for (int x = 0; x < size; x++)
                        {
                            if (matrix[y, x])
                            {
                                int px = (x + border) * pixelSize;
                                int py = (y + border) * pixelSize;
                                g.FillRectangle(blackBrush, px, py, pixelSize, pixelSize);
                            }
                        }
                    }
                }

                bmp.Save(filename, ImageFormat.Png);
            }
        }
    }

    class QRCodeV1Generator
    {
        private const int SIZE = 21; // Версия 1 всегда 21x21
        private const int VERSION = 1;
        private const int EC_LEVEL = 1; // 0=L, 1=M, 2=Q, 3=H
        
        // Для версии 1 уровня Q: 13 байт данных, 13 байт коррекции
        private const int DATA_CAPACITY = 13; 
        private const int EC_CODEWORDS = 13;
        private const int TOTAL_CODEWORDS = 26;

        // Таблицы Галуа GF(256)
        private static readonly int[] GF256_EXP = new int[256];
        private static readonly int[] GF256_LOG = new int[256];

        static QRCodeV1Generator()
        {
            InitializeGaloisField();
        }

        private static void InitializeGaloisField()
        {
            int x = 1;
            for (int i = 0; i < 255; i++)
            {
                GF256_EXP[i] = x;
                GF256_LOG[x] = i;
                x *= 2;
                if (x > 255)
                    x ^= 0x11D; // Примитивный полином для QR
            }
            GF256_EXP[255] = GF256_EXP[0];
        }

        public bool[,] Generate(string text)
        {
            Console.WriteLine("\n[1/8] Кодирование данных...");
            byte[] dataBytes = EncodeData(text);
            
            Console.WriteLine("[2/8] Генерация кодов коррекции ошибок Reed-Solomon...");
            byte[] ecBytes = GenerateErrorCorrection(dataBytes);
            
            Console.WriteLine("[3/8] Создание матрицы QR-кода...");
            bool[,] matrix = new bool[SIZE, SIZE];
            
            Console.WriteLine("[4/8] Добавление функциональных шаблонов...");
            AddFunctionPatterns(matrix);
            
            Console.WriteLine("[5/8] Размещение данных...");
            byte[] fullData = CombineDataAndEC(dataBytes, ecBytes);
            PlaceDataBits(matrix, fullData);
            
            Console.WriteLine("[6/8] Выбор оптимальной маски...");
            int bestMask = SelectBestMask(matrix);
            
            Console.WriteLine($"[7/8] Применение маски {bestMask}...");
            ApplyMask(matrix, bestMask);
            
            Console.WriteLine("[8/8] Добавление информации о формате...");
            AddFormatInformation(matrix, bestMask);
            
            return matrix;
        }

        private byte[] EncodeData(string text)
        {
            List<bool> bits = new List<bool>();
            
            // Mode Indicator: Byte mode = 0100
            AddBits(bits, 4, 4);
            
            // Character Count Indicator: 8 бит для версии 1 в режиме Byte
            AddBits(bits, text.Length, 8);
            
            // Данные в UTF-8
            byte[] textBytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(text);
            foreach (byte b in textBytes)
            {
                AddBits(bits, b, 8);
            }
            
            // Terminator (до 4 нулей)
            int capacity = DATA_CAPACITY * 8;
            int terminatorLength = Math.Min(4, capacity - bits.Count);
            for (int i = 0; i < terminatorLength; i++)
            {
                bits.Add(false);
            }
            
            // Выравнивание до байта
            while (bits.Count % 8 != 0)
            {
                bits.Add(false);
            }
            
            // Заполнение pad bytes: 11101100 и 00010001
            int padIndex = 0;
            while (bits.Count < capacity)
            {
                byte padByte = (padIndex % 2 == 0) ? (byte)0xEC : (byte)0x11;
                AddBits(bits, padByte, 8);
                padIndex++;
            }
            
            // Конвертация в байты
            byte[] result = new byte[DATA_CAPACITY];
            for (int i = 0; i < DATA_CAPACITY; i++)
            {
                byte b = 0;
                for (int j = 0; j < 8; j++)
                {
                    if (bits[i * 8 + j])
                    {
                        b |= (byte)(1 << (7 - j));
                    }
                }
                result[i] = b;
            }
            
            return result;
        }

        private void AddBits(List<bool> bits, int value, int bitCount)
        {
            for (int i = bitCount - 1; i >= 0; i--)
            {
                bits.Add(((value >> i) & 1) == 1);
            }
        }

        private byte[] GenerateErrorCorrection(byte[] data)
        {
            // Генераторный полином для 13 EC кодов
            byte[] generator = GenerateGeneratorPolynomial(EC_CODEWORDS);
            
            // Создаём рабочий массив
            byte[] result = new byte[EC_CODEWORDS];
            Array.Copy(data, result, Math.Min(data.Length, EC_CODEWORDS));
            
            // Полиномиальное деление
            for (int i = 0; i < data.Length; i++)
            {
                byte coefficient = result[0];
                
                // Сдвиг
                Array.Copy(result, 1, result, 0, EC_CODEWORDS - 1);
                result[EC_CODEWORDS - 1] = 0;
                
                // Добавляем следующий байт данных
                if (i + EC_CODEWORDS < data.Length)
                {
                    result[EC_CODEWORDS - 1] = data[i + EC_CODEWORDS];
                }
                
                // XOR с генераторным полиномом
                if (coefficient != 0)
                {
                    for (int j = 0; j < EC_CODEWORDS; j++)
                    {
                        if (generator[j] != 0)
                        {
                            result[j] ^= GFMultiply(generator[j], coefficient);
                        }
                    }
                }
            }
            
            return result;
        }

        private byte[] GenerateGeneratorPolynomial(int degree)
        {
            byte[] poly = new byte[degree + 1];
            poly[0] = 1;
            
            for (int i = 0; i < degree; i++)
            {
                // Умножаем на (x - α^i)
                for (int j = i + 1; j > 0; j--)
                {
                    poly[j] = GFMultiply(poly[j], (byte)GF256_EXP[i]);
                    if (j > 0)
                    {
                        poly[j] ^= poly[j - 1];
                    }
                }
            }
            
            return poly;
        }

        private byte GFMultiply(byte a, byte b)
        {
            if (a == 0 || b == 0)
                return 0;
            
            int logA = GF256_LOG[a];
            int logB = GF256_LOG[b];
            int logResult = (logA + logB) % 255;
            
            return (byte)GF256_EXP[logResult];
        }

        private byte[] CombineDataAndEC(byte[] data, byte[] ec)
        {
            byte[] result = new byte[data.Length + ec.Length];
            Array.Copy(data, 0, result, 0, data.Length);
            Array.Copy(ec, 0, result, data.Length, ec.Length);
            return result;
        }

        private void AddFunctionPatterns(bool[,] matrix)
        {
            // Finder patterns в трёх углах
            AddFinderPattern(matrix, 0, 0);
            AddFinderPattern(matrix, 0, SIZE - 7);
            AddFinderPattern(matrix, SIZE - 7, 0);
            
            // Separators
            AddSeparators(matrix);
            
            // Timing patterns
            for (int i = 8; i < SIZE - 8; i++)
            {
                matrix[6, i] = (i % 2 == 0);
                matrix[i, 6] = (i % 2 == 0);
            }
            
            // Dark module (всегда чёрный в позиции (4*version + 9, 8))
            matrix[4 * VERSION + 9, 8] = true;
        }

        private void AddFinderPattern(bool[,] matrix, int row, int col)
        {
            // 7x7 внешний чёрный квадрат
            for (int r = 0; r < 7; r++)
            {
                for (int c = 0; c < 7; c++)
                {
                    if (r == 0 || r == 6 || c == 0 || c == 6)
                    {
                        matrix[row + r, col + c] = true;
                    }
                }
            }
            
            // 5x5 белый квадрат (внутри уже false по умолчанию)
            
            // 3x3 центральный чёрный квадрат
            for (int r = 2; r < 5; r++)
            {
                for (int c = 2; c < 5; c++)
                {
                    matrix[row + r, col + c] = true;
                }
            }
        }

        private void AddSeparators(bool[,] matrix)
        {
            // Белые полосы 1 пиксель вокруг finder patterns
            // Верхний левый
            for (int i = 0; i < 8; i++)
            {
                matrix[7, i] = false;
                matrix[i, 7] = false;
            }
            
            // Верхний правый
            for (int i = 0; i < 8; i++)
            {
                matrix[7, SIZE - 8 + i] = false;
                matrix[i, SIZE - 8] = false;
            }
            
            // Нижний левый
            for (int i = 0; i < 8; i++)
            {
                matrix[SIZE - 8, i] = false;
                matrix[SIZE - 8 + i, 7] = false;
            }
        }

        private void PlaceDataBits(bool[,] matrix, byte[] data)
        {
            // Конвертация в биты
            List<bool> bits = new List<bool>();
            foreach (byte b in data)
            {
                for (int i = 7; i >= 0; i--)
                {
                    bits.Add(((b >> i) & 1) == 1);
                }
            }
            
            int bitIndex = 0;
            bool upward = true;
            
            // Размещение справа налево, двумя колонками за раз
            for (int col = SIZE - 1; col > 0; col -= 2)
            {
                // Пропуск вертикального timing pattern
                if (col == 6)
                    col = 5;
                
                for (int i = 0; i < SIZE; i++)
                {
                    int row = upward ? SIZE - 1 - i : i;
                    
                    // Два модуля в текущей паре колонок
                    for (int c = 0; c < 2; c++)
                    {
                        int currentCol = col - c;
                        
                        if (!IsFunctionModule(row, currentCol))
                        {
                            if (bitIndex < bits.Count)
                            {
                                matrix[row, currentCol] = bits[bitIndex];
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
            if ((row < 9 && col < 9) || 
                (row < 9 && col >= SIZE - 8) || 
                (row >= SIZE - 8 && col < 9))
            {
                return true;
            }
            
            // Timing patterns
            if (row == 6 || col == 6)
            {
                return true;
            }
            
            // Dark module и format information
            if (row == 8 || col == 8)
            {
                return true;
            }
            
            return false;
        }

        private int SelectBestMask(bool[,] matrix)
        {
            int bestMask = 0;
            int lowestPenalty = int.MaxValue;
            
            for (int mask = 0; mask < 8; mask++)
            {
                bool[,] testMatrix = (bool[,])matrix.Clone();
                ApplyMask(testMatrix, mask);
                
                int penalty = CalculateMaskPenalty(testMatrix);
                
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
            for (int row = 0; row < SIZE; row++)
            {
                for (int col = 0; col < SIZE; col++)
                {
                    if (!IsFunctionModule(row, col))
                    {
                        if (GetMaskCondition(row, col, maskPattern))
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

        private int CalculateMaskPenalty(bool[,] matrix)
        {
            int penalty = 0;
            
            // Penalty 1: Пять и более последовательных модулей одного цвета
            penalty += CalculatePenalty1(matrix);
            
            // Penalty 2: Блоки 2x2 одного цвета
            penalty += CalculatePenalty2(matrix);
            
            // Penalty 3: Паттерны похожие на finder pattern
            penalty += CalculatePenalty3(matrix);
            
            // Penalty 4: Баланс тёмных/светлых модулей
            penalty += CalculatePenalty4(matrix);
            
            return penalty;
        }

        private int CalculatePenalty1(bool[,] matrix)
        {
            int penalty = 0;
            
            // Горизонтально
            for (int row = 0; row < SIZE; row++)
            {
                int count = 1;
                bool lastColor = matrix[row, 0];
                
                for (int col = 1; col < SIZE; col++)
                {
                    if (matrix[row, col] == lastColor)
                    {
                        count++;
                    }
                    else
                    {
                        if (count >= 5)
                        {
                            penalty += count - 2;
                        }
                        count = 1;
                        lastColor = matrix[row, col];
                    }
                }
                
                if (count >= 5)
                {
                    penalty += count - 2;
                }
            }
            
            // Вертикально
            for (int col = 0; col < SIZE; col++)
            {
                int count = 1;
                bool lastColor = matrix[0, col];
                
                for (int row = 1; row < SIZE; row++)
                {
                    if (matrix[row, col] == lastColor)
                    {
                        count++;
                    }
                    else
                    {
                        if (count >= 5)
                        {
                            penalty += count - 2;
                        }
                        count = 1;
                        lastColor = matrix[row, col];
                    }
                }
                
                if (count >= 5)
                {
                    penalty += count - 2;
                }
            }
            
            return penalty;
        }

        private int CalculatePenalty2(bool[,] matrix)
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

        private int CalculatePenalty3(bool[,] matrix)
        {
            int penalty = 0;
            bool[] pattern1 = { true, false, true, true, true, false, true, false, false, false, false };
            bool[] pattern2 = { false, false, false, false, true, false, true, true, true, false, true };
            
            // Горизонтально
            for (int row = 0; row < SIZE; row++)
            {
                for (int col = 0; col <= SIZE - 11; col++)
                {
                    if (MatchesPattern(matrix, row, col, pattern1, true) ||
                        MatchesPattern(matrix, row, col, pattern2, true))
                    {
                        penalty += 40;
                    }
                }
            }
            
            // Вертикально
            for (int col = 0; col < SIZE; col++)
            {
                for (int row = 0; row <= SIZE - 11; row++)
                {
                    if (MatchesPattern(matrix, row, col, pattern1, false) ||
                        MatchesPattern(matrix, row, col, pattern2, false))
                    {
                        penalty += 40;
                    }
                }
            }
            
            return penalty;
        }

        private bool MatchesPattern(bool[,] matrix, int row, int col, bool[] pattern, bool horizontal)
        {
            for (int i = 0; i < pattern.Length; i++)
            {
                bool value = horizontal ? matrix[row, col + i] : matrix[row + i, col];
                if (value != pattern[i])
                {
                    return false;
                }
            }
            return true;
        }

        private int CalculatePenalty4(bool[,] matrix)
        {
            int darkCount = 0;
            int totalCount = SIZE * SIZE;
            
            for (int row = 0; row < SIZE; row++)
            {
                for (int col = 0; col < SIZE; col++)
                {
                    if (matrix[row, col])
                    {
                        darkCount++;
                    }
                }
            }
            
            int percent = (darkCount * 100) / totalCount;
            int deviation = Math.Abs(percent - 50) / 5;
            
            return deviation * 10;
        }

        private void AddFormatInformation(bool[,] matrix, int maskPattern)
        {
            // Format information bits для EC level Q (01) и mask pattern
            int formatInfo = (EC_LEVEL << 3) | maskPattern;
            
            // BCH код для коррекции ошибок format information
            int bchCode = CalculateBCH(formatInfo, 0x537); // Генераторный полином
            int fullFormat = (formatInfo << 10) | bchCode;
            
            // XOR с маской 101010000010010
            fullFormat ^= 0x5412;
            
            // Размещение format information
            for (int i = 0; i < 15; i++)
            {
                bool bit = ((fullFormat >> i) & 1) == 1;
                
                // Копия 1 (вокруг верхнего левого finder pattern)
                if (i < 6)
                {
                    matrix[8, i] = bit;
                }
                else if (i < 8)
                {
                    matrix[8, i + 1] = bit;
                }
                else if (i < 9)
                {
                    matrix[7, 8] = bit;
                }
                else
                {
                    matrix[14 - i, 8] = bit;
                }
                
                // Копия 2 (вдоль правой и нижней стороны)
                if (i < 8)
                {
                    matrix[SIZE - 1 - i, 8] = bit;
                }
                else
                {
                    matrix[8, SIZE - 15 + i] = bit;
                }
            }
        }

        private int CalculateBCH(int data, int generator)
        {
            int digits = 0;
            int temp = generator;
            while (temp != 0)
            {
                digits++;
                temp >>= 1;
            }
            
            data <<= (digits - 1);
            
            while (GetBitLength(data) >= digits)
            {
                data ^= generator << (GetBitLength(data) - digits);
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
}
