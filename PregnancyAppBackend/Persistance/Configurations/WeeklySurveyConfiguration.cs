using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PregnancyAppBackend.Entities;

namespace PregnancyAppBackend.Persistance.Configurations;

public class WeeklySurveyConfiguration : IEntityTypeConfiguration<WeeklySurvey>
{
    public void Configure(EntityTypeBuilder<WeeklySurvey> builder)
    {
        builder.Property(x => x.OrviSymptoms)
               .HasMaxLength(1000);
        builder.Property(x => x.GynecologicalSymptoms)
               .HasMaxLength(1000);
        builder.Property(x => x.SwellingDescription)
               .HasMaxLength(1000);
        builder.HasOne(x => x.User)
               .WithMany(u => u.WeeklySurveys)
               .HasForeignKey(x => x.UserId);
    }
}