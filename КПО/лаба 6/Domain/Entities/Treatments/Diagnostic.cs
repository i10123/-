namespace Hospital.Domain.Entities.Treatments
{
    public class Diagnostic : Treatment
    {
        public string Results { get; set; } = "Ожидание результата";
        public override string GetInfo() => $"Диагностика: {Name}";
    }
}