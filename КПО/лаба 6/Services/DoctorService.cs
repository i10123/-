using Hospital.Data;
using Hospital.Domain.Entities;
using Hospital.Domain.Entities.Staff;
using Hospital.Domain.Enums;

namespace Hospital.Services
{
    public class DoctorService
    {
        public static List<Patient> GetPatientsForDoctor(Doctor doctor, string search = "")
        {
            IEnumerable<Patient> query;

            if (doctor.IsDuty)
                // Дежурный видит поступивших
                query = Database.Patients.Where(p => p.Status == PatientStatus.Admitted);
            else
                // ЛЕЧАЩИЙ: Видит тех, кто в его отделении И уже переведен на лечение
                query = Database.Patients.Where(p =>
                    p.CurrentDepartment == doctor.Department &&
                    p.Status == PatientStatus.InDepartment);

            // Поиск
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(p =>
                    p.LastName.Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                    p.FirstName.Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                    p.PassportNumber.Contains(search) ||
                    p.DiagnosisRu.Contains(search, StringComparison.CurrentCultureIgnoreCase)
                );
            }

            return [.. query.OrderBy(p => p.LastName)];
        }

        public static string TransferToDepartment(Patient patient, DepartmentType department)
        {
            // 1. Ищем всех врачей этого отделения (кроме дежурных)
            var doctorsInDept = Database.Users.OfType<Doctor>()
                .Where(d => d.Department == department && !d.IsDuty).ToList();

            Doctor? selectedDoctor = null;

            if (doctorsInDept.Count > 0)
            {
                // 2. Сортируем их по количеству АКТИВНЫХ пациентов
                selectedDoctor = doctorsInDept
                    .OrderBy(d => Database.Patients.Count(p =>
                        p.AttendingDoctorId == d.Id &&
                        p.Status == PatientStatus.InDepartment))
                    .First();
            }

            // 3. Обновляем пациента
            patient.CurrentDepartment = department;
            patient.Status = PatientStatus.InDepartment;
            patient.AttendingDoctorId = selectedDoctor?.Id; // Может быть null, если врачей нет

            Database.SavePatients();

            return selectedDoctor != null
                ? $"Назначен врач: {selectedDoctor.FullName}"
                : "Внимание: Врач не назначен (в отделении нет врачей)";
        }

        // Выписка
        public static void DischargePatient(Patient patient)
        {
            patient.Status = PatientStatus.Discharged;
            patient.MedicalRecord.DischargeDate = DateTime.Now;
            Database.SavePatients();
        }
    }
}