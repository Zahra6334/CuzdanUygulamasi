using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CuzdanUygulamasi.Migrations
{
    /// <inheritdoc />
    public partial class FixDecimalTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "FaizOrani",
                table: "TaksitliOdemeler",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "FaizOrani",
                table: "TaksitliOdemeler",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
