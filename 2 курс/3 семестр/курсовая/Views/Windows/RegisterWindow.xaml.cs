using QR_generator.Services;
using QR_generator.Views.Dialogs;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;


namespace QR_generator.Views.Windows
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow() => InitializeComponent();

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            ResetBorders();

            string login = txtLogin.Text.Trim();

            if (txtVisiblePass.Visibility == Visibility.Visible)
                txtPass.Password = txtVisiblePass.Text;

            string pass = txtPass.Password.Trim();

            if (string.IsNullOrEmpty(login))
            {
                txtLogin.BorderBrush = Brushes.Red;
                ShowError("Введите логин!");
                return;
            }

            string loginPattern = @"^(?=.*[a-zA-Z])(?=.*\d)[a-zA-Z0-9]{3,}$";
            if (!Regex.IsMatch(login, loginPattern))
            {
                txtLogin.BorderBrush = Brushes.Red;
                ShowError("Логин должен содержать и БУКВЫ, и ЦИФРЫ (от 3х символов).");
                return;
            }

            string passPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$";

            if (!Regex.IsMatch(pass, passPattern))
            {
                SetPasswordBorderColor(Brushes.Red);
                ShowError("Пароль слабый! Нужны: строчная и заглавная буквы, цифра, спецсимвол (от 8 символов).");
                return;
            }

            if (DataManager.Register(login, pass))
            {
                new CustomMessageBox("Успешно!").ShowDialog();
                new LoginWindow().Show();
                Close();
            }
            else
            {
                txtLogin.BorderBrush = Brushes.Red;
                ShowError("Пользователь с таким логином уже существует!");
            }
        }


        private void ResetBorders()
        {
            var defaultColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#1E1E1E")!;

            txtLogin.BorderBrush = defaultColor;
            SetPasswordBorderColor(defaultColor);
            SetConfirmPasswordBorderColor(defaultColor);

            txtError.Text = "";
        }

        private void SetPasswordBorderColor(Brush brush)
        {
            txtPass.BorderBrush = brush;
            txtVisiblePass.BorderBrush = brush;
        }

        private void SetConfirmPasswordBorderColor(Brush brush)
        {
            txtPassConfirm.BorderBrush = brush;
            txtVisiblePassConfirm.BorderBrush = brush;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new();
            loginWindow.Show();
            Close();
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed) 
                DragMove();
        }

        private void BtnTogglePass_Click(object sender, RoutedEventArgs e) => ToggleVisibility(txtPass, txtVisiblePass, btnTogglePass, "EyeIcon");
        
        private void BtnTogglePassConfirm_Click(object sender, RoutedEventArgs e) => ToggleVisibility(txtPassConfirm, txtVisiblePassConfirm, btnTogglePassConfirm, "EyeIconConfirm");

        private static void ToggleVisibility(PasswordBox passBox, TextBox txtBox, Button btn, string iconName)
        {
            var path = (Path)btn.Template.FindName(iconName, btn);

            if (txtBox.Visibility == Visibility.Visible)
            {
                passBox.Password = txtBox.Text;
                txtBox.Visibility = Visibility.Collapsed;
                passBox.Visibility = Visibility.Visible;
                path.Data = Geometry.Parse("M12,9A3,3 0 0,0 9,12A3,3 0 0,0 12,15A3,3 0 0,0 15,12A3,3 0 0,0 12,9M12,17A5,5 0 0,1 7,12A5,5 0 0,1 12,7A5,5 0 0,1 17,12A5,5 0 0,1 12,17M12,4.5C7,4.5 2.73,7.61 1,12C2.73,16.39 7,19.5 12,19.5C17,19.5 21.27,16.39 23,12C21.27,7.61 17,4.5 12,4.5Z");
            }
            else
            {
                txtBox.Text = passBox.Password;
                passBox.Visibility = Visibility.Collapsed;
                txtBox.Visibility = Visibility.Visible;
                path.Data = Geometry.Parse("M11.83,9L15,12.16C15,12.11 15,12.05 15,12A3,3 0 0,0 12,9C11.94,9 11.89,9 11.83,9M7.53,9.8L9.08,11.35C9.03,11.56 9,11.77 9,12A3,3 0 0,0 12,15C12.22,15 12.44,14.97 12.65,14.92L14.2,16.47C13.53,16.8 12.79,17 12,17A5,5 0 0,1 7,12C7,11.21 7.2,10.47 7.53,9.8M2,4.27L4.28,6.55L4.73,7C3.08,8.3 1.78,10 1,12C2.73,16.39 7,19.5 12,19.5C13.55,19.5 15.03,19.2 16.38,18.66L16.81,19.08L19.73,22L21,20.73L3.27,3L2,4.27M12,7A5,5 0 0,1 17,12C17,12.64 16.87,13.26 16.64,13.82L19.57,16.75C21.07,15.5 22.27,13.86 23,12C21.27,7.61 17,4.5 12,4.5C10.6,4.5 9.27,4.75 8.07,5.2L10.17,7.35C10.74,7.13 11.35,7 12,7Z");
            }
        }

        private void ShowError(string message)
        {
            txtError.Text = message;
            txtError.Foreground = new SolidColorBrush(Color.FromRgb(255, 80, 80));
        }
    }
}