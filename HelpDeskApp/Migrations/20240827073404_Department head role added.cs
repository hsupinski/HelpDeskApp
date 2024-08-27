using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpDeskApp.Migrations
{
    /// <inheritdoc />
    public partial class Departmentheadroleadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "7f309e1d-d6e1-48a0-b117-d3e68ea26e91", "7f309e1d-d6e1-48a0-b117-d3e68ea26e91", "Department Head", "DEPARTMENT HEAD" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7f309e1d-d6e1-48a0-b117-d3e68ea26e91");
        }
    }
}
