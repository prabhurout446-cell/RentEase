using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentEase.Data;
using RentEase.Models;
using RentEase.ViewModels;

namespace RentEase.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChatController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Conversation(int propertyId, string otherUserId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return RedirectToAction("Login", "Account");

            var otherUser = await _userManager.FindByIdAsync(otherUserId);
            if (otherUser == null) return NotFound();

            var property = await _context.Properties
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.Id == propertyId);
            if (property == null) return NotFound();

            // Mark messages as read
            var unread = await _context.ChatMessages
                .Where(m => m.PropertyId == propertyId &&
                            m.SenderId == otherUserId &&
                            m.ReceiverId == currentUser.Id &&
                            !m.IsRead)
                .ToListAsync();
            unread.ForEach(m => m.IsRead = true);
            await _context.SaveChangesAsync();

            var messages = await _context.ChatMessages
                .Include(m => m.Sender)
                .Where(m => m.PropertyId == propertyId &&
                            ((m.SenderId == currentUser.Id && m.ReceiverId == otherUserId) ||
                             (m.SenderId == otherUserId && m.ReceiverId == currentUser.Id)))
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            var vm = new ChatViewModel
            {
                PropertyId = propertyId,
                Property = property,
                OtherUserId = otherUserId,
                OtherUserName = otherUser.FullName,
                Messages = messages
            };

            return View(vm);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(int propertyId, string receiverId, string message)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();

            if (string.IsNullOrWhiteSpace(message))
                return RedirectToAction("Conversation", new { propertyId, otherUserId = receiverId });

            _context.ChatMessages.Add(new ChatMessage
            {
                SenderId = currentUser.Id,
                ReceiverId = receiverId,
                PropertyId = propertyId,
                Message = message.Trim(),
                SentAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            return RedirectToAction("Conversation", new { propertyId, otherUserId = receiverId });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestPhone(int propertyId, string ownerId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();

            _context.ChatMessages.Add(new ChatMessage
            {
                SenderId = currentUser.Id,
                ReceiverId = ownerId,
                PropertyId = propertyId,
                Message = "📞 I'd like to get your phone number for further discussion.",
                IsPhoneRequested = true,
                SentAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            return RedirectToAction("Conversation", new { propertyId, otherUserId = ownerId });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SharePhone(int messageId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();

            var original = await _context.ChatMessages.FindAsync(messageId);
            if (original == null || original.ReceiverId != currentUser.Id) return Forbid();

            _context.ChatMessages.Add(new ChatMessage
            {
                SenderId = currentUser.Id,
                ReceiverId = original.SenderId,
                PropertyId = original.PropertyId,
                Message = $"📞 My phone number: {currentUser.PhoneNumber}",
                IsPhoneShared = true,
                SharedPhone = currentUser.PhoneNumber,
                SentAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            return RedirectToAction("Conversation", new { propertyId = original.PropertyId, otherUserId = original.SenderId });
        }
        public async Task<IActionResult> Inbox()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return RedirectToAction("Login", "Account");

            var allMessages = await _context.ChatMessages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Include(m => m.Property)
                .Where(m => m.SenderId == currentUser.Id || m.ReceiverId == currentUser.Id)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();

            var conversations = allMessages
                .GroupBy(m => new
                {
                    OtherUserId = m.SenderId == currentUser.Id ? m.ReceiverId : m.SenderId,
                    m.PropertyId
                })
                .Select(g =>
                {
                    var last = g.First();
                    var otherUser = last.SenderId == currentUser.Id ? last.Receiver : last.Sender;
                    return new ConversationSummary
                    {
                        OtherUserId = g.Key.OtherUserId,
                        OtherUserName = otherUser?.FullName ?? "Unknown",
                        PropertyId = g.Key.PropertyId,
                        PropertyTitle = last.Property?.Title ?? "",
                        LastMessage = last.Message.Length > 60 ? last.Message[..60] + "…" : last.Message,
                        LastMessageTime = last.SentAt,
                        UnreadCount = g.Count(m => m.ReceiverId == currentUser.Id && !m.IsRead)
                    };
                })
                .OrderByDescending(c => c.LastMessageTime)
                .ToList();

            return View(new InboxViewModel { Conversations = conversations });
        }
        [HttpGet]
        public async Task<IActionResult> UnreadCount()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Json(0);
            var count = await _context.ChatMessages.CountAsync(m => m.ReceiverId == currentUser.Id && !m.IsRead);
            return Json(count);
        }
    }
}
