using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PregnancyAppBackend.Entities;

namespace PregnancyAppBackend.Persistance.Configurations;

public class ObservationParameterNormsConfiguration : IEntityTypeConfiguration<ObservationParameterNorm>
{
    public void Configure(EntityTypeBuilder<ObservationParameterNorm> builder)
    {
        builder.Property(x => x.ParameterId)
               .IsRequired();
    }
}