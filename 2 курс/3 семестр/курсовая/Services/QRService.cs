using QRCoder;
using System.Drawing;

namespace QR_generator.Services
{
    public static class QRService
    {
        public static Bitmap? GenerateQR(string data, string colorHex, int size, QRCodeGenerator.ECCLevel eccLevel, Bitmap? logo = null)
        {
            if (string.IsNullOrWhiteSpace(data))
                return null;

            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(data, eccLevel);
            using var qrCode = new QRCode(qrCodeData);

            Color darkColor = ColorTranslator.FromHtml(colorHex);

            int modulesCount = qrCodeData.ModuleMatrix.Count;
            int pixelsPerModule = size / modulesCount;

            if (pixelsPerModule < 1) 
                pixelsPerModule = 1;

            if (logo != null)
                return qrCode.GetGraphic(pixelsPerModule, darkColor, Color.White, logo, 15, 6, true); // 15 - процент размера, 6 - толщина рамки
            else
                return qrCode.GetGraphic(pixelsPerModule, darkColor, Color.White, true);
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

        public static string GetMatrixString(string data, QRCodeGenerator.ECCLevel eccLevel)
        {
            if (string.IsNullOrWhiteSpace(data)) 
                return "";

            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(data, eccLevel);

            var matrix = qrCodeData.ModuleMatrix;
            var sb = new System.Text.StringBuilder();

            for (int i = 0; i < matrix.Count; i++)
            {
                for (int j = 0; j < matrix[i].Count; j++)
                    sb.Append(matrix[i][j] ? "1" : "0");
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}