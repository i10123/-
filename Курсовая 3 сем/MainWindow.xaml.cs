using Microsoft.Win32;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using SWM = System.Windows.Media;

namespace QR_Generator
{
    public partial class MainWindow : Window
    {
        /*Уровень коррекции ошибок — это параметр, который определяет, насколько QR-код устойчив к повреждениям. 
         *Чем выше уровень, тем больше данных можно восстановить при сканировании, но тем меньше информации можно закодировать. 
         *Я использую уровень M по умолчанию как компромисс между надёжностью и вместимостью.*/
        private string? lvl_correction_error = "M";
        private string QR_color = "#000000";
        private int size_QR = 300;
        private Bitmap? QR_bitmap;
        private ImageFormatInfo selectedFormat = new(); // PNG по умолчанию

        public MainWindow()
        {
            InitializeComponent(); // Этот метод инициализирует компоненты окна, связывая XAML-разметку с логикой C#. Без него элементы интерфейса не будут работать.
            // начальные стили для кнопок (M активна)
            btn_M.Background = new SolidColorBrush(Colors.Red);

            // Выделение кнопки чёрного цвета
            foreach (var button in stackPanel_colors.Children.OfType<Button>())
            {
                button.BorderThickness = new Thickness(0);
                button.BorderBrush = SWM.Brushes.Transparent;
            }
            btnColorBlack.BorderThickness = new Thickness(3);
            btnColorBlack.BorderBrush = SWM.Brushes.WhiteSmoke;

            foreach (var format in formatMap.OrderBy(kvp => kvp.Key))
            {
                cmbFormat.Items.Add(format.Value.DisplayName);
            }
            cmbFormat.SelectedIndex = 0; // по умолчанию PNG

        }

        private void SetLevelCorrection_Click(object clicked_btn, RoutedEventArgs e)
        {
            if (clicked_btn is not Button btn)
                return;

            lvl_correction_error = btn.Tag?.ToString() ?? "";

            // Сброс фона у всех кнопок коррекции
            foreach (var button in stackPanel_correction.Children.OfType<Button>())
            {
                button.Background = SWM.Brushes.Black;
            }

            // Выделение активной кнопки
            btn.Background = new SolidColorBrush(Colors.Red);
        }

        private void SetColor_Click(object clicked_btn, RoutedEventArgs e)
        {
            if (clicked_btn is not Button btn)
                return;

            QR_color = btn.Tag?.ToString() ?? "";

            // Сброс рамок у всех цветовых кнопок
            foreach (var button in stackPanel_colors.Children.OfType<Button>())
            {
                button.BorderThickness = new Thickness(0);
                button.BorderBrush = SWM.Brushes.Transparent;
            }

            // Установка рамки для выбранной кнопки
            btn.BorderThickness = new Thickness(3);
            btn.BorderBrush = SWM.Brushes.WhiteSmoke;

            GenerateQR();
        }

        // слайдер для размера
        private void SldSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // округление до ближайшего шага 50
            int step = 50;
            int roundedValue = (int)(Math.Round(sldSize.Value / step) * step);
            if (sldSize.Value != roundedValue)
                sldSize.Value = roundedValue;

            txtSizeLabel.Text = $"Размер: {roundedValue} px";
        }

