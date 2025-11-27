namespace Hospital.Domain.Entities.Treatments
{
    public class Medication : Treatment
    {
        public string Type { get; set; } = string.Empty; // "Таблетки" или "Инъекции"
        public string Dosage { get; set; } = string.Empty; // доза
        public int Count { get; set; } // Количество раз или штук
        public override string GetInfo() => $"{Type}: {Name}, Доза: {Dosage}, Кол-во: {Count}";
    }
}