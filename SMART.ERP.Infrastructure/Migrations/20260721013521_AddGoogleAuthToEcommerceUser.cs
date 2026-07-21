using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMART.ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGoogleAuthToEcommerceUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "PasswordSalt",
                table: "EcommerceUser",
                type: "varbinary(max)",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)");

            migrationBuilder.AlterColumn<byte[]>(
                name: "PasswordHash",
                table: "EcommerceUser",
                type: "varbinary(max)",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)");

            migrationBuilder.AlterColumn<byte[]>(
                name: "MasterPasswordSalt",
                table: "EcommerceUser",
                type: "varbinary(max)",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)");

            migrationBuilder.AlterColumn<byte[]>(
                name: "MasterPasswordHash",
                table: "EcommerceUser",
                type: "varbinary(max)",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)");

            // defaultValue 1 = AuthProvider.Local. EF genera 0 por ser el default del CLR,
            // pero 0 no es un valor valido del enum y dejaria las filas existentes invalidas.
            migrationBuilder.AddColumn<int>(
                name: "AuthProvider",
                table: "EcommerceUser",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<bool>(
                name: "EmailVerified",
                table: "EcommerceUser",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "GoogleSubjectId",
                table: "EcommerceUser",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EcommerceUser_GoogleSubjectId",
                table: "EcommerceUser",
                column: "GoogleSubjectId",
                unique: true,
                filter: "[GoogleSubjectId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EcommerceUser_GoogleSubjectId",
                table: "EcommerceUser");

            migrationBuilder.DropColumn(
                name: "AuthProvider",
                table: "EcommerceUser");

            migrationBuilder.DropColumn(
                name: "EmailVerified",
                table: "EcommerceUser");

            migrationBuilder.DropColumn(
                name: "GoogleSubjectId",
                table: "EcommerceUser");

            migrationBuilder.AlterColumn<byte[]>(
                name: "PasswordSalt",
                table: "EcommerceUser",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "PasswordHash",
                table: "EcommerceUser",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "MasterPasswordSalt",
                table: "EcommerceUser",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "MasterPasswordHash",
                table: "EcommerceUser",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)",
                oldNullable: true);
        }
    }
}
