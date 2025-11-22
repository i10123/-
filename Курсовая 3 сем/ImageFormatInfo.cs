using System.Drawing.Imaging;

namespace QR_generator
{
    public class ImageFormatInfo { 
        public string Extension { get; set; } = "png"; 
        public ImageFormat? Format { get; set; } = ImageFormat.Png; 
        public string FilterText { get; set; } = "PNG|*.png"; 
        public string DisplayName { get; set; } = "PNG"; 
    }
}
