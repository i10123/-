using System.Windows.Controls;

namespace QR_generator.Services
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
            static string GetText(TextBox t) => t.Text?.Trim() ?? "";

            return selectedIndex switch
            {
                0 => GetText(txtText),
                1 => GetText(txtUrl),
                2 => string.IsNullOrWhiteSpace(GetText(txtEmail)) ? "" : $"mailto:{GetText(txtEmail)}",
                3 => string.IsNullOrWhiteSpace(GetText(txtPhone)) ? "" : $"tel: {GetText(txtPhone)}",
                4 => $"WIFI:T:{(cmbWifiType.SelectedItem is ComboBoxItem item ? 
                item.Content.ToString() : "nopass")};S:{GetText(txtSsid)};P:{GetText(txtPassword)};;",
                5 => $"geo:{GetText(txtWidth)},{GetText(txtLong)}",
                _ => "",
            };
        }
    }
}