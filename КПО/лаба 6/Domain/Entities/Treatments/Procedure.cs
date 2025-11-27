namespace Hospital.Domain.Entities.Treatments
{
    public class Procedure : Treatment
    {
        public override string GetInfo() => $"Процедура: {Name}";
    }
}