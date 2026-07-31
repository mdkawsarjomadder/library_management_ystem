using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddIssueDateToIssueBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_issueBooks_AspNetUsers_UserId",
                table: "issueBooks");

            migrationBuilder.DropForeignKey(
                name: "FK_issueBooks_Books_BookId",
                table: "issueBooks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_issueBooks",
                table: "issueBooks");

            migrationBuilder.RenameTable(
                name: "issueBooks",
                newName: "IssueBooks");

            migrationBuilder.RenameIndex(
                name: "IX_issueBooks_UserId",
                table: "IssueBooks",
                newName: "IX_IssueBooks_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_issueBooks_BookId",
                table: "IssueBooks",
                newName: "IX_IssueBooks_BookId");

            migrationBuilder.AddColumn<DateTime>(
                name: "IssueDate",
                table: "IssueBooks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_IssueBooks",
                table: "IssueBooks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IssueBooks_AspNetUsers_UserId",
                table: "IssueBooks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IssueBooks_Books_BookId",
                table: "IssueBooks",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IssueBooks_AspNetUsers_UserId",
                table: "IssueBooks");

            migrationBuilder.DropForeignKey(
                name: "FK_IssueBooks_Books_BookId",
                table: "IssueBooks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IssueBooks",
                table: "IssueBooks");

            migrationBuilder.DropColumn(
                name: "IssueDate",
                table: "IssueBooks");

            migrationBuilder.RenameTable(
                name: "IssueBooks",
                newName: "issueBooks");

            migrationBuilder.RenameIndex(
                name: "IX_IssueBooks_UserId",
                table: "issueBooks",
                newName: "IX_issueBooks_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_IssueBooks_BookId",
                table: "issueBooks",
                newName: "IX_issueBooks_BookId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_issueBooks",
                table: "issueBooks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_issueBooks_AspNetUsers_UserId",
                table: "issueBooks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_issueBooks_Books_BookId",
                table: "issueBooks",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
