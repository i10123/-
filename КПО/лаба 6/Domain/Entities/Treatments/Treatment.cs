using System.Text.Json.Serialization;

namespace Hospital.Domain.Entities.Treatments
{
    [JsonDerivedType(typeof(Medication), typeDiscriminator: "medication")]
    [JsonDerivedType(typeof(Procedure), typeDiscriminator: "procedure")]
    [JsonDerivedType(typeof(Diagnostic), typeDiscriminator: "diagnostic")]
    public abstract class Treatment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty; // Название
        public DateTime DatePrescribed { get; set; } = DateTime.Now; // Дата назначения
        public Guid DoctorId { get; set; } // ID и Имя врача, который назначил
        public string DoctorName { get; set; } = string.Empty;
        public bool IsCompleted { get; set; } = false; // Статус выполнения
        public string ExecutionResult { get; set; } = string.Empty; // кто и когда выполнил

        public abstract string GetInfo();

        [JsonIgnore]
        public string InfoText => GetInfo();
    }
}