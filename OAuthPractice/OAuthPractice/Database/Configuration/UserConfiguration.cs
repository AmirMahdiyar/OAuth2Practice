using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OAuthPractice.Entity;

namespace OAuthPractice.Database.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(50);


            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(50);


            builder.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(100);

            builder.OwnsOne(u => u.ContactInformation, contact =>
            {
                contact.Property(c => c.Gmail)
                .HasColumnName("Email")
                .IsRequired();
                contact.Property(c => c.PhoneNumber)
                .HasColumnName("PhoneNumber")
                .IsRequired(false);
            });

            builder.HasMany(u => u.Authentications)
                   .WithOne()
                   .HasForeignKey(a => a.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
