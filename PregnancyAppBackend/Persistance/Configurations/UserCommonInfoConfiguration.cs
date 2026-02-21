using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PregnancyAppBackend.Entities;

namespace PregnancyAppBackend.Persistance.Configurations;

public class UserCommonInfoConfiguration : IEntityTypeConfiguration<UserCommonInfo>
{
    public void Configure(EntityTypeBuilder<UserCommonInfo> builder)
    { 
        builder.Property(x => x.FullName)
               .HasMaxLength(200);
        
        builder.Property(x => x.PhoneNumber)
               .HasMaxLength(20);
        
        builder.Property(x => x.TrustedPersonFullName)
               .HasMaxLength(200);
        
        builder.Property(x => x.TrustedPersonPhoneNumber)
               .HasMaxLength(20);
        
        builder.Property(x => x.TrustedPersonEmail)
               .HasMaxLength(100);
        
        builder.Property(x => x.InsuranceNumber)
               .HasMaxLength(50);

        builder.HasOne(x => x.User)
               .WithOne()
               .HasForeignKey<UserCommonInfo>(x => x.UserId);
    }
}