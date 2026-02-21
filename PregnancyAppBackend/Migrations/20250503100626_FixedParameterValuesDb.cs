using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PregnancyAppBackend.Migrations
{
    /// <inheritdoc />
    public partial class FixedParameterValuesDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("delete from AlgorithmicAnalysisParameterValues;");
            migrationBuilder.Sql("delete from ObservationParameterNorms;");
            
            
            migrationBuilder.DropIndex(
                name: "IX_AlgorithmicAnalysisParameterValues_UserId_ParameterName",
                table: "AlgorithmicAnalysisParameterValues");

            migrationBuilder.DropColumn(
                name: "ParameterName",
                table: "ObservationParameterNorms");

            migrationBuilder.DropColumn(
                name: "ParameterName",
                table: "AlgorithmicAnalysisParameterValues");

            migrationBuilder.AddColumn<Guid>(
                name: "ParameterId",
                table: "ObservationParameterNorms",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ParameterId",
                table: "AlgorithmicAnalysisParameterValues",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Parameters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParameterName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DefaultLowerBoundValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DefaultUpperBoundValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parameters", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ObservationParameterNorms_ParameterId",
                table: "ObservationParameterNorms",
                column: "ParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_AlgorithmicAnalysisParameterValues_ParameterId",
                table: "AlgorithmicAnalysisParameterValues",
                column: "ParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_AlgorithmicAnalysisParameterValues_UserId_ParameterId",
                table: "AlgorithmicAnalysisParameterValues",
                columns: new[] { "UserId", "ParameterId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AlgorithmicAnalysisParameterValues_Parameters_ParameterId",
                table: "AlgorithmicAnalysisParameterValues",
                column: "ParameterId",
                principalTable: "Parameters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ObservationParameterNorms_Parameters_ParameterId",
                table: "ObservationParameterNorms",
                column: "ParameterId",
                principalTable: "Parameters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            
            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'Aptt', 25.00, 38.00);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'Glu', 70.00, 99.00);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'Bld', 0.00, 4.00);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'Leu', 0.00, 4.00);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'HasUnordinaryTemp', 0.00, 0.00);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'HasSwelling', 0.00, 0.00);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'UrgeToPuke', 0.00, 10.00);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'BloodCoagulation', 0.80, 1.20);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'UnordinaryBloodPressure', 0.00, 0.00);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'HasGynecologicalSymptoms', 0.00, 0.00);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'Pro', 0.00, 4.00);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'Nit', 0.00, 1.00);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'WeightAdded', 0.00, 35.00);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'Nausea', 0.00, 10.00);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'UnordinaryTempOccurrences', 0.00, 10.00);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'Uro', 0.10, 1.00);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'DiastolicPressure', 60.00, 80.00);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'Ph', 4.50, 8.00);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'Sg', 1.005, 1.030);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'Bil', 0.00, 0.20);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'Inr', 0.80, 1.20);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'BloodDischarge', 0.00, 5.00);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'Temperature', 36.50, 37.50);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'Vc', 3.00, 5.00);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'SystolicPressure', 90.00, 120.00);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'HeartRate', 60.00, 100.00);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'WaterConsumed', 1.50, 3.00);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'TestParam', 0.00, 100.00);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'HemoglobinLevel', 12.00, 16.00);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'AbdomenHurts', 0.00, 10.00);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'HasOrvi', 0.00, 0.00);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'Ket', 0.00, 0.50);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'HasUnordinaryUrine', 0.00, 0.00);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'Saturation', 95.00, 100.00);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'GlucoseLevel', 70.00, 99.00);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'OxygenLevel', 95.00, 100.00);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'PregnancyWeek', 1.00, 42.00);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'Stool', 1.00, 3.00);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'Urination', 4.00, 10.00);");

            migrationBuilder.Sql(@"insert into [Parameters] ([Id]
                                  ,[ParameterName]
                                  ,[DefaultLowerBoundValue]
                                  ,[DefaultUpperBoundValue]) values (newid(), 'Pt', 10.00, 14.00);");
            
            migrationBuilder.Sql(@"
-- Insert parameter norms for each doctor user
INSERT INTO [dbo].[ObservationParameterNorms] (
    [Id],
    [ParameterId],
    [LowerBound],
    [UpperBound],
    [UserId]
)
SELECT 
    NEWID() AS Id,
    p.Id AS ParameterId,
    p.DefaultLowerBoundValue AS LowerBound,
    p.DefaultUpperBoundValue AS UpperBound,
    ur.UserId AS UserId
FROM 
    [dbo].[Parameters] p
CROSS JOIN 
    [dbo].[UserRoles] ur
WHERE 
    ur.RoleId = '39B15202-0123-48E1-9BD4-7015D1DF09F9';
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlgorithmicAnalysisParameterValues_Parameters_ParameterId",
                table: "AlgorithmicAnalysisParameterValues");

            migrationBuilder.DropForeignKey(
                name: "FK_ObservationParameterNorms_Parameters_ParameterId",
                table: "ObservationParameterNorms");

            migrationBuilder.DropTable(
                name: "Parameters");

            migrationBuilder.DropIndex(
                name: "IX_ObservationParameterNorms_ParameterId",
                table: "ObservationParameterNorms");

            migrationBuilder.DropIndex(
                name: "IX_AlgorithmicAnalysisParameterValues_ParameterId",
                table: "AlgorithmicAnalysisParameterValues");

            migrationBuilder.DropIndex(
                name: "IX_AlgorithmicAnalysisParameterValues_UserId_ParameterId",
                table: "AlgorithmicAnalysisParameterValues");

            migrationBuilder.DropColumn(
                name: "ParameterId",
                table: "ObservationParameterNorms");

            migrationBuilder.DropColumn(
                name: "ParameterId",
                table: "AlgorithmicAnalysisParameterValues");

            migrationBuilder.AddColumn<string>(
                name: "ParameterName",
                table: "ObservationParameterNorms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ParameterName",
                table: "AlgorithmicAnalysisParameterValues",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_AlgorithmicAnalysisParameterValues_UserId_ParameterName",
                table: "AlgorithmicAnalysisParameterValues",
                columns: new[] { "UserId", "ParameterName" },
                unique: true);
        }
    }
}
