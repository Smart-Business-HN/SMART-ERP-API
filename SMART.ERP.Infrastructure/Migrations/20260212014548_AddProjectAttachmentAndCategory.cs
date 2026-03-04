using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMART.ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectAttachmentAndCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectAttachmentCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectAttachmentCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectAttachment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectAttachmentCategoryId = table.Column<int>(type: "int", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificatedBy = table.Column<string>(type: "varchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectAttachment_ProjectAttachmentCategory_ProjectAttachmentCategoryId",
                        column: x => x.ProjectAttachmentCategoryId,
                        principalTable: "ProjectAttachmentCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectAttachment_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectAttachment_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAttachment_ProjectAttachmentCategoryId",
                table: "ProjectAttachment",
                column: "ProjectAttachmentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAttachment_ProjectId",
                table: "ProjectAttachment",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAttachment_UserId",
                table: "ProjectAttachment",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectAttachment");

            migrationBuilder.DropTable(
                name: "ProjectAttachmentCategory");
        }
    }
}
