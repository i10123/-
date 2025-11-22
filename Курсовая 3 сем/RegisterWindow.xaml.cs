using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace QR_Generator
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string pass = txtPass.Password.Trim();
            string passConf = txtPassConfirm.Password.Trim();

            // 1. Проверка на пустоту
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(pass))
            {
                ShowError("Заполните все поля!");
                return;
            }

            // 2. Проверка длины
            if (login.Length < 3)
            {
                ShowError("Логин слишком короткий (минимум 3 символа).");
                return;
            }
            if (pass.Length < 4)
            {
                ShowError("Пароль слишком короткий (минимум 4 символа).");
                return;
            }

            // 3. Проверка совпадения паролей
            if (pass != passConf)
            {
                ShowError("Пароли не совпадают!");
                return;
            }

            // 4. Попытка регистрации
            if (DataManager.Register(login, pass))
            {
                // Успех - показываем сообщение
                CustomMessageBox customMsg = new("Регистрация прошла успешно!\nДобро пожаловать в QR Generator.");
                customMsg.ShowDialog(); // ShowDialog ждет, пока пользователь нажмет кнопку

                // Переход к авторизации
                LoginWindow loginWin = new();
                loginWin.Show();
                Close();
            }
            else
            {
                ShowError("Такой пользователь уже существует!");
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWin = new();
            loginWin.Show();
            Close();
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed) DragMove();
        }

        private void ShowError(string message)
        {
            txtError.Text = message;
            // Небольшая анимация ошибки (можно просто цвет)
            txtError.Foreground = new SolidColorBrush(Color.FromRgb(255, 80, 80));
        }
    }
}