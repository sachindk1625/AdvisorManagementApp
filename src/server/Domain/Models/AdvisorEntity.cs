namespace Domain.Models
{
    public class AdvisorEntity
    {
        public Guid Id { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string SIN { get; set; } = default!;
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string HealthStatus { get; set; } = default!;
    }
}
