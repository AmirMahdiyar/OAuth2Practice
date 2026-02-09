using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OAuthPractice.Entity;

namespace OAuthPractice.Database.Configuration
{
    public class UserAuthenticationsConfiguration : IEntityTypeConfiguration<UserAuthentications>
    {
        public void Configure(EntityTypeBuilder<UserAuthentications> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(a => a.ProviderKey)
                .IsRequired();

            builder.Property(a => a.Providers)
                   .HasConversion<string>();
        }
    }
}
