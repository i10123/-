using Hospital.Domain.Entities.Base;
using Hospital.Domain.Enums;

namespace Hospital.Domain.Entities.Staff
{
    public class Nurse : User
    {
        public DepartmentType Department { get; set; } // Отделение
        public Nurse() => Role = Role.Nurse;

        public override string DepartmentRu => Department switch
        {
            DepartmentType.Reception => "Приемное",
            DepartmentType.Cardiology => "Кардиология",
            DepartmentType.Surgery => "Хирургия",
            DepartmentType.Neurology => "Неврология",
            _ => "-"
        };
    }
}