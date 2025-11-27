using System.Text.Json.Serialization;
using Hospital.Domain.Entities.Staff;
using Hospital.Domain.Enums;

namespace Hospital.Domain.Entities.Base
{
    [JsonDerivedType(typeof(Administrator), typeDiscriminator: "admin")]
    [JsonDerivedType(typeof(Doctor), typeDiscriminator: "doctor")]
    [JsonDerivedType(typeof(Nurse), typeDiscriminator: "nurse")]
    public abstract class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        private string login = string.Empty;
        public string Login
        {
            get => login;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Логин не может быть пустым!");

                if (value.Length < 4 || value.Length > 16)
                    throw new ArgumentException("Логин должен быть от 4 до 16 символов!");

                login = value;
            }
        }

        public string Password { get; set; } = string.Empty;

        private string last_Name = string.Empty;
        public string LastName
        {
            get => last_Name;
            set
            {
                if (HasDigits(value)) 
                    throw new ArgumentException("Фамилия не может содержать цифры!");

                last_Name = value;
            }
        }

        private string first_Name = string.Empty;
        public string FirstName
        {
            get => first_Name;
            set
            {
                if (HasDigits(value)) 
                    throw new ArgumentException("Имя не может содержать цифры!");

                first_Name = value;
            }
        }

        private string middle_Name = string.Empty;
        public string MiddleName
        {
            get => middle_Name;
            set
            {
                if (HasDigits(value)) 
                    throw new ArgumentException("Отчество не может содержать цифры!");

                middle_Name = value;
            }
        }

        public Role Role { get; set; }
        public DateTime? LastLoginDate { get; set; }

        public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();

        // виден только внутри этого класса и его наследников
        protected static bool HasDigits(string value)
        {
            return value.Any(char.IsDigit);
        }


        [JsonIgnore]
        public string RoleRu => Role switch
        {
            Role.Admin => "Администратор",
            Role.Doctor => "Врач",
            Role.Nurse => "Медсестра",
            _ => Role.ToString()
        };

        // virtual — ключевое слово. Оно означает: "У базового юзера (админа) отделения нет ("-")
        [JsonIgnore]
        public virtual string DepartmentRu => "-";

        [JsonIgnore]
        public virtual string IsDutyVisible => "Hidden";
    }
}