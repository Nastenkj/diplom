using PregnancyAppBackend.Hubs;
using PregnancyAppBackend.Services;
using PregnancyAppBackend.Services.AlgorithmicAnalysis;
using PregnancyAppBackend.Services.AuthenticationService;
using PregnancyAppBackend.Services.CommunicationLinkService;
using PregnancyAppBackend.Services.DailySurveysService;
using PregnancyAppBackend.Services.DoctorsService;
using PregnancyAppBackend.Services.Excel;
using PregnancyAppBackend.Services.MedicalHistoriesService;
using PregnancyAppBackend.Services.ObservationParameterNormService;
using PregnancyAppBackend.Services.PatientsService;
using PregnancyAppBackend.Services.StatisticsService;
using PregnancyAppBackend.Services.TokenService;
using PregnancyAppBackend.Services.UserInfoService;
using PregnancyAppBackend.Services.UserService;
using PregnancyAppBackend.Services.WeeklySurveyService;

namespace PregnancyAppBackend.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection HandleDependencyInjection(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserInfoService, UserInfoService>();
        services.AddScoped<IMedicalHistoriesService, MedicalHistoriesService>();
        services.AddScoped<IDailySurveysService, DailySurveysService>();
        services.AddScoped<IWeeklySurveysService, WeeklySurveysService>();
        services.AddScoped<IPatientsService, PatientsService>();
        services.AddScoped<IDoctorsService, DoctorsService>();
        services.AddScoped<ICommunicationLinkService, CommunicationLinkService>();
        
        services.AddScoped<INotificationService, NotificationService>();
        
        services.AddScoped<IStatisticsService, StatisticsService>();
        services.AddScoped<IObservationParameterNormService, ObservationParameterNormService>();
        services.AddScoped<IAlgorithmicAnalysisService, AlgorithmicAnalysisService>();
        services.AddScoped<IExcelExportService, ExcelExportService>();

        return services;
    }
}