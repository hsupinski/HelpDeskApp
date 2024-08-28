using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpDeskApp.Migrations.HelpDeskDb
{
    /// <inheritdoc />
    public partial class fixedtopicdepartmentrelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DepartmentTopic");

            migrationBuilder.AlterColumn<int>(
                name: "DepartmentId",
                table: "Topics",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "DepartmentIds",
                table: "Topics",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_DepartmentId",
                table: "Topics",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_Departments_DepartmentId",
                table: "Topics",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Topics_Departments_DepartmentId",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Topics_DepartmentId",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "DepartmentIds",
                table: "Topics");

            migrationBuilder.AlterColumn<int>(
                name: "DepartmentId",
                table: "Topics",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "DepartmentTopic",
                columns: table => new
                {
                    DepartmentsId = table.Column<int>(type: "int", nullable: false),
                    TopicsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentTopic", x => new { x.DepartmentsId, x.TopicsId });
                    table.ForeignKey(
                        name: "FK_DepartmentTopic_Departments_DepartmentsId",
                        column: x => x.DepartmentsId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DepartmentTopic_Topics_TopicsId",
                        column: x => x.TopicsId,
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentTopic_TopicsId",
                table: "DepartmentTopic",
                column: "TopicsId");
        }
    }
}
