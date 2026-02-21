using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PregnancyAppBackend.Migrations
{
    /// <inheritdoc />
    public partial class ChangedBloodPressureFormat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Add new columns (as nullable initially)
            migrationBuilder.AddColumn<int>(
                name: "SystolicPressure",
                table: "DailySurveys",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DiastolicPressure",
                table: "DailySurveys",
                type: "int",
                nullable: true);

            // 2. Parse and transfer data from the string column to the new integer columns
            migrationBuilder.Sql(@"
                UPDATE DailySurveys
                SET 
                    SystolicPressure = CAST(SUBSTRING(BloodPressure, 1, CHARINDEX('/', BloodPressure) - 1) AS int),
                    DiastolicPressure = CAST(SUBSTRING(BloodPressure, CHARINDEX('/', BloodPressure) + 1, LEN(BloodPressure)) AS int)
                WHERE 
                    BloodPressure IS NOT NULL 
                    AND BloodPressure LIKE '%/%'
                    AND ISNUMERIC(SUBSTRING(BloodPressure, 1, CHARINDEX('/', BloodPressure) - 1)) = 1
                    AND ISNUMERIC(SUBSTRING(BloodPressure, CHARINDEX('/', BloodPressure) + 1, LEN(BloodPressure))) = 1
            ");

            // 3. Change columns to non-nullable with default values
            migrationBuilder.AlterColumn<int>(
                name: "SystolicPressure",
                table: "DailySurveys",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "DiastolicPressure",
                table: "DailySurveys",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // 4. Drop the constraint before dropping the column
            migrationBuilder.Sql("ALTER TABLE DailySurveys DROP CONSTRAINT CK_DailySurvey_BloodPressure");

            // 5. Now drop the original column after data transfer is complete
            migrationBuilder.DropColumn(
                name: "BloodPressure",
                table: "DailySurveys");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 1. Add back the original column (nullable initially)
            migrationBuilder.AddColumn<string>(
                name: "BloodPressure",
                table: "DailySurveys",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            // 2. Convert the integer values back to the string format
            migrationBuilder.Sql(@"
                UPDATE DailySurveys
                SET BloodPressure = CAST(SystolicPressure AS nvarchar(5)) + '/' + CAST(DiastolicPressure AS nvarchar(5))
                WHERE SystolicPressure IS NOT NULL AND DiastolicPressure IS NOT NULL
            ");

            // 3. Make the string column non-nullable
            migrationBuilder.AlterColumn<string>(
                name: "BloodPressure",
                table: "DailySurveys",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            // 4. Drop the new columns
            migrationBuilder.DropColumn(
                name: "DiastolicPressure",
                table: "DailySurveys");

            migrationBuilder.DropColumn(
                name: "SystolicPressure",
                table: "DailySurveys");
            
            // 5. Recreate the check constraint
            migrationBuilder.Sql(@"
                ALTER TABLE DailySurveys 
                ADD CONSTRAINT CK_DailySurvey_BloodPressure CHECK (
                    BloodPressure LIKE '%/%' AND
                    ISNUMERIC(SUBSTRING(BloodPressure, 1, CHARINDEX('/', BloodPressure) - 1)) = 1 AND
                    ISNUMERIC(SUBSTRING(BloodPressure, CHARINDEX('/', BloodPressure) + 1, LEN(BloodPressure))) = 1
                )
            ");
        }
    }
}