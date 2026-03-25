using Microsoft.AspNetCore.Identity;

namespace RentEase.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string? ProfilePicture { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Property> Properties { get; set; } = new List<Property>();
        public ICollection<ChatMessage> SentMessages { get; set; } = new List<ChatMessage>();
        public ICollection<ChatMessage> ReceivedMessages { get; set; } = new List<ChatMessage>();
    }
}
