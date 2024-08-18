using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SimarAlertNotifier.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class Subscriber
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
