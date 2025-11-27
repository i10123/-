using System.Text.Json.Serialization;
using Hospital.Domain.Enums;

namespace Hospital.Domain.Entities
{
    public class Patient
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        private string lastName = string.Empty;
        public string LastName
        {
            get => lastName;
            set { 
                if (value.Any(char.IsDigit)) 
                    throw new ArgumentException("Цифры в фамилии!");

                lastName = value; 
            }
        }

        private string firstName = string.Empty;
        public string FirstName
        {
            get => firstName;
            set { 
                if (value.Any(char.IsDigit)) 
                    throw new ArgumentException("Цифры в имени!");

                firstName = value; 
            }
        }

        private string middleName = string.Empty;
        public string MiddleName
        {
            get => middleName;
            set { 
                if (value.Any(char.IsDigit)) 
                    throw new ArgumentException("Цифры в отчестве!");

                middleName = value; 
            }
        }

        public DateTime BirthDate { get; set; }
        public string PassportNumber { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public Gender? Gender { get; set; } = null;

        [JsonIgnore]
        public string GenderRu => Gender.HasValue
            ? (Gender.Value == Enums.Gender.Male ? "Мужской" : "Женский") : "Не указан";

        public PatientStatus Status { get; set; } = PatientStatus.Admitted;
        public DepartmentType CurrentDepartment { get; set; } = DepartmentType.Reception;
        public Guid? AttendingDoctorId { get; set; }

        public MedicalRecord MedicalRecord { get; set; } = new MedicalRecord();

        public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();

        [JsonIgnore]
        public string DaysInHospital
        {
            get
            {
                if (Status == PatientStatus.Discharged && MedicalRecord.DischargeDate.HasValue)
                {
                    var days = (MedicalRecord.DischargeDate.Value - MedicalRecord.AdmissionDate).Days;
                    return days == 0 ? "1 день" : $"{days} дн.";
                }
                return "-";
            }
        }

        [JsonIgnore]
        public string StatusRu => Status switch
        {
            PatientStatus.Admitted => "Поступил",
            PatientStatus.InDepartment => "На лечении",
            PatientStatus.Discharged => "Выписан",
            _ => "-"
        };

        [JsonIgnore]
        public string DiagnosisRu => MedicalRecord?.Diagnosis ?? "Нет диагноза";
    }
}