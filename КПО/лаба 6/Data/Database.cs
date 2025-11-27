using Hospital.Domain.Entities;
using Hospital.Domain.Entities.Base;
using Hospital.Domain.Entities.Staff;
using Hospital.Domain.Enums;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Hospital.Data
{
    public static class Database
    {
        private static readonly string users_in_File = "users.json";
        private static readonly string patients_in_File = "patients.json";
        private static readonly string loginsFile = "logins.json";

        public static List<User> Users { get; set; } = [];
        public static List<Patient> Patients { get; set; } = [];
        public static List<LoginRecord> LoginHistory { get; set; } = [];

        public static void Load()
        {
            Users = LoadFromFile<User>(users_in_File);
            Patients = LoadFromFile<Patient>(patients_in_File);
            LoginHistory = LoadFromFile<LoginRecord>(loginsFile);
            CheckORCreateAdmin();
        }

        public static void SaveUsers() => SaveToFile(users_in_File, Users);
        public static void SavePatients() => SaveToFile(patients_in_File, Patients);

        public static void AddLoginRecord(LoginRecord record)
        {
            LoginHistory.Add(record);
            SaveToFile(loginsFile, LoginHistory);
        }

        private static readonly JsonSerializerOptions jsonOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        private static List<T> LoadFromFile<T>(string path)
        {
            if (!File.Exists(path))
                return [];

            var json = File.ReadAllText(path);

            if (string.IsNullOrWhiteSpace(json))
                return [];

            return JsonSerializer.Deserialize<List<T>>(json, jsonOptions) ?? [];
        }

        private static void SaveToFile<T>(string path, List<T> data)
        {
            var json = JsonSerializer.Serialize(data, jsonOptions);
            File.WriteAllText(path, json);
        }

        private static void CheckORCreateAdmin()
        {
            if (!Users.Any(u => u.Role == Role.Admin))
            {
                var admin = new Administrator
                {
                    Login = "admin",
                    Password = HashPassword("admin"),
                    LastName = "Каракулько",
                    FirstName = "Денис",
                    MiddleName = "Александрович"
                };
                Users.Add(admin);
                SaveUsers();
            }
        }

        // метод хеширования (SHA256)
        public static string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password)); // берет строку, превращает её в массив байт и "перемалывает"
            var builder = new StringBuilder();
            foreach (var b in bytes)
            {
                // "x" означает шестнадцатеричная система
                // "2" означает, что если число маленькое например, 5, то оно должно записаться как "05"
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }
}