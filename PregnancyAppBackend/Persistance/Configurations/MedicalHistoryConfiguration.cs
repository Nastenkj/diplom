using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PregnancyAppBackend.Entities;
using PregnancyAppBackend.Enums.MedicalHistory;

namespace PregnancyAppBackend.Persistance.Configurations;

public class MedicalHistoryConfiguration : IEntityTypeConfiguration<MedicalHistory>
{
    public void Configure(EntityTypeBuilder<MedicalHistory> builder)
    {
        builder.Property(x => x.BloodGroup)
               .HasDefaultValue(BloodGroup.I);
           
        builder.Property(x => x.BloodPressure)
               .HasMaxLength(10)
               .IsRequired();
        builder.Property(x => x.AllergicReactions)
               .HasMaxLength(500);
        builder.Property(x => x.GynecologicalDiseases)
               .HasMaxLength(1000);
        builder.Property(x => x.SomaticDiseases)
               .HasMaxLength(1000);
        builder.Property(x => x.UndergoneOperations)
               .HasMaxLength(1000);
        builder.Property(x => x.HereditaryDiseasesString)
               .HasMaxLength(1000);

        builder.HasOne(x => x.User)
               .WithMany()
               .HasForeignKey(x => x.UserId);
        
        builder.Ignore(m => m.HereditaryDiseases)
               .Property(m => m.HereditaryDiseasesString)
               .HasColumnName("HereditaryDiseases");
        
        builder.HasIndex(x => x.UserId)
               .IsUnique();
    }
}