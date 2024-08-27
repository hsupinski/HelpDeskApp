using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpDeskApp.Migrations.HelpDeskDb
{
    /// <inheritdoc />
    public partial class SenderUsernameadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SenderUsername",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SenderUsername",
                table: "Messages");
        }
    }
}
