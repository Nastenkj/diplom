using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PregnancyAppBackend.Entities;

namespace PregnancyAppBackend.Persistance.Configurations;

public class AlgorithmicAnalysisParameterValueConfiguration : IEntityTypeConfiguration<AlgorithmicAnalysisParameterValue>
{
    public void Configure(EntityTypeBuilder<AlgorithmicAnalysisParameterValue> builder)
    {
        builder.HasIndex(x => new { x.UserId, x.ParameterId })
               .IsUnique();
    }
}