using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CuzdanUygulamasi.Migrations
{
    /// <inheritdoc />
    public partial class AddOdemeTaksitiTable1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OdemeTaksitleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaksitliOdemeId = table.Column<int>(type: "int", nullable: false),
                    OdenenTutar = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OdemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OdemeYontemi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OdemeTaksitleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OdemeTaksitleri_TaksitliOdemeler_TaksitliOdemeId",
                        column: x => x.TaksitliOdemeId,
                        principalTable: "TaksitliOdemeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OdemeTaksitleri_TaksitliOdemeId",
                table: "OdemeTaksitleri",
                column: "TaksitliOdemeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OdemeTaksitleri");
        }
    }
}
