using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugTrack.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStaleBugTestRunLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bugs_TestRuns_TestRunId",
                table: "Bugs");

            migrationBuilder.DropIndex(
                name: "IX_Bugs_TestRunId",
                table: "Bugs");

            migrationBuilder.DropColumn(
                name: "TestRunId",
                table: "Bugs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TestRunId",
                table: "Bugs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bugs_TestRunId",
                table: "Bugs",
                column: "TestRunId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bugs_TestRuns_TestRunId",
                table: "Bugs",
                column: "TestRunId",
                principalTable: "TestRuns",
                principalColumn: "Id");
        }
    }
}
