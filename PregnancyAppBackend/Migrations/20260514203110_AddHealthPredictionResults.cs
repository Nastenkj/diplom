using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PregnancyAppBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddHealthPredictionResults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HealthPredictionResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DailySurveyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Trimester = table.Column<int>(type: "int", nullable: false),
                    Prediction = table.Column<int>(type: "int", nullable: false),
                    PredictionText = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    NormalProbability = table.Column<double>(type: "float", nullable: false),
                    AlertProbability = table.Column<double>(type: "float", nullable: false),
                    PathologyProbability = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthPredictionResults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HealthPredictionDeviation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Feature = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NormalRange = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HealthPredictionResultId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthPredictionDeviation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HealthPredictionDeviation_HealthPredictionResults_HealthPredictionResultId",
                        column: x => x.HealthPredictionResultId,
                        principalTable: "HealthPredictionResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HealthPredictionDeviation_HealthPredictionResultId",
                table: "HealthPredictionDeviation",
                column: "HealthPredictionResultId");

            migrationBuilder.CreateIndex(
                name: "IX_HealthPredictionResults_DailySurveyId",
                table: "HealthPredictionResults",
                column: "DailySurveyId");

            migrationBuilder.CreateIndex(
                name: "IX_HealthPredictionResults_UserId",
                table: "HealthPredictionResults",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HealthPredictionResults_UserId_DailySurveyId",
                table: "HealthPredictionResults",
                columns: new[] { "UserId", "DailySurveyId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HealthPredictionDeviation");

            migrationBuilder.DropTable(
                name: "HealthPredictionResults");
        }
    }
}
