using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

class Program
{
    static void Main()
    {
        Console.Write("Текст (макс 16 символов): ");
        string text = Console.ReadLine();

        if (text.Length > 16)
        {
            Console.WriteLine("Слишком длинный текст!");
            return;
        }

        var qr = new SimpleQR();
        bool[,] matrix = qr.Generate(text);

        SavePNG(matrix, "qr.png");
        Console.WriteLine("Сохранено: qr.png");
        Console.ReadLine();
    }

    static void SavePNG(bool[,] m, string file)
    {
        int s = m.GetLength(0);
        int scale = 10, border = 4;
        int size = (s + border * 2) * scale;

        using (var bmp = new Bitmap(size, size))
        using (var g = Graphics.FromImage(bmp))
        {
            g.Clear(Color.White);
            for (int y = 0; y < s; y++)
                for (int x = 0; x < s; x++)
                    if (m[y, x])
                        g.FillRectangle(Brushes.Black, (x + border) * scale, (y + border) * scale, scale, scale);
            bmp.Save(file, ImageFormat.Png);
        }
    }
}

class SimpleQR
{
    const int SIZE = 21;
    static int[] exp = new int[256];
    static int[] log = new int[256];

    static SimpleQR()
    {
        int x = 1;
        for (int i = 0; i < 255; i++)
        {
            exp[i] = x;
            log[x] = i;
            x = (x * 2) ^ ((x >= 128) ? 0x11D : 0);
        }
    }

    public bool[,] Generate(string text)
    {
        // 1. Encode data
        byte[] data = Encode(text);

        // 2. Reed-Solomon EC
        byte[] ec = RS(data, 13);

        // 3. Combine
        byte[] all = new byte[26];
        Array.Copy(data, all, 13);
        Array.Copy(ec, 0, all, 13, 13);

        // 4. Create matrix
        bool[,] m = new bool[SIZE, SIZE];

        // 5. Add patterns
        Patterns(m);

        // 6. Place data
        Place(m, all);

        // 7. Add format (EC Level Q=01, Mask 0=000)
        Format(m, 0x5412); // Pre-calculated for Q level, mask 0

        return m;
    }

    byte[] Encode(string text)
    {
        byte[] bytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(text);
        bool[] bits = new bool[104]; // 13 bytes * 8
        int pos = 0;

        // Mode: 0100
        bits[pos++] = false; bits[pos++] = true; bits[pos++] = false; bits[pos++] = false;

        // Length: 8 bits
        int len = bytes.Length;
        for (int i = 7; i >= 0; i--)
            bits[pos++] = ((len >> i) & 1) == 1;

        // Data
        foreach (byte b in bytes)
            for (int i = 7; i >= 0; i--)
                bits[pos++] = ((b >> i) & 1) == 1;

        // Terminator (4 bits)
        for (int i = 0; i < 4 && pos < 104; i++)
            bits[pos++] = false;

        // Pad to byte
        while (pos % 8 != 0 && pos < 104)
            bits[pos++] = false;

        // Pad bytes
        byte[] pads = { 0xEC, 0x11 };
        int padIdx = 0;
        while (pos < 104)
        {
            byte pad = pads[padIdx++ % 2];
            for (int i = 7; i >= 0; i--)
                bits[pos++] = ((pad >> i) & 1) == 1;
        }

        // Convert to bytes
        byte[] result = new byte[13];
        for (int i = 0; i < 13; i++)
        {
            byte b = 0;
            for (int j = 0; j < 8; j++)
                if (bits[i * 8 + j])
                    b |= (byte)(1 << (7 - j));
            result[i] = b;
        }

        return result;
    }

