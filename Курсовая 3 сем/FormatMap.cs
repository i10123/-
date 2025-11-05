using QR_generator;
using System.Drawing.Imaging;

namespace QR_Generator
{
    public static class FormatMap
    {
        public static readonly Dictionary<int, ImageFormatInfo> Formats = new()
        {
            { 0, new ImageFormatInfo { Extension = "png", Format = ImageFormat.Png, FilterText = "PNG|*.png", DisplayName = "PNG" } },
            { 1, new ImageFormatInfo { Extension = "jpg", Format = ImageFormat.Jpeg, FilterText = "JPEG|*.jpg", DisplayName = "JPEG" } },
            { 2, new ImageFormatInfo { Extension = "bmp", Format = ImageFormat.Bmp, FilterText = "BMP|*.bmp", DisplayName = "BMP" } },
            { 3, new ImageFormatInfo { Extension = "gif", Format = ImageFormat.Gif, FilterText = "GIF|*.gif", DisplayName = "GIF" } },
            { 4, new ImageFormatInfo { Extension = "tiff", Format = ImageFormat.Tiff, FilterText = "TIFF|*.tiff", DisplayName = "TIFF" } },
            { 5, new ImageFormatInfo { Extension = "txt", Format = null, FilterText = "Text|*.txt", DisplayName = "TXT" } },
            { 6, new ImageFormatInfo { Extension = "bin", Format = null, FilterText = "Binary|*.bin", DisplayName = "BIN" } },
        };
    }
}
