using Microsoft.Win32;
using QR_generator.Helpers;
using QR_generator.Models;
using QR_generator.Services;
using QR_generator.Views.Dialogs;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SWM = System.Windows.Media;

namespace QR_generator.Views.Windows
{
    public partial class MainWindow : Window
    {
        private string lvl_correction_error = "M";
        private string QR_color = "#000000";
        private Bitmap? QR_bitmap;
        private ImageFormatInfo selectedFormat = new();
        private Bitmap? logoBitmap = null;

        public MainWindow()
        {
            InitializeComponent();

            Title = $"QR Generator - {CurrentUser.Username}";
            btn_M.Background = new SolidColorBrush(Colors.Red);

            foreach (var button in stackPanel_colors.Children.OfType<Button>())
            {
                button.BorderThickness = new Thickness(0);
                button.BorderBrush = SWM.Brushes.Transparent;
            }
            btnColorBlack.BorderThickness = new Thickness(3);
            btnColorBlack.BorderBrush = SWM.Brushes.WhiteSmoke;

            foreach (var format in FormatMap.Formats.OrderBy(kvp => kvp.Key))
                cmbFormat.Items.Add(format.Value.DisplayName);

            cmbFormat.SelectedIndex = 0;
        }
        
        private void Close_Click(object clicked_btn, RoutedEventArgs e) => Close();
        private void Minimize_Click(object clicked_btn, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        private void Generate_Click(object clicked_btn, RoutedEventArgs e) => GenerateQR();
        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            ConfirmLogoutWindow dialog = new("Вы действительно хотите\nвыйти из аккаунта?")
            {
                Owner = this
            };

            if (dialog.ShowDialog() == true)
            {
                CurrentUser.Username = "Guest";
                CurrentUser.IsLoggedIn = false;

                LoginWindow login = new();
                login.Show();
                Close();
            }
        }
        private void ShowHistory_Click(object sender, RoutedEventArgs e)
        {
            HistoryWindow historyWindow = new()
            {
                Owner = this
            };
            historyWindow.ShowDialog();
        }
        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            if (QR_bitmap != null)
            {
                Clipboard.SetImage(BitmapConverter.ToWpfImage(QR_bitmap));
                new CustomMessageBox("QR-код скопирован в буфер обмена!").ShowDialog();
            }
        }
        private void SetLevelCorrection_Click(object clicked_btn, RoutedEventArgs? e)
        {
            if (clicked_btn is Button btn)
            {
                lvl_correction_error = btn.Tag?.ToString() ?? "";

                foreach (var button in stackPanel_correction.Children.OfType<Button>())
                    button.Background = SWM.Brushes.Black;

                btn.Background = new SolidColorBrush(Colors.Red);
            }
        }
        private void SetColor_Click(object clicked_btn, RoutedEventArgs e)
        {
            if (clicked_btn is Button btn)
            {
                QR_color = btn.Tag?.ToString() ?? "";

                foreach (var button in stackPanel_colors.Children.OfType<Button>())
                {
                    button.BorderThickness = new Thickness(0);
                    button.BorderBrush = SWM.Brushes.Transparent;
                }

                btn.BorderThickness = new Thickness(3);
                btn.BorderBrush = SWM.Brushes.WhiteSmoke;
            }
        }
        private void Download_Click(object clicked_btn, RoutedEventArgs e)
        {
            if (QR_bitmap != null)
            {
                string combinedFilter = string.Join("|", FormatMap.Formats.Values.Select(f => f.FilterText)); // строка фильтров
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

                        var eccLevel = QRService.GetLvlCorrectError(lvl_correction_error);

                        using var stream = new FileStream(dialog.FileName, FileMode.Create);

                        if (selectedFormat.Extension == "txt")
                        {
                            using var writer = new StreamWriter(stream);
                            writer.WriteLine("--- Содержимое QR-кода ---");
                            writer.WriteLine(qrData);
                            writer.WriteLine("\n--- Матрица ---");
                            writer.Write(QRService.GetMatrixString(qrData, eccLevel));
                        }
                        else if (selectedFormat.Extension == "bin")
                            stream.Write(System.Text.Encoding.UTF8.GetBytes(qrData));
                    }
                    else
                        QR_bitmap.Save(dialog.FileName, selectedFormat.Format);
                }
            }
        }
        private void SldSize_ValueChanged(object clicked_btn, RoutedPropertyChangedEventArgs<double> e)
        {
            int step = 50;
            int roundedValue = (int)(Math.Round(sldSize.Value / step) * step);

            txtSizeLabel.Text = $"Размер: {roundedValue} px";

            if (QR_bitmap != null)
                GenerateQR();
        }
        private void CmbFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FormatMap.Formats.TryGetValue(cmbFormat.SelectedIndex, out var formatInfo))
                selectedFormat = formatInfo;
        }
        private void BtnUploadLogo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new()
            {
                Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp"
            };

            if (ofd.ShowDialog() == true)
            {
                try
                {
                    using (var stream = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read))
                        logoBitmap = new Bitmap(stream);

                    txtLogoName.Text = Path.GetFileName(ofd.FileName);
                    txtLogoName.Foreground = SWM.Brushes.LightGreen;

                    SetLevelCorrection_Click(btn_H, null);
                    LockEccButtons(true);

                    new CustomMessageBox("При использовании логотипа\nуровень коррекции автоматически установлен на H.").ShowDialog();
                }
                catch
                {
                    new CustomMessageBox("Не удалось загрузить изображение.", false).ShowDialog();
                }
            }
        }
        private void BtnClearLogo_Click(object sender, RoutedEventArgs e)
        {
            logoBitmap = null;
            txtLogoName.Text = "Нет файла";
            txtLogoName.Foreground = SWM.Brushes.Gray;
            LockEccButtons(false);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            string geo = "[^0-9.,]+";
            Regex regex = new(geo);
            e.Handled = regex.IsMatch(e.Text);
        }
        private void LockEccButtons(bool isLocked)
        {
            btn_L.IsEnabled = !isLocked;
            btn_M.IsEnabled = !isLocked;
            btn_Q.IsEnabled = !isLocked;

            double opacity = isLocked ? 0.3 : 1.0; // прозрачность
            btn_L.Opacity = opacity;
            btn_M.Opacity = opacity;
            btn_Q.Opacity = opacity;
        }
        private void GenerateQR()
        {
            try
            {
                int tabIndex = tabControl.SelectedIndex;

                if (tabIndex == 4) // WiFi
                {
                    if (string.IsNullOrWhiteSpace(txtSsid.Text))
                    {
                        new CustomMessageBox("Введите название сети (SSID)!", true).ShowDialog();
                        return;
                    }

                    string wifiType = (cmbWifiType.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "nopass";

                    if (wifiType != "nopass" && string.IsNullOrWhiteSpace(txtPassword.Text))
                    {
                        new CustomMessageBox("Для защищенной сети необходимо ввести пароль!", true).ShowDialog();
                        return;
                    }
                }
                else if (tabIndex == 5) // Геолокация
                {
                    if (string.IsNullOrWhiteSpace(txtWidth.Text) || string.IsNullOrWhiteSpace(txtLong.Text))
                    {
                        new CustomMessageBox("Введите широту и долготу!", true).ShowDialog();
                        return;
                    }

                    if (!double.TryParse(txtWidth.Text.Replace('.', ','), out _) ||
                        !double.TryParse(txtLong.Text.Replace('.', ','), out _))
                    {
                        new CustomMessageBox("Координаты должны быть числами!", true).ShowDialog();
                        return;
                    }
                }

                string data = QRDataBuilder.Build(tabIndex, txtText, txtUrl, txtEmail, txtPhone,
                                                    cmbWifiType, txtSsid, txtPassword, txtWidth, txtLong);

                if (string.IsNullOrWhiteSpace(data))
                {
                    new CustomMessageBox("Поле ввода пустое!\nВведите данные для генерации.", true).ShowDialog();
                    return;
                }

                var eccLevel = QRService.GetLvlCorrectError(lvl_correction_error);

                QR_bitmap = QRService.GenerateQR(data, QR_color, (int)sldSize.Value, eccLevel, logoBitmap);

                if (QR_bitmap != null)
                {
                    imgPreview.Source = BitmapConverter.ToWpfImage(QR_bitmap);
                    btnDownload.IsEnabled = true;
                    btnCopy.IsEnabled = true;

                    string shortData = data.Length > 40 ? string.Concat(data.AsSpan(0, 40), "...") : data; // ссылка на кусок оригинальной строки
                    string? tabName = tabControl.SelectedItem is TabItem t ? t.Header.ToString() : "Unknown";

                    DataManager.SaveHistory($"[{tabName}] {shortData}");
                }
            }
            catch
            {
                new CustomMessageBox($"Введенное количество символом больше, чем может передать QR-код.", true).ShowDialog();
            }
        }
    }
}