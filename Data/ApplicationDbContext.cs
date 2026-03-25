using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RentEase.Models;

namespace RentEase.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyImage> PropertyImages { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Property>(entity =>
            {
                entity.HasOne(p => p.Owner)
                    .WithMany(u => u.Properties)
                    .HasForeignKey(p => p.OwnerId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(p => p.RentAmount).HasColumnType("TEXT");
                entity.Property(p => p.DepositAmount).HasColumnType("TEXT");
            });

            builder.Entity<ChatMessage>(entity =>
            {
                entity.HasOne(m => m.Sender)
                    .WithMany(u => u.SentMessages)
                    .HasForeignKey(m => m.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.Receiver)
                    .WithMany(u => u.ReceivedMessages)
                    .HasForeignKey(m => m.ReceiverId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.Property)
                    .WithMany(p => p.ChatMessages)
                    .HasForeignKey(m => m.PropertyId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<PropertyImage>(entity =>
            {
                entity.HasOne(i => i.Property)
                    .WithMany(p => p.Images)
                    .HasForeignKey(i => i.PropertyId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
