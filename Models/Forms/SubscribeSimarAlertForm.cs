using System.ComponentModel.DataAnnotations;

namespace SimarAlertNotifier.Models.Forms
{
    public class SubscribeSimarAlertForm
    {
        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string email { get; set; } = string.Empty;

        [Required]
        [MaxLength(3)]
        public string operation { get; set; } = "add";
    }
}
