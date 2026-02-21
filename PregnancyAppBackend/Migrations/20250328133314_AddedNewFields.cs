using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PregnancyAppBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddedNewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PregnancyWeek",
                table: "WeeklySurveys",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Aptt",
                table: "DailySurveys",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "HemoglobinLevel",
                table: "DailySurveys",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Inr",
                table: "DailySurveys",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Pt",
                table: "DailySurveys",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Saturation",
                table: "DailySurveys",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Vc",
                table: "DailySurveys",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PregnancyWeek",
                table: "WeeklySurveys");

            migrationBuilder.DropColumn(
                name: "Aptt",
                table: "DailySurveys");

            migrationBuilder.DropColumn(
                name: "HemoglobinLevel",
                table: "DailySurveys");

            migrationBuilder.DropColumn(
                name: "Inr",
                table: "DailySurveys");

            migrationBuilder.DropColumn(
                name: "Pt",
                table: "DailySurveys");

            migrationBuilder.DropColumn(
                name: "Saturation",
                table: "DailySurveys");

            migrationBuilder.DropColumn(
                name: "Vc",
                table: "DailySurveys");
        }
    }
}
