namespace Hospital.Domain.Entities
{
    public class LoginRecord
    {
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Login { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
    }
}