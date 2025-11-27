using Hospital.Domain.Entities.Base;
using Hospital.Domain.Enums;

namespace Hospital.Domain.Entities.Staff
{
    public class Doctor : User
    {
        public DepartmentType Department { get; set; } // // Отделение
        public bool IsDuty { get; set; } // // Дежурный или нет
        public Doctor() => Role = Role.Doctor;

        public override string DepartmentRu => Department switch
        {
            DepartmentType.Reception => "Приемное",
            DepartmentType.Cardiology => "Кардиология",
            DepartmentType.Surgery => "Хирургия",
            DepartmentType.Neurology => "Неврология",
            _ => "-"
        };

        public override string IsDutyVisible => "Visible";
    }
}