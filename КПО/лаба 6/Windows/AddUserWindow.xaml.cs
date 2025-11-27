using Hospital.Domain.Entities.Base;
using Hospital.Domain.Entities.Staff;
using Hospital.Domain.Enums;
using Hospital.Services;
using System.Windows;
using System.Windows.Controls;

namespace Hospital.Windows
{
    public partial class AddUserWindow : Window
    {
        private readonly User? _userToEdit = null;

        public AddUserWindow()
        {
            InitializeComponent();
            TitleText.Text = "Новый сотрудник";
            SaveBtn.Content = "СОХРАНИТЬ";
        }

        public AddUserWindow(User user)
        {
            InitializeComponent();
            _userToEdit = user;
            TitleText.Text = "Редактирование сотрудника";
            SaveBtn.Content = "ОБНОВИТЬ";

            FillFields();
        }

        private void FillFields()
        {
            if (_userToEdit == null) return;

            LastNameBox.Text = _userToEdit.LastName;
            FirstNameBox.Text = _userToEdit.FirstName;
            MiddleNameBox.Text = _userToEdit.MiddleName;
            LoginBox.Text = _userToEdit.Login;

            // ИСПРАВЛЕНИЕ 1: Пароль оставляем пустым, чтобы не показывать хеш
            PasswordBox.Text = string.Empty;

            // Выбор роли
            if (_userToEdit.Role == Role.Doctor) RoleCombo.SelectedIndex = 0;
            else if (_userToEdit.Role == Role.Nurse) RoleCombo.SelectedIndex = 1;
            else if (_userToEdit.Role == Role.Admin) RoleCombo.SelectedIndex = 2;

            // Заполнение спец. полей
            if (_userToEdit is Doctor doc)
            {
                // ИСПРАВЛЕНИЕ 3 (Баг отделения):
                // Отписываемся от события, чтобы смена галочки кодом не сбрасывала отделение
                DutyCheckBox.Checked -= DutyCheckBox_Changed;
                DutyCheckBox.Unchecked -= DutyCheckBox_Changed;

                DutyCheckBox.IsChecked = doc.IsDuty;

                // Если дежурный - блокируем комбобокс и ставим Приемное
                if (doc.IsDuty)
                {
                    DepartmentCombo.IsEnabled = false;
                    DepartmentCombo.SelectedIndex = 0;
                }
                else
                {
                    DepartmentCombo.IsEnabled = true;
                    SetDepartment(doc.Department);
                }

                // Подписываемся обратно
                DutyCheckBox.Checked += DutyCheckBox_Changed;
                DutyCheckBox.Unchecked += DutyCheckBox_Changed;
            }
            else if (_userToEdit is Nurse nurse)
            {
                SetDepartment(nurse.Department);
            }
        }

        private void SetDepartment(DepartmentType dept)
        {
            DepartmentCombo.SelectedIndex = dept switch
            {
                DepartmentType.Reception => 0,
                DepartmentType.Cardiology => 1,
                DepartmentType.Surgery => 2,
                DepartmentType.Neurology => 3,
                _ => 0
            };
        }

        private void DutyCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (DutyCheckBox.IsChecked == true)
            {
                DepartmentCombo.SelectedIndex = 0; // Приемное
                DepartmentCombo.IsEnabled = false;
            }
            else
            {
                DepartmentCombo.IsEnabled = true;
            }
        }

        private void RoleCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SpecificFields == null) return;

            var selected = (RoleCombo.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (selected == "Врач")
            {
                SpecificFields.Visibility = Visibility.Visible;
                DutyCheckBox.Visibility = Visibility.Visible;
            }
            else if (selected == "Медсестра")
            {
                SpecificFields.Visibility = Visibility.Visible;
                DutyCheckBox.Visibility = Visibility.Collapsed;
            }
            else // Администратор
            {
                SpecificFields.Visibility = Visibility.Collapsed;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // ВАЛИДАЦИЯ
                if (RoleCombo.SelectedItem == null) throw new Exception("Выберите роль!");
                if (string.IsNullOrWhiteSpace(LastNameBox.Text) || string.IsNullOrWhiteSpace(FirstNameBox.Text))
                    throw new Exception("Заполните ФИО!");
                if (string.IsNullOrWhiteSpace(LoginBox.Text))
                    throw new Exception("Заполните логин!");

                string rawPassword = PasswordBox.Text;

                // ИСПРАВЛЕНИЕ 1 и 3 (Пароль):
                // Проверяем пароль, только если это НОВЫЙ юзер ИЛИ если поле НЕ ПУСТОЕ (хотят сменить)
                if (_userToEdit == null || !string.IsNullOrEmpty(rawPassword))
                {
                    if (rawPassword.Length < 6 || rawPassword.Length > 18)
                        throw new Exception("Пароль должен быть от 6 до 18 символов!");
                }

                // Проверка отделения (только для врачей и медсестер)
                if (SpecificFields.Visibility == Visibility.Visible && DepartmentCombo.SelectedItem == null)
                    throw new Exception("Выберите отделение!");

                // Парсинг отделения
                DepartmentType dept = DepartmentType.Reception;
                if (SpecificFields.Visibility == Visibility.Visible)
                {
                    string deptStr = (DepartmentCombo.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
                    if (deptStr.Contains("Кардиология")) dept = DepartmentType.Cardiology;
                    else if (deptStr.Contains("Хирургия")) dept = DepartmentType.Surgery;
                    else if (deptStr.Contains("Неврология")) dept = DepartmentType.Neurology;
                }

                // --- СОХРАНЕНИЕ ---
                if (_userToEdit == null)
                {
                    // СОЗДАНИЕ
                    User resultUser;
                    var roleStr = (RoleCombo.SelectedItem as ComboBoxItem)?.Content.ToString();

                    if (roleStr == "Врач")
                        resultUser = new Doctor { Department = dept, IsDuty = DutyCheckBox.IsChecked == true };
                    else if (roleStr == "Медсестра")
                        resultUser = new Nurse { Department = dept };
                    else
                        resultUser = new Administrator(); // Добавили создание Админа

                    resultUser.LastName = LastNameBox.Text;
                    resultUser.FirstName = FirstNameBox.Text;
                    resultUser.MiddleName = MiddleNameBox.Text;
                    resultUser.Login = LoginBox.Text;

                    // Хешируем новый пароль
                    resultUser.Password = Hospital.Data.Database.HashPassword(rawPassword);

                    AdminService.AddUser(resultUser);
                }
                else
                {
                    // ОБНОВЛЕНИЕ
                    _userToEdit.LastName = LastNameBox.Text;
                    _userToEdit.FirstName = FirstNameBox.Text;
                    _userToEdit.MiddleName = MiddleNameBox.Text;
                    _userToEdit.Login = LoginBox.Text;

                    // Меняем пароль, только если ввели новый
                    if (!string.IsNullOrEmpty(rawPassword))
                    {
                        _userToEdit.Password = Hospital.Data.Database.HashPassword(rawPassword);
                    }

                    if (_userToEdit is Doctor d)
                    {
                        d.Department = dept;
                        d.IsDuty = DutyCheckBox.IsChecked == true;
                    }
                    else if (_userToEdit is Nurse n)
                    {
                        n.Department = dept;
                    }

                    AdminService.UpdateUser();
                }

                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}