using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace PregnancyAppBackend.Infrastructure.Security;

public static class AuthenticationExtensions
{
    private static Action<AuthorizationOptions> AddTest =>
        options =>
        {
            options.AddPolicy(Policies.Test, b => b.RequireClaim(Claims.TestClaim));
            options.AddPolicy(Policies.UnusedPolicy, b => b.RequireClaim(Claims.UnusedPolicy));
        };
    
    
    private static Action<AuthorizationOptions> AddDailySurveys =>
        options =>
        {
            options.AddPolicy(Policies.GetLatestDailySurveyCreationDateUtc, 
                              b => b.RequireClaim(Claims.GetLatestDailySurveyCreationDateUtc));
            options.AddPolicy(Policies.PostDailySurvey, 
                              b => b.RequireClaim(Claims.PostDailySurvey));
            options.AddPolicy(Policies.GetDailySurveyById, 
                              b => b.RequireClaim(Claims.GetDailySurveyById));
            options.AddPolicy(Policies.GetDailySurveysForUser, 
                              b => b.RequireClaim(Claims.GetDailySurveysForUser));
        };

    private static Action<AuthorizationOptions> AddMedicalHistories =>
        options =>
        {
            options.AddPolicy(Policies.GetMedicalHistory, 
                              b => b.RequireClaim(Claims.GetMedicalHistory));
            options.AddPolicy(Policies.PostMedicalHistory, 
                              b => b.RequireClaim(Claims.PostMedicalHistory));
            options.AddPolicy(Policies.GetMedicalHistoryById, 
                              b => b.RequireClaim(Claims.GetMedicalHistoryById));
        };

    private static Action<AuthorizationOptions> AddWeeklySurveys =>
        options =>
        {
            options.AddPolicy(Policies.PostWeeklySurvey, 
                              b => b.RequireClaim(Claims.PostWeeklySurvey));
            options.AddPolicy(Policies.GetLatestWeeklySurvey, 
                              b => b.RequireClaim(Claims.GetLatestWeeklySurvey));
            options.AddPolicy(Policies.GetWeeklySurveyById, 
                              b => b.RequireClaim(Claims.GetWeeklySurveyById));
            options.AddPolicy(Policies.GetWeeklySurveysForUser, 
                              b => b.RequireClaim(Claims.GetWeeklySurveysForUser));
        };
    
    private static Action<AuthorizationOptions> AddPatients =>
        options =>
        {
            options.AddPolicy(Policies.GetAllPatients, 
                              b => b.RequireClaim(Claims.GetAllPatients));
            options.AddPolicy(Policies.GetPatient, 
                              b => b.RequireClaim(Claims.GetPatient));
            options.AddPolicy(Policies.EditPatientInfo, 
                              b => b.RequireClaim(Claims.EditPatientInfo));
        };
    
    private static Action<AuthorizationOptions> AddDoctors =>
        options =>
        {
            options.AddPolicy(Policies.GetAllDoctors, 
                              b => b.RequireClaim(Claims.GetAllDoctors));
            options.AddPolicy(Policies.GetDoctor, 
                              b => b.RequireClaim(Claims.GetDoctor));
            options.AddPolicy(Policies.EditDoctorInfo, 
                              b => b.RequireClaim(Claims.EditDoctorInfo));
        };
    
    private static Action<AuthorizationOptions> AddCommunicationLinks =>
        options =>
        {
            options.AddPolicy(Policies.CreateCommunicationLink, 
                              b => b.RequireClaim(Claims.CreateCommunicationLink));
            options.AddPolicy(Policies.GetMyCommunicationLinks, 
                              b => b.RequireClaim(Claims.GetMyCommunicationLinks));
        };

    public static IServiceCollection AddAuthenticationCustom(this IServiceCollection services, IConfiguration config)
    {
        var authCfg = new AuthConfiguration();
        config.Bind(nameof(AuthConfiguration), authCfg);
        services.AddSingleton(authCfg);

        services
           .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = authCfg.Issuer,

                    ValidateAudience = true,
                    ValidAudience = authCfg.Audience,

                    ValidateLifetime = true,

                    IssuerSigningKey = authCfg.GetSymmetricSecurityKey(),
                    ValidateIssuerSigningKey = true
                };
                
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // если запрос направлен хабу
                        var path = context.HttpContext.Request.Path;
                        if (path.StartsWithSegments("/notificationHub") && !string.IsNullOrEmpty(accessToken))
                        {
                            // получаем токен из строки запроса
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        return services;
    }

    public static IServiceCollection SetupAuthorization(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthorization(options => { AddTest(options); });
        services.AddAuthorization(options => { AddDailySurveys(options); });
        services.AddAuthorization(options => { AddWeeklySurveys(options); });
        services.AddAuthorization(options => { AddMedicalHistories(options); });
        services.AddAuthorization(options => { AddPatients(options); });
        services.AddAuthorization(options => { AddDoctors(options); });
        services.AddAuthorization(options => { AddCommunicationLinks(options); });

        return services;
    }
}