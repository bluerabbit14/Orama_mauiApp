using System.ComponentModel.DataAnnotations;

namespace Orama_App_API.Models
{
    public class PasswordHistory
    {
        public int Id { get; set; }
        
        public int UserId { get; set; }
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation property
        public virtual User User { get; set; } = null!;
    }
}
