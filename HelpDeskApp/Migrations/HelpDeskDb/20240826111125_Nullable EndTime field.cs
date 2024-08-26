using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpDeskApp.Migrations.HelpDeskDb
{
    /// <inheritdoc />
    public partial class NullableEndTimefield : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatParticipations_ApplicationUser_ParticipantId",
                table: "ChatParticipations");

            migrationBuilder.DropIndex(
                name: "IX_ChatParticipations_ParticipantId",
                table: "ChatParticipations");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "Chats",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "ParticipantId",
                table: "ChatParticipations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "ChatParticipations",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatParticipations_ApplicationUserId",
                table: "ChatParticipations",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatParticipations_ApplicationUser_ApplicationUserId",
                table: "ChatParticipations",
                column: "ApplicationUserId",
                principalTable: "ApplicationUser",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatParticipations_ApplicationUser_ApplicationUserId",
                table: "ChatParticipations");

            migrationBuilder.DropIndex(
                name: "IX_ChatParticipations_ApplicationUserId",
                table: "ChatParticipations");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "ChatParticipations");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "Chats",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ParticipantId",
                table: "ChatParticipations",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_ChatParticipations_ParticipantId",
                table: "ChatParticipations",
                column: "ParticipantId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatParticipations_ApplicationUser_ParticipantId",
                table: "ChatParticipations",
                column: "ParticipantId",
                principalTable: "ApplicationUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
