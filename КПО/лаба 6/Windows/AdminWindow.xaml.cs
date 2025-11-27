using System.Windows;
using System.Windows.Controls;
using Hospital.Domain.Entities;
using Hospital.Domain.Entities.Base;
using Hospital.Services;

namespace Hospital.Windows
{
    public partial class AdminWindow : Window
    {
        private readonly User currentUser;
        public AdminWindow(User user)
        {
            InitializeComponent();
            currentUser = user;
            UpdateAll();
        }

        private void UpdateAll()
        {
            UpdateStaff();
            UpdatePatients();
        }

        // ЛОГИКА ПЕРСОНАЛА
        private void UpdateStaff()
        {
            string search = StaffSearchBox.Text;
            StaffGrid.ItemsSource = AdminService.GetStaff(search);
        }

        private void StaffSearchBox_TextChanged(object sender, TextChangedEventArgs e) => UpdateStaff();

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            AddUserWindow addWin = new();
            if (addWin.ShowDialog() == true)
                UpdateStaff();
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button?.DataContext is User userToDelete)
            {
                if (userToDelete.Id == currentUser.Id)
                {
                    MessageBox.Show("Вы не можете удалить свою учетную запись!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }

                if (MessageBox.Show($"Удалить сотрудника {userToDelete.FullName}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    AdminService.DeleteUser(userToDelete.Id);
                    UpdateStaff();
                }
            }
        }

        // ЛОГИКА ПАЦИЕНТОВ
        private void UpdatePatients()
        {
            if (RadioDischarged == null || PatientSearchBox == null)
                return;

            bool showDischarged = RadioDischarged.IsChecked == true;
            string search = PatientSearchBox.Text;

            PatientsGrid.ItemsSource = AdminService.GetPatients(showDischarged, search);
        }

        private void PatientFilter_Changed(object sender, RoutedEventArgs e) => UpdatePatients();

        private void PatientSearchBox_TextChanged(object sender, TextChangedEventArgs e) => UpdatePatients();

        // --- ОБЩЕЕ ---
        private void Refresh_Click(object sender, RoutedEventArgs e) => UpdateAll();

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            new Login().Show();
            Close();
        }

        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button?.DataContext is User user)
            {
                AddUserWindow editWin = new(user);

                if (editWin.ShowDialog() == true)
                    UpdateStaff();
            }
        }

        // --- КНОПКИ ПАЦИЕНТОВ ---

        private void EditPatient_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button?.DataContext is Patient patient)
            {
                PatientWindow win = new(patient);
                if (win.ShowDialog() == true)
                {
                    AdminService.UpdatePatient();
                    UpdatePatients();
                }
            }
        }

        private void DeletePatient_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button?.DataContext is Patient patient)
            {
                var res = MessageBox.Show($"Вы точно хотите удалить карту пациента: {patient.FullName}?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (res == MessageBoxResult.Yes)
                {
                    AdminService.DeletePatient(patient.Id);
                    UpdatePatients();
                }
            }
        }

        private void AddPatient_Click(object sender, RoutedEventArgs e)
        {
            PatientWindow win = new();

            if (win.ShowDialog() == true)
            {
                var newPatient = win.ResultPatient;
                AdminService.AddPatient(newPatient);
                UpdatePatients();
            }
        }
    }
}