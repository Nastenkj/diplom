using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PregnancyAppBackend.Entities;

namespace PregnancyAppBackend.Persistance.Configurations;

public class DailySurveyConfiguration : IEntityTypeConfiguration<DailySurvey>
{
    public void Configure(EntityTypeBuilder<DailySurvey> builder)
    {
        builder.Property(x => x.SystolicPressure)
               .IsRequired();
        
        builder.Property(x => x.DiastolicPressure)
               .IsRequired();

        builder.ToTable(t => t.HasCheckConstraint("CK_DailySurvey_BloodPressure",
                                                  "[BloodPressure] LIKE '[0-9][0-9]/[0-9][0-9]' OR " +
                                                  "[BloodPressure] LIKE '[0-9][0-9][0-9]/[0-9][0-9]' OR " +
                                                  "[BloodPressure] LIKE '[0-9][0-9]/[0-9][0-9][0-9]' OR " +
                                                  "[BloodPressure] LIKE '[0-9][0-9][0-9]/[0-9][0-9][0-9]'"));

        builder.Property(x => x.AdditionalInformation)
               .HasMaxLength(2000);

        builder.HasOne(x => x.User)
               .WithMany(u => u.DailySurveys)
               .HasForeignKey(x => x.UserId);
    }
}