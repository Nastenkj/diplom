using Microsoft.EntityFrameworkCore;
using PregnancyAppBackend.Entities;
using PregnancyAppBackend.Entities.Security;
using PregnancyAppBackend.Persistance.Configurations;

namespace PregnancyAppBackend.Persistance;

public class DatabaseContext : DbContext, IDatabaseContext
{
    public DatabaseContext()
    {
        
    }
    public DatabaseContext(DbContextOptions options) : base(options)
    {
        
    }
    public DbSet<Parameter> Parameters { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserCommonInfo> UserCommonInfos { get; set; } = null!;
    public DbSet<MedicalHistory> MedicalHistories { get; set; } = null!;
    public DbSet<DailySurvey> DailySurveys { get; set; } = null!;
    public DbSet<WeeklySurvey> WeeklySurveys { get; set; } = null!;
    public DbSet<PatientDoctorCommunicationLink> PatientDoctorCommunicationLinks { get; set; } = null!;
    public DbSet<ObservationParameterNorm> ObservationParameterNorms { get; set; } = null!;
    public DbSet<AlgorithmicAnalysisParameterValue> AlgorithmicAnalysisParameterValues { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<ApiClaim> ApiClaims { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureRoleAuthEntities(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new UserCommonInfoConfiguration());
        modelBuilder.ApplyConfiguration(new MedicalHistoryConfiguration());
        modelBuilder.ApplyConfiguration(new PatientDoctorCommunicationLinkConfiguration());
        modelBuilder.ApplyConfiguration(new DailySurveyConfiguration());
        modelBuilder.ApplyConfiguration(new WeeklySurveyConfiguration());
        modelBuilder.ApplyConfiguration(new AlgorithmicAnalysisParameterValueConfiguration());

        DisableCascadeDeletion(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private void DisableCascadeDeletion(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var foreignKey in entityType.GetForeignKeys())
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }

    private void ConfigureRoleAuthEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>()
                    .HasMany(r => r.ApiClaims)
                    .WithMany(ac => ac.Roles)
                    .UsingEntity<RoleApiClaim>(
                         etb => etb
                               .HasOne(rl => rl.ApiClaim)
                               .WithMany(ac => ac.RoleApiClaims)
                               .HasForeignKey(rc => rc.ApiClaimId),
                         etb => etb
                               .HasOne(rc => rc.Role)
                               .WithMany(r => r.RoleApiClaims)
                               .HasForeignKey(rc => rc.RoleId),
                         etb =>
                         {
                             etb.HasKey(rc => new { rc.ApiClaimId, rc.RoleId });
                             etb.ToTable("RoleApiClaims");
                         }
                     );
        modelBuilder.Entity<User>()
                    .HasMany(u => u.Roles)
                    .WithMany(r => r.Users)
                    .UsingEntity<UserRole>(
                         etb => etb
                               .HasOne(ur => ur.Role)
                               .WithMany(r => r.UserRoles)
                               .HasForeignKey(ur => ur.RoleId),
                         etb => etb
                               .HasOne(ur => ur.User)
                               .WithMany(u => u.UserRoles)
                               .HasForeignKey(ur => ur.UserId),
                         etb =>
                         {
                             etb.HasKey(ur => new { ur.RoleId, ur.UserId });
                             etb.ToTable("UserRoles");
                         }
                     );
    }
}