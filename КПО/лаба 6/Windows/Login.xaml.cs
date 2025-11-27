using System.Windows;
using System.Windows.Input;
using Hospital.Data;
using Hospital.Domain.Entities.Staff;
using Hospital.Services;
using MaterialDesignThemes.Wpf;

namespace Hospital.Windows
{
    public partial class Login : Window
    {
        private bool isPasswordVisible = false;

        public Login()
        {
            InitializeComponent();
            Database.Load();
            // LoginBox.Text = "admin";
            // PasswordBoxHidden.Password = "admin";
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text;
            string pass = isPasswordVisible ? PasswordBoxVisible.Text : PasswordBoxHidden.Password;

            var user = AuthService.Login(login, pass);

            if (user == null)
            {
                ErrorText.Text = "Неверный логин или пароль!";
                return;
            }

            if (user is Administrator admin)
                new AdminWindow(admin).Show();

            else if (user is Doctor doctor)
                new DoctorWindow(doctor).Show();

            else if (user is Nurse nurse)
                new DoctorWindow(nurse).Show();

            Close();
        }

        private void TogglePassword_Click(object sender, RoutedEventArgs e)
        {
            if (isPasswordVisible)
            {
                // Прячем пароль (Текст -> Точки)
                PasswordBoxHidden.Password = PasswordBoxVisible.Text;

                PasswordBoxVisible.Visibility = Visibility.Hidden;
                PasswordBoxHidden.Visibility = Visibility.Visible;

                PasswordBoxHidden.Focus();

                EyeIcon.Kind = PackIconKind.Eye;
                isPasswordVisible = false;
            }
            else
            {
                // Показываем пароль (Точки -> Текст)
                PasswordBoxVisible.Text = PasswordBoxHidden.Password;

                PasswordBoxHidden.Visibility = Visibility.Hidden;
                PasswordBoxVisible.Visibility = Visibility.Visible;

                PasswordBoxVisible.Focus();
                PasswordBoxVisible.CaretIndex = PasswordBoxVisible.Text.Length;

                EyeIcon.Kind = PackIconKind.EyeOff;
                isPasswordVisible = true;
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }

        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}