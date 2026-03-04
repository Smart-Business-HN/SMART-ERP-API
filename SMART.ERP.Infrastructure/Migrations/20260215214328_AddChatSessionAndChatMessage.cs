using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMART.ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChatSessionAndChatMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatSession",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionIdentifier = table.Column<string>(type: "varchar(36)", nullable: false),
                    VisitorName = table.Column<string>(type: "varchar(100)", nullable: false),
                    VisitorEmail = table.Column<string>(type: "varchar(100)", nullable: true),
                    IsAuthenticated = table.Column<bool>(type: "bit", nullable: false),
                    EcommerceUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AssignedAdminUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    ClosedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastMessagePreview = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    LastMessageAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    UnreadAdminCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatSession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatSession_EcommerceUser_EcommerceUserId",
                        column: x => x.EcommerceUserId,
                        principalTable: "EcommerceUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChatSession_User_AssignedAdminUserId",
                        column: x => x.AssignedAdminUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChatSessionId = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SenderType = table.Column<string>(type: "varchar(10)", nullable: false),
                    SenderAdminUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SenderName = table.Column<string>(type: "varchar(100)", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessage_ChatSession_ChatSessionId",
                        column: x => x.ChatSessionId,
                        principalTable: "ChatSession",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatMessage_User_SenderAdminUserId",
                        column: x => x.SenderAdminUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessage_ChatSessionId",
                table: "ChatMessage",
                column: "ChatSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessage_SenderAdminUserId",
                table: "ChatMessage",
                column: "SenderAdminUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatSession_AssignedAdminUserId",
                table: "ChatSession",
                column: "AssignedAdminUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatSession_EcommerceUserId",
                table: "ChatSession",
                column: "EcommerceUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatMessage");

            migrationBuilder.DropTable(
                name: "ChatSession");
        }
    }
}
