using Hospital.Data;
using Hospital.Domain.Entities;
using Hospital.Domain.Entities.Base;
using Hospital.Domain.Entities.Staff;
using Hospital.Domain.Enums;

namespace Hospital.Services
{
    public class AdminService
    {
        public static List<User> GetStaff(string search = "")
        {
            var query = Database.Users.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(u =>
                    u.LastName.Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                    u.FirstName.Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                    u.Login.Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                    (u is Doctor d && d.DepartmentRu.Contains(search, StringComparison.CurrentCultureIgnoreCase))
                );
            }

            return [.. query];
        }

        public static void AddUser(User user)
        {
            if (Database.Users.Any(u => u.Login == user.Login))
                throw new Exception("Логин занят!");

            Database.Users.Add(user);
            Database.SaveUsers();
        }

        public static void UpdateUser() => Database.SaveUsers();

        public static void DeleteUser(Guid id)
        {
            var user = Database.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                Database.Users.Remove(user);
                Database.SaveUsers();
            }
        }

        // --- ПАЦИЕНТЫ ---
        public static List<Patient> GetPatients(bool showDischarged, string search = "")
        {
            // Фильтр: Выписанные или Текущие
            var query = showDischarged
                ? Database.Patients.Where(p => p.Status == PatientStatus.Discharged)
                : Database.Patients.Where(p => p.Status != PatientStatus.Discharged);

            // Поиск
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(p =>
                    p.LastName.Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                    p.FirstName.Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                    p.PassportNumber.Contains(search) ||
                    p.Phone.Contains(search) ||
                    p.Address.Contains(search, StringComparison.CurrentCultureIgnoreCase)
                );
            }

            // Сортировка по фамилии
            return [.. query.OrderBy(p => p.LastName)];
        }

        public static void DeletePatient(Guid id)
        {
            var patient = Database.Patients.FirstOrDefault(p => p.Id == id);
            if (patient != null)
            {
                Database.Patients.Remove(patient);
                Database.SavePatients();
            }
        }

        public static void UpdatePatient() => Database.SavePatients();

        public static void AddPatient(Patient patient)
        {
            Database.Patients.Add(patient);
            Database.SavePatients();
        }
    }
}