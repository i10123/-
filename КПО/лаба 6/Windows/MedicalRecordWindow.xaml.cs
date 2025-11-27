using System.Windows;
using System.Windows.Controls;
using Hospital.Data;
using Hospital.Domain.Entities;
using Hospital.Domain.Entities.Base;
using Hospital.Domain.Entities.Staff;
using Hospital.Domain.Entities.Treatments;
using Hospital.Domain.Enums;
using Hospital.Services;

namespace Hospital.Windows
{
    public partial class MedicalRecordWindow : Window
    {
        private readonly Patient patient;
        private readonly User currentUser;
        private Treatment? treatmentToEdit = null;

        public Visibility DoctorUI_Visibility { get; set; }

        public MedicalRecordWindow(Patient patient, User user)
        {
            InitializeComponent();
            this.patient = patient;
            currentUser = user;

            DoctorUI_Visibility = (currentUser is Doctor) ? Visibility.Visible : Visibility.Collapsed;
            DataContext = this;

            SetupUI();
            LoadData();
        }

        private void SetupUI()
        {
            PatientNameText.Text = patient.FullName;
            int age = DateTime.Now.Year - patient.BirthDate.Year;
            PatientInfoText.Text = $"{age} лет | Поступил: {patient.MedicalRecord.AdmissionDate:dd.MM.yyyy HH:mm}";
            StatusText.Text = patient.StatusRu;

            if (currentUser is Nurse)
            {
                // МЕДСЕСТРА
                DutyPanel.Visibility = Visibility.Collapsed;
                AttendingPanel.Visibility = Visibility.Collapsed;
                DiagnosisBox.IsReadOnly = true;
                AnamnesisBox.IsReadOnly = true;
                ActionsColumn.Visibility = Visibility.Collapsed;
                MainTabControl.SelectedIndex = 1;
            }
            else if (currentUser is Doctor doc)
            {
                // ВРАЧ
                ActionsColumn.Visibility = Visibility.Visible;

                if (doc.IsDuty && patient.Status == PatientStatus.Admitted)
                {
                    DutyPanel.Visibility = Visibility.Visible;
                    AttendingPanel.Visibility = Visibility.Collapsed;
                }
                else if (!doc.IsDuty && patient.Status == PatientStatus.InDepartment)
                {
                    DutyPanel.Visibility = Visibility.Collapsed;
                    AttendingPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    DutyPanel.Visibility = Visibility.Collapsed;
                    AttendingPanel.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void LoadData()
        {
            DiagnosisBox.Text = patient.MedicalRecord.Diagnosis;
            AnamnesisBox.Text = patient.MedicalRecord.Anamnesis;

            TreatmentsGrid.ItemsSource = null;
            TreatmentsGrid.ItemsSource = patient.MedicalRecord.Treatments.OrderByDescending(t => t.DatePrescribed);
        }

        private void SaveEMR()
        {
            patient.MedicalRecord.Diagnosis = DiagnosisBox.Text;
            patient.MedicalRecord.Anamnesis = AnamnesisBox.Text;
            Database.SavePatients();
        }

        private void AutoSave_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Проверка на null нужна, чтобы не упало при инициализации окна
            if (patient != null && patient.MedicalRecord != null)
            {
                patient.MedicalRecord.Diagnosis = DiagnosisBox.Text;
                patient.MedicalRecord.Anamnesis = AnamnesisBox.Text;
                Database.SavePatients();
            }
        }

        private void Transfer_Click(object sender, RoutedEventArgs e)
        {
            if (DepartmentCombo.SelectedItem == null) { 
                MessageBox.Show("Выберите отделение!"); 
                return; 
            }
            SaveEMR();

            string departStr = (DepartmentCombo.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "";
            DepartmentType depart = departStr switch
            {
                "Кардиология" => DepartmentType.Cardiology,
                "Хирургия" => DepartmentType.Surgery,
                "Неврология" => DepartmentType.Neurology,
                _ => DepartmentType.Reception
            };

            string msg = DoctorService.TransferToDepartment(patient, depart);
            MessageBox.Show($"Пациент направлен в отделение: {departStr}\n{msg}");
            DialogResult = true;
        }

        private void Discharge_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(DiagnosisBox.Text))
            {
                MessageBox.Show("Укажите диагноз перед выпиской!");
                return;
            }
            SaveEMR();

            // 1. Берем части имени в массив
            var nameParts = new[] { 
                patient.LastName, 
                patient.FirstName, 
                patient.MiddleName 
            };

            string safeName = string.Join("_", nameParts.Where(s => !string.IsNullOrWhiteSpace(s)));

            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                FileName = $"Эпикриз_{safeName}_{DateTime.Now:dd-MM-yyyy}.pdf"
            };

            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    PdfService.GenerateEpicrisis(patient, currentUser, saveDialog.FileName);
                    MessageBox.Show("Эпикриз успешно сохранен!", "Успех");

                    DoctorService.DischargePatient(patient);
                    DialogResult = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения PDF: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // НАЗНАЧЕНИЯ

        private void TreatmentType_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (MedFields == null) 
                return;

            var type = (TreatmentTypeCombo.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (type == "Процедура" || type == "Диагностика") 
                MedFields.Visibility = Visibility.Collapsed;
            else 
                MedFields.Visibility = Visibility.Visible;
        }

        private void AddTreatment_Click(object sender, RoutedEventArgs e)
        {
            if (TreatmentTypeCombo.SelectedItem == null || string.IsNullOrWhiteSpace(TreatNameBox.Text))
            {
                MessageBox.Show("Заполните поля!");
                return;
            }

            string type = (TreatmentTypeCombo.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "";

            if (treatmentToEdit != null)
            {
                // РЕДАКТИРОВАНИЕ
                treatmentToEdit.Name = TreatNameBox.Text;
                if (treatmentToEdit is Medication med)
                {
                    med.Type = type;
                    med.Dosage = TreatDosageBox.Text;
                    med.Count = int.TryParse(TreatCountBox.Text, out int c) ? c : 1;
                }

                treatmentToEdit = null;
                AddTreatBtn.Content = "ДОБАВИТЬ";
            }
            else
            {
                // СОЗДАНИЕ
                Treatment newTreat;
                if (type == "Таблетки" || type == "Инъекции")
                {
                    newTreat = new Medication { 
                        Type = type, 
                        Name = TreatNameBox.Text, 
                        Dosage = TreatDosageBox.Text, 
                        Count = int.TryParse(TreatCountBox.Text, out int c) ? c : 1 
                    };
                }
                else if (type == "Процедура") 
                    newTreat = new Procedure {
                        Name = TreatNameBox.Text 
                    };
                else 
                    newTreat = new Diagnostic { 
                        Name = TreatNameBox.Text 
                    };

                newTreat.DoctorId = currentUser.Id;
                newTreat.DoctorName = currentUser.Role == Role.Doctor ? currentUser.FullName : currentUser.RoleRu;

                patient.MedicalRecord.Treatments.Add(newTreat);
            }

            Database.SavePatients();
            ClearTreatFields();
            LoadData();
        }

        private void EditTreatment_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;

            // Редактировать можно только если галочка НЕ стоит
            if (btn?.DataContext is Treatment treat && !treat.IsCompleted)
            {
                TreatNameBox.Text = treat.Name;

                if (treat is Medication med)
                {
                    foreach (ComboBoxItem item in TreatmentTypeCombo.Items)
                        if (item.Content.ToString() == med.Type) 
                            TreatmentTypeCombo.SelectedItem = item;
                    TreatDosageBox.Text = med.Dosage;
                    TreatCountBox.Text = med.Count.ToString();
                }
                else if (treat is Procedure) 
                    TreatmentTypeCombo.SelectedIndex = 2;
                else if (treat is Diagnostic) 
                    TreatmentTypeCombo.SelectedIndex = 3;

                treatmentToEdit = treat;
                AddTreatBtn.Content = "СОХРАНИТЬ";
            }
            else
                MessageBox.Show("Нельзя редактировать выполненное назначение. Сначала снимите галочку.");
        }

        private void DeleteTreatment_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.DataContext is Treatment treat)
            {
                if (MessageBox.Show("Удалить назначение?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    patient.MedicalRecord.Treatments.Remove(treat);
                    Database.SavePatients();
                    LoadData();
                }
            }
        }

        private void TreatmentStatus_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;

            if (checkBox?.DataContext is Treatment treat)
            {
                if (treat.IsCompleted)
                    // ЕСЛИ ПОСТАВИЛИ ГАЛОЧКУ -> Записываем кто сделал
                    treat.ExecutionResult = $"Выполнил: {currentUser.RoleRu} {currentUser.FullName} ({DateTime.Now:dd.MM HH:mm})";
                else
                    // ЕСЛИ СНЯЛИ ГАЛОЧКУ -> Очищаем запись (Отмена выполнения)
                    treat.ExecutionResult = string.Empty;

                Database.SavePatients();
                TreatmentsGrid.Items.Refresh();
            }
        }

        private void ClearTreatFields()
        {
            TreatNameBox.Clear();
            TreatDosageBox.Clear();
            TreatCountBox.Clear();
        }
    }
}