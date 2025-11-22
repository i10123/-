using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace QR_Generator
{
    public partial class CustomMessageBox : Window
    {
        // isError = true (Красный крестик), isError = false (Зеленая галочка)
        public CustomMessageBox(string message, bool isError = false)
        {
            InitializeComponent();
            txtMessage.Text = message;

            if (isError)
            {
                // Меняем на стиль ОШИБКИ (Красный)
                txtTitle.Text = "ОШИБКА";

                // Рисуем крестик вместо галочки
                pathIcon.Data = Geometry.Parse("M19,6.41L17.59,5L12,10.59L6.41,5L5,6.41L10.59,12L5,17.59L6.41,19L12,13.41L17.59,19L19,17.59L13.41,12L19,6.41Z");

                // Красный цвет
                var redBrush = new SolidColorBrush(Color.FromRgb(255, 50, 50));
                pathIcon.Fill = redBrush;
                shadowEffect.Color = Color.FromRgb(255, 0, 0);
            }
            else
            {
                // Стиль УСПЕХА (оставляем зеленый как в XAML, меняем только заголовок)
                txtTitle.Text = "УСПЕШНО";
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed) DragMove();
        }
    }
}