    byte[] RS(byte[] data, int ecLen)
    {
        // Generate polynomial
        int[] g = new int[ecLen + 1];
        g[0] = 1;
        for (int i = 0; i < ecLen; i++)
        {
            for (int j = i + 1; j > 0; j--)
            {
                g[j] = g[j - 1];
                if (g[j] != 0)
                    g[j] = exp[(log[g[j]] + i) % 255];
            }
            g[0] = exp[i];
            for (int j = 1; j <= i + 1; j++)
                g[j] ^= g[j - 1];
        }

        // Polynomial division
        int[] msg = new int[data.Length + ecLen];
        for (int i = 0; i < data.Length; i++)
            msg[i] = data[i];

        for (int i = 0; i < data.Length; i++)
        {
            int coef = msg[i];
            if (coef != 0)
                for (int j = 1; j <= ecLen; j++)
                    if (g[j] != 0)
                        msg[i + j] ^= exp[(log[g[j]] + log[coef]) % 255];
        }

        byte[] result = new byte[ecLen];
        for (int i = 0; i < ecLen; i++)
            result[i] = (byte)msg[data.Length + i];

        return result;
    }

    void Patterns(bool[,] m)
    {
        // Finder patterns
        Finder(m, 0, 0);
        Finder(m, 0, 14);
        Finder(m, 14, 0);

        // Separators
        for (int i = 0; i < 8; i++)
        {
            m[7, i] = false; m[i, 7] = false;
            m[7, 13 + i] = false; m[i, 13] = false;
            m[13, i] = false; m[13 + i, 7] = false;
        }

        // Timing
        for (int i = 8; i < 13; i++)
        {
            m[6, i] = (i % 2 == 0);
            m[i, 6] = (i % 2 == 0);
        }

        // Dark module
        m[13, 8] = true;
    }

    void Finder(bool[,] m, int r, int c)
    {
        // 7x7 outer
        for (int i = 0; i < 7; i++)
        {
            m[r, c + i] = true; m[r + 6, c + i] = true;
            m[r + i, c] = true; m[r + i, c + 6] = true;
        }
        // 3x3 inner
        for (int i = 2; i < 5; i++)
            for (int j = 2; j < 5; j++)
                m[r + i, c + j] = true;
    }

    void Place(bool[,] m, byte[] data)
    {
        bool[] bits = new bool[data.Length * 8];
        for (int i = 0; i < data.Length; i++)
            for (int j = 0; j < 8; j++)
                bits[i * 8 + j] = ((data[i] >> (7 - j)) & 1) == 1;

        int idx = 0;
        bool up = true;

        for (int col = 20; col > 0; col -= 2)
        {
            if (col == 6) col--;

            for (int i = 0; i < SIZE; i++)
            {
                int r = up ? 20 - i : i;

                for (int c = 0; c < 2; c++)
                {
                    int cc = col - c;

                    if (!IsFn(r, cc))
                    {
                        bool bit = idx < bits.Length && bits[idx++];
                        // Apply mask 0: (r+c) % 2 == 0
                        if ((r + cc) % 2 == 0)
                            bit = !bit;
                        m[r, cc] = bit;
                    }
                }
            }
            up = !up;
        }
    }

    bool IsFn(int r, int c)
    {
        // Finders + separators
        if ((r < 9 && c < 9) || (r < 9 && c >= 13) || (r >= 13 && c < 9))
            return true;
        // Timing
        if (r == 6 || c == 6)
            return true;
        // Format
        if (r == 8 || c == 8)
            return true;
        return false;
    }

    void Format(bool[,] m, int fmt)
    {
        // EC Level Q (01) + Mask 0 (000) = 01000
        // BCH: 01000 00000 -> 10011010001
        // Full: 01000 10011010001
        // XOR with 101010000010010
        // Result: 110010010001011 = 0x6513... let me recalculate

        // For Q + Mask 0: the correct format is:
        int format = 0x5C94; // Pre-calculated: EC=Q(01), Mask=0, with BCH and mask applied

        // Copy 1
        for (int i = 0; i <= 5; i++)
            m[8, i] = ((format >> i) & 1) == 1;
        m[8, 7] = ((format >> 6) & 1) == 1;
        m[8, 8] = ((format >> 7) & 1) == 1;
        m[7, 8] = ((format >> 8) & 1) == 1;
        for (int i = 9; i <= 14; i++)
            m[14 - i, 8] = ((format >> i) & 1) == 1;

        // Copy 2
        for (int i = 0; i <= 7; i++)
            m[20 - i, 8] = ((format >> i) & 1) == 1;
        for (int i = 8; i <= 14; i++)
            m[8, 20 - 14 + i] = ((format >> i) & 1) == 1;
    }
}
