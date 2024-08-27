using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpDeskApp.Migrations.HelpDeskDb
{
    /// <inheritdoc />
    public partial class IEnumerabletoList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DepartmentHeadId",
                table: "Departments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartmentHeadId",
                table: "Departments");
        }
    }
}
