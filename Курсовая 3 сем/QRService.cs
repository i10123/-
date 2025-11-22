using QRCoder;
using System.Drawing;

namespace QR_Generator
{
    public static class QRService
    {
        // Добавили аргумент logo
        public static Bitmap? GenerateQR(string data, string colorHex, int size, QRCodeGenerator.ECCLevel eccLevel, Bitmap? logo = null)
        {
            if (string.IsNullOrWhiteSpace(data)) // Проверка на пустоту тут тоже важна
                return null;

            try
            {
                var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(data, eccLevel);
                var qrCode = new QRCode(qrCodeData);

                Color darkColor = ColorTranslator.FromHtml(colorHex);

                // Если есть логотип, используем перегрузку метода GetGraphic с лого
                if (logo != null)
                {
                    // 15 - это процент размера логотипа, 6 - толщина рамки, true - рисовать рамку
                    return qrCode.GetGraphic(size / 10, darkColor, Color.White, logo, 15, 6, true);
                }
                else
                {
                    return qrCode.GetGraphic(size / 10, darkColor, Color.White, true);
                }
            }
            catch
            {
                return null; // Если вдруг крашнулось внутри библиотеки
            }
        }

        public static QRCodeGenerator.ECCLevel GetLvlCorrectError(string? level)
        {
            if (string.IsNullOrEmpty(level))
                return QRCodeGenerator.ECCLevel.M;
            return level switch
            {
                "L" => QRCodeGenerator.ECCLevel.L,
                "Q" => QRCodeGenerator.ECCLevel.Q,
                "H" => QRCodeGenerator.ECCLevel.H,
                _ => QRCodeGenerator.ECCLevel.M,
            };
        }
    }
}