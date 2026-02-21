using Microsoft.EntityFrameworkCore.Migrations;
using PregnancyAppBackend.Entities.Security;

#nullable disable

namespace PregnancyAppBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddedNewClaims : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create new claim IDs
            var CreateCommunicationLinkId = Guid.Parse("D76DAD62-5983-43D4-A43E-3C35323DB7FA"); // doctor
            var GetMyCommunicationLinksId = Guid.Parse("1C8DFB99-8B9B-40BE-A09C-17BA94C70EF5"); // doctor + patient
            var GetAllDoctorsId = Guid.Parse("CC84FEC5-D5A5-461E-BADF-A292D3168D70"); // admin
            var GetDoctorId = Guid.Parse("F9B9E940-3679-4678-BA4E-7B1EED286D9B"); // admin + doctor
            var EditDoctorInfoId = Guid.Parse("8310B493-40A6-4581-9929-2262E9F4C29C"); // admin + doctor
            
            // 1. Insert all new API claims
            migrationBuilder.InsertData(
                table: "ApiClaims",
                columns: new[] { "Id", "Type" },
                values: new object[,]
                {
                    { CreateCommunicationLinkId, "create_communication_link" },
                    { GetMyCommunicationLinksId, "get_my_communication_links" },
                    { GetAllDoctorsId, "get_all_doctors" },
                    { GetDoctorId, "get_doctor" },
                    { EditDoctorInfoId, "edit_doctor_info" }
                }
            );
            
            // 2. Associate claims with client role
            migrationBuilder.InsertData(
                table: "RoleApiClaims",
                columns: new[] { "RoleId", "ApiClaimId" },
                values: new object[,]
                {
                    { Role.PatientId, GetMyCommunicationLinksId },
                }
            );

            // 3. Associate claims with doctor role
            migrationBuilder.InsertData(
                table: "RoleApiClaims",
                columns: new[] { "RoleId", "ApiClaimId" },
                values: new object[,]
                {
                    { Role.DoctorId, CreateCommunicationLinkId },
                    { Role.DoctorId, GetMyCommunicationLinksId },
                    { Role.DoctorId, GetDoctorId },
                    { Role.DoctorId, EditDoctorInfoId },
                }
            );

            // 4. Associate claims with admin role (same as doctor)
            migrationBuilder.InsertData(
                table: "RoleApiClaims",
                columns: new[] { "RoleId", "ApiClaimId" },
                values: new object[,]
                {
                    { Role.AdministratorId, GetAllDoctorsId },
                    { Role.AdministratorId, GetDoctorId },
                    { Role.AdministratorId, EditDoctorInfoId },
                }
            );
            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Define claim IDs (same as in Up method)
            var CreateCommunicationLinkId = Guid.Parse("D76DAD62-5983-43D4-A43E-3C35323DB7FA");
            var GetMyCommunicationLinksId = Guid.Parse("1C8DFB99-8B9B-40BE-A09C-17BA94C70EF5");
            var GetAllDoctorsId = Guid.Parse("CC84FEC5-D5A5-461E-BADF-A292D3168D70");
            var GetDoctorId = Guid.Parse("F9B9E940-3679-4678-BA4E-7B1EED286D9B");
            var EditDoctorInfoId = Guid.Parse("8310B493-40A6-4581-9929-2262E9F4C29C");
            
            // 1. Remove associations for Admin role
            migrationBuilder.DeleteData(
                table: "RoleApiClaims",
                keyColumns: new[] { "RoleId", "ApiClaimId" },
                keyValues: new object[] { Role.AdministratorId, GetAllDoctorsId }
            );
            migrationBuilder.DeleteData(
                table: "RoleApiClaims",
                keyColumns: new[] { "RoleId", "ApiClaimId" },
                keyValues: new object[] { Role.AdministratorId, GetDoctorId }
            );
            migrationBuilder.DeleteData(
                table: "RoleApiClaims",
                keyColumns: new[] { "RoleId", "ApiClaimId" },
                keyValues: new object[] { Role.AdministratorId, EditDoctorInfoId }
            );
            
            // 2. Remove associations for Doctor role
            migrationBuilder.DeleteData(
                table: "RoleApiClaims",
                keyColumns: new[] { "RoleId", "ApiClaimId" },
                keyValues: new object[] { Role.DoctorId, CreateCommunicationLinkId }
            );
            migrationBuilder.DeleteData(
                table: "RoleApiClaims",
                keyColumns: new[] { "RoleId", "ApiClaimId" },
                keyValues: new object[] { Role.DoctorId, GetMyCommunicationLinksId }
            );
            migrationBuilder.DeleteData(
                table: "RoleApiClaims",
                keyColumns: new[] { "RoleId", "ApiClaimId" },
                keyValues: new object[] { Role.DoctorId, GetDoctorId }
            );
            migrationBuilder.DeleteData(
                table: "RoleApiClaims",
                keyColumns: new[] { "RoleId", "ApiClaimId" },
                keyValues: new object[] { Role.DoctorId, EditDoctorInfoId }
            );
            
            // 3. Remove associations for Patient role
            migrationBuilder.DeleteData(
                table: "RoleApiClaims",
                keyColumns: new[] { "RoleId", "ApiClaimId" },
                keyValues: new object[] { Role.PatientId, GetMyCommunicationLinksId }
            );
            
            // 4. Remove the API claims themselves
            migrationBuilder.DeleteData(
                table: "ApiClaims",
                keyColumn: "Id",
                keyValue: CreateCommunicationLinkId
            );
            migrationBuilder.DeleteData(
                table: "ApiClaims",
                keyColumn: "Id",
                keyValue: GetMyCommunicationLinksId
            );
            migrationBuilder.DeleteData(
                table: "ApiClaims",
                keyColumn: "Id",
                keyValue: GetAllDoctorsId
            );
            migrationBuilder.DeleteData(
                table: "ApiClaims",
                keyColumn: "Id",
                keyValue: GetDoctorId
            );
            migrationBuilder.DeleteData(
                table: "ApiClaims",
                keyColumn: "Id",
                keyValue: EditDoctorInfoId
            );
        }
    }
}
