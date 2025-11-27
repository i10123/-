using Hospital.Domain.Entities.Treatments;

namespace Hospital.Domain.Entities
{
    public class MedicalRecord
    {
        public DateTime AdmissionDate { get; set; } = DateTime.Now;
        public DateTime? DischargeDate { get; set; }
        public string? Diagnosis { get; set; }
        public string Anamnesis { get; set; } = "";
        public List<Treatment> Treatments { get; set; } = [];
    }
}