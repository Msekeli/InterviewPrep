using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterviewPrep.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OverallScore",
                table: "InterviewSessions");

            migrationBuilder.RenameColumn(
                name: "Feedback",
                table: "InterviewSessions",
                newName: "Strengths");

            migrationBuilder.AddColumn<string>(
                name: "Communication",
                table: "InterviewSessions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GrowthOpportunity",
                table: "InterviewSessions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NextFocus",
                table: "InterviewSessions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Observation",
                table: "InterviewSessions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OverallImpression",
                table: "InterviewSessions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Communication",
                table: "InterviewSessions");

            migrationBuilder.DropColumn(
                name: "GrowthOpportunity",
                table: "InterviewSessions");

            migrationBuilder.DropColumn(
                name: "NextFocus",
                table: "InterviewSessions");

            migrationBuilder.DropColumn(
                name: "Observation",
                table: "InterviewSessions");

            migrationBuilder.DropColumn(
                name: "OverallImpression",
                table: "InterviewSessions");

            migrationBuilder.RenameColumn(
                name: "Strengths",
                table: "InterviewSessions",
                newName: "Feedback");

            migrationBuilder.AddColumn<decimal>(
                name: "OverallScore",
                table: "InterviewSessions",
                type: "TEXT",
                nullable: true);
        }
    }
}
