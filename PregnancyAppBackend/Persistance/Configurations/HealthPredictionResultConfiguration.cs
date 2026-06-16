using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PregnancyAppBackend.Entities;

namespace PregnancyAppBackend.Persistance.Configurations;

public class HealthPredictionResultConfiguration : IEntityTypeConfiguration<HealthPredictionResult>
{
    public void Configure(EntityTypeBuilder<HealthPredictionResult> builder)
    {
        builder.Property(x => x.PredictionText)
               .HasMaxLength(5000);

        builder.HasMany(x => x.Deviations)
               .WithOne(x => x.HealthPredictionResult!)
               .HasForeignKey(x => x.HealthPredictionResultId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.CreatedAtUtc)
               .IsRequired();

        builder.Property(x => x.Trimester)
               .IsRequired();

        builder.Property(x => x.Prediction)
               .IsRequired();

        builder.Property(x => x.NormalProbability).IsRequired();
        builder.Property(x => x.AlertProbability).IsRequired();
        builder.Property(x => x.PathologyProbability).IsRequired();

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.DailySurveyId);

        builder.HasIndex(x => new { x.UserId, x.DailySurveyId }).IsUnique();
    }
}