        private void Track_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Track track)
            {
                var slider = sldSize;
                System.Windows.Point pt = e.GetPosition(track);

                double ratio = pt.X / track.ActualWidth;
                double newValue = slider.Minimum + ratio * (slider.Maximum - slider.Minimum);

                // округление до ближайшего шага 50
                int step = 50;
                newValue = Math.Round(newValue / step) * step;

                slider.Value = newValue;
                e.Handled = true;
            }
        }

        private void Generate_Click(object clicked_btn, RoutedEventArgs e)
        {
            GenerateQR();
        }

        private void GenerateQR()
        {
            string data = GetQRData();
            if (string.IsNullOrEmpty(data))
                return;

            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(data, GetLvlCorrectError());
            var qrCode = new QRCode(qrCodeData);
            QR_bitmap = qrCode.GetGraphic(size_QR / 10, ColorTranslator.FromHtml(QR_color), System.Drawing.Color.White, true);

            imgPreview.Source = BitmapToImageSource(QR_bitmap);
            btnDownload.IsEnabled = true;
        }

        private string GetQRData()
        {
            switch (tabControl.SelectedIndex)
            {
                case 0:
                    return txtText.Text;
                case 1:
                    return txtUrl.Text;
                case 2:
                    return $"Почта:{txtEmail.Text}";
                case 3:
                    return $"Телефон:{txtPhone.Text}";
                case 4:
                    return $"WIFI:T:{cmbWifiType.SelectedItem};S:{txtSsid.Text};P:{txtPassword.Text};;";
                case 5:
                    return $"Геолокация:{txtWidth.Text},{txtLong.Text}";
                default:
                    return "";
            }
        }

        private QRCodeGenerator.ECCLevel GetLvlCorrectError()
        {
            if (string.IsNullOrEmpty(lvl_correction_error))
                return QRCodeGenerator.ECCLevel.M;

            switch (lvl_correction_error)
            {
                case "L":
                    return QRCodeGenerator.ECCLevel.L;
                case "Q":
                    return QRCodeGenerator.ECCLevel.Q;
                case "H":
                    return QRCodeGenerator.ECCLevel.H;
                default:
                    return QRCodeGenerator.ECCLevel.M;
            }
        }

        private void Download_Click(object clicked_btn, RoutedEventArgs e)
        {
            if (QR_bitmap == null)
                return;

            string combinedFilter = string.Join("|", formatMap.Values.Select(f => f.FilterText));
            int filterIndex = formatMap
                .Where(kvp => kvp.Value.Extension == selectedFormat.Extension)
                .Select(kvp => kvp.Key + 1)
                .FirstOrDefault();

            var dialog = new SaveFileDialog
            {
                Filter = combinedFilter,
                DefaultExt = selectedFormat.Extension,
                FilterIndex = filterIndex,
                FileName = $"QRCode.{selectedFormat.Extension}"
            };

            if (dialog.ShowDialog() == true)
            {
                QR_bitmap.Save(dialog.FileName, selectedFormat.Format);
            }
        }


        /*Метод BitmapToImageSource преобразует изображение из формата Bitmap, используемого библиотекой QRCoder, в BitmapImage, 
         * который можно отобразить в WPF. Я использую поток памяти и формат PNG, чтобы обеспечить совместимость и сохранить прозрачность. 
         * Это позволяет динамически обновлять QR-код в интерфейсе*/
        private static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using var memory = new MemoryStream();// Создаёт временный поток в памяти, куда будет записано изображение
            bitmap.Save(memory, ImageFormat.Png); // PNG выбран потому, что он поддерживает прозрачность и хорошо подходит для отображения
            memory.Position = 0;
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            return bitmapImage;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }

        private void CmbFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (formatMap.TryGetValue(cmbFormat.SelectedIndex, out var formatInfo))
            {
                selectedFormat = formatInfo;
            }
        }

        private readonly Dictionary<int, ImageFormatInfo> formatMap = new()
        {
            { 0, new ImageFormatInfo { Extension = "png", Format = ImageFormat.Png, FilterText = "PNG|*.png", DisplayName = "PNG" } },
            { 1, new ImageFormatInfo { Extension = "jpg", Format = ImageFormat.Jpeg, FilterText = "JPEG|*.jpg", DisplayName = "JPEG" } },
            { 2, new ImageFormatInfo { Extension = "bmp", Format = ImageFormat.Bmp, FilterText = "BMP|*.bmp", DisplayName = "BMP" } },
            { 3, new ImageFormatInfo { Extension = "gif", Format = ImageFormat.Gif, FilterText = "GIF|*.gif", DisplayName = "GIF" } },
            { 4, new ImageFormatInfo { Extension = "tiff", Format = ImageFormat.Tiff, FilterText = "TIFF|*.tiff", DisplayName = "TIFF" } },
        };

    }

    public class ImageFormatInfo
    {
        public string Extension { get; set; } = "png";
        public ImageFormat Format { get; set; } = ImageFormat.Png;
        public string FilterText { get; set; } = "PNG|*.png";
        public string DisplayName { get; set; } = "PNG";
    }

}
