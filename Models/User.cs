namespace VulnerableApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? P_key { get; set; } // Renombrado
        public string? Email { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
