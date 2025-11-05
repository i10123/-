using Microsoft.Win32;
using QR_generator;
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
using SWM = System.Windows.Media;

namespace QR_Generator
{
    public partial class MainWindow : Window
    {
        private string? lvl_correction_error = "M";
        private string QR_color = "#000000";
        private readonly int size_QR = 300;
        private Bitmap? QR_bitmap;
        private ImageFormatInfo selectedFormat = new(); // PNG по умолчанию

        public MainWindow()
        {
            InitializeComponent();
            btn_M.Background = new SolidColorBrush(Colors.Red);

            foreach (var button in stackPanel_colors.Children.OfType<Button>())
            {
                button.BorderThickness = new Thickness(0);
                button.BorderBrush = SWM.Brushes.Transparent;
            }
            btnColorBlack.BorderThickness = new Thickness(3);
            btnColorBlack.BorderBrush = SWM.Brushes.WhiteSmoke;

            foreach (var format in FormatMap.Formats.OrderBy(kvp => kvp.Key))
            {
                cmbFormat.Items.Add(format.Value.DisplayName);
            }
            cmbFormat.SelectedIndex = 0; // по умолчанию PNG

        }
        private void Close_Click(object clicked_btn, RoutedEventArgs e) => Close();
        private void Minimize_Click(object clicked_btn, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        private void Generate_Click(object clicked_btn, RoutedEventArgs e) => GenerateQR();


        private void SetLevelCorrection_Click(object clicked_btn, RoutedEventArgs e)
        {
            if (clicked_btn is not Button btn)
                return;

            lvl_correction_error = btn.Tag?.ToString() ?? "";

            foreach (var button in stackPanel_correction.Children.OfType<Button>())
            {
                button.Background = SWM.Brushes.Black;
            }

            btn.Background = new SolidColorBrush(Colors.Red);
        }
        private void SetColor_Click(object clicked_btn, RoutedEventArgs e)
        {
            if (clicked_btn is not Button btn)
                return;

            QR_color = btn.Tag?.ToString() ?? "";

            foreach (var button in stackPanel_colors.Children.OfType<Button>())
            {
                button.BorderThickness = new Thickness(0);
                button.BorderBrush = SWM.Brushes.Transparent;
            }

            btn.BorderThickness = new Thickness(3);
            btn.BorderBrush = SWM.Brushes.WhiteSmoke;

            GenerateQR();
        }
        private void Download_Click(object clicked_btn, RoutedEventArgs e)
        {
            if (QR_bitmap == null)
                return;

            string combinedFilter = string.Join("|", FormatMap.Formats.Values.Select(f => f.FilterText));
            int filterIndex = FormatMap.Formats
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
                if (selectedFormat.Format == null)
                {
                    string qrData = QRDataBuilder.Build(tabControl.SelectedIndex, txtText, txtUrl, txtEmail, txtPhone, 
                                                        cmbWifiType, txtSsid, txtPassword, txtWidth, txtLong);
                    using var stream = new FileStream(dialog.FileName, FileMode.Create);

                    if (selectedFormat.Extension == "txt")
                    {
                        using var writer = new StreamWriter(stream);
                        writer.WriteLine("QR Code raw data:");
                        writer.WriteLine(qrData);
                    }
                    else if (selectedFormat.Extension == "bin")
                    {
                        using var writer = new BinaryWriter(stream);
                        writer.Write(System.Text.Encoding.UTF8.GetBytes(qrData));
                    }
                }
                else
                    QR_bitmap.Save(dialog.FileName, selectedFormat.Format);
            }
        }


        private void SldSize_ValueChanged(object clicked_btn, RoutedPropertyChangedEventArgs<double> e)
        {
            int step = 50;
            int roundedValue = (int)(Math.Round(sldSize.Value / step) * step);
            if (sldSize.Value != roundedValue)
                sldSize.Value = roundedValue;

            txtSizeLabel.Text = $"Размер: {roundedValue} px";
        }
        private void Track_PreviewMouseLeftButtonDown(object clicked_btn, MouseButtonEventArgs e)
        {
            if (clicked_btn is Track track)
            {
                var slider = sldSize;
                System.Windows.Point pt = e.GetPosition(track);

                double ratio = pt.X / track.ActualWidth;
                double newValue = slider.Minimum + ratio * (slider.Maximum - slider.Minimum);

                int step = 50;
                newValue = Math.Round(newValue / step) * step;

                slider.Value = newValue;
                e.Handled = true;
            }
        }
        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }
        private void CmbFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FormatMap.Formats.TryGetValue(cmbFormat.SelectedIndex, out var formatInfo))
                selectedFormat = formatInfo;
        }

        private void GenerateQR()
        {
            string data = QRDataBuilder.Build(tabControl.SelectedIndex, txtText, txtUrl, txtEmail, txtPhone, 
                                                cmbWifiType, txtSsid, txtPassword, txtWidth, txtLong);
            var eccLevel = QRService.GetLvlCorrectError(lvl_correction_error);
            QR_bitmap = QRService.GenerateQR(data, QR_color, size_QR, eccLevel);

            if (QR_bitmap != null)
            {
                imgPreview.Source = BitmapConverter.ToImageSource(QR_bitmap);
                btnDownload.IsEnabled = true;
            }
        }
    }
}