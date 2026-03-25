using System.ComponentModel.DataAnnotations;

namespace RentEase.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }

        [Required]
        public string SenderId { get; set; } = string.Empty;
        public ApplicationUser? Sender { get; set; }

        [Required]
        public string ReceiverId { get; set; } = string.Empty;
        public ApplicationUser? Receiver { get; set; }

        public int PropertyId { get; set; }
        public Property? Property { get; set; }

        [Required]
        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; }
        public bool IsPhoneRequested { get; set; }
        public bool IsPhoneShared { get; set; }
        public string? SharedPhone { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
