using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace QR_Generator
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string pass = txtPass.Password.Trim();

            // 1. Проверка на пустой ввод
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(pass))
            {
                ShowError("Введите логин и пароль.");
                return;
            }

            // 2. Попытка входа
            if (DataManager.Login(login, pass))
            {
                MainWindow main = new MainWindow();
                main.Show();
                this.Close();
            }
            else
            {
                ShowError("Неверный логин или пароль!");
            }
        }

        private void BtnGoToRegister_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow regWin = new();
            regWin.Show();
            this.Close();
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed) DragMove();
        }

        // Метод для красивого отображения ошибки
        private void ShowError(string message)
        {
            txtError.Text = message;
            txtError.Foreground = Brushes.OrangeRed; // Яркий цвет ошибки
        }
    }
}