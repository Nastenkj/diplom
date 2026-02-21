using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PregnancyAppBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddedObservationParameters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ObservationParameterNorms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParameterName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LowerBound = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UpperBound = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObservationParameterNorms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObservationParameterNorms_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ObservationParameterNorms_UserId",
                table: "ObservationParameterNorms",
                column: "UserId");
            
             migrationBuilder.Sql(@"
                -- Insert standard parameter definitions
                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'AbdomenHurts', 0, 1, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'BloodDischarge', 0, 1, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'Nausea', 0, 1, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'UrgeToPuke', 0, 1, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'Temperature', 36.0, 37.5, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'SystolicPressure', 90, 140, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'DiastolicPressure', 60, 90, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'HeartRate', 60, 100, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'GlucoseLevel', 3.3, 5.5, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'HemoglobinLevel', 110, 150, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'Saturation', 95, 100, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                -- Lab values
                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'Uro', 0, 1, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'Bld', 0, 1, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'Bil', 0, 1, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'Ket', 0, 1, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'Leu', 0, 1, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'Glu', 0, 1, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'Pro', 0, 1, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'Ph', 5.0, 8.0, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'Nit', 0, 1, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'Sg', 1.005, 1.030, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'Vc', NULL, NULL, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'Pt', 11, 15, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'Aptt', 25, 38, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'Inr', 0.8, 1.2, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'BloodCoagulation', NULL, NULL, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'OxygenLevel', 95, 100, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                -- Weekly parameters
                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'HasOrvi', 0, 1, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'HasUnordinaryTemp', 0, 1, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'UnordinaryTempOccurrences', 0, 5, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'UnordinaryBloodPressure', 0, 2, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'HasGynecologicalSymptoms', 0, 1, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'HasUnordinaryUrine', 0, 1, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'HasSwelling', 0, 1, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'WaterConsumed', 1500, 3000, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'Stool', 0, 2, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'Urination', 0, 2, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'WeightAdded', 0.2, 0.5, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';

                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'PregnancyWeek', 1, 42, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObservationParameterNorms");
        }
    }
}
