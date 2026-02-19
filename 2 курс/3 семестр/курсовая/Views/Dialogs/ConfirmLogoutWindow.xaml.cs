using System.Windows;
using System.Windows.Input;

namespace QR_generator.Views.Dialogs
{
    public partial class ConfirmLogoutWindow : Window
    {
        public ConfirmLogoutWindow(string message)
        {
            InitializeComponent();
            txtMessage.Text = message;
        }

        private void BtnYes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void BtnNo_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed) 
                DragMove();
        }
    }
}