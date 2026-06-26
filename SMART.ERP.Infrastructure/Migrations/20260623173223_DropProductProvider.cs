using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMART.ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DropProductProvider : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // El esquema real puede tener el FK con un nombre distinto al de la convención EF
            // (aquí es 'FK_Product_Provider') y el índice 'IX_Product_ProviderId' puede no existir.
            // Se eliminan de forma defensiva para que la migración no falle por nombres/objetos ausentes.
            migrationBuilder.Sql(@"
                DECLARE @fk sysname;
                SELECT @fk = fk.name FROM sys.foreign_keys fk
                JOIN sys.tables t ON t.object_id = fk.parent_object_id
                WHERE t.name = 'Product' AND OBJECT_NAME(fk.referenced_object_id) = 'Provider';
                IF @fk IS NOT NULL EXEC('ALTER TABLE [Product] DROP CONSTRAINT [' + @fk + ']');");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Product_ProviderId' AND object_id = OBJECT_ID('Product'))
                    DROP INDEX [IX_Product_ProviderId] ON [Product];");

            migrationBuilder.DropColumn(
                name: "ProviderId",
                table: "Product");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProviderId",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Product_ProviderId",
                table: "Product",
                column: "ProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Provider_ProviderId",
                table: "Product",
                column: "ProviderId",
                principalTable: "Provider",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
