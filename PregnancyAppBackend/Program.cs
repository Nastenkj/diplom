using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PregnancyAppBackend.Extensions;
using PregnancyAppBackend.Extensions.Quartz;
using PregnancyAppBackend.Hubs;
using PregnancyAppBackend.Infrastructure.Security;
using PregnancyAppBackend.Middleware.CorrelationIdMiddleware;
using PregnancyAppBackend.Middleware.ErrorHandler;
using PregnancyAppBackend.Persistance;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace PregnancyAppBackend;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Information()
                        .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
                        .Enrich.FromLogContext()
                        .WriteTo.Console(outputTemplate:
                                         "[{Timestamp:HH:mm:ss} {Level:u3} {CorrelationId}] {Message}{NewLine}{Exception}")
                        .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day, outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3} {CorrelationId}] {Message}{NewLine}{Exception}")
                        .CreateLogger();

            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog();

            // Add services to the container
            builder.Services.AddAuthenticationCustom(builder.Configuration);
            builder.Services.SetupAuthorization(builder.Configuration);



            builder.Services.AddDbContext<IDatabaseContext, DatabaseContext>(options =>
                                                                                 options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddDbContextFactory<DatabaseContext>(options =>
                                                                      options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")),
                                                                  ServiceLifetime.Scoped);

            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();

            builder.Services.AddHttpContextAccessor();
            builder.Services.HandleDependencyInjection();

            builder.Services.AddQuartzServices(builder.Configuration);

            builder.Services.AddSignalR();

            builder.Services.Configure<ApiBehaviorOptions>(options =>
                                                               options.SuppressModelStateInvalidFilter = true);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // Swagger/OpenAPI Configuration
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Pregnancy App API",
                    Version = "v1",
                    Description = "API for Pregnancy Application"
                });

                // Add JWT Authentication to Swagger
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "JWT Authentication",
                    Description = "Enter JWT Bearer token",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { securityScheme, Array.Empty<string>() }
                });
            });

            builder.Services.AddControllers();

            var app = builder.Build();
            app.UseMiddleware<CorrelationIdMiddleware>();

            // Configure the HTTP request pipeline
            app.UseErrorHandler();
            app.UseWebSockets();
            app.UseRouting();

            // Swagger middleware should come before authentication/authorization
            app.UseSwagger();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pregnancy App API v1");
                    c.RoutePrefix = "swagger";
                    c.DisplayRequestDuration();
                    c.EnableDeepLinking();
                });
            }

            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHttpsRedirection();
            app.MapControllers();
            app.MapHub<NotificationHub>("/notificationHub");

            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<DatabaseContext>();

            try
            {
                await context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: {0}", ex.Message);
            }

            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application startup failed");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}