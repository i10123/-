using System.Windows;
using System.Windows.Controls;
using Hospital.Data;
using Hospital.Domain.Entities;
using Hospital.Domain.Entities.Base;
using Hospital.Domain.Entities.Staff;
using Hospital.Domain.Enums;
using Hospital.Services;

namespace Hospital.Windows
{
    public partial class DoctorWindow : Window
    {
        private readonly User currentUser;
        public Visibility DutyVisibility { get; set; }
        public DoctorWindow(User user)
        {
            InitializeComponent();
            currentUser = user;

            if (currentUser is Doctor doc && doc.IsDuty)
                DutyVisibility = Visibility.Visible;
            else
                DutyVisibility = Visibility.Collapsed;

            DataContext = this;

            SetupHeader();
            UpdateList();
        }

        private void SetupHeader()
        {
            DoctorNameText.Text = $"{currentUser.RoleRu} {currentUser.FullName}";

            if (currentUser is Doctor doc)
            {
                string status = doc.IsDuty ? "ДЕЖУРНЫЙ ВРАЧ" : doc.DepartmentRu;
                DoctorInfoText.Text = status;
                AdmitPatientBtn.Visibility = doc.IsDuty ? Visibility.Visible : Visibility.Collapsed;
            }
            else if (currentUser is Nurse nurse)
            {
                DoctorInfoText.Text = $"Отделение: {nurse.DepartmentRu}";
                AdmitPatientBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void UpdateList()
        {
            string search = SearchBox.Text;
            List<Patient> patients = [];

            if (currentUser is Doctor doc)
                patients = DoctorService.GetPatientsForDoctor(doc, search);
            else if (currentUser is Nurse nurse)
            {
                // Медсестра видит всех в отделении, кто На Лечении
                patients = [.. Database.Patients.Where(p =>
                    p.CurrentDepartment == nurse.Department &&
                    p.Status == PatientStatus.InDepartment)];

                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToLower();
                    patients = [.. patients.Where(p => p.FullName.Contains(search, StringComparison.CurrentCultureIgnoreCase))];
                }
            }

            PatientsGrid.ItemsSource = patients;
            if (patients.Count == 0) 
                EmptyText.Visibility = Visibility.Visible;
            else 
                EmptyText.Visibility = Visibility.Collapsed;
        }

        private void EditPatient_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is Patient patient)
            {
                // Используем то же окно, что и Админ
                PatientWindow win = new PatientWindow(patient);
                if (win.ShowDialog() == true)
                {
                    Database.SavePatients(); // Сохраняем изменения
                    UpdateList(); // Обновляем таблицу
                }
            }
        }

        private void DeletePatient_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is Patient patient)
            {
                if (MessageBox.Show($"Удалить пациента {patient.FullName}?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Database.Patients.Remove(patient);
                    Database.SavePatients();
                    UpdateList();
                }
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) => UpdateList();

        private void Refresh_Click(object sender, RoutedEventArgs e) => UpdateList();
        
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            new Login().Show();
            Close();
        }

        private void AdmitPatient_Click(object sender, RoutedEventArgs e)
        {
            PatientWindow win = new();
            if (win.ShowDialog() == true)
            {
                var newPatient = win.ResultPatient;
                Database.Patients.Add(newPatient);
                Database.SavePatients();
                UpdateList();
            }
        }

        private void OpenMedicalRecord_Click(object sender, RoutedEventArgs e)
        {
            if (PatientsGrid.SelectedItem is Patient selectedPatient)
                OpenCard(selectedPatient);
            else
                MessageBox.Show("Выберите пациента!");
        }

        private void RowOpen_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is Patient patient)
                OpenCard(patient);
        }

        private void OpenCard(Patient patient)
        {
            MedicalRecordWindow card = new(patient, currentUser);
            if (card.ShowDialog() == true)
                UpdateList();
            else
                UpdateList();
        }
    }
}