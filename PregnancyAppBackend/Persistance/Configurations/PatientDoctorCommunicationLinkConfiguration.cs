using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PregnancyAppBackend.Entities;

namespace PregnancyAppBackend.Persistance.Configurations;

public class PatientDoctorCommunicationLinkConfiguration : IEntityTypeConfiguration<PatientDoctorCommunicationLink>
{
    public void Configure(EntityTypeBuilder<PatientDoctorCommunicationLink> builder)
    {
       builder.HasKey(t => new { t.UserId, t.DoctorId });

       builder
         .HasOne(pdcl => pdcl.User)
         .WithMany()
         .HasForeignKey(pdcl => pdcl.UserId)
         .OnDelete(DeleteBehavior.Restrict);

       builder
         .HasOne(pdcl => pdcl.Doctor)
         .WithMany()
         .HasForeignKey(pdcl => pdcl.DoctorId)
         .OnDelete(DeleteBehavior.Restrict);

       builder
         .Property(pdcl => pdcl.CommunicationLink)
         .IsRequired();
        
       builder
         .Property(pdcl => pdcl.CreatedAtUtc)
         .HasDefaultValueSql("GETUTCDATE()");
        
       builder
         .Property(pdcl => pdcl.MeetingScheduledAtUtc)
         .IsRequired();
    }
}