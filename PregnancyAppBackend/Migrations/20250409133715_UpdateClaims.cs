using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using PregnancyAppBackend.Entities.Security;

#nullable disable

namespace PregnancyAppBackend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClaims : Migration
    {
        

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var patient = Role.PatientId;
            var getWeeklySurveysForUser = Guid.Parse("3B69E456-8E7B-4015-B5F4-E7CECD5BC5F4");
            var getWeeklySurveyById = Guid.Parse("7A51F702-B1FB-43EB-864D-13E6345ED995");
            var getDailySurveyById = Guid.Parse("7FA406B2-3598-47CF-815F-A25C6603412C");
            migrationBuilder.InsertData(
                table: "RoleApiClaims",
                columns: new[] { "RoleId", "ApiClaimId" },
                values: new object[,]
                {
                    { patient, getWeeklySurveyById },
                    { patient, getWeeklySurveysForUser },
                    { patient, getDailySurveyById },
                }
            );

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var patient = Role.PatientId;
            var getWeeklySurveysForUser = Guid.Parse("3B69E456-8E7B-4015-B5F4-E7CECD5BC5F4");
            var getWeeklySurveyById = Guid.Parse("7A51F702-B1FB-43EB-864D-13E6345ED995");
            var getDailySurveysForUser = Guid.Parse("A927AAD7-0EF2-40A1-8A54-894233572AD5");
            var getDailySurveyById = Guid.Parse("7FA406B2-3598-47CF-815F-A25C6603412C");

            // Удаляем все добавленные записи
            migrationBuilder.DeleteData(
                table: "RoleApiClaims",
                keyColumns: new[] { "RoleId", "ApiClaimId" },
                keyValues: new object[] { patient, getWeeklySurveysForUser }
            );
    
            migrationBuilder.DeleteData(
                table: "RoleApiClaims",
                keyColumns: new[] { "RoleId", "ApiClaimId" },
                keyValues: new object[] { patient, getWeeklySurveyById }
            );
    
            migrationBuilder.DeleteData(
                table: "RoleApiClaims",
                keyColumns: new[] { "RoleId", "ApiClaimId" },
                keyValues: new object[] { patient, getDailySurveysForUser }
            );
    
            migrationBuilder.DeleteData(
                table: "RoleApiClaims",
                keyColumns: new[] { "RoleId", "ApiClaimId" },
                keyValues: new object[] { patient, getDailySurveyById }
            );
        }
    }
}
