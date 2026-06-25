using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BurgerPOS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregaNumeroEnvio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "total_envios",
                table: "ordenes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "numero_envio",
                table: "lineas_orden",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "total_envios",
                table: "ordenes");

            migrationBuilder.DropColumn(
                name: "numero_envio",
                table: "lineas_orden");
        }
    }
}
