using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PregnancyAppBackend.Migrations
{
    /// <inheritdoc />
    public partial class ExpandedCommonInfosToAllUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First create the new table
            migrationBuilder.CreateTable(
                name: "UserCommonInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TrustedPersonFullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TrustedPersonPhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TrustedPersonEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    InsuranceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCommonInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCommonInfos_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // Create index
            migrationBuilder.CreateIndex(
                name: "IX_UserCommonInfos_UserId",
                table: "UserCommonInfos",
                column: "UserId",
                unique: true);
                
            // Transfer data from PatientCommonInfos to UserCommonInfos for existing patients
            migrationBuilder.Sql(@"
                INSERT INTO UserCommonInfos (
                    Id,
                    FullName,
                    PhoneNumber,
                    TrustedPersonFullName,
                    TrustedPersonPhoneNumber,
                    TrustedPersonEmail,
                    InsuranceNumber,
                    BirthDate,
                    UserId
                )
                SELECT
                    NEWID(),
                    FullName,
                    PhoneNumber,
                    TrustedPersonFullName,
                    TrustedPersonPhoneNumber,
                    TrustedPersonEmail,
                    InsuranceNumber,
                    BirthDate,
                    UserId
                FROM PatientCommonInfos;
            ");
            
            // Create minimal records for users who don't have PatientCommonInfos
            migrationBuilder.Sql(@"
                INSERT INTO UserCommonInfos (
                    Id,
                    FullName,
                    PhoneNumber,
                    TrustedPersonFullName,
                    TrustedPersonPhoneNumber,
                    TrustedPersonEmail,
                    InsuranceNumber,
                    BirthDate,
                    UserId
                )
                SELECT
                    NEWID(),
                    'Dummy Name',
                    '+79182879679',
                    '',
                    '',
                    '',
                    '',
                    '1900-01-01',
                    u.Id
                FROM Users u
                LEFT JOIN PatientCommonInfos pci ON u.Id = pci.UserId
                LEFT JOIN UserCommonInfos uci ON u.Id = uci.UserId
                WHERE pci.Id IS NULL AND uci.Id IS NULL;
            ");
            
            // Now drop the old table after data has been transferred
            migrationBuilder.DropTable(
                name: "PatientCommonInfos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // First create the original table
            migrationBuilder.CreateTable(
                name: "PatientCommonInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    InsuranceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TrustedPersonEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TrustedPersonFullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TrustedPersonPhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientCommonInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientCommonInfos_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // Create index for original table
            migrationBuilder.CreateIndex(
                name: "IX_PatientCommonInfos_UserId",
                table: "PatientCommonInfos",
                column: "UserId",
                unique: true);
                
            // Transfer data back for patients - only for users who originally had PatientCommonInfos
            // (We can't perfectly restore this, but we get as close as we can)
            migrationBuilder.Sql(@"
                INSERT INTO PatientCommonInfos (
                    Id,
                    FullName,
                    PhoneNumber,
                    TrustedPersonFullName,
                    TrustedPersonPhoneNumber,
                    TrustedPersonEmail,
                    InsuranceNumber,
                    BirthDate,
                    UserId
                )
                SELECT
                    NEWID(),
                    FullName,
                    PhoneNumber,
                    TrustedPersonFullName,
                    TrustedPersonPhoneNumber,
                    TrustedPersonEmail,
                    InsuranceNumber,
                    BirthDate,
                    UserId
                FROM UserCommonInfos
                WHERE FullName != 'Dummy Name' OR PhoneNumber != '+79182879679';
            ");
            
            // Finally drop the new table
            migrationBuilder.DropTable(
                name: "UserCommonInfos");
        }
    }
}