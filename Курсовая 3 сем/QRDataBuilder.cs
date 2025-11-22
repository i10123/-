using System.Windows.Controls;

namespace QR_Generator
{
    public static class QRDataBuilder
    {
        public static string Build(
            int selectedIndex,
            TextBox txtText,
            TextBox txtUrl,
            TextBox txtEmail,
            TextBox txtPhone,
            ComboBox cmbWifiType,
            TextBox txtSsid,
            TextBox txtPassword,
            TextBox txtWidth,
            TextBox txtLong)
        {
            // Функция для безопасного получения текста (удаляет пробелы по краям)
            static string GetText(TextBox t) => t.Text?.Trim() ?? "";

            return selectedIndex switch
            {
                0 => GetText(txtText),
                1 => GetText(txtUrl),
                2 => string.IsNullOrWhiteSpace(GetText(txtEmail)) ? "" : $"Почта:{GetText(txtEmail)}",
                3 => string.IsNullOrWhiteSpace(GetText(txtPhone)) ? "" : $"Телефон:{GetText(txtPhone)}",
                4 => $"WIFI:T:{cmbWifiType.SelectedItem};S:{GetText(txtSsid)};P:{GetText(txtPassword)};;",
                5 => $"Геолокация:{GetText(txtWidth)},{GetText(txtLong)}",
                _ => "",
            };
        }
    }
}