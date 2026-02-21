using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PregnancyAppBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddedNewService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MedicalHistories_UserId",
                table: "MedicalHistories");

            migrationBuilder.CreateTable(
                name: "AlgorithmicAnalysisParameterValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ParameterName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlgorithmicAnalysisParameterValues", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicalHistories_UserId",
                table: "MedicalHistories",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AlgorithmicAnalysisParameterValues_UserId_ParameterName",
                table: "AlgorithmicAnalysisParameterValues",
                columns: new[] { "UserId", "ParameterName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlgorithmicAnalysisParameterValues");

            migrationBuilder.DropIndex(
                name: "IX_MedicalHistories_UserId",
                table: "MedicalHistories");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalHistories_UserId",
                table: "MedicalHistories",
                column: "UserId");
        }
    }
}
