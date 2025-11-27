using Hospital.Data;
using Hospital.Domain.Entities;
using Hospital.Domain.Entities.Base;

namespace Hospital.Services
{
    public class AuthService
    {
        public static User? Login(string login, string password)
        {
            string passwordHash = Database.HashPassword(password);
            var user = Database.Users.FirstOrDefault(u => u.Login == login && u.Password == passwordHash);

            // Создаем запись для истории
            var record = new LoginRecord
            {
                Login = login,
                Timestamp = DateTime.Now
            };

            if (user != null)
            {
                user.LastLoginDate = DateTime.Now;
                Database.SaveUsers();

                // Успешный вход
                record.IsSuccess = true;
                record.Role = user.Role.ToString();
                Database.AddLoginRecord(record); // Пишем в файл

                return user;
            }

            // Неудачный вход
            record.IsSuccess = false;
            record.Role = "Unknown";
            Database.AddLoginRecord(record); // Тоже пишем, чтобы админ видел попытки взлома

            return null;
        }
    }
}