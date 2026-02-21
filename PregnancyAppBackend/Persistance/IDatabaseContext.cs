using Microsoft.EntityFrameworkCore;
using PregnancyAppBackend.Entities;
using PregnancyAppBackend.Entities.Security;

namespace PregnancyAppBackend.Persistance;

public interface IDatabaseContext
{
    public DbSet<Parameter> Parameters { get; set; }
    DbSet<User> Users { get; set; }
    DbSet<Role> Roles { get; set; }
    DbSet<ApiClaim> ApiClaims { get; set; }

    DbSet<UserCommonInfo> UserCommonInfos { get; set; }
    DbSet<MedicalHistory> MedicalHistories { get; set; }
    DbSet<DailySurvey> DailySurveys { get; set; }
    DbSet<WeeklySurvey> WeeklySurveys { get; set; }
    DbSet<PatientDoctorCommunicationLink> PatientDoctorCommunicationLinks { get; set; }
    DbSet<ObservationParameterNorm> ObservationParameterNorms { get; set; }
    DbSet<AlgorithmicAnalysisParameterValue> AlgorithmicAnalysisParameterValues { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}