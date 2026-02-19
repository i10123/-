using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace QR_generator.Views.Dialogs
{
    public partial class CustomMessageBox : Window
    {
        // isError: true - крестик, false - галочка
        public CustomMessageBox(string message, bool isError = false)
        {
            InitializeComponent();
            txtMessage.Text = message;

            if (isError)
            {
                txtTitle.Text = "ОШИБКА";
                pathIcon.Data = Geometry.Parse("M19,6.41L17.59,5L12,10.59L6.41,5L5,6.41L10.59,12L5,17.59L6.41,19L12,13.41L17.59,19L19,17.59L13.41,12L19,6.41Z");

                var redBrush = new SolidColorBrush(Color.FromRgb(255, 50, 50));
                pathIcon.Fill = redBrush;
                shadowEffect.Color = Color.FromRgb(255, 0, 0);
            }
            else
                txtTitle.Text = "УСПЕШНО";
        }

        private void BtnPON_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed) 
                DragMove();
        }
    }
}