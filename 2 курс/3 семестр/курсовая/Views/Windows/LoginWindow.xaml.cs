using QR_generator.Services;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace QR_generator.Views.Windows
{
    public partial class LoginWindow : Window
    {
        public LoginWindow() => InitializeComponent();

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();

            // Если пароль сейчас виден, берем его из TextBox
            if (txtVisiblePass.Visibility == Visibility.Visible)
                txtPass.Password = txtVisiblePass.Text;

            string pass = txtPass.Password.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(pass))
            {
                ShowError("Введите логин и пароль.");
                return;
            }

            if (DataManager.Login(login, pass))
            {
                MainWindow main = new();
                main.Show();
                Close();
            }
            else
                ShowError("Неверный логин или пароль!");
        }

        private void BtnGoToRegister_Click(object sender, RoutedEventArgs e) { 
            RegisterWindow regWindow = new();
            regWindow.Show(); 
            Close(); 
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed) 
                DragMove();
        }

        private void BtnTogglePass_Click(object sender, RoutedEventArgs e)
        {
            // Находим иконку внутри кнопки
            var path = (Path)btnTogglePass.Template.FindName("EyeIcon", btnTogglePass);

            if (txtVisiblePass.Visibility == Visibility.Visible)
            {
                // СКРЫВАЕМ ПАРОЛЬ
                txtPass.Password = txtVisiblePass.Text;
                txtPass.Visibility = Visibility.Visible;
                txtVisiblePass.Visibility = Visibility.Collapsed;
                path.Data = Geometry.Parse("M12,9A3,3 0 0,0 9,12A3,3 0 0,0 12,15A3,3 0 0,0 15,12A3,3 0 0,0 12,9M12,17A5,5 0 0,1 7,12A5,5 0 0,1 12,7A5,5 0 0,1 17,12A5,5 0 0,1 12,17M12,4.5C7,4.5 2.73,7.61 1,12C2.73,16.39 7,19.5 12,19.5C17,19.5 21.27,16.39 23,12C21.27,7.61 17,4.5 12,4.5Z");
            }
            else
            {
                // ПОКАЗЫВАЕМ ПАРОЛЬ
                txtVisiblePass.Text = txtPass.Password;
                txtVisiblePass.Visibility = Visibility.Visible;
                txtPass.Visibility = Visibility.Collapsed;
                path.Data = Geometry.Parse("M11.83,9L15,12.16C15,12.11 15,12.05 15,12A3,3 0 0,0 12,9C11.94,9 11.89,9 11.83,9M7.53,9.8L9.08,11.35C9.03,11.56 9,11.77 9,12A3,3 0 0,0 12,15C12.22,15 12.44,14.97 12.65,14.92L14.2,16.47C13.53,16.8 12.79,17 12,17A5,5 0 0,1 7,12C7,11.21 7.2,10.47 7.53,9.8M2,4.27L4.28,6.55L4.73,7C3.08,8.3 1.78,10 1,12C2.73,16.39 7,19.5 12,19.5C13.55,19.5 15.03,19.2 16.38,18.66L16.81,19.08L19.73,22L21,20.73L3.27,3L2,4.27M12,7A5,5 0 0,1 17,12C17,12.64 16.87,13.26 16.64,13.82L19.57,16.75C21.07,15.5 22.27,13.86 23,12C21.27,7.61 17,4.5 12,4.5C10.6,4.5 9.27,4.75 8.07,5.2L10.17,7.35C10.74,7.13 11.35,7 12,7Z");
            }
        }

        private void ShowError(string message)
        {
            txtError.Text = message;
            txtError.Foreground = Brushes.OrangeRed;
        }
    }
}