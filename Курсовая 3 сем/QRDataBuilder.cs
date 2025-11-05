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
            return selectedIndex switch
            {
                0 => txtText.Text,
                1 => txtUrl.Text,
                2 => $"Почта:{txtEmail.Text}",
                3 => $"Телефон:{txtPhone.Text}",
                4 => $"WIFI:T:{cmbWifiType.SelectedItem};S:{txtSsid.Text};P:{txtPassword.Text};;",
                5 => $"Геолокация:{txtWidth.Text},{txtLong.Text}",
                _ => "",
            };
        }
    }
}