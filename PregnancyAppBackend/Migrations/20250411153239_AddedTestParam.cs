using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PregnancyAppBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddedTestParam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- Insert standard parameter definitions
                INSERT INTO [dbo].[ObservationParameterNorms]
                ([Id], [ParameterName], [LowerBound], [UpperBound], [UserId])
                SELECT NEWID(), 'TestParam', 0, 10, UserId
                FROM [dbo].[UserRoles] 
                WHERE [RoleId] = '39B15202-0123-48E1-9BD4-7015D1DF09F9';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- Delete the TestParam entries added in the Up method
                DELETE FROM [dbo].[ObservationParameterNorms]
                WHERE [ParameterName] = 'TestParam';");
        }
    }
}
