using System.Windows;
using System.Windows.Controls;
using Hospital.Domain.Entities;
using Hospital.Domain.Enums;

namespace Hospital.Windows
{
    public partial class PatientWindow : Window
    {
        private readonly Patient patient;
        public Patient ResultPatient => patient;

        public PatientWindow(Patient? patient = null)
        {
            InitializeComponent();

            if (patient == null)
            {
                this.patient = new Patient();
                TitleText.Text = "Новый пациент";
                SaveBtn.Content = "СОХРАНИТЬ";
                StatusCombo.SelectedIndex = 0;

                // По умолчанию ставим ТЕКУЩЕЕ время поступления
                AdmissionDatePicker.SelectedDate = DateTime.Now;
                AdmissionTimePicker.SelectedTime = DateTime.Now;
            }
            else
            {
                this.patient = patient;
                TitleText.Text = "Редактирование пациента";
                SaveBtn.Content = "ОБНОВИТЬ";
                FillFields();
            }
        }

        private void FillFields()
        {
            LastNameBox.Text = patient.LastName;
            FirstNameBox.Text = patient.FirstName;
            MiddleNameBox.Text = patient.MiddleName;
            BirthDatePicker.SelectedDate = patient.BirthDate;
            PassportBox.Text = patient.PassportNumber;
            PhoneBox.Text = patient.Phone;
            AddressBox.Text = patient.Address;

            GenderCombo.SelectedIndex = patient.Gender == Gender.Male ? 0 : 1;
            StatusCombo.SelectedIndex = (int)patient.Status;

            AdmissionDatePicker.SelectedDate = patient.MedicalRecord.AdmissionDate;
            AdmissionTimePicker.SelectedTime = patient.MedicalRecord.AdmissionDate;

            if (patient.MedicalRecord.DischargeDate != null)
            {
                DischargeDatePicker.SelectedDate = patient.MedicalRecord.DischargeDate;
                DischargeTimePicker.SelectedTime = patient.MedicalRecord.DischargeDate;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(LastNameBox.Text) || string.IsNullOrWhiteSpace(FirstNameBox.Text))
                    throw new Exception("ФИО обязательно!");
                if (BirthDatePicker.SelectedDate == null)
                    throw new Exception("Дата рождения обязательна!");
                if (GenderCombo.SelectedItem == null)
                    throw new Exception("Выберите пол!");

                // СОХРАНЕНИЕ БАЗОВЫХ ПОЛЕЙ
                patient.LastName = LastNameBox.Text;
                patient.FirstName = FirstNameBox.Text;
                patient.MiddleName = MiddleNameBox.Text;
                patient.BirthDate = BirthDatePicker.SelectedDate.Value;
                patient.PassportNumber = PassportBox.Text;
                patient.Phone = PhoneBox.Text;
                patient.Address = AddressBox.Text;

                string? genderStr = (GenderCombo.SelectedItem as ComboBoxItem)?.Content.ToString();
                patient.Gender = genderStr == "Мужской" ? Gender.Male : Gender.Female;

                // 1. Поступление
                // Если пользователь стер дату, ставим DateTime.Now
                DateTime admDate = AdmissionDatePicker.SelectedDate ?? DateTime.Now.Date;
                DateTime admTime = AdmissionTimePicker.SelectedTime ?? DateTime.Now;

                // Склеиваем: Дата (00:00) + Время
                patient.MedicalRecord.AdmissionDate = admDate.Date + admTime.TimeOfDay;

                // 2. Выписка
                if (DischargeDatePicker.SelectedDate != null)
                {
                    DateTime disDate = DischargeDatePicker.SelectedDate.Value;
                    // Если время не указали, пусть будет 12:00 (время выписки по стандарту) или текущее
                    DateTime disTime = DischargeTimePicker.SelectedTime ?? DateTime.Today.AddHours(12);

                    patient.MedicalRecord.DischargeDate = disDate.Date + disTime.TimeOfDay;
                }
                else
                    patient.MedicalRecord.DischargeDate = null;

                // ЛОГИКА СТАТУСА
                if (StatusCombo.SelectedIndex >= 0)
                {
                    var newStatus = (PatientStatus)StatusCombo.SelectedIndex;

                    // Если статус "Выписан", но дату выписки забыли поставить -> ставим текущую
                    if (newStatus == PatientStatus.Discharged && patient.MedicalRecord.DischargeDate == null)
                        patient.MedicalRecord.DischargeDate = DateTime.Now;

                    // Если статус НЕ "Выписан", но дата выписки стоит -> очищаем дату (чтобы не путать)
                    else if (newStatus != PatientStatus.Discharged && patient.MedicalRecord.DischargeDate != null)
                        patient.MedicalRecord.DischargeDate = null;

                    patient.Status = newStatus;
                }

                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}